using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace InstructorShell.Processes {
    [SkipLocalsInit]
    public class RunningAppInfo {
        public string Title { get; init; }
        public IntPtr Handle { get; init; }
        public Process Process { get; init; }
        public int ID { get; init; }
        public object parent { get; set; }

        public override string ToString() {
            return $"title:{Title};\nhandle:{Handle};\nprocess:{Process.ToString()};\nid:{ID}.";
        }
    }
}
