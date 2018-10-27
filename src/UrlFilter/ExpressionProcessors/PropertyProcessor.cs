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

        public bool CanProcess(string operand)
        {
            Func<Expression, MethodInfo, Expression> item = Expression.Property;
            throw new NotImplementedException();
        }

        public void Process(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            throw new NotImplementedException();
        }
    }
}
