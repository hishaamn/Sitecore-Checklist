using System.Linq;
using Sitecore.Configuration;
using Sitecore.Module.Checklist.Models;
using Sitecore.Rules.Conditions;
using Sitecore.StringExtensions;

namespace Sitecore.Module.Checklist.Rules.Conditions
{
    public class IsItemExistCondition<T> : StringOperatorCondition<T> where T : ChecklistParameterRuleContext
    {
        protected override bool Execute(T ruleContext)
        {
            var itemName = ruleContext.ItemName;
            var startPath = ruleContext.StartPath;
            var database = ruleContext.DatabaseName;
            var templateName = ruleContext.TemplateName;
            var isChildrenOnly = ruleContext.IsChildrenOnly;

            bool isItemPresent;

            if (isChildrenOnly.ToLower().Equals("true"))
            {
                var childItems = Factory.GetDatabase(database).GetItem(startPath).GetChildren().ToList();

                isItemPresent = !templateName.IsNullOrEmpty() ? childItems.Exists(e => e.Name.ToLower().Equals(itemName.ToLower()) && e.TemplateName.Equals(templateName)) : childItems.Exists(e => e.Name.ToLower().Equals(itemName.ToLower()));
                
                if (isItemPresent)
                {
                    ruleContext.Diagnostic.Add(new Diagnostic
                    {
                        Status = "SUCCESS",
                        Message = $"Item {itemName} exists",
                        ConditionName = "Is item exists?"
                    });

                    return true;
                }

                ruleContext.Diagnostic.Add(new Diagnostic
                {
                    Status = "FAILED",
                    Message = $"Item {itemName} does not exist",
                    ConditionName = "Is item exists?"
                });

                return false;
            }

            var descendantItems = Factory.GetDatabase(database).GetItem(startPath).Axes.GetDescendants().ToList();

            isItemPresent = !templateName.IsNullOrEmpty() ? descendantItems.Exists(e => e.Name.ToLower().Equals(itemName.ToLower()) && e.TemplateName.Equals(templateName)) : descendantItems.Exists(e => e.Name.ToLower().Equals(itemName.ToLower()));

            if (isItemPresent)
            {
                ruleContext.Diagnostic.Add(new Diagnostic
                {
                    Status = "SUCCESS",
                    Message = $"Item {itemName} exists",
                    ConditionName = "Is item exists?"
                });

                return true;
            }

            ruleContext.Diagnostic.Add(new Diagnostic
            {
                Status = "FAILED",
                Message = $"Item {itemName} does not exist",
                ConditionName = "Is item exists?"
            });

            return false;
        }
    }
}
