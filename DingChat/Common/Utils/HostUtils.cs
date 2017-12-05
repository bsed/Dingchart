using System.Net;
using System.Net.Sockets;

namespace cn.lds.chatcore.pcw.Common.Utils {
/// <summary> 主机
/// </summary>
public class HostUtils {
    #region 网络

    /// <summary> 获取当前主机的 IP 地址
    /// </summary>
    /// <returns></returns>
    public static string GetIpAddress() {
        IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

        foreach (IPAddress ip in ips) {
            if (ip.AddressFamily.Equals(AddressFamily.InterNetwork)) {
                return ip.ToString();
            }
        }

        return "127.0.0.1";
    }

    #endregion
}
}