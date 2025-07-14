using CustomDesktopShell.UDPWork;

using System.Windows.Controls;
using System.Windows.Media;


namespace InstructorShell {
    /// <summary>
    /// Логика взаимодействия для ControllerParametersPage.xaml
    /// </summary>
    public partial class ControllerParametersPage : Page {
        double savedStartupVerticalBarHeight;
        double savedStartupCircleBarHeight;
        double savedStartupCircleBarWidth;

        byte tormoza = 0;
        public ControllerParametersPage() {
            InitializeComponent();

            Task.Run(async () => {
                await Task.Delay(500);
                savedStartupVerticalBarHeight = vertical_bar.ActualHeight;
                savedStartupCircleBarHeight = circle_bar.ActualHeight;
                savedStartupCircleBarWidth = circle_bar.ActualWidth;
            });

            UDPMessaging.OnMessageGet += (message) => {
                if (message[0] == 202) {
                    Dispatcher.Invoke(() => {
                        float rusUpDown = BitConverter.ToSingle(message, 1);
                        float rusLeftRight = BitConverter.ToSingle(message, 5);
                        float rud = BitConverter.ToSingle(message, 9);
                        float pedali = BitConverter.ToSingle(message, 13);

                        tormoza = message[21];

                        pointer_vertical.Width = vertical_bar.ActualWidth;
                        pointer_vertical2.Width = pointer_vertical.ActualWidth;
                        pointer_circle.Width = pointer_vertical.ActualWidth * (tormoza == 0 ? 1 : 0.3d);


                        pointer_vertical.RenderTransform = new TranslateTransform() {
                            Y = ((rud * -300) * (vertical_bar.ActualHeight / savedStartupVerticalBarHeight) + 160 * (vertical_bar.ActualHeight / savedStartupVerticalBarHeight))
                        };
                        pointer_vertical2.RenderTransform = new TranslateTransform() {
                            X = ((pedali * -150) * (circle_bar.ActualWidth / savedStartupCircleBarWidth))
                        };
                        pointer_circle.RenderTransform = new TranslateTransform() {
                            Y = (rusUpDown * 100) * (circle_bar.ActualHeight / savedStartupCircleBarHeight),
                            X = (rusLeftRight * 100) * (circle_bar.ActualWidth / savedStartupCircleBarWidth)
                        };

                        // ПЕРЕХВАЧЕНЫ ПАРАМЕТРЫ УПРАВЛЕНИЯ. ПРИДУМАЙ С ЭТИМ ЧТО-НИБУДЬ
                    });
                }
            };
            UDPMessaging.OnMessageGet += (message) => {
                if (message[0] == 203) {
                    Dispatcher.Invoke(() => {
                        float turbo = message[4];
                        float dvigatel = message[5];

                        text_data.Text = $"ТУРБО: {(turbo == 0 ? "не запущен" : "запущен")}";
                        text_data2.Text = $"ДВИГАТЕЛЬ: {(dvigatel == 0 ? "не запущен" : "запущен")}";
                        // ПЕРЕХВАЧЕНЫ ПАРАМЕТРЫ УПРАВЛЕНИЯ. ПРИДУМАЙ С ЭТИМ ЧТО-НИБУДЬ
                    });
                }
            };
        }
    }
}
