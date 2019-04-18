using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UrlFilter.ExpressionProcessors;
using UrlFilter.ExpressionReducers;

namespace UrlFilter
{
    public class LogicalReducer
    {
        private readonly IComparisonReducer comparisonReducer;
        private readonly IUnaryProcessor unaryProcessor;
        private readonly ILogicalProcessor logicalProcessor;

        public LogicalReducer(IComparisonReducer comparisonReducer, IUnaryProcessor unaryProcessor, ILogicalProcessor logicalProcessor)
        {
            this.comparisonReducer = comparisonReducer;
            this.unaryProcessor = unaryProcessor;
            this.logicalProcessor = logicalProcessor;
        }

        public Expression ReduceLogical(string expressionTextSegment, ParameterExpression parameterExpression)
        {
            var comparisonSegmentStart = 0;
            Expression leftExpression = null;
            string logicalType = string.Empty;
            var segments = expressionTextSegment.Split(' ');

            for (int i = 0; i < segments.Length; i++)
            {
                if(logicalProcessor.CanProcess(segments[i]))
                {
                    if(leftExpression == null)
                    {
                        var comparison = segments.Skip(comparisonSegmentStart).Take(i - comparisonSegmentStart).ToArray();

                        leftExpression = comparisonReducer.ReduceComparison(comparison[0], comparison[1], comparison[2], parameterExpression);
                    }
                }
            }
            return null;
        }
    }
}
