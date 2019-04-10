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
            if(segment.IsLastSegment(queryString))
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
                if (segments.Any(segment => segment.ContainsEntirely(currentIndex, i)))
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
    }
}
