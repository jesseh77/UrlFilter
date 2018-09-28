using BenchmarkDotNet.Running;

namespace UrlFilter.PerfTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ExpressionGenerationTests>();
        }
    }
}
