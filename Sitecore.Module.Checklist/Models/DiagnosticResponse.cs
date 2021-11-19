using System.Collections.Generic;

namespace Sitecore.Module.Checklist.Models
{
    public class DiagnosticResponse
    {
        public string RuleName { get; set; }

        public List<Diagnostic> Diagnostics { get; set; }

        public string OverallStatus { get; set; }
    }
}
