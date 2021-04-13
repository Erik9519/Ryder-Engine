namespace Ryder_Engine.Components.MonitorModules
{
    struct SystemStatus
    {
        public struct Program
        {
            public string name;
            public string icon;
        }
        public struct FanController
        {
            public float ambient;
            public float liquid;
        }
        public MSIMonitor.Status.CPU cpu;
        public MSIMonitor.Status.GPU gpu;
        public MSIMonitor.Status.RAM ram;
        public NetworkMonitor.Status network;
        public StorageMonitor.Status[] storage;
        public FanController fanController;
        public uint total_power;
        public short framerate;
    }
}
