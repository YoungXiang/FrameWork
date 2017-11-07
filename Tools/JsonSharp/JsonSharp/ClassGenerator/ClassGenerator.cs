using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSharp
{
    public interface IClassGenerator
    {
        string Generate(ClassDefine cd);
    }
}
