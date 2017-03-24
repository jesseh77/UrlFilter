using System.Linq.Expressions;

namespace UrlFilter
{
    internal class Token
    {
        public string TokenValue { get; set; }
        public OperatorPrecedence.Precedence GroupPriority { get; set; }
        public Expression OperatorExpression { get; set; }
        public bool IsNot { get; set; }
        public bool HasExpression => OperatorExpression != null;
    }
}
