using System;
using System.Collections.Generic;
using System.Text;

namespace JsonSharp
{
    public class DataMemberDefine
    {
        public DataMemberType mType;    // type of this variable : int
        public string mName;            // name of this variable : pid
    }

    public class ClassDefine
    {
        public string className;
        public List<DataMemberDefine> members;

        public ClassDefine()
        {
            className = "Undefined";
            members = new List<DataMemberDefine>();
        }
    }
}
