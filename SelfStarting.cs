using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBackUp
{
    public class SelfStarting
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 开启或关闭自启动
        /// </summary>
        /// <param name="isopen"></param>
        //public static void OpenOrClose(bool? isopen)
        //{
        //    if (isopen == true)
        //    {
        //        string StartupPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonStartup);
        //        string dir = Directory.GetCurrentDirectory();
        //        string exeDir = dir + @"\FileBackUp.exe.lnk";
        //        File.Copy(exeDir, StartupPath + @"\FileBackUp.exe.lnk", true);
        //    }
        //    else
        //    {
        //        string StartupPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonStartup);
        //        System.IO.File.Delete(StartupPath + @"\FileBackUp.exe.lnk");
        //    }
        //}


        /// <summary>
        /// 开机自动启动
        /// </summary>
        /// <param name="started">设置开机启动，或取消开机启动</param>
        /// <param name="exeName">注册表中的名称</param>
        /// <returns>开启或停用是否成功</returns>
        public static void OpenOrClose(bool? started)
        {
            RegistryKey key = null;
            try
            {

                string exeDir = System.Windows.Forms.Application.ExecutablePath;
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开注册表子项

                if (key == null)//如果该项不存在的话，则创建该子项
                {
                    key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                if (started == true)
                {
                    try
                    {
                        object ob = key.GetValue("FileBackUp", -1);

                        if (!ob.ToString().Equals(exeDir))
                        {
                            if (!ob.ToString().Equals("-1"))
                            {
                                key.DeleteValue("FileBackUp");//取消开机启动
                            }
                            key.SetValue("FileBackUp", exeDir);//设置为开机启动
                        }
                        key.Close();

                    }
                    catch (Exception ex)
                    {
                        logger.Error($"{ex}");
                    }
                }
                else
                {
                    try
                    {
                        key.DeleteValue("FileBackUp");//取消开机启动
                        key.Close();
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"{ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                if (key != null)
                {
                    key.Close();
                }
                logger.Error($"{ex}");
            }
        }

    }
}
