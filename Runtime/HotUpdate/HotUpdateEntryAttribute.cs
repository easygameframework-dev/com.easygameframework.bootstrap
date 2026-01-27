using System;

namespace EasyGameFramework.Bootstrap
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HotUpdateEntryAttribute : Attribute
    {
        public int Priority { get; set; }
    }
}
