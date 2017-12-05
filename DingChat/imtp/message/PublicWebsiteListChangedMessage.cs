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
using java.util;
using cn.lds.im.sdk.message.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Utils;

namespace cn.lds.chatcore.pcw.imtp.message {
class PublicWebsiteListChangedMessage : Message {
    /**
    * 新加应用
    */
    public List<String> addedAppIds;

    /**
     * 新加应用的名称
     */
    public List<String> addedAppName;

    /**
     * 移除应用
     */
    public List<String> removedAppIds;

    /**
     * 移除应用的名称
     */
    public List<String> remoedAppName;

    public List<String> getAddedAppIds() {
        return addedAppIds;
    }

    public void setAddedAppIds(List<String> addedAppIds) {
        this.addedAppIds = addedAppIds;
    }

    public List<String> getAddedAppName() {
        return addedAppName;
    }

    public void setAddedAppName(List<String> addedAppName) {
        this.addedAppName = addedAppName;
    }

    public List<String> getRemovedAppIds() {
        return removedAppIds;
    }

    public void setRemovedAppIds(List<String> removedAppIds) {
        this.removedAppIds = removedAppIds;
    }

    public List<String> getRemoedAppName() {
        return remoedAppName;
    }

    public void setRemoedAppName(List<String> remoedAppName) {
        this.remoedAppName = remoedAppName;
    }

    public override MsgType getType() {
        return MsgType.PublicWebsiteListChanged;
    }


    public override String createContentJsonStr() {
        return null;
    }

    public override void parse(MsgType type, SendMessage sendMessage) {
        try {
            base.parse(type, sendMessage);
            //            /* 新加应用 */
            //            this.setAddedAppIds(this.getListString(json,"addedAppIds"));
            //            /* 新加应用的名称 */
            //            this.setAddedAppName(this.getListString(json, "addedAppName"));
            //            /* 移除应用 */
            //            this.setRemovedAppIds(this.getListString(json, "removedAppIds"));
            //            /* 移除应用的名称 */
            //            this.setRemoedAppName(this.getListString(json, "remoedAppName"));
        } catch (Exception e) {
            Log.Error(typeof(PublicWebsiteListChangedMessage), e);
            this.setParseError(true);
        }
    }

    /**
     * 从JSON中获取指定的String list对象
     * @param json
     * @param key
     * @return
     */
    public List<String> getListString(JObject json, String key) {
        List<String> lst = new List<String>();
        try {
            //JSONArray jsonArray = json.getJSONArray(key);
            //int length = jsonArray.length();
            //for (int i = 0; i < length; i++) {
            //    JObject o = jsonArray.getJSONObject(i);
            //    lst.add(ToolsHelper.toString(o));
            //}
        } catch (Exception e) {
            Log.Error(typeof(PublicWebsiteListChangedMessage), e);
        }
        return lst;
    }

}
}
