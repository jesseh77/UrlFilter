using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UrlFilter.ExpressionProcessors;

namespace UrlFilter
{
    public class ExpressionProcessor
    {
        private readonly Dictionary<string, IProcessExpression> _processors;
        //private readonly IProcessExpression propertyProcessor = new PropertyProcessor(Expression.Property);
        private readonly IProcessExpression constantProcessor = new ConstantProcessor(Expression.Constant);

        public ExpressionProcessor()
        {
            _processors = GetExpressionProcessors();
        }

        public IEnumerable<ExpressionSegment> Process(string queryString, ExpressionSegment currentSegment, ParameterExpression parameterExpression, List<ExpressionSegment> segments)
        {
            var activeQuerySegments = SplitQueryTextSegments(queryString, currentSegment.StartIndex, currentSegment.EndIndex).ToList();

            var propertyAndConstantSegments = activeQuerySegments
                .Where(x => !segments.Any(y => y.ContainsEntirely(x)) && !_processors.ContainsKey(x.SegmentText(queryString)))
                .ToList();

            foreach (var segment in propertyAndConstantSegments)
            {
                //propertyProcessor.Process(segment, parameterExpression);
            }

            return null;
        }
        public Dictionary<string, IProcessExpression> GetExpressionProcessors()
        {
            return new Dictionary<string, IProcessExpression>
            {
                {"gt", new ValueProcessor("gt", Expression.GreaterThan) },
                {"ge", new ValueProcessor("ge", Expression.GreaterThanOrEqual) },
                {"lt", new ValueProcessor("lt", Expression.LessThan) },
                {"le", new ValueProcessor("le", Expression.LessThanOrEqual) },
                {"eq", new ValueProcessor("eq", Expression.Equal) },
                {"ne", new ValueProcessor("ne", Expression.NotEqual) },
                {"not", new UnaryProcessor("not", Expression.Not) },
                {"and", new LogicalProcessor("and", Expression.AndAlso) },
                {"or", new LogicalProcessor("or", Expression.OrElse) }
            };
        }

        public IEnumerable<ExpressionSegment> SplitQueryTextSegments(string queryString, int start, int end)
        {
            const char spaceChar = ' ';
            const char singleQuote = '\'';

            var query = Uri.UnescapeDataString(queryString);
            int segmentStart = start;
            bool isSegmentQuoted = false;

            for (int i = segmentStart; i <= end; i++)
            {
                if (i == end ||
                    !isSegmentQuoted && query[i].Equals(spaceChar) ||
                    isSegmentQuoted && query[i].Equals(singleQuote))
                {
                    isSegmentQuoted = false;                    
                    yield return new ExpressionSegment { StartIndex = segmentStart, EndIndex = i - 1 };
                    segmentStart = i + 1;
                }
            }
        }
    }
}
