using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Timers;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
namespace Timer
{

    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, String lpString, int nMaxCount); // объявляем метод на C#

        [DllImport("user32.dll")]
        public static extern UInt32 GetWindowThreadProcessId(IntPtr hwnd, ref Int32 pid);

        formDay frmDay = new formDay();
        
        public static string current_date = DateTime.Now.ToShortDateString();
        public static int day;
        public static int time_day;
        public static int time_all;
        public static string text;
        public static string[] selectedDay = {""};
        public string selected;
        public static string[] words;
        public static RegistryKey currentUserKey = Registry.CurrentUser;
        public static RegistryKey Time = currentUserKey.CreateSubKey("Time");
        public static string textbox;
        public Form1()
        {
            InitializeComponent();

            if (Time.GetValue("time_all") is null)
                time_all = 0;
            else
                time_all = Convert.ToInt32(Time.GetValue("time_all").ToString());

            if (Time.GetValue(current_date) is null)
            {
                time_day = 0;
            }
            else 
            {
                string[] tmp = Time.GetValue(current_date).ToString().Split(new char[] { '!' });
                time_day = Convert.ToInt32(tmp[0]);
            }
            
            ShowTime();
            StatDays();
            
            this.ShowInTaskbar = false;

        }


        private void NotifyIcon2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.ShowInTaskbar = true;
                this.Show();
            }


        }//-- Открыть/заркыть окно в трее
        
        private void Timer3_Tick(object sender, EventArgs e)//-- Окно в фокусе время идет
        {
            IntPtr h = GetForegroundWindow();
            int pid = 0;
            GetWindowThreadProcessId(h, ref pid);
            Process p = Process.GetProcessById(pid);
            string text = p.MainWindowTitle;
            if (text.IndexOf("Visual") > 0 || text.IndexOf("Atom") > 0 || text.IndexOf("Notepad++") > 0)
            {
                CheckDate();
                time_all++;
                time_day++;
                ShowTime();
            }
        }

        private void Timer4_Tick(object sender, EventArgs e)//В промежуток времени сохраняем время в localstor
        {
            SaveDataDay(); //сохраняем в реестор
            StatDays();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//Окно закрывается сохраняем regedit
        {
            SaveDataDay(); //сохраняем в реестор
            Time.Close();
            notifyIcon2.Visible = false;
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Hide();
        }
        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void CheckDate()//Проверяем на новую дату
        {
            string datenow = DateTime.Now.ToShortDateString();
            if (current_date != datenow)
            {
                SaveDataDay(); //сохраняем в реестор
                current_date = datenow;
                time_day = 0;
            }
        }

        private void SaveDataDay() 
        {
            Time.SetValue("time_all", time_all);

            if (Time.GetValue(current_date.ToString()) is null)
            {
                Time.SetValue(current_date.ToString(), time_day + "!" );
            }
            else 
            {
                // Парсим строку из реестра по текущей дате!
                string[] temp = Time.GetValue(current_date.ToString()).ToString().Split(new char[] { '!' });
                //Сохраняем в реестор время и комментарий из temp[1]
                Time.SetValue(current_date.ToString(), time_day + "!" + temp[1]);
            }
            
        }
        private void ShowTime()//-- Время заносится в секундах а мы выводим в часах минутах секундах
        {
            int ta = 10000 * 3600 - time_all;
            int ha = ta / 3600;
            int t2a = ta - ha * 3600;
            int ma = t2a / 60;
            int sa = t2a - ma * 60;

            int td = time_day;
            int hd = td / 3600;
            int t2d = td - hd * 3600;
            int md = t2d / 60;
            int sd = t2d - md * 60;

            Label1.Text = ha + ":" + Format(ma) + ":" + Format(sa);
            label6.Text = Format(hd) + ":" + Format(md) + ":" + Format(sd);
            notifyIcon2.Text = label6.Text;
        }
        public static string Format(int t) //Форматируем день что бы имел формат 00:00:00
        {
            return (t < 10 ? "0" + t.ToString() : t.ToString());
        }
        
        private void StatDays()//Проходимся по всем дням и выводим их в список дней
        {
            
            
            Grid.Rows.Clear();
            foreach (string valueName in Time.GetValueNames())
            {
                //Все дни кроме ключа time_all
                if (valueName != "time_all")
                {
                    string[] tmp = Time.GetValue(valueName).ToString().Split(new char[] { '!' });

                    int td = Convert.ToInt32(tmp[0]);
                    int hd = td / 3600;
                    int t2d = td - hd * 3600;
                    int md = t2d / 60;
                    int sd = t2d - md * 60;

                    Grid.Rows.Add(valueName, Format(hd) + ":" + Format(md) + ":" + Format(sd), tmp[1]);
                }
            }
        }
        private void Grid_DoubleClick(object sender, EventArgs e)
        {
            frmDay.Text = Grid.CurrentRow.Cells[0].Value.ToString();
            frmDay.timeDay.Text = Grid.CurrentRow.Cells[1].Value.ToString();
            frmDay.textDay.Text = Grid.CurrentRow.Cells[2].Value.ToString();
            frmDay.ShowDialog();
            if (frmDay.DialogResult == DialogResult.OK)
            {
                StatDays();
            }
        }
    }
}
