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
using System.Threading;

namespace MZSSK_proj2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            temps = new List<double>();
            buff = new List<double>();
            xs = new List<double>();
            endFlag = false;
            T1 = 0;
            T2 = 100;
            resetChart();
            updater = new Action<List<double>, List<double>>((xs, temps) => chart1.Series[0].Points.DataBindXY(xs, temps));
            //chart1.Series[0].Points.DataBindXY(xs, temps);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void resetChart()
        {
            temps.Clear();
            for (int i = 0; i < N; ++i)
            {
                if (i < N / 2) temps.Add(T1);
                else temps.Add(T2);
                xs.Add(-L + 2 * L / N * (i + 1));
                buff.Add(0);
            }
            chart1.Series[0].Points.DataBindXY(xs, temps);
        }

        private List<double> temps;
        private List<double> buff;
        private List<double> xs;
        private bool endFlag;
        private double T1, T2;
        private int N = 101;
        private double L = 1;

        //private double kdtdx2 = 0.0005 * 20;
        private double kdtdx2 = 0.01;
        private Action<List<double>, List<double>> updater;


        private void processTemp(int s, int e)
        {
            for (int i = s; i < e; ++i)
            {
                buff[i] = temps[i] + kdtdx2 * (temps[i - 1] + temps[i + 1] - 2 * temps[i]);
            }
            buff[0] = temps[0];
            buff[N - 1] = temps[N - 1];
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        private void Button1_Click(object sender, EventArgs e)
        { // reset
            endFlag = !endFlag;
            if (endFlag)
            {
                //resetChart();
                backgroundWorker1.RunWorkerAsync();
            }
            //else resetChart();
            //resetChart();
            Debug.WriteLine("Done");
        }

        private static Semaphore p2, p3, p4, p5;
        private static Semaphore p2Done, p3Done, p4Done, p5Done;

        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            while (endFlag)
            {
                p2.WaitOne();
                Debug.WriteLine("BW 2");
                processTemp(1, N / 4);
                p2Done.Release();
            }
        }

        private void BackgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            while (endFlag)
            {
                p3.WaitOne();
                Debug.WriteLine("BW 3");
                processTemp(N / 4, N / 2);
                p3Done.Release();
            }
        }

        private void BackgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            while (endFlag)
            {
                p4.WaitOne();

                Debug.WriteLine("BW 4");
                processTemp(N / 2, 3 * N / 4);
                p4Done.Release();
            }
        }

        private void BackgroundWorker5_DoWork(object sender, DoWorkEventArgs e)
        {
            while (endFlag)
            {
                p5.WaitOne();
                Debug.WriteLine("BW 5");
                processTemp(3 * N / 4, N - 1);
                p5Done.Release();
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            p2 = new Semaphore(1, 1);
            p3 = new Semaphore(1, 1);
            p4 = new Semaphore(1, 1);
            p5 = new Semaphore(1, 1);
            p2Done = new Semaphore(0, 1);
            p3Done = new Semaphore(0, 1);
            p4Done = new Semaphore(0, 1);
            p5Done = new Semaphore(0, 1);
            backgroundWorker2.RunWorkerAsync();
            backgroundWorker3.RunWorkerAsync();
            backgroundWorker4.RunWorkerAsync();
            backgroundWorker5.RunWorkerAsync();
            Debug.WriteLine("STARTED");

            uint iter = 0;
            while (endFlag)
            {
                //poolDone.WaitAll();
                //Debug.WriteLine("ENTER");
                p2Done.WaitOne();
                p3Done.WaitOne();
                p4Done.WaitOne();
                p5Done.WaitOne();
                //WaitHandle.WaitAll(new WaitHandle[] { poolDone });
                Debug.WriteLine("ALL DONE");
                //processTemp(1, N - 1);
                Swap(ref buff, ref temps);
                //poolDone.Release(0);
                //break;
                p2.Release();
                p3.Release();
                p4.Release();
                p5.Release();
                if (iter++ > 10)
                {
                    Invoke(updater, xs, temps);
                    iter = 0;
                }
            }
        }
    }
}
