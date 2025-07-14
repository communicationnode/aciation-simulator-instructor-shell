using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CustomDesktopShell;
using CustomDesktopShell.UDPWork;

namespace InstructorShell {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static MainWindow instance;
        private protected static bool panelActivateState = false;
        private protected static Jint.Engine jintEngine;


        public MainWindow() {
            InitializeComponent();
            instance = this;

            var weatherPanelPage = new WeatherPanel();
            var testPanelPage = new InstuctorTestPanel();
            var mapPanelPage = new AirMapPanel();

            UDPMessaging.Initialize();
            RunPanelAnimation();
            JintEngineStart();          

            Action<bool> SetOpenPanelState = (state) => {
                panelActivateState = state;
                (MainWindow.instance.close_panel_btn.Content as Image).Source = new BitmapImage(new Uri(state is true ? "/Images/data-shield/79.png" : "/Images/data-shield/77.png", UriKind.Relative));
            };

            MainWindow.instance.instructor_frames.Content = weatherPanelPage;

            MainWindow.instance.weather_btn.Click += (_, _) => {
                ChangePageAnimation(weatherPanelPage);
                SetOpenPanelState(true);
            };
            MainWindow.instance.test_panel_btn.Click += (_, _) => {
                ChangePageAnimation(testPanelPage);
                SetOpenPanelState(true);
            };
            MainWindow.instance.map_panel_btn.Click += (_, _) => {
                ChangePageAnimation(mapPanelPage);
                SetOpenPanelState(true);
            };
            MainWindow.instance.close_panel_btn.Click += (_, _) => {
                if (MainWindow.instance.instructor_grid.RowDefinitions[0].Height.Value != 0) { SetOpenPanelState(false); }
                else { SetOpenPanelState(true); }
            };

            
        }
        private protected void RunPanelAnimation() {
            double animationSpeed = 0.3d;
            Task.Run(async () => {
                while (true) {
                    await Task.Delay(1);
                    Dispatcher.Invoke(new Action(() => {
                        MainWindow.instance.instructor_grid.RowDefinitions[0].Height = new GridLength(MainWindow.instance.instructor_grid.RowDefinitions[0].Height.Value + ((panelActivateState is true ? 280.0d : 0.0d) - MainWindow.instance.instructor_grid.RowDefinitions[0].Height.Value) * animationSpeed, GridUnitType.Star);
                        MainWindow.instance.instructor_grid.RowDefinitions[1].Height = new GridLength(MainWindow.instance.instructor_grid.RowDefinitions[1].Height.Value + ((panelActivateState is true ? 44 : 9.0d) - MainWindow.instance.instructor_grid.RowDefinitions[1].Height.Value) * animationSpeed, GridUnitType.Star);
                    }));
                }
            });
        }
        private protected void ChangePageAnimation(object page) {
            if (panelActivateState is false) {
                MainWindow.instance.instructor_frames.Content = page;
                return;
            }
            Task.Run(async () => {
                panelActivateState = false;
                Dispatcher.Invoke(() => { close_panel_btn.Visibility = Visibility.Collapsed; });
                await Task.Delay(400);

                panelActivateState = true;
                Dispatcher.Invoke(() => {
                    close_panel_btn.Visibility = Visibility.Visible;
                    MainWindow.instance.instructor_frames.Content = page;
                });
            });
        }
        private protected void JintEngineStart() {

            jintEngine = new Jint.Engine().
                SetValue("offset", (Action<float, float>)((x, y) => { AirMapPanel.ForceOffset(x, y); })).
                SetValue("soffset", (Action<float, float>)((x, y) => { AirMapPanel.ForceStartOffset(x, y); })).
                SetValue("divide", (Action<double>)((val) => { AirMapPanel.customDivide = val; })).
                SetValue("angle", (Action<double>)((val) => { AirMapPanel.angle = val; }));

            jintBox.TextChanged += (s, arg) => {
                try { jintEngine.Execute(jintBox.Text); } catch { }
            };
        }
    }
}