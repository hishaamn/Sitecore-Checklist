using Sitecore.Module.Checklist.Models;
using Sitecore.Rules.Actions;

namespace Sitecore.Module.Checklist.Rules.Setters
{
    public class SetStartPath<T> : RuleAction<T> where T : ChecklistParameterRuleContext
    {
        public string Value { get; set; }

        public override void Apply(T ruleContext)
        {
            ruleContext.StartPath = this.Value;
        }
    }
}
