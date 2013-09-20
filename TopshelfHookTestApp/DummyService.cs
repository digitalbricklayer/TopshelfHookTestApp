namespace TopshelfHookTestApp
{
    internal class DummyService : IDummyService
    {
        public const string ServiceDescription = "A dummy service for figuring out how to get device notification whilst running Topshelf";
        public const string ServiceDisplayName = "Dummy Topshelf Service";
        public const string ServiceName = "DummyTopshelfSvc";

        private DeviceService deviceService;

        public void Start()
        {
            this.deviceService = new DeviceService();
            this.deviceService.Start();
        }

        public void Stop()
        {
            // Stop is not called
            this.deviceService.Stop();
        }
    }
}