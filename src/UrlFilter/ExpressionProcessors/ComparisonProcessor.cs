using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public class ComparisonProcessor : BaseBinaryProcessor, IComparisonProcessor
    {
        public override Dictionary<string, ExpressionType> AcceptedExpressionTypes()
        {
            return new Dictionary<string, ExpressionType>
            {
                {"eq", ExpressionType.Equal},
                {"gt", ExpressionType.GreaterThan},
                {"ge", ExpressionType.GreaterThanOrEqual},
                {"lt", ExpressionType.LessThan},
                {"le", ExpressionType.LessThanOrEqual},
                {"ne", ExpressionType.NotEqual}
            };
        }
    }
}
