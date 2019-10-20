using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace DiveLog.API.HangfireExtensions
{
	public class DivelogHangfireAuthorizationFilter : IDashboardAuthorizationFilter
	{
		public bool Authorize([NotNull] DashboardContext context)
		{
			var httpContext = context.GetHttpContext();
			return httpContext.Request.Host.Host.Contains("localhost");
		}
	}
}
