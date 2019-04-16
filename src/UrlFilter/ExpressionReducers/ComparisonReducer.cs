using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;

namespace UrlFilter.ExpressionReducers
{
    public class ComparisonReducer
    {
        private ComparisonProcessor comparisonProcessor = new ComparisonProcessor();
        private PropertyProcessor propertyProcessor = new PropertyProcessor(new PropertyInfoProvider());
        private ValueProcessor valueProcessor = new ValueProcessor();


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
            if(propertyProcessor.CanProcess(leftValue, paramExpression))
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
