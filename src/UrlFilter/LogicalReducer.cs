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
            var depth = 0;
            Expression currentExpression = Expression.Empty();
            for (int i = 0; i < queryText.Length; i++)
            {
                if(blockStart == 0 && i == queryText.Length - 1) { return ProcessBlock(queryText, parameterExpression); }

                var currentChar = queryText[i];
                if (currentChar.Equals('('))
                {
                    blockStart = i + 1;
                    depth++;
                }

                if(currentChar.Equals(')'))
                {
                    var blockText = queryText.Substring(blockStart, i - blockStart);
                    depth--;
                    if(depth == 0)
                    {
                        currentExpression = ProcessBracket(blockText, parameterExpression);
                    }                    
                }
            }

            throw new NotImplementedException();
        }

        private Expression ProcessBracket(string bracketedExpression, ParameterExpression paramExpression)
        {
            return ReduceLogical(bracketedExpression, paramExpression);
        }

        public Expression ProcessBlock(string blockText, ParameterExpression parameterExpression, Expression left = null, string expType = "and")
        {
            var segments = blockText.Split(' ');
            var expressionType = expType;
            Expression leftExpression = left ?? Expression.Empty();
            var skipTo = 0;
            for (int i = 0; i < segments.Length; i++)
            {
                if (i < skipTo) continue;
                if(unaryProcessor.CanProcess(segments[i]))
                {
                    var comparisonExpression = ReduceSegment(segments, i + 1, parameterExpression);
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
                    leftExpression = ReduceSegment(segments, i - 3, parameterExpression);
                    expressionType = segments[i];
                }
                else if(i == segments.Length - 1)
                {
                    leftExpression = ReduceSegment(segments, i - 2, parameterExpression);
                }
            }
            return leftExpression;
        }

        private Expression ReduceSegment(string[] segments, int start, ParameterExpression paramExpression)
        {
            return comparisonReducer.ReduceComparison(segments[start], segments[start + 1], segments[start + 2], paramExpression);
        }
    }
}
