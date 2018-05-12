using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cser
{
    class scom
    {
        protected SerialPort comm = new SerialPort();
        public scom()
        {
            comm.ReadTimeout = 500;
            comm.WriteTimeout = 500;
            comm.BaudRate = 128000;
            comm.PortName = "com3";
        }
        public void Start()
        {                       
            comm.Open();
        }
    }
}
