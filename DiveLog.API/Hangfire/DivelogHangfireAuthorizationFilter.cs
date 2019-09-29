using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace DiveLog.API.Hangfire
{
	public class DivelogHangfireAuthorizationFilter : IDashboardAuthorizationFilter
	{
		public bool Authorize([NotNull] DashboardContext context)
		{
			var httpContext = context.GetHttpContext();
			return true;
		}
	}
}
