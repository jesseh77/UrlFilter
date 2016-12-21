using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    internal class UnaryProcessor : IExpressionProcessor
    {
        public OperatorPrecedence.Precedence Precedence => OperatorPrecedence.Precedence.Unary;
        public void Process(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            var current = tokens.First;
            while (current.Next != null)
            {
                if (current.Value.GroupPriority == Precedence)
                {
                    current.Next.Next.Value.IsNot = true;
                    var next = current.Next;
                    tokens.Remove(current);
                    current = next;
                }
                else
                {
                    current = current.Next;
                }
            }
        }
    }
}
