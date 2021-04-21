using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace Ryder_Engine.Components.MonitorModules
{
    public class FanController
    {
        SerialPort serialPort = null;
        public string port = "";
        public float ambient, liquid;

        public FanController(string port)
        {
            connect(port);
        }

        public void connect(string port)
        {
            if (this.port != port || (serialPort != null && !serialPort.IsOpen) || serialPort == null)
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                    serialPort = null;
                }

                string[] ports = scanPorts();
                foreach (string p in ports)
                {
                    if (p == port)
                    {
                        serialPort = new SerialPort(port, 500000);
                        serialPort.ReadTimeout = 250;
                        serialPort.Open();
                        discardBuffer();
                        break;
                    }
                }
            }
        }

        public void update()
        {
            string rsp = sendMsgAndReceive(String.Format("Sr\n"));
            if (rsp != null)
            {
                string[] f_status = rsp.Split(',');
                ambient = float.Parse(f_status[0]);
                liquid = float.Parse(f_status[1]);
            }
        }

        public void discardBuffer()
        {
            serialPort.DiscardInBuffer();
        }

        public bool disconnect()
        {
            if (serialPort != null)
            {
                serialPort.Close();
                serialPort = null;
                return true;
            }
            return false;
        }

        public string sendMsgAndReceive(string msg)
        {
            if (serialPort != null)
            {
                serialPort.Write(msg);
                return serialPort.ReadTo("\n");
            }
            return null;
        }

        public string[] scanPorts()
        {
            return SerialPort.GetPortNames();
        }
    }
}
