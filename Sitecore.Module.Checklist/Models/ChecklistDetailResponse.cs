using System.Collections.Generic;

namespace Sitecore.Module.Checklist.Models
{
    public class ChecklistDetailResponse
    {
        public ChecklistDetail ChecklistDetail { get; set; }

        public List<ChecklistReport> ChecklistReports { get; set; }

        public string RulePanel { get; set; }
    }
}
