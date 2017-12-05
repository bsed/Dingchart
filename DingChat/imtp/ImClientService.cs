using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.im.sdk;
using cn.lds.im.sdk.notification;
using cn.lds.im.sdk.api;
using cn.lds.im.sdk.enums;
using cn.lds.im.sdk.bean;
using System.Threading;
using EventBus;
using cn.lds.im.sdk.message.util;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.Event.EventData;
using cn.lds.chatcore.pcw.Event.Publisher;
using cn.lds.chatcore.pcw.Common.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Common.Utils;
namespace cn.lds.chatcore.pcw.imtp {
public class ImClientService {
    private static ImClientService imClientService;
    bool imConnect = false;
    bool networkConnect = false;
    ImClient client = null;
    ImtpCallbackListener callbackListener = null;

    bool autoConnectToIm = true;
    bool connecting = false;
    //默认IM连接超时时间间隔为10秒
    int defaultConnectTimeOutInterval = 10;
    //默认ping时间间隔为20秒
    int defaultPingInterval = 20;
    //默认超时时间间隔为60秒
    int defaultTimeoutInterval = 60;
    //默认连接检查时间间隔为10秒
    int defaultTaskInterval = 5;
    String token = null;
    DeviceType deviceType = DeviceType.PC;
    OsType osType = OsType.WINDOWS;

    String deviceId = Computer.DeviceId;
    String osVer = Computer.SystemType;
    String clientNo = App.AccountsModel.no;


    Timer stateTimer = null;
    int interval = 10000;

    private Object locker = new Object();

    long lastConnectTime = 0L;

    private ImtpConnectType imtpConnectType;

    private ImClientService() {
        callbackListener = new ImtpCallbackListener();
    }


    //public void sendMessage(Message message) {
    //    String strFromClientId = this.clientNo;
    //    String strToClientId = message.getTo();// message.getTo();
    //    message.setFrom(strFromClientId);
    //    message.setTimestamp(DateTimeHelper.getTimeStamp());
    //    String sendingMsg = message.createContentJsonStr();
    //    //String strMessageId = LdttAndroidClient.getInstance().generateMessageId();
    //    SendMessage sendMessage = new SendMessage();
    //    sendMessage.setFromClientId(strFromClientId);
    //    sendMessage.setToClientId(strToClientId);
    //    sendMessage.setMessageId(message.getMessageId());
    //    sendMessage.setMessage(sendingMsg);
    //    sendMessage.setMessageType(message.getType().GetHashCode());

    //    sendMessage.setTime(message.getTimestamp());

    //    if (this.client != null) {
    //        this.client.sendMessage(sendMessage);
    //    }
    //}

