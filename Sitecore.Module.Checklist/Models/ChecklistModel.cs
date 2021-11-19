using System.Collections.Generic;

namespace Sitecore.Module.Checklist.Models
{
    public class ChecklistModel
    {
        public string ItemName { get; set; }

        public string ScriptName { get; set; }

        public string ItemId { get; set; }

        public string LastRun { get; set; }

        public string ItemImageUrl { get; set; }

        public string OverallStatus { get; set; }
    }

    public class ChecklistResponseModel
    {
        public List<ChecklistModel> ScriptModel { get; set; }

        public List<ChecklistReport> ChecklistReports { get; set; }
    }
}
