using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UrlFilter.ExpressionProcessors
{
    class CustomProcessor : IExpressionProcessor
    {
        public OperatorPrecedence.Precedence Precedence => OperatorPrecedence.Precedence.Custom;
        public void Process(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            throw new NotImplementedException();
        }
    }
}
