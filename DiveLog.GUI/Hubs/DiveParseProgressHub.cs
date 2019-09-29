using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DiveLog.GUI.Hubs
{
	public class DiveParseProgressHub : Hub
	{
		public async Task AssociateJob(string jobId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
		}
	}
}
