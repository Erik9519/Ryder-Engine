using System;
using System.Diagnostics;
using MSIAfterburner;

namespace Ryder_Engine.Components.MonitorModules
{
    public class MSIMonitor
    {
        private AfterBurner msi;

        public Sensor[] sensors = null;

        public MSIMonitor()
        {
            msi = new AfterBurner();
        }

        public void update()
        {
            sensors = msi.getSensors();
            for (short i = 0; i < sensors.Length; i++)
            {
                // MSI outputs the GPU voltage in V but we require mV as the readings must be integers
                if (sensors[i].type == MSIAfterburner.Type.ID_GPU_VOLTAGE)
                {
                    sensors[i].value *= 1000;
                } 
                // MSI sometimes outputs bogus Framerate values and thus its value must be clamped
                else if (sensors[i].type == MSIAfterburner.Type.ID_FRAMERATE)
                {
                    sensors[i].value = sensors[i].value >= 3E+37 ? 0 : sensors[i].value;
                }
            }

        }
    }
}
