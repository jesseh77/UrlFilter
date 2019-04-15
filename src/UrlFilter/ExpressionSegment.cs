using System.Linq.Expressions;

namespace UrlFilter
{
    public class ExpressionSegment
    {
        public string Text { get; set; }
        public Expression Expression { get; set; }
    }
}