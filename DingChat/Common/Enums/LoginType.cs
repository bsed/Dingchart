using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using cn.lds.chatcore.pcw.Attributes;

namespace cn.lds.chatcore.pcw.Common.Enums {
/**
 * 登录类型（用户请求登录API使用）
 */
public enum LoginType {
    [Description("000")]
    UNKNOWN,	/** 未知 */
    [Description("id_pass")]
    [Default]
    id_pass,	/** loginId+密码登录 */
    [Description("mobile_pass")]
    mobile_pass,	/** mobile+密码登录 */
    [Description("mobile_captcha")]
    mobile_captcha,	/** mobile+短信验证码登录 */
    [Description("nonceToken")]
    nonceToken,	/** 一次性认证token */
    [Description("authorizationCode")]
    authorizationCode,  /** 微信登录号 */
    [Description("barCode")]
    barCode  /** 扫码登录 */


}
}