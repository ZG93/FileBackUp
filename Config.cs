using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileBackUp
{

    public class Config
    {
        public const string CString = "application.json";

        //自启动
        public bool SelfStarting { get; set; } = false;
        public Dictionary<string, string> Dics { get; set; }

        private static Config _config;
        public static Config GetConfig()
        {
            if (_config != null)
                return _config;

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), CString)))
                throw new System.IO.FileNotFoundException("没有发现配置文件。");

            var json = System.IO.File.ReadAllText(CString, Encoding.UTF8);
            _config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(json);
            return _config;
        }

        public static void Write(Config config)
        {
            File.WriteAllText(CString, Newtonsoft.Json.JsonConvert.SerializeObject(config));
            _config = config;
        }
    }
}
