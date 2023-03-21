using System;
using System.IO;

namespace Peach.DataAccess
{
    public static class FileExtend
    {
        public static bool SaveAllTsByDirc(this string path, string name = "files.txt")
        {
            try
            {
                var pathdic = Path.GetDirectoryName(path);
                if (!Directory.Exists(pathdic))
                    return false;
                if (File.Exists(path))
                    File.Delete(path);
                string[] files = Directory.GetFiles(pathdic, "*.ts");
                StreamWriter streamWriter = new StreamWriter(path, true);
                for (int i = 0; i < files.Length; i++)
                {
                    streamWriter.WriteLine($"file '{files[i]}'");
                }
                streamWriter.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



    }
}
