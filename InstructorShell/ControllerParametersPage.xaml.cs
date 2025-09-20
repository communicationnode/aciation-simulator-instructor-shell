using CustomDesktopShell;
using CustomDesktopShell.UDPWork;
using InstructorShell.DataClasses;
using InstructorShell.Processes;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InstructorShell {

    [StructLayout(LayoutKind.Auto)]
    [SkipLocalsInit]
    public partial class ControllerParametersPage : Page {

        // fields
        byte tormoza = 0;
        float rud = 0.001234f;
        float pedali = 0.001234f;
        float rusUpDown = 0.001234f;
        float rusLeftRight = 0.001234f;
        double savedStartupCircleBarWidth;
        double savedStartupCircleBarHeight;
        double savedStartupVerticalBarHeight;
        string restartEngineAddressData;
        string[] processTargets;
        DateTime lastProcessLaunchDateTime = DateTime.Now;
        DateTime lastRestartEngineDateTime = DateTime.Now;
        ToolTipText rusToolTip = new ToolTipText("Значение руса: ");
        ToolTipText rudToolTip = new ToolTipText("Значение руда: ");
        ToolTipText pedaliToolTip = new ToolTipText("Значение педалей: ");
        ToolTipText temperaturesToolTip = new ToolTipText("GPU: <loading...>\nCPU: <loading...>");
        public static ToolTipText debuggerToolTipText = new ToolTipText("Системная информация");

        // all finded process targets
        List<RunningAppInfo> findedTargets = new List<RunningAppInfo>();


        // constructor
        public ControllerParametersPage() {
            InitializeComponent();

            MainWindow.instance.SetTooltip(circle_bar, rusToolTip);
            MainWindow.instance.SetTooltip(vertical_bar, rudToolTip);
            MainWindow.instance.SetTooltip(text1, debuggerToolTipText);
            MainWindow.instance.SetTooltip(vertical_bar2, pedaliToolTip);
            MainWindow.instance.SetTooltip(temperatureIcon, temperaturesToolTip);
            MainWindow.instance.SetTooltip(restartEngineButton, new ToolTipText("Упрощенный перезапуск двигателя."));
            MainWindow.instance.SetTooltip(turboIcon, new ToolTipText($"Это поле указывает на текущее состояние турбо."));
            MainWindow.instance.SetTooltip(dvigatelIcon, new ToolTipText("Это поле указывает на текущее состояние двигателя."));
            MainWindow.instance.SetTooltip(killTargetsImmediatelyButton, new ToolTipText("Если симуляция была выключена, а кнопка все еще активна, нажмите на нее, чтобы полностью прервать все фоновые процессы программного обеспечения симуляции."));

            TemperaturesMonitoring();
            RefreshBarsScale();
            SubscribeToUDPPackets();
            SubcribeToControls();

            processTargets = File.ReadAllLines(@$"Configs\ProcessNames.ini");
            restartEngineAddressData = File.ReadAllLines($@"Configs\restartEngineAddress.ini")[0];

            killTargetsImmediatelyButtonViewBox.Visibility = System.Windows.Visibility.Collapsed;
        }


        // methods
        private void SubscribeToUDPPackets() {

            // check 202 packet type [controllers and other fuck]
            UDPMessaging.OnMessageGet += (message) => {

                if (message[0] == 202) {

                    // repeat to remote
                    if (MainWindow.repeaterEndPoints != null && MainWindow.remoteMode is false) {
                        UDPMessaging.SendToRepeaters(message);
                    }

                    Dispatcher.Invoke(() => {
                        rusUpDown = BitConverter.ToSingle(message, 1);
                        rusLeftRight = BitConverter.ToSingle(message, 5);
                        rud = BitConverter.ToSingle(message, 9);
                        pedali = BitConverter.ToSingle(message, 13);
                        tormoza = message[21];
                        UpdatePointersScale();
                        UpdatePointersTransform();
                        UpdateTooltips();
                    });
                }
            };

            // check 203 packet type
            UDPMessaging.OnMessageGet += (message) => {
                if (message[0] == 203) {

                    // repeat to remote
                    if (MainWindow.repeaterEndPoints != null && MainWindow.remoteMode is false) {
                        UDPMessaging.SendToRepeaters(message);
                    }

                    Dispatcher.Invoke(() => {
                        //  TUBRO    => message[4];
                        //  DVIGATEL => message[5];

                        text_data.Text = $"ТУРБО: {(message[4] == 0 ? "не запущен" : "запущен")}";
                        text_data2.Text = $"ДВИГАТЕЛЬ: {(message[5] == 0 ? "не запущен" : "запущен")}";

                        if (message[5] == 0) {
                            restartEngineButton.Visibility = Visibility.Collapsed;
                        }
                        else {
                            restartEngineButton.Visibility = Visibility.Visible;
                        }
                    });

                    EncryptedSTData.UpdateDataBySpecificDatagram(message);
                }
            };

            // check 155 packet type
            UDPMessaging.OnMessageGet += (message) => {
                try {
                    Dispatcher.Invoke(() => {
                        if (message[0] == 155) {
                            switch (message[1]) {
                                //remote RUN simulation
                                case 1:
                                    RunSimulationProcess();
                                    break;
                                //remote KILL simulation
                                case 2:
                                    KillSimulationProcess();
                                    break;
                                //remote RESTART engine
                                case 3:
                                    RestartSimulationEngine();
                                    break;
                            }
                        }
                    });
                }
                catch { }
            };

            // check 154 packet type
            UDPMessaging.OnMessageGet += (message) => {
                Dispatcher.Invoke(() => {
                    //MainWindow.instance.Title = $"{message[0]}, {message[1]}, [{DateTime.Now}]";
                    if (message[0] == 154) {
                        switch (message[1]) {
                            //remote show KILL button
                            case 1:
                                runSimulationButttonViewBox.Visibility = System.Windows.Visibility.Collapsed;
                                killTargetsImmediatelyButtonViewBox.Visibility = System.Windows.Visibility.Visible;
                                break;
                            //remote show RUN button
                            case 2:
                                runSimulationButttonViewBox.Visibility = System.Windows.Visibility.Visible;
                                killTargetsImmediatelyButtonViewBox.Visibility = System.Windows.Visibility.Collapsed;
                                break;
                        }
                    }
                });
            };
            // check 41 packet type
            UDPMessaging.OnMessageGet += (message) => {
                Dispatcher.Invoke(() => {
                    if (message[0] == 41) {
                        UDPMessaging.SendToEndPoint(new byte[] { 41, 1 }, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5600));
                    }
                    else if (message[0] == 42) {
                        UDPMessaging.SendToEndPoint(new byte[] { 42, 1 }, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5687));
                    }
                    else if (message[0] == 43) {
                        recalibrationButton.Background = message[1] == 2 ? new SolidColorBrush(Color.FromArgb(138, 71, 71, 71)) : new SolidColorBrush(Color.FromArgb(138, 215, 25, 25));
                        recalibrationButton.Content = message[1] == 2 ? "Калибровать\nуправление" : "Завершить\nкалибровку";
                    }
                });
            };

        }
        private void SubcribeToControls() {
            // local fieds

            // local action for change button text
            Action<string> changeTextOnRunButton = (newText) => { (runSimulationButtton.Content as TextBlock).Text = newText; };
            Action<string> changeTextOnTerminateButton = (newText) => { (killTargetsImmediatelyButton.Content as TextBlock).Text = newText; };

            // events
            runSimulationButtton.Click += (_, _) => {

                // base protection for multiply launching
                if (Math.Abs((DateTime.Now - lastProcessLaunchDateTime).TotalSeconds) < 10) {
                    changeTextOnRunButton.Invoke($"Попробуйте через: {10 - Math.Abs((lastProcessLaunchDateTime - DateTime.Now).Seconds)} секунд");
                    return;
                }
                lastProcessLaunchDateTime = DateTime.Now;

                // default run if panel is using NOT REMOTE mode
                if (MainWindow.remoteMode is false) {
                    RunSimulationProcess();
                }
                // if panel in REMOTE mode
                else {
                    UDPMessaging.SendToRepeaters(new byte[] { 155, 1 });
                }

            };

            killTargetsImmediatelyButton.Click += (_, _) => {

                // default kill if panel is using NOT REMOTE mode
                if (MainWindow.remoteMode is false) {
                    KillSimulationProcess();
                }
                // if panel in REMOTE mode
                else {
                    UDPMessaging.SendToRepeaters(new byte[] { 155, 2 });
                }
            };

            // try send "228" byte array as command to remote address
            restartEngineButton.Click += (_, _) => {

                if (Math.Abs((DateTime.Now - lastRestartEngineDateTime).TotalSeconds) < 15) {
                    return;
                }
                lastRestartEngineDateTime = DateTime.Now;

                // default restart if panel is using NOT REMOTE mode
                if (MainWindow.remoteMode is false) {
                    RestartSimulationEngine();
                }
                // if panel in REMOTE mode
                else {
                    UDPMessaging.SendToRepeaters(new byte[] { 155, 3 });
                }
            };

            // try send "41" byte array as command to console
            recalibrationButton.Click += (_, _) => {

                if (MainWindow.remoteMode is false) {
                    UDPMessaging.SendToEndPoint(new byte[] { 41, 1 }, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5600));
                }
                else {
                    UDPMessaging.SendToRepeaters(new byte[] { 41, 1 });
                }
            };

            // send to UE message that i wanna reset camera position in packet "42"
            cameraResetButton.Click += (_, _) => {
                if (MainWindow.remoteMode is false) {
                    UDPMessaging.SendToEndPoint(new byte[] { 42, 1 }, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5687));
                }
                else {
                    UDPMessaging.SendToRepeaters(new byte[] { 42, 1 });
                }
            };

            // if target process launched - hide button
            ProcessManager.ProcessStarted += (info) => {
                foreach (string targetTitle in processTargets) {
                    if (info.Title == targetTitle) {
                        findedTargets.Add(info);

                        AirMapPanel.instance.EnableRecording();

                        MainWindow.instance.ForceMapPage();

                        runSimulationButttonViewBox.Visibility = System.Windows.Visibility.Collapsed;
                        killTargetsImmediatelyButtonViewBox.Visibility = System.Windows.Visibility.Visible;


                        if (MainWindow.remoteMode is false) {
                            UDPMessaging.SendToRepeaters(new byte[] { 154, 1 });
                        }

                        break;
                    }
                }
            };

            // if target process ended - return button
            ProcessManager.ProcessEnded += (info) => {

                if (findedTargets.Count == 0) {
                    return;
                }
                foreach (RunningAppInfo target in findedTargets) {
                    if (info.ID == target.ID && info.Title == target.Title) {
                        findedTargets.Remove(target);
                        break;
                    }
                }

                if (findedTargets.Count == 0) {

                    runSimulationButttonViewBox.Visibility = System.Windows.Visibility.Visible;
                    killTargetsImmediatelyButtonViewBox.Visibility = System.Windows.Visibility.Collapsed;

                    changeTextOnTerminateButton.Invoke("Аварийно завершить симуляцию");
                    changeTextOnRunButton.Invoke("Запустить симуляцию");
                    if (findedTargets.Count == 0) {
                        AirMapPanel.instance.DisableRecording();
                    }

                    if (MainWindow.remoteMode is false) {
                        UDPMessaging.SendToRepeaters(new byte[] { 154, 2 });
                    }
                }
            };

            MainWindow.PanelWasOpened += delegate {
                MainWindow.instance.Dispatcher.Invoke(delegate {
                    AttachProcessButton(controller_process_stackpanel);
                });
            };

            MainWindow.PanelWasClosed += delegate {
                MainWindow.instance.Dispatcher.Invoke(delegate {
                    DeattachProcessButton(controller_process_stackpanel);
                });
            };
        }
        private void AttachProcessButton(in StackPanel stackPanel) {
            try {
                controller_process_buttons_border.Child = null;
                stackPanel.Margin = new Thickness(2);
                MainWindow.instance.instuctor_grid_panel.Children.Add(stackPanel);
                Grid.SetColumn(stackPanel, 1);
                stackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            }
            catch { }
        }
        private void DeattachProcessButton(in StackPanel stackPanel) {
            try {
                stackPanel.Margin = new Thickness(8);
                MainWindow.instance.instuctor_grid_panel.Children.Remove(stackPanel);
                controller_process_buttons_border.Child = stackPanel;
            }
            catch { }
        }
        private void RefreshBarsScale() {
            Task.Run(async () => {
                await Task.Delay(500);
                savedStartupVerticalBarHeight = vertical_bar.ActualHeight;
                savedStartupCircleBarHeight = circle_bar.ActualHeight;
                savedStartupCircleBarWidth = circle_bar.ActualWidth;
            });
        }
        private void UpdatePointersScale() {

            //MainWindow.instance.panel_name.Content = columnCircleBar.Width.Value;
            pointer_vertical2.Width = vertical_bar2.ActualWidth;
            pointer_vertical.Width = vertical_bar2.ActualWidth;
            pointer_circle.Width = vertical_bar2.ActualWidth * (tormoza == 0 ? 1 : 0.1d);
        }
        private void UpdatePointersTransform() {

            // change RUD pointer position by RUD value;
            pointer_vertical.RenderTransform = new TranslateTransform() {
                Y = ((Math.Clamp(rud, 0, 1) * -116) * (vertical_bar.ActualHeight / savedStartupVerticalBarHeight) + 57 * (vertical_bar.ActualHeight / savedStartupVerticalBarHeight))
            };

            // change PEDALI pointer position by PEDALI value;
            pointer_vertical2.RenderTransform = new TranslateTransform() {
                X = ((Math.Clamp(pedali, -1, 1) * -45) * (circle_bar.ActualWidth / savedStartupCircleBarWidth))
            };

            // change RUS pointer position by RUS value;
            pointer_circle.RenderTransform = new TranslateTransform() {
                Y = (Math.Clamp(rusUpDown, -1, 1) * 60) * (circle_bar.ActualHeight / savedStartupCircleBarHeight),
                X = (Math.Clamp(rusLeftRight, -1, 1) * 60) * (circle_bar.ActualWidth / savedStartupCircleBarWidth)
            };
        }
        private void UpdateTooltips() {
            rusToolTip.text = $"Значение руса:\n" +
                        $"Вверх / Вниз = {rusUpDown.ToString("0.000")}\n" +
                        $"Влево / Вправо = {rusLeftRight.ToString("0.000")}";
            rudToolTip.text = $"Значение руда: {rud.ToString("0.000")}";
            pedaliToolTip.text = $"Значение педалей: {pedali.ToString("0.000")}";
        }
        private void TemperaturesMonitoring() {
            TemperatureMonitor temperatureMonitor = new TemperatureMonitor();

            Task.Run(async () => {
                while (true) {
                    await Task.Delay(2000);

                    if (!ToolTipPage.visible) { continue; }

                    DateTime gttime = DateTime.Now;

                    (float? cpuTemp, float? gpuTemp) temps = temperatureMonitor.GetTemperatures();

                    await Dispatcher.BeginInvoke(() => {
                        float? gpu = temps.gpuTemp;
                        float? cpu = temps.cpuTemp;
                        temperaturesToolTip.text = $"" +
                        $"[{gttime.Hour}:{gttime.Minute}:{gttime.Second}]\n" +
                        $"GPU: {gpu?.ToString("0.00")}\n" +
                        $"CPU: {cpu?.ToString("0.00")}";
                    });
                }
            });
        }
        private void PointersTest() {
            float sinV = 0;
            float cosV = 0;
            uint scVal = 0;

            Task.Run([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            [SkipLocalsInit] async () => {
                while (true) {
                    await Task.Delay(5);
                    await main_grid.Dispatcher.BeginInvoke([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                    [SkipLocalsInit] () => {
                        scVal++;
                        sinV = (float)Math.Sin(scVal * 0.05f);
                        cosV = (float)Math.Cos(scVal * 0.05f);

                        rusUpDown = Math.Clamp(sinV, -1, 1);
                        rusLeftRight = Math.Clamp(cosV, -1, 1);

                        rud = Math.Clamp(sinV, 0, 1);
                        pedali = Math.Clamp(cosV, -1, 1);

                        UpdatePointersTransform();
                        UpdateTooltips();
                    });
                }
            });
        }
        private void RunSimulationProcess() {
            (runSimulationButtton.Content as TextBlock).Text = "Попытка запустить процесс";

            string loadConfigPath = File.ReadAllText(@$"Configs\LoadAppPath.ini");

            if (loadConfigPath.Length < 1) {
                (runSimulationButtton.Content as TextBlock).Text = "Файл запуска: null";
                return;
            }

            (runSimulationButtton.Content as TextBlock).Text = "Ожидается запуск процесса";

            try {
                Process.Start(loadConfigPath);
            }
            catch {
                (runSimulationButtton.Content as TextBlock).Text = "invalid process";
                return;
            }

            (runSimulationButtton.Content as TextBlock).Text = "Процесс запущен удачно";
        }
        private void KillSimulationProcess() {
            foreach (RunningAppInfo target in findedTargets) {
                try {
                    (killTargetsImmediatelyButton.Content as TextBlock).Text = $"Завершаем: {target.ID}";
                    target.Process.Kill();
                }
                catch { }
            }
        }
        private void RestartSimulationEngine() {
            try {
                IPAddress.TryParse(restartEngineAddressData.Split(':')[0], out IPAddress parsedAddress);
                int.TryParse(restartEngineAddressData.Split(':')[1], out int parsedPort);

                UDPMessaging.SendToEndPoint(new byte[1] { 228 }, new IPEndPoint(parsedAddress, parsedPort));
            }
            catch { }
        }
    }
}
