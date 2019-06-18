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
                            dive.DiveDate = DateTimeExtensions.FromJulianDate(Convert.ToDouble(reader["stringStartDate"]));

                            //var dive = new LogEntryDTO
                            //{
                            //    ExternalId = reader["id"].ToString(),
                            //    DiveDate = DateTimeExtensions.FromJulianDate((double)reader["stringStartDate"]),
                            //    DiveLength = TimeSpan.FromMinutes(Convert.ToInt32(reader["maxTime"])),
                            //    FractionHe = 0,
                            //    FractionO2 = 0,
                            //    DiveType = DTO.Types.DiveType.CCR,
                            //    MaxDepth = Convert.ToDecimal(reader["maxDepth"]),
                            //    SampleRate = 0
                            //};

                            dives.Add(dive);                            
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
