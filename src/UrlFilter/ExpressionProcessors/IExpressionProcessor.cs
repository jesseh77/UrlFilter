using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    internal interface IExpressionProcessor
    {
        bool canProcess(string operand);
        void Process(LinkedList<Token> tokens, ParameterExpression paramExpression);
    }
}
