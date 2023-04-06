using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCClientInterface
{
    public class OpcUaStatusEventArgs
    {
        /// <summary>
        /// 是否异常
        /// </summary>
        public bool Error { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 转化为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Error ? "[异常]" : "[正常]" + Time.ToString("  yyyy-MM-dd HH:mm:ss  ") + Text;
        }
    }
    /// <summary>
    /// 读取属性过程中用于描述的
    /// </summary>
    internal class OpcNodeAttribute
    {
        /// <summary>
        /// 属性的名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 属性的类型描述
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 操作结果状态描述
        /// </summary>
        public StatusCode StatusCode { get; set; }
        /// <summary>
        /// 属性的值，如果读取错误，返回文本描述
        /// </summary>
        public object Value { get; set; }

    }

    public class OPCNode
    {
        public OPCNode(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// 属性的名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图片键值
        /// </summary>
        public string ImageKey { get; set; }
        /// <summary>
        /// 属性的类型描述
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 属性的值，如果读取错误，返回文本描述
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 节点标记
        /// </summary>
        public object Tag { get; set; }

        public string Auth { get; set; }

        public object NodeId { get; set; }

        public string Description { get; set; }
    }
}
