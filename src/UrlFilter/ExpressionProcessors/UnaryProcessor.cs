using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public class UnaryProcessor : BaseBinaryProcessor, IUnaryProcessor
    {
        public override Dictionary<string, ExpressionType> AcceptedExpressionTypes()
        {
            return new Dictionary<string, ExpressionType>
            {
                {"not", ExpressionType.Not}
            };
        }

        public Expression Process(string comparisonType, Expression expression)
        {
            return base.Process(comparisonType, expression, Expression.Empty());
        }
    }
}
