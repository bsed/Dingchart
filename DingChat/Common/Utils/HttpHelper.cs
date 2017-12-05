using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using cn.lds.chatcore.pcw.Models;
using cn.lds.chatcore.pcw.Models.Tables;

namespace cn.lds.chatcore.pcw.Common.Utils {
public class HttpHelper {

    public static string CurrentUrl = string.Empty;
    public static string CurrentCookie = string.Empty;



    public static string UrlEncode(string url) {
        byte[] bs = Encoding.GetEncoding("GB2312").GetBytes(url);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bs.Length; i++) {
            if (bs[i] < 128)
                sb.Append((char)bs[i]);
            else {
                sb.Append("%" + bs[i++].ToString("x").PadLeft(2, '0'));
                sb.Append("%" + bs[i].ToString("x").PadLeft(2, '0'));
            }
        }
        return sb.ToString();
    }


    [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);
    private static string GetCookies(string url) {
        uint datasize = 1024;
        StringBuilder cookieData = new StringBuilder((int)datasize);
        if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x4000, IntPtr.Zero)) {
            if (datasize < 0)
                return null;

            cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00004000, IntPtr.Zero))
                return null;
        }
        return cookieData.ToString();
    }

    public static AccountsTable ConvertCookiesToModel(String strPageUri) {
        AccountsTable accountsModel = new AccountsTable();

        String currentCookie = HttpUtility.UrlDecode(GetCookies(strPageUri));
        if (!string.IsNullOrEmpty(currentCookie)) {
            String[] cookieValues = currentCookie.Split(new char[] {';'});

            foreach (String cookieValue in cookieValues) {
                String key = cookieValue.IndexOf("=") > 0 ?
                             cookieValue.Substring(0, cookieValue.IndexOf("=")).Trim() : String.Empty;
                String value = cookieValue.IndexOf("=") > 0 ?
                               cookieValue.Substring(cookieValue.IndexOf("=") + 1).Trim() : String.Empty;

                if (key.ToUpper().Equals("ID")) {
                    accountsModel.id = Convert.ToInt32(value);
                } else if (key.ToUpper().Equals("ACCOUNT")) {
                    accountsModel.no = value;
                } else if (key.ToUpper().Equals("NAME")) {
                    accountsModel.name = value;
                } else if (key.ToUpper().Equals("NONCETOKEN")) {
                    accountsModel.nonceToken = value;
                } else if (key.ToUpper().Equals("LOGINID")) {
                    accountsModel.loginId = value;
                } else if (key.ToUpper().Equals("CLIENTUSERID")) {
                    accountsModel.clientuserId = value;
                } else if (key.ToUpper().Equals("CLIENTUSERID")) {
                    accountsModel.clientuserId = value;
                } else if (key.ToUpper().Equals("NO")) {
                    accountsModel.no = value;
                }
            }

            //username=1; password=1; id=1; Account=测试用户001; Name=1; NonceToken=U0010010001PLOGIN20161201120109123S0001Z001D123456789012C123456789012S12; LoginId=U0010010001; ClientUserId=U0010010001P; no=C1N7HQAKZTKZB
            // string currentCookie = HttpUtility.UrlDecode(RestRequestHelper.CurrentCookie);

            //AccountsModel accountsModel = new AccountsModel();
            //accountsModel.id = Convert.ToInt32(GetStr(currentCookie, "id=", "; Account"));
            //accountsModel.Account = GetStr(currentCookie, "Account=", "; Name");
            //accountsModel.Name = GetStr(currentCookie, "Name=", "; NonceToken");
            //accountsModel.NonceToken = GetStr(currentCookie, "NonceToken=", "; LoginId");
            //accountsModel.LoginId = GetStr(currentCookie, "LoginId=", "; ClientUserId");
            //accountsModel.ClientUserId = GetStr(currentCookie, "ClientUserId=", "; no");
            //if (currentCookie.Contains("no="))
            //{

            //    ProgramSettingHelper.NO = currentCookie.Substring(currentCookie.LastIndexOf("no=") + 3);
            //}

            //ProgramSettingHelper.LoginUser = accountsModel;
        }
        return accountsModel;
    }




    public static String getFullUrl(String url) {
        String retUrl = url;
        if(String.IsNullOrEmpty(url)) {
            return retUrl;
        }

        if (retUrl.ToUpper().StartsWith("HTTP://")
                || retUrl.ToUpper().StartsWith("HTTPS://")
                || retUrl.ToUpper().StartsWith("FTP://")) {
            return retUrl;

        } else {
            retUrl = ProgramSettingHelper.Host
                     + ((ProgramSettingHelper.Port == 80 || ProgramSettingHelper.Port == 443)
                        ? "" : ( ":" + ProgramSettingHelper.Port.ToString()));
            retUrl += (url.StartsWith("/") ? url : ("/" + url));

            return retUrl;

        }

    }

    //*********************
}
}
