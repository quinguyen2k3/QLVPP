using System;

namespace QLVPP.Attributes
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Class,
        Inherited = true,
        AllowMultiple = false
    )]
    public class AllowCommonAccessAttribute : Attribute { }
}
