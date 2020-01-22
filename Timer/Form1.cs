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


        public static DateTime current_date = DateTime.Now;
        public static int day;
        public static int time_day;
        public static string text_day;
        public static int time_all;
        public static string text;

        static RegistryKey currentUserKey = Registry.CurrentUser;
        static RegistryKey Time = currentUserKey.CreateSubKey("Time");
        
        public Form1()
        {
            InitializeComponent();

            if (Time.GetValue("time_all") is null)
                time_all = 0;
            else
                time_all = Convert.ToInt32(Time.GetValue("time_all").ToString());

            if (Time.GetValue(current_date.ToShortDateString()) is null)
            {
                time_day = 0;
                text_day = "";
            }
            else 
            {
                Parsing_Key(Time.GetValue(current_date.ToShortDateString()).ToString());
                
            }

            Time.SetValue("time_all", time_all);
            text = time_day + "!" + textBox1.Text;
            Time.SetValue(current_date.ToShortDateString(), text);
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
        private void Label1_Click(object sender, EventArgs e)
        {

        }//--
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
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//Окно закрывается сохраняем regedit
        {
            Time.SetValue("time_all", time_all);
            text = time_day + "!" + textBox1.Text;
            Time.SetValue(current_date.ToShortDateString(), text);
            Time.Close();
            notifyIcon2.Visible = false;
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Hide();
        }//--
        private void label3_Click(object sender, EventArgs e)
        {

        }//--
        private void CheckDate()//Проверяем на новую дату
        {
            DateTime datenow = DateTime.Now;
            if (current_date.ToShortDateString() != datenow.ToShortDateString())
            {
                current_date = datenow;
                text = time_day + "!" + textBox1.Text;
                Time.SetValue(current_date.ToShortDateString(), text);
                Time.SetValue("time_all", time_all);
                time_day = 0;
                text_day = "";
                textBox1.Text = "";
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
            notifyIcon2.Text = Format(hd) + ":" + Format(md) + ":" + Format(sd);
        }
        private string Format(int t) //Форматируем день что бы имел формат 00:00:00
        {
            return (t < 10 ? "0" + t.ToString() : t.ToString());
        }
        private void Timer4_Tick(object sender, EventArgs e)//В промежуток времени сохраняем время в localstor
        {
            text = time_day + "!" + textBox1.Text;
            Time.SetValue(current_date.ToShortDateString(), text);
            Time.SetValue("time_all", time_all);
            
            StatDays();
        }
        private void StatDays()//Проходимся по всем дням и выводим их в список дней
        {
            listBox1.Items.Clear();
            
            foreach (string valueName in Time.GetValueNames())
            {
                //Все дни кроме ключа time_all
                if (valueName != "time_all")
                {
                    if (valueName != "text") 
                    {
                    
                    Parsing_Key(Time.GetValue(valueName).ToString());
                    int timelist = Convert.ToInt32(time_day);

                    int td = timelist;
                    int hd = td / 3600;
                    int t2d = td - hd * 3600;
                    int md = t2d / 60;
                    int sd = t2d - md * 60;

                    listBox1.Items.Add(valueName + " - " + Format(hd) + ":" + Format(md) + ":" + Format(sd));
                    }
                    
                    
                }
            }
        }
        private void Parsing_Key(string text) 
        {
            //StringSplitOptions.RemoveEmptyEntries
            //Time.GetValue("time_all").ToString()
            textBox1.Text = "";
            string[] words = text.Split(new char[] { '!' });
            for (int i = 0; i < words.Length; i++) 
            {
                if (i == 0)
                {
                    time_day = Convert.ToInt32(words[i]);
                }
                else
                {  
                    textBox1.Text += (words[i] + "\n");
                }
                    
            }
                
            
            
        }
        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {

        } //--
    }
}
