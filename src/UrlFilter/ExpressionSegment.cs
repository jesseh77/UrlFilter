using System.Linq.Expressions;

namespace UrlFilter
{
    public class ExpressionSegment
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public bool IsProcessed => Expression != null;
        public int Length => EndIndex - StartIndex + 1;
        public Expression Expression { get; set; }

        public bool ContainsEntirely(ExpressionSegment child)
        {
            return ContainsEntirely(child.StartIndex, child.EndIndex);
        }

        public bool ContainsEntirely(int childStartIndex, int childEndIndex)
        {
            return StartIndex <= childStartIndex && EndIndex >= childEndIndex;
        }

        public bool IsLastSegment(string queryString)
        {
            return StartIndex == 0 && EndIndex == queryString.Length - 1;
        }

        public string SegmentText(string queryString)
        {
            return queryString.Substring(StartIndex, Length);
        }
    }
}
