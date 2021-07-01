using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBackUp
{
    public class FileMonitorLoaded
    {
        static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 开启备份时，主动备份一次
        /// </summary>
        /// <param name="mPath"></param>
        /// <param name="bPath"></param>
        public static void Thread(string mPath, string bPath)
        {
            Task.Factory.StartNew(() =>
            {
                CopyFile(mPath, bPath);

                lock (MainWindow.LockObject)
                {
                    if (!MainWindow.cancelFiles.Contains(mPath))
                    {
                        FileListener fileListener = new FileListener();
                        fileListener.WatcherStrat(mPath, "*", true, true, bPath);
                        MainWindow.FileLs.Add(mPath, fileListener);
                    }
                }

            }, TaskCreationOptions.LongRunning);
        }

        public static void CopyFile(string mPath, string bPath)
        {
            //遍历文件夹
            DirectoryInfo theFolder = new DirectoryInfo(mPath);

            FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo NextFile in thefileInfo)  //遍历文件
            {
                Copy(NextFile.FullName, mPath, bPath);
            }

            //遍历子文件夹
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            foreach (DirectoryInfo NextFolder in dirInfo)
            {
                string newpath = NextFolder.FullName.Replace(mPath, bPath);
                DirectoryInfo newFolder = new DirectoryInfo(newpath);
                if (!newFolder.Exists)
                {
                    newFolder.Create();
                }

                CopyFile(NextFolder.FullName, newpath);
            }
        }

        public static void Copy(string FullName, string mPath, string bPath)
        {
            try
            {
                string newpath = FullName.Replace(mPath, bPath);

                if (!Utilits.FileCompare(FullName, newpath))
                {
                    FileInfo aa = new FileInfo(FullName);
                    aa.CopyTo(newpath, true);
                }
            }
            catch (Exception exc)
            {
                Logger.Error($"{exc}");
            }
        }

    }
}
