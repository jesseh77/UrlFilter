using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace UrlFilter.ExpressionProcessors
{
    public class PropertyProcessor : IProcessExpression
    {
        public ExpressionCategory ExpressionCategory => ExpressionCategory.Property;
        private readonly Func<Expression, MethodInfo, Expression> expression;

        public PropertyProcessor(Func<Expression, MethodInfo, Expression> expression)
        {
            this.expression = expression;
        }

        public bool CanProcess(string operand, ParameterExpression paramExpression)
        {
            Func<Expression, MethodInfo, Expression> item = Expression.Property;
            throw new NotImplementedException();
        }

        public void Process(ExpressionSegment segment, ParameterExpression paramExpression)
        {
            throw new NotImplementedException();
        }
    }
}
