using System;

namespace Wei.DapperExtension.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string _name)
        {
            Name = _name;
        }

        public string Name { get;}
    }
}
