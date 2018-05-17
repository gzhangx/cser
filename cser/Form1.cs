using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cser
{
    public partial class Form1 : Form
    {
        Bitmap Backbuffer;
        public Form1()
        {
            InitializeComponent();
            this.SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.DoubleBuffer, true);

            System.Reflection.PropertyInfo controlProperty = typeof(System.Windows.Forms.Control)
        .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            controlProperty.SetValue(panel1, true, null);

            //panel1.dd();   
        }

        W32Serial comm = new W32Serial();
        public List<Point> gpoints = new List<Point>();
        object lockobj = new object();

        X4Tran tran;
        private void Start_Click(object sender, EventArgs e)
        {
            try
            {
                comm.Open();
                //comm.Open();

            } catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        double zeroAng = 0;
        public List<Point> angleLen = new List<Point>();
        private void button1_Click(object sender, EventArgs e)
        {
            if (tran == null)
            {
                tran = new X4Tran((x, y, angle, len) =>
                {
                    lock (lockobj)
                    {
                        gpoints.Add(new Point(x, y));
                        angleLen.Add(new Point((int)(angle/Math.PI*180), len));
                        panel1.BeginInvoke(new Action(() =>
                        {
                            panel1.Invalidate();
                        }));
                    }
                }, z=>
                {
                    zeroAng = z;
                    Console.WriteLine("zero angle is " + z);
                    lock (lockobj)
                    {
                        lpoints.Clear();
                        
                        {
                            lpoints.AddRange(gpoints);
                            gpoints.Clear();
                            if (!lpoints.Any()) return;
                            panel1.AddPoints(lpoints, angleLen);
                            angleLen.Clear();
                        }
                    }
                });
            }
            comm.Start(tran);
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            comm.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comm.Info();
        }

        List<Point> lpoints = new List<Point>();

        private void Form1_Load(object sender, EventArgs e)
        {
            Backbuffer = new Bitmap(panel1.Width, panel1.Height);
        }

    }
}

