using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Drawing;


namespace InstructorShell.Processes {



    /// <summary>
    /// Этот класс занимается получением списка главных запущенных процессов, которые обычно отрисовываются на панели задач
    /// </summary>
    [SkipLocalsInit]
    public static class ProcessManager {

        // fields
        public readonly static Queue<RunningAppInfo> runningAppsInfoQueue = new Queue<RunningAppInfo>();

        public static event Action<RunningAppInfo> ProcessStarted = (appInfo) => { };
        public static event Action<RunningAppInfo> ProcessEnded = (appInfo) => { };


        // methods

        [SkipLocalsInit]
        public static void Watch() {
            ManagementEventWatcher psStartEvt = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
            ManagementEventWatcher psStopEvt = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");

            psStartEvt.EventArrived += (s, e) => {
                try {
                    Process process = Process.GetProcessById(int.Parse(e.NewEvent.Properties["ProcessID"].Value.ToString()));
                    RunningAppInfo runningAppInfo = new RunningAppInfo() {
                        Title = process.ProcessName,
                        Handle = process.Handle,
                        Process = process,
                        ID = process.Id
                    };
                    MainWindow.instance.Dispatcher.Invoke(new Action(() => {
                        //Log.CreateLog($"enqueued process: {process.ProcessName}",runningAppInfo.ToString());
                        ProcessStarted(runningAppInfo);
                    }));
                    runningAppsInfoQueue.Enqueue(runningAppInfo);
                }
                catch { }
            };

            psStopEvt.EventArrived += (s, e) => {
                try {
                    int invalidID = int.Parse(e.NewEvent.Properties["ProcessID"].Value.ToString());

                    for (int i = 0; i < runningAppsInfoQueue.Count; i++) {
                        var checkingAppInfo = runningAppsInfoQueue.Dequeue();
                        if (checkingAppInfo.ID != invalidID) {
                            runningAppsInfoQueue.Enqueue(checkingAppInfo);
                        }
                        else {
                            MainWindow.instance.Dispatcher.Invoke(() => {
                                //Log.CreateLog($" -- dequed process: {checkingAppInfo.Title}; id:{checkingAppInfo.ID}");
                                ProcessEnded(checkingAppInfo);
                            });
                            break;
                        }
                    }
                }
                catch { }
            };

            psStartEvt.Start();
            psStopEvt.Start();
        }

        [SkipLocalsInit]
        public static void BringToFront(Process process) {
            IntPtr handle = process.MainWindowHandle;
            if (handle == IntPtr.Zero) return;

            ShowWindow(handle, SW_RESTORE);
            SetForegroundWindow(handle);
        }


        // external methods

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        public static void SetWindowToBottom() {
            var hWnd = new WindowInteropHelper(MainWindow.instance).Handle;
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }
    }


}