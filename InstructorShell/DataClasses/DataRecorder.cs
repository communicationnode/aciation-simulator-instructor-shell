using System.Runtime.InteropServices;

namespace InstructorShell.DataClasses {

    [StructLayout(LayoutKind.Auto)]
    internal static class DataRecorder {

        // fields

        internal static bool enabled = false;
        internal static List<DataEntity> recordedData { get; private set; } = new List<DataEntity>();
    }
}
