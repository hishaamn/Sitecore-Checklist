using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.Module.Checklist.Models;
using Sitecore.Rules;
using Sitecore.SecurityModel;

namespace Sitecore.Module.Checklist.Runtime
{
    public class ChecklistExecutor
    {
        public string Execute(string ruleId)
        {
            var database = Factory.GetDatabase("master");

            var ruleItem = database.GetItem(new ID(ruleId), Language.Parse("en"));

            var ruleValue = ruleItem.Fields["Rule"].Value;

            var rules = RuleFactory.ParseRules<ChecklistParameterRuleContext>(database, XElement.Parse(ruleValue));

            if (!rules.Rules.Any())
            {
                return "Rules empty";
            }

            var allRules = rules.Rules;

            var diagnosticRuleContext = new ChecklistParameterRuleContext
            {
                Diagnostic = new List<Diagnostic>()
            };

            var responses = new List<DiagnosticResponse>();

            foreach (var rule in allRules)
            {
                if (rule.Name.ToLower().Contains("setters") && rule.Actions != null)
                {
                    foreach (var ruleAction in rule.Actions)
                    {
                        ruleAction.Apply(diagnosticRuleContext);
                    }
                }

                if (rule.Condition == null)
                {
                    continue;
                }

                rule.Evaluate(diagnosticRuleContext);

                var status = diagnosticRuleContext.Diagnostic.Any(a => a.Status.ToLower().Equals("failed")) ? "FAILED" : "SUCCESS";
                
                var responseModel = new DiagnosticResponse
                {
                    RuleName = rule.Name,
                    Diagnostics = diagnosticRuleContext.Diagnostic,
                    OverallStatus = status
                };

                responses.Add(responseModel);

                diagnosticRuleContext.Diagnostic = new List<Diagnostic>();
            }

            var response = JsonConvert.SerializeObject(responses);

            var currentDateTime = DateTime.Now;

            var executionDateTime = DateUtil.ToIsoDate(currentDateTime);

            using (new SecurityDisabler())
            {
                var status = responses.Any(a => a.OverallStatus.ToLower().Equals("failed")) ? "FAILED" : "SUCCESS";

                ruleItem.Editing.BeginEdit();
                ruleItem.Fields["Last Execution Date"].Value = executionDateTime;
                ruleItem.Fields["Overall Status"].Value = response;
                ruleItem.Editing.EndEdit(true);

                var reportContainerItem = Factory.GetDatabase("master").GetItem("/sitecore/system/Modules/Checklist/Reports");

                var reportItem = reportContainerItem.Children.FirstOrDefault(w => w.Name.Equals(ruleItem.Name));

                var reportName = currentDateTime.ToString("yyyyMMddHHmm");

                if (reportItem != null)
                {
                    var reportTemplate = Factory.GetDatabase("master").GetTemplate(new ID("{F20F412F-5F69-44B5-8120-4D3DBD6DB2A8}"));

                    var newReportItem = reportItem.Children.FirstOrDefault(f => f.Name.Equals(reportName));

                    if (newReportItem != null)
                    {
                        newReportItem.Editing.BeginEdit();
                        newReportItem.Fields["Last Execution Date"].Value = executionDateTime;
                        newReportItem.Fields["Report"].Value = response;
                        newReportItem.Fields["Overall Status"].Value = status;
                        newReportItem.Editing.EndEdit();
                    }
                    else
                    {
                        newReportItem = reportItem.Add(reportName, reportTemplate);//DATETIME NAME

                        if (newReportItem != null)
                        {
                            newReportItem.Editing.BeginEdit();
                            newReportItem.Fields["Last Execution Date"].Value = executionDateTime;
                            newReportItem.Fields["Report"].Value = response;
                            newReportItem.Fields["Overall Status"].Value = status;
                            newReportItem.Editing.EndEdit();
                        }
                    }
                }
                else
                {
                    var reportScriptContainer = Factory.GetDatabase("master").GetTemplate(new ID("{53102742-476D-47AB-8D09-42DAC8A16AFE}"));

                    var newReportScriptContainerItem = reportContainerItem.Add(ruleItem.Name, reportScriptContainer);

                    if (newReportScriptContainerItem != null)
                    {
                        var reportTemplate = Factory.GetDatabase("master").GetTemplate(new ID("{F20F412F-5F69-44B5-8120-4D3DBD6DB2A8}"));
                        
                        var newReportItem = newReportScriptContainerItem.Add(reportName, reportTemplate);

                        if (newReportItem != null)
                        {
                            newReportItem.Editing.BeginEdit();
                            newReportItem.Fields["Last Execution Date"].Value = executionDateTime;
                            newReportItem.Fields["Report"].Value = response;
                            newReportItem.Fields["Overall Status"].Value = response;
                            newReportItem.Editing.EndEdit();
                        }
                    }
                }
            }

            return response;
        }
    }
}
