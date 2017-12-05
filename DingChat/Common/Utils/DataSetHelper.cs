using System;
using System.Data;

namespace cn.lds.chatcore.pcw.Common.Utils {
public class DataSetHelper {
    public static DataSet CXmlToDataSet(string xmlFilename) {
        if (!string.IsNullOrEmpty(xmlFilename)) {
            try {
                //ds获取Xmlrdr中的数据
                DataSet ds = new DataSet();
                ds.ReadXml(xmlFilename, XmlReadMode.Auto);
                return ds;
            } catch (Exception e) {
                Log.Error(typeof(DataSetHelper), "本地DB开启失败");
                throw new LdException("程序出现错误请联系管理员");
            }
        } else {
            return null;
        }
    }
}

}
