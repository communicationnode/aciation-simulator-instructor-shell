using System.Net;
using System.Windows.Controls;

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
        }

        public void SendTimeOfDay(double value) {
            byte[] packet = new byte[2];
            packet[0] = 1;
            packet[1] = (byte)(value * 2.2f);

            IPEndPoint receiver = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5687);
            UDPWork.UDPMessaging.SendToEndPoint(packet, receiver);
        }
        public void SendWind(double value) {
            byte[] packet = new byte[2];
            packet[0] = 2;
            packet[1] = (byte)(value);

            IPEndPoint receiver = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5687);
            UDPWork.UDPMessaging.SendToEndPoint(packet, receiver);
        }
        public void SendWeather(byte value) {
            byte[] packet = new byte[2];
            packet[0] = 3;
            packet[1] = value;

            IPEndPoint receiver = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5687);
            UDPWork.UDPMessaging.SendToEndPoint(packet, receiver);
        }
    }
}
