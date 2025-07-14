using System.Windows;
using System.Windows.Controls;


namespace CustomDesktopShell {
    /// <summary>
    /// Страничка приложения, в которой системные гномы крутят карту с гугла и подгоняют самолетик под нее
    /// </summary>
    public partial class AirMapPanel : Page {
        /* values */
        private protected const double ETALON_WIDTH = 5832;
        private protected const double ETALON_HEIGHT = 2533;
        private protected const double H_COEF = 5832 / 2533;

        private static protected double currentWidth = ETALON_WIDTH;
        private static protected double lerpedWidth = ETALON_HEIGHT;

        private static protected double coefficientX = 1;
        private static protected double coefficientY = 1;

        private static protected bool goUp = false;
        private static protected bool goDown = false;
        private static protected bool goLeft = false;
        private static protected bool goRight = false;
        private static protected float speed = 20;

        private static protected double startOffsetX = 2871;
        private static protected double startOffsetY = 1315;
        private static protected double customOffsetX = 0;
        private static protected double customOffsetY = 0;

        private static protected double radioHeight = 0;
        private static protected double radioHeightLerped = 0;

        public static double angle = 101.4d; //поворот XY осей
        public static double customDivide = 0.00025d;


        /* constructor */
        public AirMapPanel() {
            InitializeComponent();
            AirMapImage.Width = currentWidth;
            AirMapImage.Height = AirMapImage.Width / H_COEF;

            ZoomInButton.Click += (_, _) => {
                lerpedWidth += 128;
                if (lerpedWidth > ETALON_WIDTH * 2f) lerpedWidth = ETALON_WIDTH * 2f;
            };
            ZoomOutButton.Click += (_, _) => {
                lerpedWidth -= 128;
                if (lerpedWidth < ETALON_WIDTH / 5) lerpedWidth = ETALON_WIDTH / 5;
            };

            RecUp.MouseEnter += (_, _) => { goUp = true; };
            RecDown.MouseEnter += (_, _) => { goDown = true; };
            RecLeft.MouseEnter += (_, _) => { goLeft = true; };
            RecRight.MouseEnter += (_, _) => { goRight = true; };

            RecUp.MouseLeave += (_, _) => { goUp = false; };
            RecDown.MouseLeave += (_, _) => { goDown = false; };
            RecLeft.MouseLeave += (_, _) => { goLeft = false; };
            RecRight.MouseLeave += (_, _) => { goRight = false; };

            UDPWork.UDPMessaging.OnMessageGet += (packet) => {
                byte packetType = packet[0];

                if (packetType != 101) return;

                double posX = BitConverter.ToDouble(packet, 1);
                double posZ = BitConverter.ToDouble(packet, 9);
                double radio_height = BitConverter.ToDouble(packet, 17);

                customOffsetX = posX * customDivide;
                customOffsetY = posZ * customDivide;
                radioHeight = radio_height;
            };

            /* немного магии */
            Task.Run(async () => {
                while (true) {
                    try {
                        await Task.Delay(50);
                        currentWidth += (lerpedWidth - currentWidth) * 0.1f;
                        MainGrid.Dispatcher.Invoke(() => {
                            AirMapImage.Width = currentWidth;
                            AirMapImage.Height = AirMapImage.Width / H_COEF;

                            coefficientX = currentWidth / ETALON_WIDTH;
                            coefficientY = (AirMapImage.Width / H_COEF) / ETALON_HEIGHT;

                            double angleRadians = angle * Math.PI / 180;

                            var convertedOffsetX = (customOffsetX) * coefficientX;
                            var convertedOffsetY = (customOffsetY) * coefficientY;

                            double xRotated = convertedOffsetX * Math.Cos(angleRadians) - convertedOffsetY * Math.Sin(angleRadians);
                            double yRotated = convertedOffsetX * Math.Sin(angleRadians) + convertedOffsetY * Math.Cos(angleRadians);


                            if (goDown || goUp || goLeft || goRight) {
                                speed = Math.Clamp(speed + 0.2f, 0, 20);
                            }
                            else if ((goDown | goUp | goLeft | goRight) == false) {
                                speed = Math.Clamp(speed - 0.7f, 0, 20);
                            }

                            if (goDown) { AirMapScrollViewer.ScrollToVerticalOffset(AirMapScrollViewer.VerticalOffset + speed); }
                            if (goUp) { AirMapScrollViewer.ScrollToVerticalOffset(AirMapScrollViewer.VerticalOffset - speed); }
                            if (goLeft) { AirMapScrollViewer.ScrollToHorizontalOffset(AirMapScrollViewer.HorizontalOffset - speed); }
                            if (goRight) { AirMapScrollViewer.ScrollToHorizontalOffset(AirMapScrollViewer.HorizontalOffset + speed); }

                            var a = AirMapImage.TranslatePoint(new Point(AirMapImage.Width, AirMapImage.Height), AirMapContentGrid);
                            //debugger.Content = $"Текущее смещение самолета: x-{(int)(customOffsetX)}; z-{(int)(customOffsetY)}";
                            //debugger.Content = $"{(int)(a.X)}\\{(int)(a.Y)} | cX:{(int)(coefficientX * 10)}; cY:{(int)(coefficientY * 10)}";

                            radioHeightLerped += (radioHeight - radioHeightLerped) * 0.1f;
                            debugger.Content = $"Высота самолета: {(radioHeight / 100).ToString("0.0")} м";

                            Canvas.SetLeft(AirPlane, ((startOffsetX * coefficientX) + xRotated));
                            Canvas.SetTop(AirPlane, ((startOffsetY * coefficientY) + yRotated));
                            AirMapContentGrid.Height = AirMapImage.Height;
                            AirMapContentGrid.Width = AirMapImage.Width;
                        });
                    }
                    catch { break; }
                }
            });
        }

        /* methods */
        public static void ForceOffset(double x, double y) {
            customOffsetX = x * customDivide;
            customOffsetY = y * customDivide;
        }
        public static void ForceStartOffset(double x, double y) {
            startOffsetX = x;
            startOffsetY = y;
        }
    }
}
