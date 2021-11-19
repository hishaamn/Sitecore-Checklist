using System;
using System.Collections.Generic;
using Hangfire;
using Hangfire.SqlServer;
using Sitecore.Configuration;
using Sitecore.Owin.Pipelines.Initialize;

namespace Sitecore.Module.Checklist.Hangfire
{
    public class Initializer : InitializeProcessor
    {
        public override void Process(InitializeArgs args)
        {
            var app = args.App;

            app.UseHangfireAspNet(GetHangfireServers);

            var hangfireDashboardUrl = Settings.GetSetting("HangfireDashboardUrl");

            app.UseHangfireDashboard(hangfireDashboardUrl, new DashboardOptions
            {
                Authorization = new[]
                {
                    new AuthorizationDashboard()
                }
            });
        }

        private IEnumerable<IDisposable> GetHangfireServers()
        {
            var dbConnectionName = Settings.GetSetting("ChecklistDbConnectionName");

            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(dbConnectionName, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                });

            yield return new BackgroundJobServer();
        }
    }
}
