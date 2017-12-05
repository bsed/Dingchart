using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using Newtonsoft.Json.Linq;

namespace cn.lds.chatcore.pcw.Common.Services {
class ServicesMethod {


    private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
    /// <summary>
    /// 创建POST方式的HTTP请求
    /// </summary>
    /// <param Name="url">请求的URL</param>
    /// <param Name="parameters">随同请求POST的参数名称及参数值字典</param>
    /// <param Name="timeout">请求的超时时间</param>
    /// <param Name="userAgent">请求的客户端浏览器信息，可以为空</param>
    /// <param Name="requestEncoding">发送HTTP请求时所用的编码</param>
    /// <param Name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
    /// <returns></returns>
    public static string CreatePostHttpResponse(string url, IDictionary<string, string> parameters, CookieCollection cookies) {
        if (string.IsNullOrEmpty(url)) {
            throw new ArgumentNullException("url");
        }

        HttpWebRequest request = null;
        //如果是发送HTTPS请求
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
        } else {
            request = WebRequest.Create(url) as HttpWebRequest;
        }
        request.Method = "POST";
        request.ContentType = "application/json;charset=UTF-8";

        //提高效率
        request.ServicePoint.Expect100Continue = false;
        request.ServicePoint.UseNagleAlgorithm = false;
        request.ServicePoint.ConnectionLimit = 65500;
        //request.AllowWriteStreamBuffering = false;


        request.UserAgent = DefaultUserAgent;


        //request.Timeout = ServiceCode.RequestTimeout;
        if (cookies != null) {
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);
        } else {
            //设置HttpWebRequest的CookieContainer为刚才建立的那个myCookieContainer
            request.CookieContainer = new CookieContainer();
            App.CookieContainer = request.CookieContainer;
        }
        //如果需要POST数据
        if (!(parameters == null || parameters.Count == 0)) {
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            foreach (string key in parameters.Keys) {
                if (i > 0) {
                    buffer.AppendFormat(",\"{0}\":\"{1}\"", key, parameters[key]);
                } else {
                    buffer.AppendFormat("\"{0}\":\"{1}\"", key, parameters[key]);
                    i++;
                }
            }
            //Console.Write(buffer.ToString());
            buffer.Append("}");  // 追加一个字符：B
            buffer.Insert(0, "{"); // 在最前面插入一个：A
            Console.Write(buffer.ToString());
            byte[] data = System.Text.Encoding.UTF8.GetBytes(buffer.ToString());
            using (Stream stream = request.GetRequestStream()) {
                stream.Write(data, 0, data.Length);
            }
        }
        System.Net.HttpWebResponse response;
        response = (System.Net.HttpWebResponse)request.GetResponse();
        StreamReader streamReader = new StreamReader(response.GetResponseStream());
        string responseContent = streamReader.ReadToEnd();
        if (responseContent.Contains("errors")) {
            return null;
        }
        response.Close();
        streamReader.Close();
        return responseContent;
    }




    /// <summary>
    /// 创建GET方式的HTTP请求
    /// </summary>
    public static string CreateGetHttpResponse(string url, IDictionary<string, string> parameters,EventDataType eventDataType) {
        HttpWebRequest request = null;
        try {
            //将Get请求的参数拼接到url中
            if (parameters != null && parameters.Count > 0) {
                StringBuilder stringBuilder = new StringBuilder(url);
                foreach (string key in parameters.Keys) {
                    stringBuilder.Append("/");
                    stringBuilder.Append(key);
                    stringBuilder.Append("/");
                    stringBuilder.Append(parameters[key]);
                }
                url = stringBuilder.ToString();
            }

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                //对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;    //http版本，默认是1.1,这里设置为1.0
            } else {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";

            //提高效率
            request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.ServicePoint.ConnectionLimit = 65500;
            //request.AllowWriteStreamBuffering = false;


            //设置代理UserAgent和超时
            request.UserAgent = DefaultUserAgent;

            //request.Timeout = ServiceCode.RequestTimeout;
            request.CookieContainer = App.CookieContainer;//*

        } catch (Exception ex) {
            Log.Error(typeof (ServicesMethod), ex);
        }
        HttpWebResponse response = request.GetResponse() as HttpWebResponse;

        StreamReader streamReader = new StreamReader(response.GetResponseStream());
        string responseContent = streamReader.ReadToEnd();

        if (responseContent.Contains("errors")) {
            return null;
        }

        success(responseContent, eventDataType);

        response.Close();
        streamReader.Close();


        return responseContent;
    }
    private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) {
        return true; //总是接受
    }


    /// <summary>
    /// API请求成功处理方法
    /// </summary>
    /// <param Name="responseContent"></param>
    /// <param Name="?"></param>
    /// <param Name="eventDataType"></param>
    public static void success(String responseContent,EventDataType eventDataType) {
        EventData<Object> data = new EventData<Object>();
        var jObject = JObject.Parse(responseContent);
        data.eventDataType = eventDataType;
        data.data = jObject;
        String strStatus = jObject["status"].ToString();
        if ("success".Equals(strStatus)) {
            data.eventType = EventType.HttpRequest;
        } else {
            data.eventType = EventType.HttpRequestError;
            // todo somting
        }

        EventBusHelper.getInstance().fireEvent(data);
    }

    /// <summary>
    /// API请求成功处理方法
    /// </summary>
    /// <param Name="responseContent"></param>
    /// <param Name="?"></param>
    /// <param Name="eventDataType"></param>
    public static void faasdfsdf(String responseContent, EventDataType eventDataType) {
        EventData<Object> data = new EventData<Object>();
        var jObject = JObject.Parse(responseContent);
        data.eventDataType = eventDataType;
        data.data = jObject;
        data.eventType = EventType.HttpRequestError;
        EventBusHelper.getInstance().fireEvent(data);
    }
}
}
