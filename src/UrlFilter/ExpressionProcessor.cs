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
        private readonly List<IProcessExpression> _processors;

        public ExpressionProcessor()
        {
            _processors = GetExpressionProcessors();
        }

        public IEnumerable<ExpressionSegment> Process(string queryString, ExpressionSegment currentSegment, ParameterExpression parameterExpression, List<ExpressionSegment> segments)
        {
            var section = SplitQueryTextSegments(queryString, currentSegment.StartIndex, currentSegment.EndIndex).ToList();

            foreach (var processor in _processors)
            {
                foreach (var item in section)
                {

                    if(processor.CanProcess()
                }
            }
        }
        public List<IProcessExpression> GetExpressionProcessors()
        {
            return new List<IProcessExpression>
            {                
                new ValueProcessor("gt", Expression.GreaterThan),
                new ValueProcessor("ge", Expression.GreaterThanOrEqual),
                new ValueProcessor("lt", Expression.LessThan),
                new ValueProcessor("le", Expression.LessThanOrEqual),
                new ValueProcessor("eq", Expression.Equal),
                new ValueProcessor("ne", Expression.NotEqual),
                new UnaryProcessor("not", Expression.Not),
                new LogicalProcessor("and", Expression.AndAlso),
                new LogicalProcessor("or", Expression.OrElse),
                new PropertyProcessor(Expression.Property),
                new ConstantProcessor(Expression.Constant)
            };
        }

        public IEnumerable<ExpressionSegment> SplitQueryTextSegments(string queryString, int start, int end)
        {
            const char spaceChar = ' ';
            const char singleQuote = '\'';

            var query = Uri.UnescapeDataString(queryString);
            int segmentStart = start;
            bool isSegmentQuoted = false;

            for (int i = segmentStart; i < end; i++)
            {
                if ((i == query.Length - 1) ||
                    !isSegmentQuoted && query[i].Equals(spaceChar) ||
                    isSegmentQuoted && query[i].Equals(singleQuote))
                {
                    isSegmentQuoted = false;
                    segmentStart = i + 1;
                    yield return new ExpressionSegment { StartIndex = segmentStart, EndIndex = i - 1 };
                }
            }
        }

        public static string SegmentText(ExpressionSegment segment, string queryString)
        {
            return queryString.Substring(segment.StartIndex, segment.Length);
        }
    }
}
