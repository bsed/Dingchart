using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Common;

namespace cn.lds.chatcore.pcw.Business {
public class LocalCacheHelper {

    private static String path;

    private static XmlDocument GetXmlDocument() {
        XmlDocument xmlDocument = null;
        XmlReader reader = null;
        try {
            String forderPath = App.DefaultCacheRootPath;
            path = forderPath + "LastLoginUser.xml";

            //如果不存在就创建file文件夹
            if (Directory.Exists(forderPath) == false) {
                Directory.CreateDirectory(forderPath);
            }
            xmlDocument = new XmlDocument();
            if (File.Exists(path)) {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true; //忽略文档里面的注释
                reader = XmlReader.Create(@path, settings);
                xmlDocument.Load(path);
                reader.Close();
            } else {
                XmlNode node = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "");
                xmlDocument.AppendChild(node);
                XmlNode root = xmlDocument.CreateElement("root");
                xmlDocument.AppendChild(root);
                xmlDocument.Save(path);
            }
            return xmlDocument;
        } catch (Exception e) {
            Log.Error(typeof (LocalCacheHelper), e);
                reader.Close();
                XmlNode node = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDocument.AppendChild(node);
            XmlNode root = xmlDocument.CreateElement("root");
            xmlDocument.AppendChild(root);
            xmlDocument.Save(path);
            return xmlDocument;
        } finally {
            if (reader != null) {
                reader.Close();
            }
        }
        return null;
    }

    /// <summary>
    /// 设置最后登录人
    /// </summary>
    /// <param Name="loginId"></param>
    /// <param Name="userNo"></param>
    /// <param Name="userName"></param>
    public static void SetLastLoginUser() {
        try {
            XmlDocument xmlDocument = GetXmlDocument();
            if (xmlDocument != null) {
                XmlNode root = xmlDocument.SelectSingleNode("root");
                XmlUtils.SetNodeValue(xmlDocument, root, "loginId", App.AccountsModel.loginId);
                XmlUtils.SetNodeValue(xmlDocument, root, "userNo", App.AccountsModel.no);
                XmlUtils.SetNodeValue(xmlDocument, root, "mobile", App.AccountsModel.mobile);
                XmlUtils.SetNodeValue(xmlDocument, root, "userName", App.AccountsModel.nickname);
                XmlUtils.SetNodeValue(xmlDocument, root, "avatarStorageId", App.AccountsModel.avatarStorageRecordId);
                xmlDocument.Save(path);
            }

        } catch (Exception e) {
            Log.Error(typeof (LocalCacheHelper), e);
        }
    }

    /// <summary>
    /// 判断是否存在上次登录的用户
    /// </summary>
    /// <returns></returns>
    public static LastLoginUserBean GetLastLoginUser() {
        LastLoginUserBean lastLoginUserBean = null;
        try {
            XmlDocument xmlDocument = GetXmlDocument();
            if (xmlDocument != null) {
                String loginId = XmlUtils.GetNodeValue(xmlDocument, "loginId");
                String userNo = XmlUtils.GetNodeValue(xmlDocument, "userNo");
                String userName = XmlUtils.GetNodeValue(xmlDocument, "userName");
                String avatarStorageId = XmlUtils.GetNodeValue(xmlDocument, "avatarStorageId");
                String mobile = XmlUtils.GetNodeValue(xmlDocument, "mobile");

                if (string.IsNullOrEmpty(mobile)) {
                    return lastLoginUserBean;
                }
                if (string.IsNullOrEmpty(userNo)) {
                    return lastLoginUserBean;
                }
                lastLoginUserBean = new LastLoginUserBean();
                lastLoginUserBean.loginId = loginId;
                lastLoginUserBean.userNo = userNo;
                lastLoginUserBean.userName = userName;
                lastLoginUserBean.mobile = mobile;
                lastLoginUserBean.avatarStorageId = avatarStorageId;
                return lastLoginUserBean;
            }
        } catch (Exception e) {
            Log.Error(typeof (LocalCacheHelper), e);
        }
        return lastLoginUserBean;
    }

    ///// <summary>
    ///// 设置登录二维码（因为二维码可能过期，所以注释掉这个）
    ///// </summary>
    ///// <param Name="loginBarcode"></param>
    //public static void SetLoginBarcode(String loginBarcode) {
    //    try {
    //        XmlDocument xmlDocument = GetXmlDocument();
    //        if (xmlDocument != null) {
    //            XmlNode root = xmlDocument.SelectSingleNode("root");
    //            XmlUtils.SetNodeValue(xmlDocument, root, "loginBarcode", loginBarcode);
    //            xmlDocument.Save(path);
    //        }
    //    } catch (Exception e) {
    //        Log.Error(typeof (LocalCacheHelper), e);
    //    }
    //}

    /// <summary>
    /// 获取登录二维码
    /// </summary>
    public static String GetLoginBarcode() {
        String loginBarcode = "";
        try {
            XmlDocument xmlDocument = GetXmlDocument();
            if (xmlDocument != null) {
                loginBarcode = XmlUtils.GetNodeValue(xmlDocument, "loginBarcode");
            }
        } catch (Exception e) {
            Log.Error(typeof (LocalCacheHelper), e);
        }
        return loginBarcode;
    }
}
}
