using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace DiveLog.GUI.Helpers
{
    public class FileHelper : IFileHelper
    {
		private IConfiguration _builder;
		public FileHelper(IConfiguration builder)
		{
			_builder = builder ?? throw new ArgumentNullException(nameof(builder));
		}

		public async Task<string> AddDataToStorage(IFormFile data)
		{
			var uniqueId = $"{Guid.NewGuid().ToString()}.db";

			if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")))
			{
				Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"));
			}

			var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", uniqueId);
			var storeUpload = bool.TryParse(_builder.GetSection("StoreUploads").Value, out _);

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

		public async Task<string> AddDataToAzureStorage(object data)
		{
			var uniqueId = $"{Guid.NewGuid().ToString()}.db";

			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_builder.GetConnectionString("DiveLogShearwaterAzureBlobStorage"));
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

		public void DeleteUpload(string path)
		{
			try
			{
				File.Delete(path);
			}
			catch (IOException ex)
			{
				System.Diagnostics.Debug.Fail(ex.Message, ex.GetBaseException().Message);
			}
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
