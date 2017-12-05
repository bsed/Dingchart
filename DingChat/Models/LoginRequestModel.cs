using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.im.sdk.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

///登录请求参数
namespace cn.lds.chatcore.pcw.Models {
class LoginRequestModel {
    /** 登录类型 */
    public LoginType loginType;
    /** 登录名 */
    public string loginId;
    /** 手机号码 */
    public string mobile;
    /** 密码 */
    public string captcha;
    /** 短信验证码 */
    public string password;
    /** 一次性认证token */
    public string nonceToken;
    /** 软件类型 */
    public string softwareType;
    /** 软件版本 */
    public string softwareVersion;
    /** 微信登录号 */
    public string authorizationCode;

    /** 设备编号 */
    public string deviceId;
    /** 设备类型 */
    //public DeviceType deviceType;
    /** 操作系统类型 */
    //public OsType osType;
    /** 操作系统版本 */
    public string osVersion;


}
}
