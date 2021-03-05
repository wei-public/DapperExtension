using System;

namespace Wei.DapperExtension.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {

        public TableAttribute(string _tableName)
        {
            Name = _tableName;
        }
        public string Name { get;}
    }
}
