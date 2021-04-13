using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace Ryder_Engine.Components.MonitorModules
{
    class FanController
    {
        SerialPort serialPort = null;
        public bool connected = false;
        public float ambient, liquid;

        public FanController(string port)
        {
            string[] ports = scanPorts();
            foreach (string p in ports)
            {
                if (p == port)
                {
                    connectTo(port, null);
                    discardBuffer();
                    break;
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

        public bool connectTo(string port, SerialDataReceivedEventHandler handler)
        {
            if (serialPort == null)
            {
                serialPort = new SerialPort(port, 500000);
                if (handler != null) serialPort.DataReceived += handler;
                serialPort.ReadTimeout = 250;
                serialPort.Open();
                connected = true;
                return true;
            }
            return false;
        }

        public bool isConnected()
        {
            return serialPort != null;
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
