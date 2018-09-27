using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.ExpressionProcessors
{
    internal class UnaryProcessor : IExpressionProcessor
    {
        private readonly List<string> _operands;

        public ExpressionCategory ExpressionCategory => ExpressionCategory.Unary;

        public bool CanProcess(string operand)
        {
            if (string.IsNullOrWhiteSpace(operand)) return false;
            return _operands.Contains(operand.ToLower());
        }

        public UnaryProcessor(List<string> operands)
        {
            _operands = operands;
        }

        public void Process(LinkedList<Token> tokens, ParameterExpression paramExpression)
        {
            var current = tokens.First;
            while (current.Next != null)
            {
                if (CanProcess(current.Value.TokenValue))
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
