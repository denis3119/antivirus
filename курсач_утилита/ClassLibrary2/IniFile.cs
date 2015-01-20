using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ClassLibrary2
{
    class IniFile   // revision 10
    {
        readonly string _path;
        readonly string _exe = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string Default, StringBuilder retVal, int size, string filePath);

        public IniFile(string iniPath = null)
        {
            _path = new FileInfo(iniPath ?? _exe + ".ini").FullName;
        }

        public string Read(string key, string section = "курсач_утилита")
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section ?? _exe, key, "", retVal, 255, _path);
            return retVal.ToString();
        }

        public void Write(string key, string value, string section = "курсач_утилита")
        {
            WritePrivateProfileString(section ?? _exe, key, value, _path);
        }

     
    }
}