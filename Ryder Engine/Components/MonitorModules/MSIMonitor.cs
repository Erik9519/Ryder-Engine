using MSIAfterburner;

namespace Ryder_Engine.Components.MonitorModules
{
    class MSIMonitor
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
        }
    }
}
