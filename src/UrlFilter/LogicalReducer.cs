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

        public Expression ReduceLogical(string queryText, ParameterExpression parameterExpression)
        {
            var blockStart = 0;
            for (int i = 0; i < queryText.Length; i++)
            {
                var currentChar = queryText[i];
                if (currentChar.Equals('('))
                {
                    blockStart = i;
                }

                if(currentChar.Equals(')'))
                {
                    var blockText = new ArraySegment<char>(queryText.ToArray(), blockStart, i - blockStart).ToString();
                    var expression = ProcessBlock(blockText, parameterExpression);
                }
            }

            throw new NotImplementedException();
        }

        private Expression ProcessBlock(string blockText, ParameterExpression parameterExpression)
        {
            var segments = blockText.Split(' ');
            var expressionType = "and";
            Expression leftExpression = Expression.Empty();
            var skipTo = 0;
            for (int i = 0; i < segments.Length; i++)
            {
                if (i < skipTo) continue;
                if(unaryProcessor.CanProcess(segments[i]))
                {
                    var comparisonExpression = comparisonReducer.ReduceComparison(segments[i + 1], segments[i + 2], segments[i + 3], parameterExpression);
                    var expression = Expression.Not(comparisonExpression);

                    leftExpression = logicalProcessor.Process(expressionType, leftExpression, expression);
                    skipTo = i + 4;
                }
                else if(logicalProcessor.CanProcess(segments[i]))
                {
                    if(i == skipTo)
                    {
                        expressionType = segments[i];
                        continue;
                    }
                    leftExpression = comparisonReducer.ReduceComparison(segments[i - 3], segments[i - 2], segments[i - 1], parameterExpression);
                    expressionType = segments[i];
                }
                else if(i == segments.Length - 1)
                {
                    leftExpression = comparisonReducer.ReduceComparison(segments[i - 2], segments[i - 1], segments[i], parameterExpression);
                }
            }
            return leftExpression;
        }
    }
}
