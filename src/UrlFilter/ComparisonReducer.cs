using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;

namespace UrlFilter.ExpressionReducers
{
    public class ComparisonReducer : IComparisonReducer
    {
        private IComparisonProcessor comparisonProcessor;
        private IPropertyProcessor propertyProcessor;
        private IValueProcessor valueProcessor;

        public ComparisonReducer(IComparisonProcessor comparisonProcessor, IPropertyProcessor propertyProcessor, IValueProcessor valueProcessor)
        {
            this.comparisonProcessor = comparisonProcessor;
            this.propertyProcessor = propertyProcessor;
            this.valueProcessor = valueProcessor;
        }

        public Expression ReduceComparison(string leftValue, string comparisonOperator, string rightValue, ParameterExpression paramExpression)
        {
            if (!comparisonProcessor.CanProcess(comparisonOperator)) return null;
            Expression leftExpression = null;
            Expression rightExpression = null;

            ProcessExpressions(leftValue, rightValue, out leftExpression, out rightExpression, paramExpression);

            return comparisonProcessor.Process(comparisonOperator, leftExpression, rightExpression);
        }

        private void ProcessExpressions(string leftValue, string rightValue, out Expression leftExpression, out Expression rightExpression, ParameterExpression paramExpression)
        {
            if (propertyProcessor.CanProcess(leftValue, paramExpression))
            {
                leftExpression = propertyProcessor.Process(leftValue, paramExpression);
                rightExpression = valueProcessor.Process(rightValue, leftExpression.Type);
            }
            else
            {
                rightExpression = propertyProcessor.Process(rightValue, paramExpression);
                leftExpression = valueProcessor.Process(leftValue, rightExpression.Type);
            }
        }
    }
}
