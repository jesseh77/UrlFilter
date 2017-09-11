using System.Linq;

namespace DemoApi.data
{
    public interface ILoadDemoData<T> where T : class
    {
        IQueryable<T> Stats { get; set; }
    }
}