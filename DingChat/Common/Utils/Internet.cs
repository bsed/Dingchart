using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Web;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;



namespace cn.lds.chatcore.pcw.Common.Utils {
public class Internet {

    //public static void Main(string[] args)
    //{

    //    Internet.getInstance().start();

    //    Console.ReadKey();
    //}






    #region 利用API方式获取网络链接状态
    private static int NETWORK_ALIVE_LAN = 0x00000001;
    private static int NETWORK_ALIVE_WAN = 0x00000002;
    private static int NETWORK_ALIVE_AOL = 0x00000004;

    [DllImport("sensapi.dll")]
    private extern static bool IsNetworkAlive(ref int flags);
    [DllImport("sensapi.dll")]
    private extern static bool IsDestinationReachable(string dest, IntPtr ptr);

    [DllImport("wininet.dll")]
    private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

    string[] urls = { "www.sina.com", "www.baidu.com", "www.163.com" };

    private static Internet internet = null;

    Timer stateTimer = null;
    int interval = 5000;
    int port = 80;
    int connectTimeOut = 3000;
    /**网络连接状态*/
    bool connectStatus = false;

    int connectSuccessCount = 0;

    public static Internet getInstance() {
        if (internet == null) {
            internet = new Internet();
        }
        return internet;
    }

    private Internet() {

    }

    public void Stop() {
        if (stateTimer != null) {
            stateTimer.Dispose();
            stateTimer = null;
        }
    }

    public void start() {
        if (stateTimer == null) {
            var autoEvent = new AutoResetEvent(false);
            Internet internet = getInstance();
            stateTimer = new Timer(internet.getStatus,
                                   autoEvent, 1000, interval);
        }
    }

    public void getStatus(Object stateInfo) {
        //System.Console.WriteLine("-----1-----");
        lock (this) {
            connectStatus = false;
            connectSuccessCount = 0;

            bool IsConnected = Internet.IsConnected();

            bool IsHostAlive = false;



            foreach (string url in urls) {
                TcpClient tc = TcpClientConnector.Connect(url, port, connectTimeOut);//推荐使用（采用线程池强行中断超时时间）
                try {
                    if (tc != null) {
                        tc.GetStream().Close();
                        tc.Close();
                        IsHostAlive = true;
                    } else
                        IsHostAlive = false;
                } catch (Exception e) {
                    Log.Error(typeof(Internet), e);
                    IsHostAlive = false;
                }

                if (IsHostAlive && IsConnected) {
                    connectSuccessCount++;
                    break;

                }
            }
            //System.Console.WriteLine("-----2-----:" + connectSuccessCount);
            if (connectSuccessCount > 0) {
                // if (!connectStatus) {
                connectStatus = true;
                //此处应该发送事件出去
                //System.Console.WriteLine("***************** 网络连接成功 ***********");

                FrameEvent<Object> eventData = new FrameEvent<Object>();
                eventData.frameEventDataType = FrameEventDataType.NETWORK_SUCCESS;
                EventBusHelper.getInstance().fireEvent(eventData);
                // }
            } else {
                // if (connectStatus) {
                connectStatus = false;
                //此处应该发送事件出去
                //System.Console.WriteLine("***************** 网络连接失败 ***********");

                FrameEvent<Object> eventData = new FrameEvent<Object>();
                eventData.frameEventDataType = FrameEventDataType.NETWORK_ERROR;
                EventBusHelper.getInstance().fireEvent(eventData);
                //}
            }
        }
    }



    public static bool IsConnected() {
        int desc = 0;
        bool state = InternetGetConnectedState(out desc, 0);
        return state;
    }

    public static bool IsLanAlive() {
        return IsNetworkAlive(ref NETWORK_ALIVE_LAN);
    }
    public static bool IsWanAlive() {
        return IsNetworkAlive(ref NETWORK_ALIVE_WAN);
    }
    public static bool IsAOLAlive() {
        return IsNetworkAlive(ref NETWORK_ALIVE_AOL);
    }
    public static bool IsDestinationAlive(string Destination) {
        return (IsDestinationReachable(Destination, IntPtr.Zero));
    }
    #endregion

    /// <summary>
    /// 在指定时间内尝试连接指定主机上的指定端口。 （默认端口：80,默认链接超时：5000毫秒）
    /// </summary>
    /// <param Name="HostNameOrIp">主机名称或者IP地址</param>
    /// <param Name="port">端口</param>
    /// <param Name="timeOut">超时时间</param>
    /// <returns>返回布尔类型</returns>
    public static bool IsHostAlive(string HostNameOrIp, int? port, int? timeOut) {
        TcpClient tc = new TcpClient();
        tc.SendTimeout = timeOut ?? 5000;
        tc.ReceiveTimeout = timeOut ?? 5000;

        bool isAlive;
        try {
            tc.Connect(HostNameOrIp, port ?? 80);
            isAlive = true;
        } catch (Exception e) {
            Log.Error(typeof(Internet), e);
            isAlive = false;
        } finally {
            tc.Close();
        }
        return isAlive;
    }

}

public class TcpClientConnector {
    /// <summary>
    /// 在指定时间内尝试连接指定主机上的指定端口。 （默认端口：80,默认链接超时：5000毫秒）
    /// </summary>
    /// <param Name= "hostname ">要连接到的远程主机的 DNS 名。</param>
    /// <param Name= "port ">要连接到的远程主机的端口号。 </param>
    /// <param Name= "millisecondsTimeout ">要等待的毫秒数，或 -1 表示无限期等待。</param>
    /// <returns>已连接的一个 TcpClient 实例。</returns>
    public static TcpClient Connect(string hostname, int? port, int? millisecondsTimeout) {
        ConnectorState cs = new ConnectorState();
        cs.Hostname = hostname;
        cs.Port = port ?? 80;
        ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectThreaded), cs);
        if (cs.Completed.WaitOne(millisecondsTimeout ?? 5000, false)) {
            if (cs.TcpClient != null) return cs.TcpClient;
            return null;
            //throw cs.Exception;
        } else {
            if (cs != null) cs.Abort();
            return null;
            //throw new SocketException(11001); // cannot connect
        }
    }

    private static void ConnectThreaded(object state) {
        ConnectorState cs = (ConnectorState)state;
        cs.Thread = Thread.CurrentThread;
        try {
            TcpClient tc = new TcpClient(cs.Hostname, cs.Port);
            if (cs.Aborted) {
                try {
                    tc.GetStream().Close();
                } catch (Exception e) {
                    //Log.Error(typeof(Internet), e);
                }
                try {
                    tc.Close();
                } catch (Exception e) {
                    // Log.Error(typeof(Internet), e);
                }
            } else {
                cs.TcpClient = tc;
                cs.Completed.Set();
            }
        } catch (Exception e) {
            // Log.Error(typeof(Internet), e);
            cs.Exception = e;
            cs.Completed.Set();
        }
    }

    private class ConnectorState {
        public string Hostname;
        public int Port;
        public volatile Thread Thread;
        public readonly ManualResetEvent Completed = new ManualResetEvent(false);
        public volatile TcpClient TcpClient;
        public volatile Exception Exception;
        public volatile bool Aborted;
        public void Abort() {
            if (Aborted != true) {
                Aborted = true;
                try {
                    if (Thread != null) Thread.Abort();
                } catch (Exception e) {
                    Log.Error(typeof(Internet), e);
                }
            }
        }
    }
}
}
