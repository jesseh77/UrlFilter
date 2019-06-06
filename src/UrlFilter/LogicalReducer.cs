using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UrlFilter.ExpressionProcessors;
using UrlFilter.ExpressionReducers;

namespace UrlFilter
{
    public class LogicalReducer : ILogicalReducer
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
            var blockStart = -1;
            var blockEnd = 0;
            var depth = 0;
            var expressionType = "and";
            var query = SanitizeQueryText(queryText);
            var subQuery = string.Empty;
            var isSubQueryLogical = false;
            var isSubQueryNot = false;
            Expression currentExpression = Expression.Empty();
            for (int i = 0; i < query.Length; i++)
            {
                if (blockStart == -1 && i == query.Length - 1) { return ProcessBlock(query, parameterExpression, null, expressionType); }

                var currentChar = query[i];
                if (currentChar.Equals('('))
                {
                    if (blockStart == -1 && i != 0)
                    {
                        //refactor
                        var subQuerySegments = query.Substring(0, i - 1).Split(' ');
                        expressionType = subQuerySegments.Last();
                        subQuery = string.Join(" ", subQuerySegments.Take(subQuerySegments.Length - 1));
                    }

                    if (depth == 0)
                    {
                        blockStart = i + 1;
                    }

                    if (blockEnd > 0 && depth == 0)
                    {
                        var gapExpression = query.Substring(blockEnd + 2, blockStart - blockEnd - 3);
                        var subQuerySegments = gapExpression.Trim().Split(' ');
                        if (subQuerySegments.Count() >= 5)
                        {
                            expressionType = subQuerySegments.Last();
                            subQuery = string.Join(" ", subQuerySegments.Take(gapExpression.Length - 1));
                            currentExpression = ReduceLogical(subQuery, parameterExpression);
                        }

                        if (subQuerySegments.Count() == 1)
                        {
                            expressionType = subQuerySegments[0];
                            isSubQueryLogical = true;
                        }

                        if(subQuerySegments.Count() == 2)
                        {
                            expressionType = subQuerySegments[0];
                            isSubQueryLogical = true;
                            isSubQueryNot = true;                            
                        }
                    }

                    depth++;
                }

                if (currentChar.Equals(')'))
                {
                    depth--;
                    blockEnd = i - blockStart;
                    if (depth == 0)
                    {
                        var blockText = query.Substring(blockStart, blockEnd);
                        var subExpression = ReduceLogical(blockText, parameterExpression);
                        
                        if (isSubQueryLogical)
                        {
                            if (isSubQueryNot)
                            {
                                subExpression = unaryProcessor.Process("not", subExpression);
                            }
                            currentExpression = logicalProcessor.Process(expressionType, currentExpression, subExpression);
                            isSubQueryLogical = false;
                            subQuery = string.Empty;
                        }
                        else
                        {
                            currentExpression = subExpression;
                        }
                    }
                }

                if (i == query.Length)
                {
                    subQuery = query.Substring(blockEnd + 1, i - blockEnd);
                }
            }

            if (subQuery.Length == 0) { return currentExpression; }
            var resultingExpression = ProcessBlock(subQuery, parameterExpression, currentExpression, expressionType);
            return resultingExpression;
        }

        private static string SanitizeQueryText(string queryText)
        {
            return queryText.Replace("   ", " ")
                .Replace("  ", " ")
                .Replace("( ", "(")
                .Replace(" )", ")");
        }

        public Expression ProcessBlock(string blockText, ParameterExpression parameterExpression, Expression left, string expType)
        {
            var segments = splitSegments(blockText).ToArray();
            var expressionType = expType;
            Expression leftExpression = left;
            var skipTo = 0;
            for (int i = 0; i < segments.Length; i++)
            {
                if (i < skipTo) continue;
                if (unaryProcessor.CanProcess(segments[i]))
                {
                    var comparisonExpression = ReduceSegment(segments, i + 1, parameterExpression);
                    var expression = Expression.Not(comparisonExpression);

                    if(leftExpression is null)
                    {
                        leftExpression = expression;
                    }
                    leftExpression = logicalProcessor.Process(expressionType, leftExpression, expression);
                    skipTo = i + 4;
                }
                else if (logicalProcessor.CanProcess(segments[i]))
                {
                    if (i == skipTo)
                    {
                        expressionType = segments[i];
                        continue;
                    }
                    leftExpression = ReduceSegment(segments, i - 3, parameterExpression);
                    expressionType = segments[i];
                }
                else if (i == segments.Length - 1)
                {
                    leftExpression = ReduceSegment(segments, i - 2, parameterExpression);
                }
            }
            return leftExpression;
        }

        public IEnumerable<string> splitSegments(string blockText)
        {
            var isQuoted = false;
            var blockStart = 0;
            for (int i = 0; i < blockText.Length; i++)
            {
                var currChar = blockText[i];
                if(currChar.Equals(' '))
                {
                    if (isQuoted) { continue; }
                    var block = blockText.Substring(blockStart, i - blockStart).Trim();
                    yield return block;
                    blockStart = i + 1;
                }

                if(currChar.Equals('\''))
                {
                    if(isQuoted)
                    {
                        var block = blockText.Substring(blockStart, i - blockStart).Trim();
                        yield return block;
                        isQuoted = false;
                    }
                    else
                    {
                        isQuoted = true;
                    }
                    blockStart = i + 1;                    
                }

                if(i == blockText.Length - 1)
                {
                    if (!currChar.Equals('\''))
                    {
                        yield return blockText.Substring(blockStart).Trim();
                    }                    
                }
            }
        }

        private Expression ReduceSegment(string[] segments, int start, ParameterExpression paramExpression)
        {
            return comparisonReducer.ReduceComparison(segments[start], segments[start + 1], segments[start + 2], paramExpression);
        }
    }
}
