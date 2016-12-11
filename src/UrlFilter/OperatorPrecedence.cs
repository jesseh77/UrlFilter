using System.Collections.Generic;

namespace UrlFilter
{
    internal class OperatorPrecedence
    {
        private static readonly Dictionary<string, Precedence> Precedences = GetPresedences();

        internal Precedence GetOperatorPrecedence(string operation)
        {
            var operationName = operation.Trim().ToLowerInvariant();
            Precedence precedence;
            if (Precedences.TryGetValue(operationName, out precedence))
            {
                return precedence;
            }
            return Precedence.Value;
        }

        private static Dictionary<string, Precedence> GetPresedences()
        {
            return new Dictionary<string, Precedence>
            {
                {"(", Precedence.Grouping },
                {")", Precedence.Grouping },
                {"not", Precedence.Unary },
                {"gt", Precedence.Relational },
                {"ge", Precedence.Relational },
                {"lt", Precedence.Relational },
                {"le", Precedence.Relational },
                {"eq", Precedence.Equality },
                {"ne", Precedence.Equality },
                {"and", Precedence.ConditionalAnd },
                {"or", Precedence.ConditionalOr }
            };
        }

        internal enum Precedence
        {
            Value,
            ConditionalOr,
            ConditionalAnd,
            Equality,
            Relational,
            Unary,
            Grouping
        }
    }
}
