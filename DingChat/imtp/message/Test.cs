using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.lds.chatcore.pcw.imtp.message {
class Test {
    //public static void Main(string[] s)
    //{
    //    Console.WriteLine("ddddd");
    //    Console.WriteLine(MsgType.UNKNOWN.GetHashCode());
    //    Console.WriteLine(MsgType.UNKNOWN);
    //    Console.WriteLine(MsgType.UNKNOWN.GetTypeCode());

    //    TextMessage msg = ReflactClass.createInstance<TextMessage>();
    //    msg.setText("x地方x");
    //    Console.WriteLine(msg.getText());


    //    Console.ReadKey();
    //}
}



public class ReflactClass {
    public static T createInstance<T>() where T : new() {
        return new T();
    }
}
}
