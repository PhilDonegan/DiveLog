using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiveLog.GUI.Helpers
{
	public interface IFileUploadManager
	{
		void Add(string id, string path);
		void Delete(string id);
		string Get(string id);
	}
}
