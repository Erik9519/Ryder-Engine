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
                if (sensors[i].name.Contains("voltage") && sensors[i].name.Contains("GPU"))
                {
                    sensors[i].value *= 1000;
                }
            }

        }
    }
}
