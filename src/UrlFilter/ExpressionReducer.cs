using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UrlFilter
{
    public class ExpressionReducer
    {
        public Expression ProcessQueryText<T>(string queryString, ParameterExpression parameterExpression)
        {
            var segmentExpression = ProcessSegments(queryString, parameterExpression, Enumerable.Empty<ExpressionSegment>());
            return segmentExpression.Single().Expression;
        }

        public IEnumerable<ExpressionSegment> ProcessSegments(string queryString, ParameterExpression parameterExpression, IEnumerable<ExpressionSegment> segments)
        {
            return null;
        }

        public ExpressionSegment GetNextSegment(string queryString, IEnumerable<ExpressionSegment> segments)
        {
            return null;
        }
    }
}
