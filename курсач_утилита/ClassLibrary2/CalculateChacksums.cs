using System;
using System.IO;
using System.Security.Cryptography;

namespace ClassLibrary2
{
    public class CalculateChacksums
    {


        public string ComputeMd5Checksum(string path)
        {
            var fs = File.OpenRead(path);
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                var fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                var checkSum = md5.ComputeHash(fileData);
                var result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                fs.Close();
                return result;
            }
        }
    }
}
