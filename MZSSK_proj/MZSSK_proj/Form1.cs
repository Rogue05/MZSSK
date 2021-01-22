using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace MZSSK_proj
{
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Debug.WriteLine("constructor fired");
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private bool w1flag = false;
        private bool w2flag = false;

        private void Button1_Click(object sender, EventArgs e)
        {
            w1flag = !w1flag;
            Debug.WriteLine("b1 pressed");
            if (w1flag)
                backgroundWorker1.RunWorkerAsync();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            w2flag = !w2flag;
            Debug.WriteLine("b2 pressed");
            if (w2flag)
                backgroundWorker2.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Debug.WriteLine("bw1 start");
            random_number_generator(label1, ref w1flag);
            Debug.WriteLine("bw1 stop");
        }

        private void random_number_generator(Label label, ref bool flag)
        {
            int i = 0;
            int seed, prev_seed = 123;
            double x, probe;
            var updater = new Action<string>(s => label.Text = s);
            while (flag)
            {
                i = i + 1;
                seed = (2269 * prev_seed + 1) % 0xFFFF;
                x = 2.0 * (double)seed / 0xFFFF;
                prev_seed = seed;
                if (i > 10000000)
                {
                    probe = Math.Asin(Math.Sin(x * Math.PI));
                    Console.WriteLine(probe);
                    Invoke(updater, probe.ToString());
                    i = 0;
                }

            }
        }

        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            Debug.WriteLine("bw2 start");
            random_number_generator(label2, ref w2flag);
            Debug.WriteLine("bw2 stop");
        }
    }
}
