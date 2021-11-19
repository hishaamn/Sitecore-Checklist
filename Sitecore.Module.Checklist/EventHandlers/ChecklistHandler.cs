using System;
using System.Linq;
using Hangfire;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Events;
using Sitecore.Module.Checklist.Runtime;

namespace Sitecore.Module.Checklist.EventHandlers
{
    public class ChecklistHandler
    {
        public void ExecuteChecklist(object sender, EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            if (eventArgs == null)
            {
                return;
            }

            var eventName = eventArgs.EventName;

            var masterDb = Factory.GetDatabase("master");

            var eventItems = masterDb.GetItem("/sitecore/system/Modules/Checklist/Events").Children;

            if (eventItems != null && eventItems.Any())
            {
                var respectiveEvents = eventItems.Where(w => w.Fields["Event Name"].Value.ToLower().Equals(eventName.ToLower()));

                foreach (var respectiveEvent in respectiveEvents)
                {
                    var multilistChecklist = (MultilistField)respectiveEvent.Fields["Selected Checklist"];

                    var checklists = multilistChecklist.GetItems();

                    if (checklists.Any())
                    {
                        foreach (var checklist in checklists)
                        {
                            var nameValueListString = checklist["Conditions"];

                            //Converts the string to NameValueCollection
                            var nameValueList = Web.WebUtil.ParseUrlParameters(nameValueListString);

                            //Apply logic
                            foreach (var nameValue in nameValueList)
                            {
                                // Do Stuff
                            }
                            var checklistExecutor = new ChecklistExecutor();

                            BackgroundJob.Enqueue(() => checklistExecutor.Execute(checklist.ToString()));
                        }
                    }
                }
            }
        }
    }
}