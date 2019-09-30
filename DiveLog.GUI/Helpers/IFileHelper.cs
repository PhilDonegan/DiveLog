using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.GUI.Helpers
{
	public interface IFileHelper
	{
		Task<string> AddDataToAzureStorage(object data);
		Task<string> AddDataToStorage(Microsoft.AspNetCore.Http.IFormFile data);
		void DeleteUpload(string path);
	}
}
