using System.Linq;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Module.Checklist.Models;
using Sitecore.Rules.Conditions;
using Sitecore.StringExtensions;

namespace Sitecore.Module.Checklist.Rules.Conditions
{
    public class DoesFieldContainsValueCondition<T> : StringOperatorCondition<T> where T : ChecklistParameterRuleContext
    {
        public string FieldName { get; set; }

        public string FieldValue { get; set; }

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, nameof(ruleContext));

            var itemName = ruleContext.ItemName;
            var startPath = ruleContext.StartPath;
            var database = ruleContext.DatabaseName;
            var templateName = ruleContext.TemplateName;
            var isChildrenOnly = ruleContext.IsChildrenOnly;

            if (isChildrenOnly.ToLower().Equals("true"))
            {
                var childItems = Factory.GetDatabase(database).GetItem(startPath).GetChildren().ToList();

                var item = !templateName.IsNullOrEmpty() ? childItems.Find(f => f.Name.ToLower().Equals(itemName.ToLower()) && f.TemplateName.Equals(templateName)) : childItems.Find(f => f.Name.ToLower().Equals(itemName.ToLower()));

                if (item != null)
                {
                    if (item.Fields[this.FieldName] == null)
                    {
                        ruleContext.Diagnostic.Add(SetDiagnosticResponse("FAILED", $"Field {this.FieldName} cannot be found", $"Does field {this.FieldName} has value {this.FieldValue}?"));
                    }

                    var fieldValue = item.Fields[this.FieldName].Value;

                    if (!fieldValue.IsNullOrEmpty() && fieldValue.Equals(this.FieldValue))
                    {
                        ruleContext.Diagnostic.Add(SetDiagnosticResponse("SUCCESS", $"Field {this.FieldName} contains value {this.FieldValue}", $"Does field {this.FieldName} has value {this.FieldValue}?"));

                        return true;
                    }

                    ruleContext.Diagnostic.Add(SetDiagnosticResponse("FAILED", $"Field {this.FieldName} does not contain value {this.FieldValue}\n\rCurrent value: {fieldValue}", $"Does field {this.FieldName} has value {this.FieldValue}?"));

                    return false;
                }
            }

            var descendantItems = Factory.GetDatabase(database).GetItem(startPath).Axes.GetDescendants().ToList();

            var descendantItem = !templateName.IsNullOrEmpty() ? descendantItems.Find(f => f.Name.ToLower().Equals(itemName.ToLower()) && f.TemplateName.Equals(templateName)) : descendantItems.Find(f => f.Name.ToLower().Equals(itemName.ToLower()));

            if (descendantItem != null)
            {
                if (descendantItem.Fields[this.FieldName] == null)
                {
                    ruleContext.Diagnostic.Add(SetDiagnosticResponse("FAILED", $"Field {this.FieldName} cannot be found", $"Does field {this.FieldName} has value {this.FieldValue}?"));
                }

                var fieldValue = descendantItem.Fields[this.FieldName].Value;

                if (!fieldValue.IsNullOrEmpty() && fieldValue.Equals(this.FieldValue))
                {
                    ruleContext.Diagnostic.Add(SetDiagnosticResponse("SUCCESS", $"Field {this.FieldName} contains value {this.FieldValue}", $"Does field {this.FieldName} has value {this.FieldValue}?"));

                    return true;
                }

                ruleContext.Diagnostic.Add(SetDiagnosticResponse("FAILED", $"Field {this.FieldName} does not contain value {this.FieldValue}\n\rCurrent value: {fieldValue}", $"Does field {this.FieldName} has value {this.FieldValue}?"));

                return false;
            }

            ruleContext.Diagnostic.Add(SetDiagnosticResponse("FAILED", $"Item {itemName} cannot be found", $"Does field {this.FieldName} has value {this.FieldValue}?"));

            return false;
        }

        private static Diagnostic SetDiagnosticResponse(string status, string message, string conditionName)
        {
            return new Diagnostic
            {
                Status = status,
                Message = message,
                ConditionName = conditionName
            };
        }
    }
}
