using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClassLibrary2
{
    public class Files
    {
        public IEnumerable<Fil> TraverseTree(string root,bool allfiles)
        {
            // Data structure to hold names of subfolders to be 
            // examined for files.
            var files1 = new List<Fil>();
            var dirs = new Stack<string>(20);

            if (!Directory.Exists(root))
            {
                var f = new Fil { Name = root, Exp = null };
                var o = new List<Fil> { f };
                return o;
            }
            dirs.Push(root);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = Directory.GetDirectories(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                string[] files;
                try
                {
                    files = allfiles ? Directory.GetFiles(currentDir).ToArray() : Directory.GetFiles(currentDir)
                        .Where(file => (new FileInfo(file).CreationTimeUtc.Date.Month == DateTime.Now.Month) 
                            || (new FileInfo(file).CreationTimeUtc.Date.Month == DateTime.Now.Month - 1)).ToArray();
                    
                }

                catch (UnauthorizedAccessException e)
                {

                    Console.WriteLine(e.Message);
                    continue;
                }

                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                // Perform the required action on each file here. 
                // Modify this block to perform your required task. 
                foreach (var file in files)
                {
                    try
                    {
                        // Perform whatever action is required in your scenario.
                        var fi = new FileInfo(file);
                        var str = new Fil { Name = fi.FullName, Exp = fi.Extension };
                        files1.Add(str);
                    }
                    catch (FileNotFoundException e)
                    {
                        // If file was deleted by a separate application 
                        //  or thread since the call to TraverseTree() 
                        // then just continue.
                        Console.WriteLine(e.Message);
                    }
                }

                // Push the subdirectories onto the stack for traversal. 
                // This could also be done before handing the files. 
                foreach (var str in subDirs)
                    dirs.Push(str);
            }
            return files1;
        }           //список файлов

        public int CountFiles(string root)
        {
            return TraverseTree(root,true).Count();
        }
        public static void WriteIniFile(string fileName, string section, string value, string pathApplication)
        {
            var myIni = new IniFile(pathApplication + "\\" + fileName);
            myIni.Write(section, value);
        }

        public static string ReadIniFile(string fileName, string section, string pathApplication)
        {
            var myIni = new IniFile(pathApplication + "\\" + fileName);
            var ret = myIni.Read(section);
            return ret;
        }
    }
}