using System;

namespace Wei.DapperExtension.Attributes
{
    /// <summary>
    /// 主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        /// <summary>
        /// KeyAttribute
        /// </summary>
        /// <param name="isIncrement">是否自增长的主键</param>
        public KeyAttribute(bool isIncrement = true)
        {
            IsIncrement = isIncrement;
        }

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool IsIncrement { get;}
    }
}
