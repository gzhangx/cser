﻿using System;
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
        }

        W32Serial comm = new W32Serial();
        X4Tran tran = new X4Tran();
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
    }
}
