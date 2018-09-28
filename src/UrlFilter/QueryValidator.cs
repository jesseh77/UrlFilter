namespace UrlFilter
{
    public class QueryValidator
    {
        public void ValidateQueryText(string queryString)
        {
            ValidateParenthesis(queryString);
        }

        public void ValidateParenthesis(string queryString)
        {
            var openParenCount = 0;
            var closedParenCount = 0;

            foreach (var character in queryString)
            {
                if (character == '(')
                {
                    if (openParenCount == 0)
                    {
                        if (closedParenCount != 0)
                        {
                            throw new QueryStringException("Query string may not start with the close parenthesis ')' character");
                        }
                    }
                    openParenCount++;
                }

                if (character == ')')
                {
                    if (openParenCount == 0)
                    {
                        throw new QueryStringException("Query string may not end with the open parenthesis '(' character");
                    }
                    closedParenCount++;
                }
            }
            if (openParenCount != closedParenCount)
                throw new QueryStringException(
                    "Query string must contain the same number of open '(' and close ')' parenthesis");
        }
    }
}
