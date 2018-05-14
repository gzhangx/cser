﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cser
{
    public class W32Serial
    {
        protected void throwWinErr(string text)
        {
            int err = Marshal.GetLastWin32Error();
            string errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
            throw new Exception($"{text} {err} {errorMessage}");
        }
        protected IntPtr m_hCommPort;
        public void Open()
        {
            SerialPort comm = new SerialPort();
            comm.BaudRate = 128000;
            m_hCommPort = GWin32.CreateFile("COM3",
               FileAccess.Read | FileAccess.Write, //GENERIC_READ | GENERIC_WRITE,//access ( read and write)
            FileShare.None, //0,    //(share) 0:cannot share the COM port                        
            IntPtr.Zero, //0,    //security  (None)                
            FileMode.Open, //OPEN_EXISTING,// creation : open_existing
            FileAttributes.System, //FILE_FLAG_OVERLAPPED,// we want overlapped operation
            IntPtr.Zero //0// no templates file for COM port...
            );

            if (m_hCommPort == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                string errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                throwWinErr("Open com failed ");
            }

            const uint EV_RXCHAR = 1, EV_TXEMPTY = 4;
            if (!GWin32.SetCommMask(m_hCommPort, EV_RXCHAR | EV_TXEMPTY))
            {
                throwWinErr("Failed to Set Comm Mask");
            }

            GWin32.DCB dcb = new GWin32.DCB();
            dcb.DCBLength = (uint)Marshal.SizeOf(dcb);
            if (!GWin32.GetCommState(m_hCommPort, ref dcb))
            {
                throwWinErr("CSerialCommHelper : Failed to Get Comm State");
            }

            dcb.BaudRate = (uint)comm.BaudRate;
            dcb.ByteSize = (byte)comm.DataBits;
            dcb.Parity = comm.Parity;
            dcb.StopBits = comm.StopBits;
            dcb.DsrSensitivity = false;
            dcb.DtrControl = GWin32.DtrControl.Enable;
            dcb.OutxDsrFlow = false;

            if (!GWin32.SetCommState(m_hCommPort, ref dcb))
            {
                throwWinErr("CSerialCommHelper : Failed to Set Comm State");
            }
        }
    }
}