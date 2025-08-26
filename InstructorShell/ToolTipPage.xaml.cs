using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Controls;


namespace InstructorShell {

    [StructLayout(LayoutKind.Auto)]
    [SkipLocalsInit]
    public partial class ToolTipPage : Page {

        //values
        public static ToolTipPage instance { get; private set; }
        public static ToolTipText currentToolTip = new ToolTipText("NaN");
        public static bool visible = false;


        //constructor

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        public ToolTipPage() {
            InitializeComponent();
            instance = this;

            Task.Run([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            [SkipLocalsInit] async () => {
                while (true) {
                    await Task.Delay(1);

                    if ( !visible ) { continue; }

                    await Dispatcher.BeginInvoke([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                    [SkipLocalsInit] () => {
                        textBox.Text = currentToolTip.text;
                    });
                }
            });
        }
    }


    [SkipLocalsInit]
    public class ToolTipText {

        // fields

        public string text;


        // constuctor

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        public ToolTipText(in string text) {
            this.text = text;
        }
    }
}
