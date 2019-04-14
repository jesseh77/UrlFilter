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
        public ExpressionProcessor()
        {
            
        }

        public IEnumerable<ExpressionSegment> Process(string queryString, ExpressionSegment currentSegment, ParameterExpression parameterExpression, List<ExpressionSegment> segments)
        {
            return null;
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
