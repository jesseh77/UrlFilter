using System.Linq.Expressions;

namespace UrlFilter
{
    public class ExpressionSegment
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int Length => EndIndex - StartIndex + 1;
        public Expression Expression { get; set; }        
    }
}
