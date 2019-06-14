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

namespace DiveLog.Parsers
{
    public class Shearwater : IParser
    {
        public List<LogEntryDTO> ProcessDives(object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var connectionString = AddDataToAzureStorage(data);
            return null;
        }

        private string AddDataToAzureStorage(object data)
        {
            var uniqueId = Guid.NewGuid().ToString();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(builder.GetConnectionString("DiveLogShearwaterAzureBlobStorage"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("shearwater-blob-container");
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(uniqueId);
            blob.UploadFromByteArrayAsync(ObjectToByteArray(data), 0, 0);

            return null;
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
