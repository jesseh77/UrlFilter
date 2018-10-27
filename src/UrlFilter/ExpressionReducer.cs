using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UrlFilter
{
    public class ExpressionReducer
    {
        private ExpressionProcessor expressionProcessor = new ExpressionProcessor();
        public Expression ProcessQueryText<T>(string queryString, ParameterExpression parameterExpression)
        {
            var segmentExpression = ProcessSegments(queryString, parameterExpression, Enumerable.Empty<ExpressionSegment>());
            return segmentExpression.Single().Expression;
        }

        public IEnumerable<ExpressionSegment> ProcessSegments(string queryString, ParameterExpression parameterExpression, IEnumerable<ExpressionSegment> segments)
        {
            var segment = GetNextSegment(queryString, segments);
            if(IsLastSegment(segment, queryString))
            {
                return expressionProcessor.Process(queryString, segment, parameterExpression, segments.ToList());
            }

            var updatedSegments = expressionProcessor.Process(queryString, segment, parameterExpression, segments.ToList());
            return ProcessSegments(queryString, parameterExpression, updatedSegments);
        }

        public ExpressionSegment GetNextSegment(string queryString, IEnumerable<ExpressionSegment> segments)
        {
            var currentIndex = 0;
            for (int i = 0; i < queryString.Length; i++)
            {
                if (segments.Any(segment => ContainsEntirely(segment, currentIndex, i)))
                {
                    continue;
                }

                if (queryString[i].Equals('('))
                {
                    currentIndex = i;
                }
                
                if (queryString[i].Equals(')'))
                {
                    return new ExpressionSegment { StartIndex = currentIndex, EndIndex = queryString.Length - i };
                }
            }
            return new ExpressionSegment { StartIndex = currentIndex, EndIndex = queryString.Length - 1 };
        }

        private static bool ContainsEntirely(ExpressionSegment parent, ExpressionSegment child)
        {
            return ContainsEntirely(parent, child.StartIndex, child.EndIndex);
        }

        private static bool ContainsEntirely(ExpressionSegment parent, int childStartIndex, int childEndIndex)
        {
            return parent.StartIndex <= childStartIndex && parent.EndIndex >= childEndIndex;
        }

        private static bool IsLastSegment(ExpressionSegment segment, string queryString)
        {
            return segment.StartIndex == 0 && segment.EndIndex == queryString.Length - 1;
        }
    }
}
