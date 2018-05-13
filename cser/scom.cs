using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cser
{
    class scom
    {
        protected SerialPort comm = new SerialPort();
        protected Thread _thread;
        protected bool threadStarted = false;
        public scom()
        {
            //comm.ReadTimeout = 500;
            comm.WriteTimeout = 500;
            comm.Parity = Parity.None;
            comm.DataBits = 8;
            comm.StopBits = StopBits.One;
            //comm.WriteBufferSize = 2048;
            //comm.ReadBufferSize = 2048;
            comm.BaudRate = 128000;
            comm.PortName = "COM3";
        }
        public void Open()
        {                       
            comm.Open();
        }

        public void Close()
        {
            threadStarted = false;
            comm.Close();
            _thread.Join();
            _thread = null;
        }


        public void Start()
        {
            //Write(new byte[] { 0xA5, 0x90 });
            Write(new byte[] { 0xA5, 0x60 });
            threadStarted = true;
            if (_thread != null) return;
            var buf = new byte[2048];
            _thread = new Thread(() =>
            {
                while (threadStarted)
                {
                    int blen = comm.Read(buf, 0, buf.Length);
                    if (blen < 0) break;
                    Console.WriteLine($"Got item {blen} {BitConverter.ToString(buf,0,blen)}");
                }
                Console.WriteLine("thread done");
            });
            _thread.Start();
        }

        public void Stop()
        {
            Write(new byte[] { 0xA5, 0x65 });
        }
        protected void Write(byte[] buf)
        {
            comm.Write(buf, 0, buf.Length);
        }
    }
}
