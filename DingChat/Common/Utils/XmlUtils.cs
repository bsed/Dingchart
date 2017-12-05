using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace cn.lds.chatcore.pcw.Common.Utils {
public class XmlUtils {


    /// <summary>
    /// 创建节点
    /// </summary>
    /// <param Name="xmldoc"></param>  xml文档
    /// <param Name="parentnode"></param>父节点
    /// <param Name="name"></param>  节点名
    /// <param Name="value"></param>  节点值
    ///
    public static String GetNodeValue(XmlDocument xmlDoc, String name) {
        XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName(name);
        if (xmlNodeList != null && xmlNodeList.Count>0) {
            XmlNode xmlNode = xmlNodeList[0];
            return xmlNode.InnerText.ToStr();
        }
        return "";
    }
    /// <summary>
    /// 创建节点
    /// </summary>
    /// <param Name="xmldoc"></param>  xml文档
    /// <param Name="parentnode"></param>父节点
    /// <param Name="name"></param>  节点名
    /// <param Name="value"></param>  节点值
    ///
    public static void SetNodeValue(XmlDocument xmlDoc, XmlNode xmlNodeParent, String name, String value) {
        XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName(name);
        if (xmlNodeList != null && xmlNodeList.Count > 0) {
            XmlNode xmlNode = xmlNodeList[0];
            xmlNode.InnerText = value;
        } else {
            XmlNode xmlNode = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            xmlNode.InnerText = value;
            xmlNodeParent.AppendChild(xmlNode);
        }

    }
}
}
