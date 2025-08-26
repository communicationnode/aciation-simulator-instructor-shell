using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CustomDesktopShell;
using InstructorShell.Processes;
using CustomDesktopShell.UDPWork;
using System.Reflection;
using Acornima.Ast;
using Jint.Runtime;
using System.Windows.Media;

namespace InstructorShell {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window {

        // fields

        // singletone
        public static MainWindow instance;

        // bools
        public static bool panelActivateState { get; private set; } = false;
        public static bool remoteMode { get; private set; } = false;

        // events
        public static event Action PanelWasOpened = () => { };
        public static event Action PanelWasClosed = () => { };

        // pages
        WeatherPanel weatherPanelPage;
        InstuctorTestPanel testPanelPage;
        AirMapPanel mapPanelPage;



        // constructor

        public MainWindow() {

            // program as single instance
            Mutex? programOneInstanceMarker = new Mutex(true, "Global\\AirplaneShellApp", out bool created);
            if (CheckIfProgramIsFirstInstance(created) is true) { return; }
            UpdateMutex(programOneInstanceMarker);

            InitializeComponent();

            instance = this;

            foreach (var arg in App.args) {
                if (arg == "-remote") {
                    remoteMode = true;
                    panel_name.Text = "Панель инструктора [remote]";
                }
                else if (arg == "-lowenergy") {
                    RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
                }
            }

            authorRights_label.Text = $"Version: {Assembly.GetExecutingAssembly().GetName().Version} .Разработал \"-\". Год выпуска - 2025";

            //RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;

            UDPMessaging.Initialize();
            RunPanelAnimation();

            AllowControls();
            MarginCursorGrid();

            SetTooltip(panel_name, new ToolTipText("Мониторинг состояния самолета на карте местности с возможностью управления погодными условиями"));
            SetTooltip(authorRights_label, new ToolTipText("Разработал -"));

            ProcessManager.Watch();
        }


        // methods
        private static bool CheckIfProgramIsFirstInstance(in bool created) {
            if (!created) { return true; }
            return false;
        }
        private static void UpdateMutex(Mutex? programOneInstanceMarker) {
            Task.Run(async () => {
                while (true) {
                    await Task.Delay(1000);
                    programOneInstanceMarker?.WaitOne();
                }
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        private protected void RunPanelAnimation() {

            double animationSpeed = 0.5d;
            float logo_height = 200;

            Task.Run([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            [SkipLocalsInit] async () => {
                while (true) {
                    await Task.Delay(1);

                    await Dispatcher.BeginInvoke([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                    [SkipLocalsInit] () => {

                        if (panelActivateState is true) {
                            controlsFrame.Visibility = Visibility.Hidden;
                        }
                        else {
                            controlsFrame.Visibility = Visibility.Visible;
                        }

                        MainWindow.instance.instructor_grid.RowDefinitions[0].Height = new GridLength(
                                MainWindow.instance.instructor_grid.RowDefinitions[0].Height.Value + ((panelActivateState is true ? 9370.0d : 0.0d) - MainWindow.instance.instructor_grid.RowDefinitions[0].Height.Value) * animationSpeed,
                                GridUnitType.Star);

                        logo_height += ((panelActivateState is true ? 200f : 256f) - logo_height) * 0.04f;
                        MainWindow.instance.base_background_logo.Height = logo_height;
                    });
                }
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        public void ChangePageAnimation(object page) {
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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        private protected void AllowControls() {

            weatherPanelPage = new WeatherPanel();
            testPanelPage = new InstuctorTestPanel();
            mapPanelPage = new AirMapPanel();


            Action<bool> SetOpenPanelState = (state) => {
                panelActivateState = state;

                if (panelActivateState is true) { PanelWasOpened(); }
                else { PanelWasClosed(); }

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
                if (MainWindow.instance.instructor_grid.RowDefinitions[0].Height.Value != 0) {
                    SetOpenPanelState(false);
                }
                else {
                    SetOpenPanelState(true);
                }
            };
        }

        public void ForceMapPage() {
            ChangePageAnimation(mapPanelPage);
            panelActivateState = true;
            PanelWasOpened();
            (MainWindow.instance.close_panel_btn.Content as Image).Source = new BitmapImage(new Uri("/Images/data-shield/79.png", UriKind.Relative));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        private protected void MarginCursorGrid() {
            Task.Run([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            [SkipLocalsInit] async () => {

                float width = 0;
                float height = 0;
                float offsetX = 0;

                while (true) {

                    await Task.Delay(1);
                    await this.Dispatcher.BeginInvoke([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                    [SkipLocalsInit] () => {

                        if (ToolTipPage.visible) {
                            Point mousePos = Mouse.GetPosition(this);
                            cursorGrid.Margin = new Thickness(mousePos.X + offsetX, mousePos.Y, 0, 0);
                        }

                        width += ((ToolTipPage.visible is true ? 512 : 0) - width) * 0.35f;
                        height += ((ToolTipPage.visible is true ? 512 : 0) - height) * 0.35f;
                        offsetX += ((ToolTipPage.visible is true ? 32 : 0) - offsetX) * 0.1f;

                        toolTipFrame.MaxHeight = height;
                        toolTipFrame.MaxWidth = width;
                    });
                }
            });
        }

        /// <summary>Bind initialized <see cref="ToolTipText"/> to selected <see cref="UIElement"/></summary>
        /// <param name="element">Element that will show tooltip with custom text</param>
        /// <param name="toolTip">Instance with string reference. Text can be changed in runtime</param>

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        public void SetTooltip(in UIElement element, ToolTipText toolTip) {
            element.MouseMove += (sender, e) => { ToolTipPage.currentToolTip = toolTip; };
            element.MouseEnter += (sender, e) => { ToolTipPage.visible = true; };
            element.MouseLeave += (sender, e) => { ToolTipPage.visible = false; };
        }
    }
}