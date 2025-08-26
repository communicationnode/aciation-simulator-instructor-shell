using System.Windows;

namespace InstructorShell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] args { get; private set; }


        protected override void OnStartup(StartupEventArgs e) {

            App.args = e.Args;

            base.OnStartup(e);
        }
    }

}
