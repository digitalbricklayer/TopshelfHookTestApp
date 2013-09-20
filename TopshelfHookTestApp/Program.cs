using System;
using Topshelf;

namespace TopshelfHookTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var host = HostFactory.New(x =>
            {
                x.Service<IDummyService>(s =>
                {
                    s.ConstructUsing(() => new DummyService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription(DummyService.ServiceDescription);
                x.SetDisplayName(DummyService.ServiceDisplayName);
                x.SetServiceName(DummyService.ServiceName);
            });

            host.Run();
        }
    }
}
