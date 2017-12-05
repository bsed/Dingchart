using cn.lds.chatcore.pcw.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.HtmlAdapter {
[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
[System.Runtime.InteropServices.ComVisibleAttribute(true)]
public class TestAdpter: WebAdapter {
    // 脚本调用C#的方法
    public void JsCallSharp(String str1, String str2) {
        Console.WriteLine("JS调用C#实例：str1=" + str1 + ",str2=" + str2);
    }
}
}
