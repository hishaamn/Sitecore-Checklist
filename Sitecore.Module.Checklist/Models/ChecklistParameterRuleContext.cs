using System.Collections.Generic;
using Sitecore.Rules;

namespace Sitecore.Module.Checklist.Models
{
    public class ChecklistParameterRuleContext : RuleContext
    {
        public string ItemName { get; set; }

        public string StartPath { get; set; }

        public string TemplateName { get; set; }

        public string DatabaseName { get; set; }

        public string IsChildrenOnly { get; set; }

        public List<Diagnostic> Diagnostic { get; set; }
    }

    public class Diagnostic
    {
        public string ConditionName { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }
    }
}
