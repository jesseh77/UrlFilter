using Nancy;
using System;
using System.IO;

namespace DemoApi
{
    public class RootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Content\");
            return path;
        }
    }
}