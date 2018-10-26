using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using UrlFilter.ExpressionProcessors;

namespace UrlFilter
{
    public class ExpressionProcessor
    {
        private readonly Dictionary<string, IProcessExpression> _processors;

        public ExpressionProcessor()
        {
            _processors = GetExpressionProcessors();
        }

        public IEnumerable<ExpressionSegment> Process(string queryString, ExpressionSegment currentSegment, ParameterExpression parameterExpression, IEnumerable<ExpressionSegment> segments)
        {

        }
        public Dictionary<string, IProcessExpression> GetExpressionProcessors()
        {
            return new Dictionary<string, IProcessExpression>
            {
                {"not", new UnaryProcessor("not", Expression.Not) },
                {"gt", new ValueProcessor("gt", Expression.GreaterThan) },
                {"ge", new ValueProcessor("ge", Expression.GreaterThanOrEqual) },
                {"lt", new ValueProcessor("lt", Expression.LessThan) },
                {"le", new ValueProcessor("le", Expression.LessThanOrEqual) },
                {"eq", new ValueProcessor("eq", Expression.Equal) },
                {"ne", new ValueProcessor("ne", Expression.NotEqual) },
                {"and", new LogicalProcessor("and", Expression.AndAlso) },
                {"or", new LogicalProcessor("or", Expression.OrElse) }
            };
        }
    }
}
