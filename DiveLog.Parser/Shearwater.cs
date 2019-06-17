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
            return dives;
        }

        private List<LogEntryDTO> ExtractDives(SQLiteConnection sqliteConnection)
        {
            sqliteConnection.Open();
            string sql = "SELECT * FROM dive_logs";
            using (var command = new SQLiteCommand(sql, sqliteConnection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Id: {reader["id"]}, Dive#:{reader["number"]}, MaxDepth: {reader["maxDepth"].ToString()}");
                    }
                }
            }

            return null;
        }

        private SQLiteConnection CreateSqliteConnection(string path)
        {
            return new SQLiteConnection($"Data Source={path}");
        }

        private async Task<string> AddDataToStorage(IFormFile data)
        {
            var uniqueId = $"{Guid.NewGuid().ToString()}.db";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uniqueId);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await data.CopyToAsync(stream);
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
