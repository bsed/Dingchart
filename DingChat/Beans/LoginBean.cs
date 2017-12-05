using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class LoginBean {
    /*
     {
    "accountExpired": false,
    "accountLocked": false,
    "authorities": [],
    "credentialsExpired": false,
    "email": null,
    "loginId": "quwei",
    "mobile": "13998529445",
    "Name": null,
    "nickname": "曲玮",
    "no": "C1NPG9FDA7R7Q",
    "nonceToken": null,
    "orgs": [
    20
    ],
    "subscriptionOpenId": null
    }
     */

    public String loginId;
    public String mobile;
    public String name;
    public String nickname;
    public String no;
    public String nonceToken;

    public Dictionary<string, LoginBeanTenants> tenants;
}
}
