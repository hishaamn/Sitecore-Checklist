using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Module.Checklist.Models;
using Sitecore.Module.Checklist.Runtime;
using Sitecore.Resources;
using Sitecore.Shell.Applications.Rules;
using Sitecore.Sites;
using Sitecore.StringExtensions;
using Sitecore.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace Sitecore.Module.Checklist.Controllers
{
    public class ChecklistController : Controller
    {
        [HttpGet]
        public ActionResult GetAllChecklist()
        {
            var checklistItems = Factory.GetDatabase("master").GetItem("/sitecore/system/Modules/Checklist/Scripts").GetChildren().OrderByDescending(o => o["__Updated"]).ToList();

            var responseList = new List<ChecklistModel>();
            var checkListReports = new List<ChecklistReport>();

            var mostRecentCount = 0;

            foreach (var checklistItem in checklistItems)
            {
                var icon = checklistItem.Appearance.Icon;

                if (icon.StartsWith("~"))
                {
                    icon = StringUtil.EnsurePrefix('/', icon);
                }

                else if (!(icon.StartsWith("/") && icon.Contains(":")))
                {
                    icon = Images.GetThemedImageSource(icon);
                }

                var executionDate = DateUtil.IsoDateToDateTime(checklistItem.Fields["Last Execution Date"].Value);

                var script = new ChecklistModel
                {
                    ItemName = checklistItem.Name,
                    ItemId = checklistItem.ID.ToString(),
                    ScriptName = checklistItem.Name,
                    LastRun = executionDate.Year != 1 ? executionDate.ToString("dd MMMM yyyy") : "NOT YET RUN",
                    ItemImageUrl = icon
                };

                responseList.Add(script);

                if (mostRecentCount < 5)
                {
                    var reports = Factory.GetDatabase("master")
                        .GetItem($"/sitecore/system/Modules/Checklist/Reports/{checklistItem.Name}")?.GetChildren()
                        .OrderByDescending(o => o["__Updated"]).First();

                    if (reports != null)
                    {
                        var checklistReport = new ChecklistReport
                        {
                            ChecklistReportImageUrl = icon,
                            ChecklistReportName = checklistItem.Name,
                            ChecklistReportId = reports.ID.ToString(),
                            OverallStatus = reports.Fields["Overall Status"].Value,
                            ExecutionDate = DateUtil.IsoDateToDateTime(reports.Fields["Last Execution Date"].Value).ToString("dd MMMM yyyy")
                        };

                        checkListReports.Add(checklistReport);
                    }
                }

                mostRecentCount++;
            }

            var scriptResponse = new ChecklistResponseModel
            {
                ScriptModel = responseList,
                ChecklistReports = checkListReports
            };

            return new JsonResult { Data = scriptResponse, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult GetChecklistDetail(string checklistId)
        {
            var checklistItem = Factory.GetDatabase("master").GetItem(new ID(checklistId));

            var executionDate = DateUtil.IsoDateToDateTime(checklistItem.Fields["Last Execution Date"].Value);

            var checklistDetail = new ChecklistDetail
            {
                ItemId = checklistId,
                ItemName = checklistItem.Name,
                ItemPath = checklistItem.Paths.FullPath,
                LastExecution = executionDate.Year != 1 ? executionDate.ToString("dd MMMM yyyy") : "NOT YET RUN"
            };

            var latestReport = JsonConvert.DeserializeObject<List<DiagnosticResponse>>(checklistItem.Fields["Overall Status"].Value);

            if (latestReport == null)
            {
                checklistDetail.OverallStatus = "NOT YET RUN";
            }
            else
            {
                checklistDetail.OverallStatus = latestReport.Any(a => a.OverallStatus.ToLower().Equals("failed")) ? "FAILED" : "SUCCESS";
            }

            var reports = Factory.GetDatabase("master")
                .GetItem($"/sitecore/system/Modules/Checklist/Reports/{checklistItem.Name}")?.GetChildren()
                .OrderByDescending(o => o["__Updated"]).ToList();

            var checkListReports = new List<ChecklistReport>();

            if (reports != null && reports.Any())
            {
                foreach (var report in reports)
                {
                    var checklistReport = new ChecklistReport
                    {
                        ChecklistReportName = checklistItem.Name,
                        ChecklistReportId = report.ID.ToString(),
                        OverallStatus = report.Fields["Overall Status"].Value,
                        ExecutionDate = DateUtil.IsoDateToDateTime(report.Fields["Last Execution Date"].Value).ToString("dd MMMM yyyy")
                    };

                    checkListReports.Add(checklistReport);
                }
            }

            var output = new HtmlTextWriter(new StringWriter());

            Context.ContentDatabase = Factory.GetDatabase("master");

            new RulesRenderer(checklistItem["Rule"])
            {
                IsEditable = false

            }.Render(output);

            var response = new ChecklistDetailResponse
            {
                ChecklistDetail = checklistDetail,
                ChecklistReports = checkListReports,
                RulePanel = output.InnerWriter.ToString()
            };

            return new JsonResult { Data = response, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult CreateChecklist()
        {
            var shellSite = SiteContext.GetSite("shell");

            using (new SiteContextSwitcher(shellSite))
            {
                var urlString = new UrlString(UIUtil.GetUri("control:PowerShellRunner"));
                urlString.Append("db", "master");
                urlString.Append("scriptId", "{0DDA7155-B2EE-4E0A-AF0B-E354FAB9CCF4}");
                urlString.Append("scriptDb", "master");

                return new JsonResult { Data = urlString.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpGet]
        public ActionResult EditChecklist(string checklistId)
        {
            var shellSite = SiteContext.GetSite("shell");

            using (new SiteContextSwitcher(shellSite))
            {
                var urlString = new UrlString(UIUtil.GetUri("control:PowerShellRunner"));
                urlString.Append("id", checklistId);
                urlString.Append("db", "master");
                urlString.Append("scriptId", "{08487CD2-AA9C-448A-9182-A149D0BFA16D}");
                urlString.Append("scriptDb", "master");

                return new JsonResult { Data = urlString.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpGet]
        public ActionResult DeleteChecklist(string checklistId)
        {
            var shellSite = SiteContext.GetSite("shell");

            using (new SiteContextSwitcher(shellSite))
            {
                var urlString = new UrlString(UIUtil.GetUri("control:PowerShellRunner"));
                urlString.Append("id", checklistId);
                urlString.Append("db", "master");
                urlString.Append("scriptId", "{AAFCBA63-74CC-4DB1-901A-441BE1048487}");
                urlString.Append("scriptDb", "master");

                return new JsonResult { Data = urlString.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpGet]
        public ActionResult GetReportDetail(string reportId)
        {
            var reportItem = Factory.GetDatabase("master").GetItem(new ID(reportId));

            var reportJson = reportItem.Fields["Report"].Value;

            if (!reportJson.IsNullOrEmpty())
            {
                var reports = JsonConvert.DeserializeObject<List<DiagnosticResponse>>(reportJson);

                var output = new HtmlTextWriter(new StringWriter());

                foreach (var diagnosticResponse in reports)
                {
                    foreach (var diagnosticDetail in diagnosticResponse.Diagnostics)
                    {
                        output.Write("<div>");
                        output.Write($"<div><b>Condition:</b> {diagnosticDetail.ConditionName}</div>");
                        output.Write($"<div><b>Message:</b> {diagnosticDetail.Message}</div>");
                        output.Write($"<div><b>Status:</b> {diagnosticDetail.Status}</div>");
                        output.Write("</div>");
                        output.Write("<hr>");
                        output.Write("</b>");
                    }
                }

                return new JsonResult { Data = output.InnerWriter.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult { Data = "ERROR", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public ActionResult TriggerChecklist(string checklistId)
        {
            var executor = new ChecklistExecutor();

            executor.Execute(checklistId);

            return new JsonResult { Data = checklistId, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
