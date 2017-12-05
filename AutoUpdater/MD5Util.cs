using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Security.Cryptography;


namespace cn.lds.chatcore {
class MD5Util {

    //static void Main(string[] args) {
    //    String md5 = GetMD5HashFromFile("d:\\env_install.tar.gz");
    //    Console.WriteLine(md5);
    //    Console.ReadKey();
    //}

    public static String GetMD5HashFromFile(String fileName) {
        try {
            FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++) {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString().ToUpper();
        } catch (Exception ex) {
            return null;
        }

    }
}
}
