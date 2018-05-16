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
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (tran == null)
            {
                tran = new X4Tran((x, y) =>
                {
                    lock (lockobj)
                    {
                        gpoints.Add(new Point(x, y));
                        panel1.BeginInvoke(new Action(() =>
                        {
                            panel1.Invalidate();
                        }));
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
        private void panel1_Paint(object sender, PaintEventArgs e)
        {            
            lock(lockobj)
            {
                lpoints.AddRange(gpoints);
                gpoints.Clear();
            }
            if (!lpoints.Any()) return;
            int w = panel1.Width/2;
            int h = panel1.Height/2;

            int max = 1500; // Math.Max(lpoints.Max(p => Math.Abs(p.X)), lpoints.Max(p => Math.Abs(p.Y)))*2+1;
            lpoints.ForEach(p =>
            {
                int x = (p.X * h / max) + w;
                int y = ((p.Y * h) / max) + h;
                e.Graphics.FillRectangle(Brushes.Black, x, y, 1, 1);
            });

        }
    }
}

