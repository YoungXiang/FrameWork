using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSharp
{
    public interface IClassParser
    {
        ClassDefine Parse(string classStr);
    }
}
