using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    internal interface IExpressionProcessor
    {
        OperatorPrecedence.Precedence Precedence { get; }
        void Process(LinkedList<Token> tokens, ParameterExpression paramExpression);
    }
}
