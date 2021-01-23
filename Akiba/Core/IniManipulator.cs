namespace Akiba.Core
{
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class IniManipulator
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string @default, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        private readonly string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        private readonly string iniPath;

        public IniManipulator(string iniPath) => this.iniPath = new FileInfo(iniPath).FullName;

        public string Read(string key, string section = null)
        {
            var buffer = new StringBuilder(255);

            _ = GetPrivateProfileString(section ?? this.assemblyName, key, string.Empty, buffer, buffer.Capacity, this.iniPath);

            return buffer.ToString();
        }

        public void Write(string key, string value, string section = null) => _ = WritePrivateProfileString(section ?? this.assemblyName, key, value, this.iniPath);
    }
}
