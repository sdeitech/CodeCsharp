using System;
using App.Common.Enums;
using App.Application.Constant.DynamicQuestionnaire;

namespace App.Services.Test
{
    /// <summary>
    /// Simple test to validate our matrix rule implementation compiles correctly
    /// </summary>
    public class SimpleMatrixTest
    {
        public void TestMatrixConditions()
        {
            // Test matrix condition types are available
            var conditions = new[]
            {
                RuleConstants.RowHasSelection,
                RuleConstants.RowHasColumn,
                RuleConstants.ColumnSelected,
                RuleConstants.ScoreGreaterThan,
                RuleConstants.ScoreLessThan,
                RuleConstants.ScoreEqualTo,
                RuleConstants.ScoreInRange
            };

            Console.WriteLine($"Matrix conditions available: {conditions.Length}");
            
            foreach (var condition in conditions)
            {
                Console.WriteLine($"- {condition}");
            }
        }
    }
}
