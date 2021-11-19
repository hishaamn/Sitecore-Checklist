using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Pipelines;

namespace Sitecore.Module.Checklist.Routings
{
    public class RouteRegistration
    {
        public void Process(PipelineArgs args)
        {
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("GetReportDetail", "api/checklist/getreportdetail", new
            {
                action = "GetReportDetail",
                controller = "Checklist"
            });

            routes.MapRoute("DeleteChecklist", "api/checklist/deletechecklist", new
            {
                action = "DeleteChecklist",
                controller = "Checklist"
            });

            routes.MapRoute("EditChecklist", "api/checklist/editchecklist", new
            {
                action = "EditChecklist",
                controller = "Checklist"
            });

            routes.MapRoute("ChecklistDetail", "api/checklist/getchecklistdetail", new
            {
                action = "GetChecklistDetail",
                controller = "Checklist",
            });

            routes.MapRoute("AllChecklist", "api/checklist/getallchecklist", new
            {
                action = "GetAllChecklist",
                controller = "Checklist"
            });

            routes.MapRoute("TriggerChecklist", "api/checklist/triggerchecklist", new
            {
                action = "TriggerChecklist",
                controller = "Checklist"
            });

            routes.MapRoute("CreateChecklist", "api/checklist/createchecklist", new
            {
                action = "CreateChecklist",
                controller = "Checklist",
            });
        }
    }
}