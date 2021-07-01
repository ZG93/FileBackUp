using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBackUp
{
    public class Utilits
    {
        public static String ComputeMD5(String fileName)
        {
            String hashMD5 = String.Empty;
            //检查文件是否存在，如果文件存在则进行计算，否则返回空值
            if (System.IO.File.Exists(fileName))
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                {
                    //计算文件的MD5值
                    System.Security.Cryptography.MD5 calculator = System.Security.Cryptography.MD5.Create();
                    Byte[] buffer = calculator.ComputeHash(fs);
                    calculator.Clear();

                    hashMD5 = Bin2HexString(buffer);
                }//关闭文件流
            }//结束计算
            return hashMD5;
        }//ComputeMD5


        /// <summary>
        /// 判断两个文件是否一样
        /// </summary>
        /// <param name="oldpath"></param>
        /// <param name="newpath"></param>
        /// <returns>true 文件一样</returns>
        public static bool FileCompare(string oldpath, string newpath)
        {
            try
            {
                if (string.Compare(ComputeMD5(oldpath), ComputeMD5(newpath)) == 0)
                    return true;
            }
            catch (Exception)
            {

            }

            return false;
        }

        /// <summary>
        /// byte数组转hexString
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Bin2HexString(byte[] bytes)
        {
            string hexString = string.Empty;

            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }

                hexString = strB.ToString();
            }
            return hexString;
        }

        /// <summary>
        /// hexString转byte[]
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexString2Bin(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);

            if (hex == "")
            {
                byte[] zero = new byte[0];
                return zero;
            }

            if (string.IsNullOrWhiteSpace(hex))
                return null;

            if (hex.Length % 2 != 0)
            {
                hex = "0" + hex;
            }

            byte[] bin = new byte[hex.Length / 2];
            for (int i = 0; i < bin.Length; i++)
                bin[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

            return bin;
        }
    }
}
