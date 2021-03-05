using System;

namespace Wei.DapperExtension.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class WriteAttribute : Attribute
    {
        /// <summary>
        /// 可写特性
        /// </summary>
        /// <param name="isWrite">指定字段在数据库中是否可写</param>
        public WriteAttribute(bool isWrite)
        {
            IsWrite = isWrite;
        }

        /// <summary>
        /// 指定字段在数据库中是否可写
        /// </summary>
        public bool IsWrite { get; }
    }
}