    public void setImtpConnectType(ImtpConnectType imtpConnectType) {
        this.imtpConnectType = imtpConnectType;
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param Name="sendMessage"></param>
    public void sendMessage(SendMessage sendMessage) {
        try {
            if (this.client != null) {
                this.client.sendMessage(sendMessage);

            }
            //System.Console.WriteLine("发送消息：" + sendMessage);
        } catch (Exception e) {
            Log.Error(typeof(ImClientService), e);
        }
    }
    public static ImClientService getInstance() {
        if (imClientService == null) {
            imClientService = new ImClientService();
        }
        EventBusHelper eventBusHelper = EventBusHelper.getInstance();
        eventBusHelper.register(imClientService);
        return imClientService;
    }

    private void startAutoConnect() {
        if (stateTimer == null) {
            var autoEvent = new AutoResetEvent(false);
            ImClientService imClientService = getInstance();
            stateTimer = new Timer(imClientService.connect,
                                   autoEvent, 1000, interval);

        }
    }

    public void setAutoConnectToIm(bool autoConnectToIm) {
        this.autoConnectToIm = autoConnectToIm;
    }

    public void connect(Object stateInfo) {
        lock (this) {
            long timeSkip = DateTimeHelper.getTimeStamp() - this.lastConnectTime;

            if (this.connecting && (timeSkip > 60 * 1000)) {
                this.connecting  = false;
                this.disConnectFromIm();
            }

            //Console.WriteLine("***************111*******imConnect:" + imConnect + "  ******connecting:" + this.connecting);
            if (this.autoConnectToIm && this.networkConnect && !imConnect && !this.connecting) {
                this.connectToIm();
            }
        }
    }


    public void disConnectFromIm() {
        //1. autoConnectToIm = false
        Log.Info(typeof(ImClientService), "开始断开IM连接，disConnectFromIm...");
        this.autoConnectToIm = false;
        this.connecting = false;
        if (stateTimer != null) {
            stateTimer.Dispose();
            stateTimer = null;
        }
        //注销设备，断开im

        try {
            String url = ServiceCode.UNREGISTER_CONFIG_SERVER_URL;
            if (this.imtpConnectType == ImtpConnectType.real) {
                url = ServiceCode.UNREGISTER_CONFIG_SERVER_URL;
            } else {
                url = ServiceCode.UNREGISTER_CONFIG_SERVER_URL_QRCODE;
            }
            // 准备参数
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            Dictionary<String, Object> body = new Dictionary<String, Object>();
            body.Add("token", this.token);
            body.Add("clientId", this.clientNo);
            body.Add("deviceId", this.deviceId);
            body.Add("deviceType", this.deviceType.getValue());
            body.Add("osType", this.osType.getValue());
            String data = RestRequestHelper.postSync(url, parameters, body, null);
        } catch (Exception e) {
            Log.Error(typeof(ImClientService),e);
        }

        try {
            if (this.client != null) {
                this.client.disConnect();
            }
        } catch (Exception e) {
            Log.Error(typeof(ImClientService), e);
        }


    }

    public void connectToIm() {
        lock (locker) {
            try {
                //Console.WriteLine("》》》》》》》》 调用connectToIm "+ DateTimeHelper.getTimeStamp());
                Log.Info(typeof (ImClientService), "开始进行IM连接，connectToIm...");
                lastConnectTime = DateTimeHelper.getTimeStamp();
                connecting = true;
                this.autoConnectToIm = true;
                //连接tms注册设备，然后进行im连接

                //C1N76X1JYF8GK 曲玮


                Dictionary<String, Object> parameters = new Dictionary<String, Object>();
                Dictionary<String, Object> body = new Dictionary<String, Object>();



                String url = ServiceCode.CONFIG_SERVER_URL;
                if (this.imtpConnectType == ImtpConnectType.real) {
                    url = ServiceCode.CONFIG_SERVER_URL;
                    this.clientNo = App.AccountsModel.no;
                    body.Add("clientId", this.clientNo);
                } else {
                    url = ServiceCode.CONFIG_SERVER_URL_QRCODE;
                    this.clientNo = this.deviceId;
                    body.Add("clientId", this.clientNo);
                }
                body.Add("deviceId", this.deviceId);
                body.Add("deviceType", this.deviceType.getValue());
                body.Add("osType", this.osType.getValue());
                String data = RestRequestHelper.postSync(url, parameters, body, null);
                if (data == null) {
                    this.connecting = false;
                } else {
                    JObject json = JObject.Parse(data);

                    String tcsIp = json.GetValue("host").ToStr();
                    int tcsPort = json.GetValue("socketPort").ToObject<int>();
                    token = json.GetValue("token").ToStr();
                    if (this.token == null || "".Equals(this.token)) {
                        // 如果没有获取到token，就随机赋值一个时间戳
                        this.token = DateTimeHelper.getTimeStamp().ToStr();
                    }
                    String clientId = json.GetValue("clientId").ToStr();


                    String session = "session";
                    //api请求成功或者失败，connecting=false


                    ImConnectOptions options = new ImConnectOptions(tcsIp, tcsPort,
                            this.clientNo, this.deviceId, this.token, session,
                            this.osType, this.osVer, this.deviceType);

                    //其他属性需要查看现在android的设置！！！！！
                    options.setCallback(this.callbackListener);
                    options.setConnectTimeOutSecond(this.defaultConnectTimeOutInterval);
                    options.setPingInterval(this.defaultPingInterval);
                    options.setTimeOut(this.defaultTimeoutInterval);
                    if (this.imtpConnectType == ImtpConnectType.real) {
                        options.setQrCodeType(null);
                    } else {
                        options.setQrCodeType(QRCodeType.WINDOWS);
                    }
                    if (client == null) {
                        client = new ImClient(options);
                    } else {
                        client.setImConnectOptions(options);
                    }
                    try {
                        client.connect();
                    } catch (Exception e) {
                        this.connecting = false;
                        Log.Error(typeof (ImClientService), e);
                    }
                }
            } catch (Exception e) {
                this.connecting = false;
                Log.Error(typeof (ImClientService), e);
            }

            this.startAutoConnect();
            //同时启动网络监测
            Internet.getInstance().start();
        }

    }
    //event notify
    [EventSubscriber]
    public void onNetworkConnected(FrameEvent<Object> eventData) {

        if (eventData.frameEventDataType.Equals(FrameEventDataType.NETWORK_SUCCESS)) {

            String a = System.Reflection.Assembly.GetEntryAssembly().Location.Substring(System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1).Replace(".exe", "");

            this.networkConnect = true;
            if (!this.imConnect && !this.connecting && this.autoConnectToIm) {
                Log.Info(typeof(ImClientService),"网络已经连接，准备连接IM");
                this.connectToIm();
            }



        }
    }
    //event notify
    [EventSubscriber]
    public void onNetworkConnectionLost(FrameEvent<Object> eventData) {

        if (eventData.frameEventDataType.Equals(FrameEventDataType.NETWORK_ERROR)) {
            Log.Info(typeof(ImClientService), "网络连接已经断开");
            this.networkConnect = false;
        }
    }

    //event notify
    [EventSubscriber]
    public void onImConnected(FrameEvent<Object> eventData) {

        if (eventData.frameEventDataType.Equals(FrameEventDataType.IM_CONNECTED)) {
            Log.Info(typeof(ImClientService), "IM连接成功");
            this.connecting = false;
            this.imConnect = true;


        }

    }

    //event notify
    [EventSubscriber]
    public void onImConnectionLost(FrameEvent<Object> eventData) {

        if (eventData.frameEventDataType.Equals(FrameEventDataType.IM_CONNECTION_LOST)) {
            Log.Info(typeof(ImClientService), "IM连接丢失");
            this.connecting = false;
            this.imConnect = false;

            FrameEvent<Object> frameEvent = new FrameEvent<Object>();
            eventData.frameEventDataType = FrameEventDataType.DISCONNECT_FROM_IM;
            EventBusHelper.getInstance().fireEvent(frameEvent);
        }
    }

    [EventSubscriber]
    public void onImConnectionError(FrameEvent<Object> eventData) {

        if (eventData.frameEventDataType.Equals(FrameEventDataType.IM_CONNECTION_ERROR)) {
            Log.Info(typeof(ImClientService), "IM连接出现异常！！！！！！");
            this.connecting = false;
            this.imConnect = false;

            FrameEvent<Object> frameEvent = new FrameEvent<Object>();
            eventData.frameEventDataType = FrameEventDataType.DISCONNECT_FROM_IM;
            EventBusHelper.getInstance().fireEvent(frameEvent);
        }
    }

    public bool isImConnected() {
        return this.imConnect;
    }

    /// <summary>
    /// 生成消息ID
    /// </summary>
    /// <returns></returns>
    public String generateMessageId() {
        //String messageId = MessageUtil.generateMessageId();
        return Guid.NewGuid().ToString();

    }
}
}
