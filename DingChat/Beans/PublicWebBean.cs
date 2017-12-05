using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.Beans {
public class PublicWebBean {

    /**
    * 公众号ID
    */
    public String appId {
        get;
        set;
    }

    /**
    * 开放平台服务帐号的所有者用户编号
    */
    public String userNo {
        get;
        set;
    }

    /**
    * 公众号名称
    */

    public String name {
        get;
        set;
    }


    /**
    * 开放平台服务帐号的功能介绍
    */

    public String introduction {
        get;
        set;
    }

    /**
    * 头像
    */
    public String logoId {
        get;
        set;
    }


    /**
    * 开放平台服务帐号的状态
    */

    public String status {
        get;
        set;
    }

    /**
    * 是否提供公众帐号服务
    */
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Boolean includeSubscription {
        get;
        set;
    }

    /**
    * 是否提供网站应用服务
    */
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Boolean includeWebsite {
        get;
        set;
    }

    /**
    * 是否提供第三方本地app
    */

    public String clientType {
        get;
        set;
    }

    /**
    * 是否提供第三方本地app
    */
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Boolean includeMobileApp {
        get;
        set;
    }

    /**
    * 第三方应用包名
    */

    public String androidAppOpenUrl {
        get;
        set;
    }

    /**
    * 第三方应用下载地址
    */
    public String androidDownloadUrl {
        get;
        set;
    }

    /**
    * 第三方应用启动参数
    */

    public List<Dictionary<String,String>> mobileAppParameters {
        get;
        set;
    }

    /**
    * 是否已关注
    */
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Boolean followed {
        get;
        set;
    }

    /**
    * 本地排序
    */
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Int64 sort {
        get;
        set;
    }

    /**
    * 服务器排序
    */
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Int64 appSortIndex {
        get;
        set;
    }

    /**
    * 应用分类ID
    */

    public String appClassificationId {
        get;
        set;
    }

    public String appclaasificationKey {
        get;
        set;
    }

    /**
    * 应用分类name
    */

    public String appClassificationName {
        get;
        set;
    }

    /**
    * 应用地址（用于排序，有地址的排前面，无地址的排后面）
    */

    public String url {
        get;
        set;
    }

    /**
    * 所有人
    */

    public String ownerName {
        get;
        set;
    }

    /**
    * 网站应用的状态(inUse--服务中, inConstruction--建设中
    */

    public String websiteStatus {
        get;
        set;
    }

    /**
    * 是否置顶
    */
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Boolean enableTopmost {
        get;
        set;
    }

    /**
    * 是否允许收消息
    */
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Boolean allowReceiveMessages {
        get;
        set;
    }

    /**
    * 是否允许分享地址
    */
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Boolean allowShareMyLocation {
        get;
        set;
    }

    /**
    * 是否是常用应用
    */
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Boolean commonWebsite {
        get;
        set;
    }

    /**
    * 设为常用时间
    */

    public String commonWebsiteTime {
        get;
        set;
    }

    /**
    * 是否包含应用内组件
    */
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Boolean includeComponent {
        get;
        set;
    }

    /**
    * 应用内组件的Url
    */

    public String componentPhoneUrl {
        get;
        set;
    }
    /**
     * 应用内组件的PC端Url
     */

    public String componentPcUrl {
        get;
        set;
    }

}
}
