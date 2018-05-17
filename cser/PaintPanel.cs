using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cser
{
    public class PaintPanel : Panel
    {
        Bitmap Backbuffer;
        List<Point> _points = new List<Point>();
        object lockobj = new object();
        public void AddPoints(List<Point> o)
        {
            lock(lockobj)
            {
                _points.Clear();
                _points.AddRange(o);
            }
        }
        public PaintPanel()
        {
            this.SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                true);
            Backbuffer = new Bitmap(Width, Height);
        }
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            Backbuffer = new Bitmap(Width, Height);
        }
        public void dd()
        {
            this.DoubleBuffered = true;

            // or

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.DoubleBuffer, true);
            UpdateStyles();
        }
        protected override void OnPaint(PaintEventArgs e) {
            List<Point> points = new List<Point>();
            lock(lockobj)
            {
                points.AddRange(_points);
            }
            int w = Width / 2;
            int h = Height / 2;

            int max = 3000; // Math.Max(lpoints.Max(p => Math.Abs(p.X)), lpoints.Max(p => Math.Abs(p.Y)))*2+1;
            using (var g = Graphics.FromImage(Backbuffer))
            {
                g.Clear(Color.White);

                points.ForEach(p =>
                {
                    int x = (p.X * h / max) + w;
                    int y = ((p.Y * h) / max) + h;
                    g.FillRectangle(Brushes.Black, x, y, 1, 1);
                });
            }
            e.Graphics.DrawImageUnscaled(Backbuffer, 0, 0);
        }
    }
}
