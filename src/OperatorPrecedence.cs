using System;
using System.Collections.Generic;

namespace UrlFilter
{
    internal static class OperatorPrecedence
    {
        private static readonly Dictionary<string, Precedence> Precedences = GetPresedences();

        public static Precedence GetOperatorPrecedence(string operation)
        {
            Precedence precedence;
            if (Precedences.TryGetValue(operation, out precedence))
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

        public enum Precedence
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
