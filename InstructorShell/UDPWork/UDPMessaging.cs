using InstructorShell;
using System.Net;
using System.Net.Sockets;


namespace CustomDesktopShell.UDPWork {
    internal static class UDPMessaging {
        /* values */
        private static bool crashed = false;
        private static Action OnCrashed = () => { };
        public static readonly IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Any, 5688);

        public static Action<byte[]> OnMessageGet = (_) => { };
        public static UdpClient receiver;


        /* methods */
        internal static void Initialize() {
            receiver = new UdpClient(localEndpoint);

            MainWindow.instance.Closing += (obj, args) => {
                receiver.Close();
            };

            OnCrashed += delegate {
                receiver.Close();
                receiver = new UdpClient(localEndpoint);
                crashed = false;
                StartReceive();
            };
            StartReceive();
        }
        internal static void StartReceive() {
            if (crashed) { return; }

            Task.Run(() => {
                while (crashed is false) {
                    try {
                        IPEndPoint randomSender = null;
                        byte[] packet = receiver.Receive(ref randomSender);
                        OnMessageGet(packet);
                    }
                    catch (Exception ex) {
                        crashed = true;
                        receiver.Close();
                        OnCrashed();
                        return;
                    }
                }
            });
        }
        internal static void SendToEndPoint(byte[] datagram, IPEndPoint endPoint) {
            try {
                receiver.Send(datagram, endPoint);
            }
            catch (Exception ex) {
                crashed = true;
                receiver.Close();
                OnCrashed();
            }
        }
    }
}
