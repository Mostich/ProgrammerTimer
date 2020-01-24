using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Timer
{
    public partial class formDay : Form
    {
        public formDay()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text = GetTimeInt(timeDay.Text) + "!" + textDay.Text;
            Form1.Time.SetValue(this.Text, text);
            
        }

        private int GetTimeInt(string t) 
        {
            string[] t2 = t.Split(new char[] { ':' });
            return Convert.ToInt32(t2[0]) * 3600 + Convert.ToInt32(t2[1]) * 60 + Convert.ToInt32(t2[2]);
        }

    }
}
