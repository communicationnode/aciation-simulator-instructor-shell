using System.Net;
using System.Windows.Controls;
using CustomDesktopShell.UDPWork;
using InstructorShell;

namespace CustomDesktopShell {
    /// <summary>
    /// Логика взаимодействия для WeatherPanel.xaml
    /// </summary>
    public partial class WeatherPanel : Page {
        public WeatherPanel() {
            InitializeComponent();

            SunButton.Click += (_, _) => { SendWeather(0); };
            CloudButton.Click += (_, _) => { SendWeather(1); };
            RainButton.Click += (_, _) => { SendWeather(2); };
            SnowButton.Click += (_, _) => { SendWeather(3); };
            DustButton.Click += (_, _) => { SendWeather(4); };
            MegaRainButton.Click += (_, _) => { SendWeather(5); };
            MegaSnowButton.Click += (_, _) => { SendWeather(6); };
            MegaDustButton.Click += (_, _) => { SendWeather(7); };

            WindSlider.ValueChanged += (_, arg) => { SendWind(arg.NewValue); };
            TimeOfDaySlider.ValueChanged += (_, arg) => { SendTimeOfDay(arg.NewValue); };

            AllowUdpPackets();
        }

        void AllowUdpPackets() {
            UDPMessaging.OnMessageGet += (packet) => {
                if (packet[0] == 1 || packet[0] == 2 || packet[0] == 3) {
                    IPEndPoint receiver = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5687);
                    UDPWork.UDPMessaging.SendToEndPoint(packet, receiver);
                }
            };
        }


        public void SendTimeOfDay(double value) {

            byte[] packet = new byte[2];
            packet[0] = 1;
            packet[1] = (byte)(value);

            if (MainWindow.remoteMode is false) {

                IPEndPoint receiver = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5687);
                UDPWork.UDPMessaging.SendToEndPoint(packet, receiver);
            }
            else {
                if (MainWindow.repeaterEndPoints is null) { return; }
                foreach (var endPoint in MainWindow.repeaterEndPoints) {
                    try {
                        
                        UDPMessaging.SendToEndPoint(packet, endPoint);
                    }
                    catch { }
                }
            }


        }
        public void SendWind(double value) {

            byte[] packet = new byte[2];
            packet[0] = 2;
            packet[1] = (byte)(value);

            if (MainWindow.remoteMode is false) {

                IPEndPoint receiver = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5687);
                UDPWork.UDPMessaging.SendToEndPoint(packet, receiver);
            }
            else {
                if (MainWindow.repeaterEndPoints is null) { return; }
                foreach (var endPoint in MainWindow.repeaterEndPoints) {
                    try {
                        
                        UDPMessaging.SendToEndPoint(packet, endPoint);
                    }
                    catch { }
                }
            }


        }
        public void SendWeather(byte value) {

            byte[] packet = new byte[2];
            packet[0] = 3;
            packet[1] = value;

            if (MainWindow.remoteMode is false) {

                IPEndPoint receiver = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5687);
                UDPWork.UDPMessaging.SendToEndPoint(packet, receiver);
            }
            else {
                if (MainWindow.repeaterEndPoints is null) { return; }
                foreach (var endPoint in MainWindow.repeaterEndPoints) {
                    try {
                        
                        UDPMessaging.SendToEndPoint(packet, endPoint);
                    }
                    catch { }
                }
            }
        }
    }
}
