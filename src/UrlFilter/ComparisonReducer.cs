using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;

namespace UrlFilter.ExpressionReducers
{
    public class ComparisonReducer : IComparisonReducer
    {
        private IComparisonProcessor comparisonProcessor;
        private IPropertyProcessor propertyProcessor;
        private IValueProcessor valueProcessor;
        private readonly INotNullExpressionProcessor notNullExpressionProcessor;

        public ComparisonReducer(IComparisonProcessor comparisonProcessor, IPropertyProcessor propertyProcessor, IValueProcessor valueProcessor, INotNullExpressionProcessor notNullExpressionProcessor)
        {
            this.comparisonProcessor = comparisonProcessor;
            this.propertyProcessor = propertyProcessor;
            this.valueProcessor = valueProcessor;
            this.notNullExpressionProcessor = notNullExpressionProcessor;
        }

        public Expression ReduceComparison(string leftValue, string comparisonOperator, string rightValue, ParameterExpression paramExpression)
        {
            if (!comparisonProcessor.CanProcess(comparisonOperator)) return null;
            ProcessExpressions(leftValue, rightValue, out Expression leftExpression, out Expression rightExpression, out Expression notNullExpression, paramExpression);

            var comparisonExpression = comparisonProcessor.Process(comparisonOperator, leftExpression, rightExpression);
            if(notNullExpression is null)
            {
                return comparisonExpression;
            }
            return Expression.AndAlso(notNullExpression, comparisonExpression);
        }

        private void ProcessExpressions(string leftValue, string rightValue, out Expression leftExpression, out Expression rightExpression, out Expression notNullExpression, ParameterExpression paramExpression)
        {
            if (propertyProcessor.CanProcess(leftValue, paramExpression))
            {
                leftExpression = propertyProcessor.Process(leftValue, paramExpression);
                rightExpression = valueProcessor.Process(rightValue, leftExpression.Type);
                notNullExpression = notNullExpressionProcessor.NotNullPropertyExpression(leftValue, paramExpression);
            }
            else
            {
                rightExpression = propertyProcessor.Process(rightValue, paramExpression);
                leftExpression = valueProcessor.Process(leftValue, rightExpression.Type);
                notNullExpression = notNullExpressionProcessor.NotNullPropertyExpression(rightValue, paramExpression);
            }
        }
    }
}
