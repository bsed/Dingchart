using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Services;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.imtp;
using cn.lds.chatcore.pcw.Views.Windows;
using Newtonsoft.Json;
using System.Threading;
using cn.lds.chatcore.pcw.Business;
using cn.lds.chatcore.pcw.imtp.message;

namespace cn.lds.chatcore.pcw.Services {
public class LoginByBarcodeService {
    private static LoginByBarcodeService instance = null;
    public static LoginByBarcodeService getInstance() {
        if (instance == null) {
            instance = new LoginByBarcodeService();
        }
        return instance;
    }
    //校验码
    public String checkCode;
    // 手机扫码或者登录人的NO（扫码收到等待登录消息或启动时有上次登录人时，重置该值）
    public String toClientId;
    /// <summary>
    ///执行IM连接（扫码登录，临时连接IM）
    /// </summary>
    /// <param Name="ojb"></param>
    public void conectToIm() {
        try {
            ThreadPool.QueueUserWorkItem(conectToImQrcode);
        } catch (Exception e) {
            Log.Error(typeof(LoginByBarcodeService), e);
        }
    }
    /// <summary>
    /// 运行IM连接（扫码登录，临时连接IM）
    /// </summary>
    /// <param Name="ojb"></param>
    private void conectToImQrcode(object ojb) {
        try {
            ImClientService.getInstance().setImtpConnectType(ImtpConnectType.qrCode);
            ImClientService.getInstance().connectToIm();
        } catch (Exception e) {
            Log.Error(typeof(LoginByBarcodeService), e);
        }
    }

    /// <summary>
    ///  生成一个校验码
    /// </summary>
    public void CreateCheckCode() {
        try {
            this.checkCode = DateTimeHelper.getTimeStamp().ToStr();
        } catch (Exception e) {
            Log.Error(typeof(LoginByBarcodeService), e);
        }
    }

    /// <summary>
    ///断开IM连接（扫码登录，临时连接IM）
    /// </summary>
    /// <param Name="ojb"></param>
    public void disConectToIm() {
        try {
            ImClientService.getInstance().disConnectFromIm();
            // 调用API注销IM

        } catch (Exception e) {
            Log.Error(typeof(LoginByBarcodeService), e);
        }
    }

    /// <summary>
    /// 获取登录用二维码
    /// </summary>
    public void GetQrcode() {
        try {
            String url = ServiceCode.GET_LOGIN_QRCODE;
            url = url.Replace("{deviceId}", Computer.DeviceId);
            // 准备参数
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            Dictionary<String, Object> body = new Dictionary<String, Object>();
            String data = RestRequestHelper.postSync(url, parameters, body, null);
            if (data != null && !String.Empty.Equals(data)) {
                QrCodeBean qrCodeBean = JsonConvert.DeserializeObject<QrCodeBean>(data);
                //下载二维码
                DownloadServices.getInstance().DownloadQrcodeMethod(qrCodeBean.qrcodeId, DownloadType.SYSTEM_BARCODE, null);

            }
        } catch (Exception e) {
            Log.Error(typeof(LoginByBarcodeService), e);
        }

    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param Name="sendMessageBean"></param>
    private void send(SendMessageBean sendMessageBean) {
        try {
            sendMessageBean.fromClientId = Computer.DeviceId;
            sendMessageBean.messageId = ImClientService.getInstance().generateMessageId();
            sendMessageBean.time = DateTimeHelper.getTimeStamp();
            sendMessageBean.toClientId = this.toClientId;

            ContactsApi.sendDispatchQrMessage(sendMessageBean);
        } catch (Exception e) {
            Log.Error(typeof(LoginByBarcodeService), e);
        }
    }

    /// <summary>
    /// 发送登录请求消息
    /// 再次登录点击登陆按钮时发送该消息
    /// </summary>
    public void sendLoginRequestMessage() {
        try {
            SendMessageBean sendMessageBean = new SendMessageBean();
            sendMessageBean.messageType = MsgType.LoginRequest.GetHashCode();
            LoginRequestMessage loginRequestMessage = new LoginRequestMessage();
            loginRequestMessage.checkCode = this.checkCode;
            loginRequestMessage.clientId = Computer.DeviceId;
            loginRequestMessage.sendTimestamp = DateTimeHelper.getTimeStamp();
            sendMessageBean.message = loginRequestMessage.createContentJsonStr();
            this.send(sendMessageBean);
        } catch (Exception e) {
            Log.Error(typeof(LoginByBarcodeService), e);
        }
    }

    /// <summary>
    /// 发送登录状态消息
    /// 登录到系统，关闭系统时发送
    /// </summary>
    public void sendLoginStatusMessage(PcLoginStatus pcLoginStatus) {
        try {
            // 如果未登录，则不需要发送登录状态消息
            if (string.IsNullOrEmpty(App.AccountsModel.no)) {
                return;
            }
            SendMessageBean sendMessageBean = new SendMessageBean();
            sendMessageBean.messageType = MsgType.LoginStatus.GetHashCode();
            LoginStatusMessage loginStatusMessage = new LoginStatusMessage();
            loginStatusMessage.checkCode = this.checkCode;
            loginStatusMessage.clientId = App.AccountsModel.no;
            loginStatusMessage.sendTimestamp = DateTimeHelper.getTimeStamp();
            loginStatusMessage.status = pcLoginStatus.GetHashCode().ToStr();
            sendMessageBean.message = loginStatusMessage.createContentJsonStr();
            this.send(sendMessageBean);
        } catch (Exception e) {
            Log.Error(typeof(LoginByBarcodeService), e);
        }
    }
}
}
