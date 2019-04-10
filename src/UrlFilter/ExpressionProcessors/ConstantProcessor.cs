using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace UrlFilter.ExpressionProcessors
{
    public class ConstantProcessor : IProcessExpression
    {
        public ExpressionCategory ExpressionCategory => ExpressionCategory.Constant;
        private readonly Func<object, Expression> expression;

        public ConstantProcessor(Func<object, Expression> expression)
        {
            this.expression = expression;
        }

        public bool CanProcess(string operand, ParameterExpression paramExpression)
        {
            return true;
        }

        public void Process(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            throw new NotImplementedException();
        }
    }
}
