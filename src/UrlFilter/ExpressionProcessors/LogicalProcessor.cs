using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    public class LogicalProcessor : BaseBinaryProcessor, ILogicalProcessor
    {
        public override Dictionary<string, ExpressionType> AcceptedExpressionTypes()
        {
            return new Dictionary<string, ExpressionType>
            {
                {"not", ExpressionType.Not},
                {"and", ExpressionType.AndAlso},
                {"or", ExpressionType.OrElse}
            };
        }
    }
}
