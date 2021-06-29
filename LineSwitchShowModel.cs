using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBackUp
{
    public class LineSwitchShowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _MonitorPath;
        private string _BackUpPath;
        public string MonitorPath
        {
            get { return this._MonitorPath; }
            set
            {
                this._MonitorPath = value;
                OnPropertyChanged("MonitorPath");
            }
        }
        public string BackUpPath
        {
            get { return this._BackUpPath; }
            set
            {
                this._BackUpPath = value;
                OnPropertyChanged("BackUpPath");
            }
        }

        public LineSwitchShowModel(string monitorPath, string backUpPath)
        {
            this.MonitorPath = monitorPath;
            this.BackUpPath = backUpPath;
        }

        public void OnPropertyChanged(string propname)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
            }
        }
    }
}
