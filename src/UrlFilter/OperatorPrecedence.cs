using System.Collections.Generic;

namespace UrlFilter
{
    public class OperatorPrecedence
    {
        private static readonly Dictionary<string, Precedence> Precedences = GetPresedences();

        public Precedence GetOperatorPrecedence(string operation)
        {
            var operationName = operation.Trim().ToLowerInvariant();
            Precedence precedence;
            if (Precedences.TryGetValue(operationName, out precedence))
            {
                return precedence;
            }
            return Precedence.Value;
        }

        public static Dictionary<string, Precedence> GetPresedences()
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

        public enum Precedence
        {
            Grouping,
            Unary,
            Relational,
            Equality,
            ConditionalOr,
            ConditionalAnd,
            Value
        }
    }
}
