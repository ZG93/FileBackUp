using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileBackUp
{
    public class FileListener
    {
        public static FileSystemWatcher watcher = new FileSystemWatcher();
        public static string srcPath;
        public static string TargetPath;


        /// <summary>
        /// 初始化监听
        /// </summary>
        /// <param name="StrWarcherPath">需要监听的目录</param>
        /// <param name="FilterType">需要监听的文件类型(筛选器字符串)</param>
        /// <param name="IsEnableRaising">是否启用监听</param>
        /// <param name="IsInclude">是否监听子目录</param>
        /// <param name="targetPath">备份目录</param>
        public void WatcherStrat(string StrWarcherPath, string FilterType, bool IsEnableRaising, bool IsInclude, string targetPath)
        {
            srcPath = StrWarcherPath;
            TargetPath = targetPath;
            watcher = new FileSystemWatcher(StrWarcherPath, FilterType);

            //初始化监听
            watcher.BeginInit();
            //设置监听文件类型
            watcher.Filter = FilterType;
            //设置是否监听子目录
            watcher.IncludeSubdirectories = IsInclude;
            //设置是否启用监听?
            watcher.EnableRaisingEvents = IsEnableRaising;
            //设置需要监听的更改类型(如:文件或者文件夹的属性,文件或者文件夹的创建时间;NotifyFilters枚举的内容)
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            //设置监听的路径
            watcher.Path = StrWarcherPath;
            //注册创建文件或目录时的监听事件
            //watcher.Created += new FileSystemEventHandler(watch_created);
            //注册当指定目录的文件或者目录发生改变的时候的监听事件
            watcher.Changed += new FileSystemEventHandler(watch_changed);
            //注册当删除目录的文件或者目录的时候的监听事件
            watcher.Deleted += new FileSystemEventHandler(watch_deleted);
            //当指定目录的文件或者目录发生重命名的时候的监听事件
            watcher.Renamed += new RenamedEventHandler(watch_renamed);
            //结束初始化
            watcher.EndInit();
        }

        /// <summary>
        /// 启动或者停止监听
        /// </summary>
        /// <param name="IsEnableRaising">True:启用监听,False:关闭监听</param>
        public void WatchStartOrSopt(bool IsEnableRaising)
        {
            watcher.EnableRaisingEvents = IsEnableRaising;
        }

        /// <summary>
        /// 创建文件或者目录时的监听事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void watch_created(object sender, FileSystemEventArgs e)
        {
            FugaiFile m = new FugaiFile();
            m.sender = sender;
            m.e = e;
            m.srcPath = srcPath;
            m.TargetPath = TargetPath;

            Thread t = new Thread(new ThreadStart(m.C));
            t.Start();
        }

        private static void watch_changed(object sender, FileSystemEventArgs e)
        {
            FugaiFile m = new FugaiFile();
            m.sender = sender;
            m.e = e;
            m.srcPath = srcPath;
            m.TargetPath = TargetPath;

            Thread t = new Thread(new ThreadStart(m.C));
            t.Start();

        }

        private static void watch_deleted(object sender, FileSystemEventArgs e)
        {
            //事件内容
            //MessageBox.Show("监听到删除事件" + e.FullPath);
        }

        private static void watch_renamed(object sender, RenamedEventArgs e)
        {
            RFugaiFile m = new RFugaiFile();
            m.sender = sender;
            m.e = e;
            m.srcPath = srcPath;
            m.TargetPath = TargetPath;

            Thread t = new Thread(new ThreadStart(m.ReName));
            t.Start();
        }
    }

    class FugaiFile
    {
        public object sender;
        public FileSystemEventArgs e;
        public string srcPath;
        public string TargetPath;

        public void C()
        {
            String fullname = e.FullPath;
            string newpath = fullname.Replace(srcPath, TargetPath);

            FileInfo nf = new FileInfo(newpath);
            if (!nf.Directory.Exists)
            {
                nf.Directory.Create();
            }

            //DirectoryInfo newFolder = new DirectoryInfo(newpath);
            //if (!newFolder.Exists)
            //{
            //    newFolder.Create();
            //}

            if (!Utilits.FileCompare(fullname, newpath))
            {
                FileInfo aa = new FileInfo(e.FullPath);
                aa.CopyTo(newpath, true);
            }
        }
    }

    class RFugaiFile
    {
        public object sender;
        public RenamedEventArgs e;
        public string srcPath;
        public string TargetPath;

        public void ReName()
        {
            string oldpath = e.OldFullPath.Replace(srcPath, TargetPath);
            string newpath = e.FullPath.Replace(srcPath, TargetPath);

            if (File.Exists(oldpath))
            {
                File.Move(oldpath, newpath);
            }

            if (Directory.Exists(oldpath))
            {
                Directory.Move(oldpath, newpath);
            }
        }
    }

}
