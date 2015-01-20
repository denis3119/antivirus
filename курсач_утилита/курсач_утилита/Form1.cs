using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClassLibrary2;
using курсач_утилита.Properties;

namespace курсач_утилита
{

    public partial class Form1 : Form
    {
        private void Form1_Resize(object sender, EventArgs e)
        {
            // проверяем наше окно, и если оно было свернуто, делаем событие       
            if (WindowState != FormWindowState.Minimized) return;
            // прячем наше окно из панели
            ShowInTaskbar = false;
            // делаем нашу иконку в трее активной
            notifyIcon1.Visible = true;
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // делаем нашу иконку скрытой
            //    notifyIcon1.Visible = false;
            // возвращаем отображение окна в панели
            ShowInTaskbar = true;
            //разворачиваем окно
            WindowState = FormWindowState.Normal;
        }

        public Form1()
        {
            InitializeComponent();
            // делаем невидимой нашу иконку в трее
            //    notifyIcon1.Visible = false;
            // добавляем Эвент или событие по 2му клику мышки,
            //вызывая функцию  notifyIcon1_MouseDoubleClick
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // добавляем событие на изменение окна
            Resize += Form1_Resize;

            Application.ApplicationExit += OnApplicationExit;
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            var t = DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace(':', '.').Replace('/','.');
            //  File.WriteAllText(Application.StartupPath+@"\log\"+t+".txt", @"содержимое - что будем записывать в него");
            var streamWriter = new StreamWriter(Application.StartupPath + @"\log\" + t + ".txt");
            var tex = textBox1.Text;
            streamWriter.WriteLine(tex);
            streamWriter.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            int seach = comboBox1.SelectedIndex;
            int proverka = comboBox2.SelectedIndex;
            var log = numericUpDown1.Value;
            var temp = Application.StartupPath;
            var file = new Files();
            const string f = "config.ini";
            Files.WriteIniFile(f, "search", seach.ToString(CultureInfo.InvariantCulture), temp);
            Files.WriteIniFile(f, "prov", proverka.ToString(CultureInfo.InvariantCulture), temp);
            Files.WriteIniFile(f, "log", Convert.ToInt32(log).ToString(CultureInfo.InvariantCulture), temp);
            log_add("Настройки сохранены");
        }

        private void button2_Click(object sender, EventArgs e) //speed
        {
            DoTask();

        }

        private void DoTask()
        {
            try
            {
                log_add("Быстрая проверка");

                var tempPath = Application.StartupPath;
                var inifile = new Files();
                Files.WriteIniFile("config.ini", "lastscan", DateTime.Now.ToString(CultureInfo.InvariantCulture), tempPath);
                listBox1.Items.Clear();
                const string path = "c:\\тест"; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11
                var count = new Files().CountFiles(path);
                var t = new SeachAndDeleteVirus();
                label5.Text = count.ToString(CultureInfo.InvariantCulture);


                var temp = t.VirusSearchAllMethods(path, tempPath, (comboBox1.SelectedIndex == 0),
                    comboBox2.SelectedIndex, Convert.ToInt32(numericUpDown1.Value));
                label7.Text = temp.Count.ToString(CultureInfo.InvariantCulture);

                foreach (var variable in temp)
                {
                    listBox1.Items.Add(variable);
                }

                log_add("Проверено файлов: " + label5.Text + " из них вирусов: " + label7.Text);
                log_add("----------");
            }

            catch (IOException ex)
            {
                if (ex.Source != null)
                {
                    log_add(ex.Message + " (in- " + ex.Source + ")");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                log_add(DateTime.Now.ToString(CultureInfo.InvariantCulture));

                // textBox1.Lines = "Выборочная проверка";
                log_add("Выборочная проверка");
                var inifile = new Files();
                Files.WriteIniFile("config.ini", "lastscan", DateTime.Now.ToString(CultureInfo.InvariantCulture), Application.StartupPath);
                folderBrowserDialog1.ShowDialog();
                //   var files = new Files();
                button3.Enabled = false;

                var t = new SeachAndDeleteVirus();
                if (!string.IsNullOrEmpty(folderBrowserDialog1.SelectedPath))
                {
                    var count = new Files().CountFiles(folderBrowserDialog1.SelectedPath);
                    log_add("Выбрана папка: " + folderBrowserDialog1.SelectedPath);
                    label5.Text = count.ToString(CultureInfo.InvariantCulture);
                   var templist  = t.VirusSearchAllMethods(folderBrowserDialog1.SelectedPath, Application.StartupPath,
                        (comboBox1.SelectedIndex == 0), comboBox2.SelectedIndex, Convert.ToInt32(numericUpDown1.Value))
                        .Count.ToString(CultureInfo.InvariantCulture);
                    label7.Text = templist.Count().ToString();
                    foreach (var VARIABLE in templist)
                    {
                        listBox1.Items.Add(VARIABLE);
                    }
                    //  t.VirusSearchAllMethods("c:\\1.exe");
                    log_add("Проверено файлов: " + label5.Text + " из них вирусов: " + label7.Text);
                    Show_message_tray("Проверка завершена успешно");
                }
                else
                {
                    log_add("Пустая папка не может быть выбрана");
                    Show_message_tray("Проверка завершена с ошибкой, проверьте лог");
                }
                log_add("----------");
                log_add(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                button3.Enabled = true;
            }
            catch (IOException ex)
            {
                if (ex.Source != null)
                {
                    log_add(ex.Message + " (in- " + ex.Source + ")");
                }
            }
        }

        private void Show_message_tray(string message)
        {
            notifyIcon1.BalloonTipText = message;
            notifyIcon1.BalloonTipTitle = Resources.Form1_Show_message_tray_Уведомление;
            notifyIcon1.ShowBalloonTip(4000);
        }
        private void log_add(string newline)
        {
            textBox1.Text = textBox1.Text + Environment.NewLine + newline;
        }
        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private int toint(string s)
        {
            return Convert.ToInt32(s);
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            var file = new Files();
            const string f = "config.ini";
            var temp = Application.StartupPath;
            var se = Files.ReadIniFile(f, "search", temp);
            var prov = Files.ReadIniFile(f, "prov", temp);
            var log = Files.ReadIniFile(f, "log", temp);
            comboBox1.SelectedIndex = toint(se);
            comboBox2.SelectedIndex = toint(prov);
            numericUpDown1.Value = toint(log);
        }



       

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();
                log_add("Полная проверка");
                var drives = Environment.GetLogicalDrives();
                foreach (var s in drives)
                {

                    var t = new SeachAndDeleteVirus();
                    var list = t.VirusSearchAllMethods(s, Application.StartupPath,
                        (comboBox1.SelectedIndex == 0), comboBox2.SelectedIndex, Convert.ToInt32(numericUpDown1.Value));
                    foreach (var variable in list)
                    {
                        listBox1.Items.Add(variable);
                    }

                }
                var inifile = new Files();
                Files.WriteIniFile("config.ini", "lastscan", DateTime.Now.ToString(CultureInfo.InvariantCulture), Application.StartupPath);
                log_add("Найдено вирусов: " + listBox1.Items.Count);

                log_add("----------");
            }

            catch (IOException ex)
            {
                if (ex.Source != null)
                {
                    log_add(ex.Message + " (in- " + ex.Source + ")");
                }
            }
        }

        private void tabPage5_Enter(object sender, EventArgs e)
        {
            listBox2.Items.Clear();

            var p = new DirectoryInfo(Application.StartupPath + "\\quarantine\\").EnumerateFiles();
            var fileInfos = p as IList<FileInfo> ?? p.ToList();
            label10.Text = @"Всего: " + fileInfos.Count();
            foreach (var temp in fileInfos.Select(fileInfo => fileInfo.Name))
            {
                listBox2.Items.Add(temp);
            }
        }



        private void вернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedListboxIndex = listBox2.SelectedIndex;
            if (selectedListboxIndex == -1) return;
            var temp = listBox2.Items[selectedListboxIndex].ToString().Split('.');
            folderBrowserDialog1.ShowDialog();
            if (string.IsNullOrEmpty(folderBrowserDialog1.SelectedPath)) return;
            Directory.CreateDirectory(folderBrowserDialog1.SelectedPath);
            File.Move(Application.StartupPath + "\\quarantine\\" + listBox2.Items[selectedListboxIndex],
                folderBrowserDialog1.SelectedPath+'\\' + temp[0] + "." + temp[1]);
            log_add("Файл перенесен из карантина: " + folderBrowserDialog1.SelectedPath + '\\' + temp[0] + "." + temp[1]);
            tabPage5_Enter(sender,e);
        }

        

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedListboxIndex = listBox2.SelectedIndex;
            if (selectedListboxIndex != -1)
            {
                File.Delete(Application.StartupPath + "\\quarantine\\" + listBox2.Items[selectedListboxIndex]);
            }
            tabPage5_Enter(sender,e);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage2);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage4);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage3);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage5);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text += Resources.Form1_Form1_Load_Все_логи_можно_найти_в_папке_с_программой_после_закрытия_приложения_;
        }




    }
}
