using System;
using System.Collections.Generic;
using System.Text;
using Wei.DapperExtension.Attributes;

namespace Wei.DapperExtionsion.Test.Entities
{

    public class TestModelInt
    {
        public int Id { get; set; }
        public string MethodName { get; set; }
        public string Result { get; set; }
    }

    /// <summary>
    /// 联合主键
    /// </summary>
    public class TestModelMultipeKey 
    {

        /// <summary>
        /// 联合主键-1
        /// </summary>
        [Key(false)]
        public string TypeId { get; set; }

        /// <summary>
        /// 联合主键-2
        /// </summary>
        [Key(false)]
        public string Type { get; set; }

        public string MethodName { get; set; }
        public string Result { get; set; }
    }
}
