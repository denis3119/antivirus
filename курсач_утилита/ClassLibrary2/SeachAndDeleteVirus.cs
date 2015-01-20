using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ClassLibrary2
{
    public class SeachAndDeleteVirus
    {
        private static List<string> Readfile(string path)
        {
            var s = File.OpenText(path);
            string read;
            var list = new List<string>();
            while ((read = s.ReadLine()) != null)
            {
                list.Add(read);
            }
            s.Close();
            return list;
        }

        
        public List<string> VirusSearchAllMethods(string path, string pathApplication, bool allfiles, int checkOption,
                                                    int sizefile)
        {
            var viruslist = new List<string>();
            var md5 = Readfile(pathApplication + "\\md5.txt");
            var viruslistbits = Readfile(pathApplication + "\\b_virus.txt");

            var files = new Files().TraverseTree(path, allfiles);
            //список исполняемых файлов
            var fileswithoutnotvirus = (from file in files
                                        where file.Exp == null || file.Exp.Contains("exe") ||
                                              file.Exp.Contains("dll") || file.Exp.Contains("vbs") ||
                                              file.Exp.Contains("bat") || file.Exp.Contains("sys")
                                        select file.Name).ToList();

            var check = new CalculateChacksums();
            foreach (var file in fileswithoutnotvirus)
            {
                if(new FileInfo(file).Length>=sizefile*1024*1024 && sizefile!=0)    continue;
                if ((checkOption == 1) || (checkOption == 0))
                {
                    var temp = check.ComputeMd5Checksum(file);
                    if (md5.Contains(temp))
                    {
                        var tuu = new FileInfo(file);

                   //     File.Move(file, pathApplication + "\\quarantine\\" + tuu.Name   + ".vir");
                        File.Move(file, pathApplication + "\\quarantine\\" + tuu.Name + ".vir(" 
                            + DateTime.Now.ToString(CultureInfo.InvariantCulture)
                            .Replace('\\', '.').Replace(':', '.').Replace('/', '.') + ")");
                        viruslist.Add(file);
                        continue;
                    } // if (!allfiles) continue;
                }
                if ((checkOption != 0) && (checkOption != 2)) continue;
                var fs = File.OpenRead(file);
                {

                    var fileData = new byte[fs.Length];
                    fs.Read(fileData, 0, (int)fs.Length);
                    foreach (var viruslistbit in viruslistbits)
                    {
                        var str = "";
                        var i = 0;
                        foreach (var b in fileData)
                        {
                            str += b;
                            i++;
                        }
                        if (!str.Contains(viruslistbit)) continue;
                        var tuu = new FileInfo(file);
                        //File.Move(file, Application.StartupPath + "\\quarantine\\" + tuu.Name + tuu.Extension + ".vir");
                        fs.Close();
                        File.Move(file, pathApplication + "\\quarantine\\" + tuu.Name+ ".vir("+DateTime.Now+")");
                        viruslist.Add(file);
                    }
                }
            }
            return viruslist;
        }





    }
}