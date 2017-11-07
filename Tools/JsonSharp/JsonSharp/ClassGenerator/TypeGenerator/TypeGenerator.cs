using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSharp
{
    public interface ITypeGenerator
    {
        string Generate(DataMemberType inputType);
    }
}
