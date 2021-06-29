using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;

namespace FileBackUp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ObservableCollection<LineSwitchShowModel> Lines_Model = new ObservableCollection<LineSwitchShowModel>();

        public static Dictionary<string, FileListener> FileLs = new Dictionary<string, FileListener>();

        public static Config oConfig = Config.GetConfig();

        System.Windows.Forms.NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            icon();
        }

        private void icon()
        {
            this.notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                BalloonTipText = "Hello, 文件监视器", //设置程序启动时显示的文本
                Text = "文件监视器",//最小化到托盘时，鼠标点击时显示的文本
                Icon = new Icon("FileBackUp.ico"),//程序图标
                Visible = true
            };

            //右键菜单--打开菜单项
            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("Open");
            open.Click += new EventHandler(ShowWindow);
            //右键菜单--退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("Exit");
            exit.Click += new EventHandler(CloseWindow);
            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            this.notifyIcon.ShowBalloonTip(1000);
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void ShowWindow(object sender, EventArgs e)
        {
            WindowState = WindowState.Normal;
            this.Visibility = System.Windows.Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            var ws = WindowState;
            if (ws == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddrListView.ItemsSource = Lines_Model;
            foreach (var m in oConfig.Dics)
            {
                FileListener fileListener = new FileListener();
                fileListener.WatcherStrat(m.Key, "*", true, true, m.Value);
                FileLs.Add(m.Key, fileListener);
                Lines_Model.Add(new LineSwitchShowModel(m.Key, m.Value));

                FileMonitorLoaded.Thread(m.Key, m.Value);
            }

            WindowState = WindowState.Minimized;

        }

        public void ShowStatus(string str)
        {
            this.Status.Text = str;
        }

        /// <summary>
        /// 自启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelfStarting_Click(object sender, RoutedEventArgs e)
        {
            SelfStarting.OpenOrClose(this.selfStart_CheckBox.IsChecked);
            oConfig.SelfStarting = (bool)this.selfStart_CheckBox.IsChecked;
            Config.Write(oConfig);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown(0);
        }

        /// <summary>
        /// 填加监控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBackUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.MonitorDicPath.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("监控目录不能为空");
                return;
            }

            if (this.BackUpPath.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("备份目录不能为空");
                return;
            }

            if (oConfig.Dics.ContainsKey(this.MonitorDicPath.Text))
            {
                System.Windows.MessageBox.Show("已启用此目录监控备份");
                return;
            }

            Lines_Model.Add(new LineSwitchShowModel(this.MonitorDicPath.Text, this.BackUpPath.Text));

            oConfig.Dics.Add(this.MonitorDicPath.Text, this.BackUpPath.Text);
            Config.Write(oConfig);

            FileListener fileListener = new FileListener();
            fileListener.WatcherStrat(this.MonitorDicPath.Text, "*", true, true, this.BackUpPath.Text);
            FileLs.Add(this.MonitorDicPath.Text, fileListener);

            FileMonitorLoaded.Thread(this.MonitorDicPath.Text, this.BackUpPath.Text);

            this.MonitorDicPath.Clear();
            this.BackUpPath.Clear();
        }

        /// <summary>
        /// 选择备份目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectBackUp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.Description = "请选择备份目录";
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.BackUpPath.Text = folderBrowser.SelectedPath;
            }
        }

        /// <summary>
        /// 选择监控目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectMonitorDic_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.Description = "请选择监控目录";
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.MonitorDicPath.Text = folderBrowser.SelectedPath;
            }
        }

        /// <summary>
        /// 取消备份
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBackUp_Click(object sender, RoutedEventArgs e)
        {
            var sc = this.AddrListView.SelectedItem as LineSwitchShowModel;
            if (sc == null)
                return;

            Lines_Model.Remove(sc);

            if (oConfig.Dics.ContainsKey(sc.MonitorPath))
                oConfig.Dics.Remove(sc.MonitorPath);
            Config.Write(oConfig);

            if (FileLs.ContainsKey(sc.MonitorPath))
            {
                FileLs[sc.MonitorPath].WatchStartOrSopt(false);
                FileLs.Remove(sc.MonitorPath);
            }
        }

    }
}
