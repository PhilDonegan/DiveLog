using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using DiveLog.DTO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Data.SQLite;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using DiveLog.Parser.Extension;

namespace DiveLog.Parsers
{
    public class Shearwater : IParser
    {
        private IConfigurationRoot builder;

        public async Task<List<LogEntryDTO>> ProcessDivesAsync(IFormFile data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.Development.json", true, true)
                .Build();

            var path = await AddDataToStorage(data);
            var sqliteConnection = CreateSqliteConnection(path);
            var dives = ExtractDives(sqliteConnection);
            DeleteUpload(path);
            return dives;
        }

        private void DeleteUpload(string path)
        {
            File.Delete(path);
        }

        private List<LogEntryDTO> ExtractDives(SQLiteConnection sqliteConnection)
        {
            List<LogEntryDTO> dives = new List<LogEntryDTO>();
            try
            {
                sqliteConnection.Open();
                string sql = "SELECT id, CAST(startDate as nvarchar(10)) as stringStartDate, maxTime, maxDepth FROM dive_logs";
                using (var command = new SQLiteCommand(sql, sqliteConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        { 
                            var dive = new LogEntryDTO();
                            dive.ExternalId = reader["id"].ToString();
                            dive.DiveDate = DateTime.FromOADate(Convert.ToDouble(reader["stringStartDate"]));
                            dive.MaxDepth = Convert.ToDecimal(reader["maxDepth"]);
                            dive.DiveLength = TimeSpan.FromMinutes(Convert.ToInt32(reader["maxTime"]));
                            dive.DiveType = DTO.Types.DiveType.CCR;

                            dive.DataPoints = new List<DataPointDTO>();
                            dives.Add(dive);
                        } 
                    }
                }

                string sqllog = "SELECT id, diveLogId, currentTime, currentDepth, fractionO2, fractionHe, waterTemp, averagePPO2 FROM dive_log_records WHERE diveLogId =";
                foreach(var dive in dives)
                {
                    using (var command = new SQLiteCommand(sqllog + dive.ExternalId, sqliteConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (dive.FractionHe == 0 && dive.FractionO2 == 0)
                                {
                                    dive.FractionO2 = Convert.ToDecimal(reader["fractionO2"]);
                                    dive.FractionHe = Convert.ToDecimal(reader["fractionHe"]);
                                }

                                var dataPoint = new DataPointDTO();
                                dataPoint.AveragePPO2 = Convert.ToDecimal(reader["averagePPO2"]);
                                dataPoint.Depth = Convert.ToDecimal(reader["currentDepth"]);
                                dataPoint.Time = Convert.ToInt32(reader["currentTime"]);
                                dataPoint.WaterTemp = Convert.ToInt16(reader["waterTemp"]);

                                dive.DataPoints.Add(dataPoint);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            finally
            {
                sqliteConnection.Close();
            }

            return dives;
        }

        private SQLiteConnection CreateSqliteConnection(string path)
        {
            return new SQLiteConnection($"Data Source={path};datetimeformat={CultureInfo.CurrentCulture}");
        }

        private async Task<string> AddDataToStorage(IFormFile data)
        {
            var uniqueId = $"{Guid.NewGuid().ToString()}.db";

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"));
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", uniqueId);
            var storeUpload = bool.TryParse(builder.GetSection("StoreUploads").Value, out _);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await data.CopyToAsync(stream);
                if (storeUpload)
                {
                    var debugUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "debug");
                    if (!Directory.Exists(debugUploadPath))
                    {
                        Directory.CreateDirectory(debugUploadPath);
                    }

                    File.Copy(path, Path.Combine(debugUploadPath, uniqueId));                    
                }
            }

            return path;
        }

        private async Task<string> AddDataToAzureStorage(object data)
        {
            var uniqueId = $"{Guid.NewGuid().ToString()}.db";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(builder.GetConnectionString("DiveLogShearwaterAzureBlobStorage"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("shearwater-blob-container");

            if (await blobContainer.CreateIfNotExistsAsync())
            {
                await blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(uniqueId);
            var byteArray = ObjectToByteArray(data);
            await blob.UploadFromByteArrayAsync(byteArray, 0, byteArray.Count<byte>());

            return uniqueId;
        }

        private static byte[] ObjectToByteArray(object obj)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
