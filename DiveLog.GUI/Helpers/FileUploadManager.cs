using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.GUI.Helpers
{
	public class FileUploadManager : IFileUploadManager
	{
		private Dictionary<string, string> _uploads;

		public FileUploadManager()
		{
			_uploads = new Dictionary<string, string>();
		}

		public void Add(string id, string path)
		{
			_uploads.Add(id, path);
		}

		public string Get(string id)
		{
			if (!_uploads.ContainsKey(id))
			{
				throw new KeyNotFoundException("Key not found in upload dictionary.");
			}

			return _uploads[id];
		}

		public void Delete(string id)
		{
			if (!_uploads.ContainsKey(id))
			{
				throw new KeyNotFoundException("Key not found in upload dictionary.");
			}

			_uploads.Remove(id);
		}
	}
}
