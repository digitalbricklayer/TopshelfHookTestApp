using System;
using System.Runtime.InteropServices;
using Topshelf;

namespace TopshelfHookTestApp
{
    internal class DummyService : ServiceControl
    {
        public const string ServiceDescription = "A dummy service for figuring out how to get device notification whilst running Topshelf";
        public const string ServiceDisplayName = "Dummy Topshelf Service";
        public const string ServiceName = "DummyTopshelfSvc";

        private IntPtr deviceEventHandle;
        private Win32.ServiceControlHandlerEx myCallback;
        private HostControl startHostControl;

        public bool Start(HostControl hostControl)
        {
            this.startHostControl = hostControl;

            myCallback = new Win32.ServiceControlHandlerEx(ServiceControlHandler);
            var serviceStatusHandle = Win32.RegisterServiceCtrlHandlerEx(DummyService.ServiceName,
                                                                            myCallback,
                                                                            IntPtr.Zero);
            var deviceInterface = new Win32.DEV_BROADCAST_DEVICEINTERFACE();
            int size = Marshal.SizeOf(deviceInterface);
            deviceInterface.dbcc_size = size;
            deviceInterface.dbcc_devicetype = Win32.DBT_DEVTYP_DEVICEINTERFACE;
            IntPtr buffer = default(IntPtr);
            buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(deviceInterface, buffer, true);
            deviceEventHandle = Win32.RegisterDeviceNotification(serviceStatusHandle,
                                                                    buffer,
                                                                    Win32.DEVICE_NOTIFY_SERVICE_HANDLE |
                                                                    Win32.DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
            if (deviceEventHandle == IntPtr.Zero)
            {
                // TODO handle error
            }
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            // Stop is not called
            return true;
        }

        private int ServiceControlHandler(int control, int eventType, IntPtr eventData, IntPtr context)
        {
            /*
             * Messages are received here after the Start method has been 
             * called including the control stop.
             */
            if (control == Win32.SERVICE_CONTROL_STOP || control == Win32.SERVICE_CONTROL_SHUTDOWN)
            {
                if (deviceEventHandle != IntPtr.Zero)
                {
                    Win32.UnregisterDeviceNotification(deviceEventHandle);
                    deviceEventHandle = IntPtr.Zero;
                    this.startHostControl.Stop();
                }
            }
            else if (control == Win32.SERVICE_CONTROL_DEVICEEVENT)
            {
                switch (eventType)
                {
                    case Win32.DBT_DEVICEARRIVAL:
                        break;

                    case Win32.DBT_DEVICEQUERYREMOVE:
                        break;
                }
            }

            return 0;
        }
    }
}