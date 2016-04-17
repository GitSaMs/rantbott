using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Twitchbot
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Command x = new Command();
            x.Com = textBox1.Text;
            x.Answer = textBox2.Text;
            string s =x.ToString();
            StreamWriter sw = new StreamWriter("commands.txt",true);
            sw.WriteLine(s);
            sw.Close();
            Close();
            

        }
    }
}
