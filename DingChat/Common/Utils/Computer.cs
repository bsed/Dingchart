using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace cn.lds.chatcore.pcw.Common.Utils {


public class Computer {
    public static string CpuID; //1.cpu序列号
    public static string MacAddress; //2.mac序列号
    public static string DiskID; //3.硬盘id
    public static string IpAddress; //4.ip地址
    public static string LoginUserName; //5.登录用户名
    public static string ComputerName; //6.计算机名
    public static string SystemType; //7.系统类型
    public static string TotalPhysicalMemory; //8.内存量 单位：M
    public static string DeviceId;//9.联合cpuid和diskid来生成

    //static void Main(string[] args)
    //{
    //    System.Console.WriteLine("CpuID: " + Computer.CpuID);
    //    System.Console.WriteLine("DiskID: " + Computer.DiskID);
    //    System.Console.WriteLine("MacAddress: " + Computer.MacAddress);
    //    System.Console.WriteLine("LoginUserName: " + Computer.LoginUserName);
    //    System.Console.WriteLine("ComputerName: " + Computer.ComputerName);
    //    System.Console.WriteLine("SystemType: " + Computer.SystemType);
    //    System.Console.WriteLine("TotalPhysicalMemory: " + Computer.TotalPhysicalMemory);
    //    System.Console.WriteLine("DeviceId: " + Computer.DeviceId);
    //    System.Console.ReadKey();
    //}



    static Computer() {
        CpuID = GetCpuID();
        MacAddress = GetMacAddress();
        DiskID = GetDiskID();
        IpAddress = GetIPAddress();
        LoginUserName = GetUserName();
        SystemType = GetSystemType();
        TotalPhysicalMemory = GetTotalPhysicalMemory();
        ComputerName = GetComputerName();
        String deviceString = "CPUID:" + CpuID + "-DISKID:" + DiskID;
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] deviceBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(deviceString));

        for (int i = 0; i < deviceBytes.Length; i++) {
            DeviceId = DeviceId + deviceBytes[i].ToString("X");
        }
        DeviceId = "CPC_" + DeviceId;
    }
    //1.获取CPU序列号代码

    static string GetCpuID() {
        try {
            string cpuInfo = "";//cpu序列号
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc) {
                cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
            }
            moc = null;
            mc = null;
            return cpuInfo;
        } catch(Exception e) {
            Log.Error(typeof(Computer), e);
            return "unknow";
        } finally {
        }

    }

    //2.获取网卡硬件地址

    static string GetMacAddress() {
        try {
            string mac = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc) {
                if ((bool)mo["IPEnabled"] == true) {
                    mac = mo["MacAddress"].ToString();
                    break;
                }
            }
            moc = null;
            mc = null;
            return mac;
        } catch(Exception e) {
            Log.Error(typeof(Computer), e);
            return "unknow";
        } finally {
        }

    }

    //3.获取硬盘ID
    static string GetDiskID() {
        try {
            String HDid = "";
            ManagementClass mc = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc) {
                HDid = (string)mo.Properties["Model"].Value;
            }
            moc = null;
            mc = null;
            return HDid;
        } catch(Exception e) {
            Log.Error(typeof(Computer), e);
            return "unknow";
        } finally {
        }

    }

    //4.获取IP地址

    static string GetIPAddress() {
        try {
            string st = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc) {
                if ((bool)mo["IPEnabled"] == true) {
                    //st=mo["IpAddress"].ToString();
                    System.Array ar;
                    ar = (System.Array)(mo.Properties["IpAddress"].Value);
                    st = ar.GetValue(0).ToString();
                    break;
                }
            }
            moc = null;
            mc = null;
            return st;
        } catch(Exception e) {
            Log.Error(typeof(Computer), e);
            return "unknow";
        } finally {
        }

    }

    /// 5.操作系统的登录用户名
    static string GetUserName() {
        try {
            string un = "";

            un = Environment.UserName;
            return un;
        } catch(Exception e) {
            Log.Error(typeof(Computer), e);
            return "unknow";
        } finally {
        }

    }



    //6.获取计算机名
    static string GetComputerName() {
        try {
            return System.Environment.MachineName;

        } catch(Exception e) {
            Log.Error(typeof(Computer), e);
            return "unknow";
        } finally {
        }
    }



    ///7 PC类型
    static string GetSystemType() {
        try {
            string st = "";
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc) {

                st = mo["SystemType"].ToString();

            }
            moc = null;
            mc = null;
            return st;
        } catch(Exception e) {
            Log.Error(typeof(Computer), e);
            return "unknow";
        } finally {
        }
    }



    ///8.物理内存
    static string GetTotalPhysicalMemory() {
        try {

            string st = "";
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc) {

                st = mo["TotalPhysicalMemory"].ToString();

            }
            moc = null;
            mc = null;
            return st;
        } catch(Exception e) {
            Log.Error(typeof(Computer), e);
            return "unknow";
        } finally {
        }

    }


}
}
