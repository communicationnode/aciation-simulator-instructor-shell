using CustomDesktopShell.UDPWork;
using InstructorShell;
using InstructorShell.DataClasses;
using InstructorShell.DataOutputXML;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CustomDesktopShell {

    /// <summary>
    /// Страничка приложения, в которой системные гномы крутят карту с гугла и подгоняют самолетик под нее
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    [SkipLocalsInit]
    public partial class AirMapPanel : Page {

        // fields
        internal static AirMapPanel instance = null;
        internal static Queue<MapObject> mapObjectsUpdatingQueue { get; private set; } = new Queue<MapObject>();

        //map scaling parameters
        public const double TO_RADIANS_MULTIPLIER = Math.PI / 180;
        private protected const double ETALON_NEEDED_WIDTH = 5832;
        private protected const double STANDART_NEEDED_HEIGHT = 2533;
        private protected const double HEIGHT_COEFFICIENT = 5832 / 2533;

        //map scaling parameters
        private static protected double currentMapWidth = ETALON_NEEDED_WIDTH;
        private static protected double lerpedMapWidth = STANDART_NEEDED_HEIGHT;

        //world coordinates coef
        public static double coefficientX = 1;
        public static double coefficientY = 1;

        //mouse drag-n-move map
        private static protected bool mapDragMove = false;
        private static protected Point cachedMousePos;
        private static protected Point cachedMapOffset;

        //airplane position
        private static protected MapObject? airplane;

        // airplane offset parameters
        // XY axys world angle
        public static double angle = 101.4d;
        // coordinate offset multiplier
        public static double coordinateWorldScaleDivide = 0.00025d;


        // constuctor
        [SkipLocalsInit]
        public AirMapPanel() {
            InitializeComponent();

            instance = this;

            AirMapImage.Width = currentMapWidth;
            AirMapImage.Height = AirMapImage.Width / HEIGHT_COEFFICIENT;

            airplane = MapObject.Spawn(2871, 1315, 32, 32, icon: "dev_arrow.png", name: "Испытуемый").AddToUpdateQueue();
            airplane.RotateElement(-90 - 34);
            MainWindow.instance.SetTooltip(airplane.uiElement, new ToolTipText(airplane.name));

            AllowUDPPackets();
            AllowControls();
            AllowDragMoveMap();
            UpdateWorld();
            QueueLoop();
            StartPointCreate();
            StartRecordTimer();

            /*Task.Run(async () => {
                while (true) {
                    await Task.Delay(4000);
                    // DEBUG FUNCTION
                    EncryptedSTData.AllRandomize();
                }
            });*/

            StatisticRecordButtonTextBox.Text = $"{(DataRecorder.enabled is false ? "Запись" : "Остановить")}";


            if (MainWindow.remoteMode) {
                StatisticRecordButton.Visibility = Visibility.Collapsed;
            }
        }

        /* methods */
        //jint invokable by
        public static void ForceStartOffset(in double x, in double y) {
            airplane.position.X = x;
            airplane.position.Y = y;
        }
        public static void ForceOffset(in double x, in double y) {
            airplane.offset.X = x * coordinateWorldScaleDivide;
            airplane.offset.Y = y * coordinateWorldScaleDivide;
        }

        //local invokable
        private void AllowUDPPackets() {
            UDPWork.UDPMessaging.OnMessageGet += [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            [SkipLocalsInit] (packet) => {

                // validate packet type
                byte packetType = packet[0];
                if (packetType != 101) return;

                // repeat to remote
                if (MainWindow.repeaterEndPoints != null && MainWindow.remoteMode is false) {
                    foreach (var endPoint in MainWindow.repeaterEndPoints) {
                        try {
                            UDPMessaging.SendToEndPoint(packet, endPoint);
                        }
                        catch { }
                    }
                }

                // encrypt data from datagram
                double encryptedPositionX = BitConverter.ToDouble(packet, 1);
                double encryptedPositionZ = BitConverter.ToDouble(packet, 9);
                double encryptedRadHeight = BitConverter.ToDouble(packet, 17);


                // apply encrypted to fields
                airplane.SetOffset(new Point(
                    encryptedPositionX * coordinateWorldScaleDivide,
                    encryptedPositionZ * coordinateWorldScaleDivide));
                airplane.radioHeight = encryptedRadHeight;


                if (packet.Length >= 25) {
                    double encryptedRotationZ = BitConverter.ToDouble(packet, 25);
                    AirMapContentGrid.Dispatcher.Invoke([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                    [SkipLocalsInit] () => {
                        airplane.RotateElement(encryptedRotationZ + 10);
                    });
                }
            };
        }
        private void AllowDragMoveMap() {
            Task.Run([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            [SkipLocalsInit] async () => {
                while (true) {
                    await Task.Delay(15);

                    MainGrid.Dispatcher.Invoke([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                    [SkipLocalsInit] () => {
                        Point distance = new Point() {
                            X = cachedMousePos.X - Mouse.GetPosition(this).X,
                            Y = cachedMousePos.Y - Mouse.GetPosition(this).Y,
                        };

                        if (mapDragMove is false) {

                        }
                        else {
                            AirMapScrollViewer.ScrollToVerticalOffset(distance.Y + cachedMapOffset.Y);
                            AirMapScrollViewer.ScrollToHorizontalOffset(distance.X + cachedMapOffset.X);
                        }
                    });
                }
            });
        }
        private void AllowControls() {
            //by buttons
            ZoomInButton.Click += (_, _) => {
                lerpedMapWidth += 128;
                if (lerpedMapWidth > ETALON_NEEDED_WIDTH * 2f) lerpedMapWidth = ETALON_NEEDED_WIDTH * 2f;
            };
            ZoomOutButton.Click += (_, _) => {
                lerpedMapWidth -= 128;
                if (lerpedMapWidth < ETALON_NEEDED_WIDTH / 5) lerpedMapWidth = ETALON_NEEDED_WIDTH / 5;
            };

            //by mouse
            AirMapImage.MouseLeftButtonDown += (_, _) => {
                cachedMapOffset = new Point() {
                    Y = AirMapScrollViewer.VerticalOffset,
                    X = AirMapScrollViewer.HorizontalOffset
                };
                cachedMousePos = Mouse.GetPosition(this);
                mapDragMove = true;
                MainGrid.Dispatcher.Invoke(() => { AirMapImage.Cursor = Cursors.SizeAll; });
            };

            AirMapImage.MouseLeftButtonUp += (_, _) => {
                mapDragMove = false;
                MainGrid.Dispatcher.Invoke(() => { AirMapImage.Cursor = Cursors.Arrow; });
            };

            AirMapImage.MouseLeave += (_, _) => {
                mapDragMove = false;
                MainGrid.Dispatcher.Invoke(() => { AirMapImage.Cursor = Cursors.Hand; });
            };

            StatisticRecordButton.Click += (_, _) => {
                ToggleRecording();
            };

            //by wtf i don't rememba
            AirMapScrollViewer.PreviewMouseWheel += (sender, e) => {
                e.Handled = true;
                lerpedMapWidth += e.Delta * 2;

                if (lerpedMapWidth > ETALON_NEEDED_WIDTH * 3f) lerpedMapWidth = ETALON_NEEDED_WIDTH * 3f;
                if (lerpedMapWidth < ETALON_NEEDED_WIDTH / 8) lerpedMapWidth = ETALON_NEEDED_WIDTH / 8;
            };
        }
        private void QueueLoop() {
            Task.Run(async () => {
                while (true) {

                    await Task.Delay(1);
                    if (mapObjectsUpdatingQueue.Count <= 0) continue;

                    for (ushort step = 0; step < mapObjectsUpdatingQueue.Count; step++) {
                        MapObject enq = mapObjectsUpdatingQueue.Dequeue();

                        // if enq is dead -> continue
                        if (enq.uiElement is null) {
                            enq.isQueueable = false;
                            continue;
                        }

                        if (DataRecorder.enabled is false && enq.name is "point") {
                            await Dispatcher.BeginInvoke(() => {

                                // free all objects, declared as "point", if record is disabled
                                if (DataRecorder.enabled is false && enq.name is "point") {
                                    enq.Free();
                                    return;
                                }
                            });
                        }

                        else {
                            await Dispatcher.BeginInvoke(() => {
                                enq.UpdateLocation();
                            });
                        }

                        // if enq still alive -> return instance to queue
                        if (enq.isQueueable) {
                            mapObjectsUpdatingQueue.Enqueue(enq);
                        }
                    }
                }
            });
        }
        private void ResizeMap() {
            // map size applying
            AirMapImage.Width = currentMapWidth;
            AirMapImage.Height = AirMapImage.Width / HEIGHT_COEFFICIENT;

            // resize coefficient update
            coefficientX = currentMapWidth / ETALON_NEEDED_WIDTH;
            coefficientY = (AirMapImage.Width / HEIGHT_COEFFICIENT) / STANDART_NEEDED_HEIGHT;

            // apply content grid size by airmap size parameters
            AirMapContentGrid.Height = AirMapImage.Height;
            AirMapContentGrid.Width = AirMapImage.Width;
        }
        private void UpdateWorld() {

            // run infinity world updating
            Task.Run([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            [SkipLocalsInit] async () => {

                Color defaultButton = Color.FromArgb(255, 0, 0, 0);
                Color BippedButton = Color.FromArgb(255, 118, 4, 0);

                while (true) {
                    try {
                        await Task.Delay(1);

                        currentMapWidth += (lerpedMapWidth - currentMapWidth) * 0.075f;

                        // redirect actions to UI dispatcher
                        await MainGrid.Dispatcher.BeginInvoke([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        [SkipLocalsInit] () => {

                            if (MainWindow.panelActivateState is false || MainWindow.instance.instructor_frames.Content.GetType() != this.GetType()) {
                                return;
                            }

                            ResizeMap();

                            PanelInformerLabel.Text = $"" +
                                    $"Скорость: {EncryptedSTData.Data.PRIBORS_AIRSPEED.ToString("0.0")}\n" +
                                    $"Крен: {EncryptedSTData.Data.PRIBORS_KREN.ToString("0.0")}\n" +
                                    $"Тангаж: {EncryptedSTData.Data.PRIBORS_TANGAJ.ToString("0.0")}\n" +
                                    $"Вариометр: {EncryptedSTData.Data.PRIBORS_VY.ToString("0.0")}\n" +
                                    $"Барометр. Высота: {EncryptedSTData.Data.PRIBORS_BAROM_HEIGHT.ToString("0.0")}\n" +
                                    $"Радиокомпас: {EncryptedSTData.Data.PRIBORS_RADIOCOMPASS.ToString("0.0")}\n" +
                                    $"Время записи (секунд): {EncryptedSTData.Data.recorded_second}";

                            StatisticRecordButton.Background = new SolidColorBrush() { Color = EncryptedSTData.Data.recorded_second % 2 == 0 ? defaultButton : BippedButton };
                        });
                    }
                    catch { break; }
                }
            });
        }
        private void StartPointCreate() {
            Task.Run([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            [SkipLocalsInit] async () => {
                Point cached;
                while (true) {

                    await Task.Delay(256);

                    if (!DataRecorder.enabled) { continue; }

                    if (Math.Abs(cached.X - airplane.offset.X) > 4 || Math.Abs(cached.Y - airplane.offset.Y) > 4) {

                        await AirMapContentGrid.Dispatcher.BeginInvoke([MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                        [SkipLocalsInit] () => {

                            MapObject point = MapObject.Spawn(2871, 1315, 8, 8, "NaN", "point").AddToUpdateQueue().SetOffset(airplane.offset);
                            cached = airplane.offset;

                            MainWindow.instance.SetTooltip(
                                point.uiElement,
                                new ToolTipText(
                                    $"Скорость: {EncryptedSTData.Data.PRIBORS_AIRSPEED.ToString("0.0")}\n" +
                                    $"Крен: {EncryptedSTData.Data.PRIBORS_KREN.ToString("0.0")}\n" +
                                    $"Тангаж: {EncryptedSTData.Data.PRIBORS_TANGAJ.ToString("0.0")}\n" +
                                    $"Вариометр: {EncryptedSTData.Data.PRIBORS_VY.ToString("0.0")}\n" +
                                    $"Барометр. Высота: {EncryptedSTData.Data.PRIBORS_BAROM_HEIGHT.ToString("0.0")}\n" +
                                    $"Время записи (секунд): {EncryptedSTData.Data.recorded_second}"));
                        });
                    }
                }
            });
        }
        public void ToggleRecording() {

            DataRecorder.enabled = !DataRecorder.enabled;

            StatisticRecordButtonTextBox.Text = $"{(DataRecorder.enabled is false ? "Запись" : "Остановить")}";

            if (DataRecorder.enabled is false) {

                if (MainWindow.remoteMode is false) {
                    XMLOutputEntity.DrawGraph(DataRecorder.recordedData.ToArray());
                    DataRecorder.recordedData.Clear();

                    GC.Collect(2, GCCollectionMode.Aggressive, true, true);
                }
                else {
                    DataRecorder.recordedData.Clear();
                    GC.Collect(2, GCCollectionMode.Aggressive, true, true);
                }

            }
            else {
                DataRecorder.recordedData.Clear();
                EncryptedSTData.Data.recorded_second = 0;
            }
        }
        public void EnableRecording() {
            if (DataRecorder.enabled is true) return;

            DataRecorder.enabled = true;

            StatisticRecordButtonTextBox.Text = "Остановить";

            DataRecorder.recordedData.Clear();
            EncryptedSTData.Data.recorded_second = 0;
        }
        public void DisableRecording() {
            if (DataRecorder.enabled is false) return;
            DataRecorder.enabled = false;

            StatisticRecordButtonTextBox.Text = "Запись";

            if (MainWindow.remoteMode is false) {
                XMLOutputEntity.DrawGraph(DataRecorder.recordedData.ToArray());
            }
            DataRecorder.recordedData.Clear();

            GC.Collect(2, GCCollectionMode.Aggressive, true, true);
        }
        private void StartRecordTimer() {

            Task.Run(async () => {
                while (true) {
                    if (!DataRecorder.enabled) {
                        EncryptedSTData.Data.recorded_second = 0;
                        continue;
                    }

                    await Task.Delay(1000);
                    EncryptedSTData.Data.recorded_second++;

                    if (MainWindow.remoteMode is false) {
                        DataRecorder.recordedData.Add(EncryptedSTData.WriteEntity());
                    }
                }
            });
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [SkipLocalsInit]
    internal sealed class MapObject {
        // fields
        public bool isQueueable = false;
        public static readonly BitmapImage defaultIco = new BitmapImage(new Uri($"pack://application:,,,/Images/Widgets/point.png")) {
            CacheOption = BitmapCacheOption.OnLoad,
            CreateOptions = BitmapCreateOptions.PreservePixelFormat,
        };
        internal double radioHeight = 0;
        internal string name = "undefined";
        internal Point position;
        internal Point offset;
        internal UIElement? uiElement = null;

        // methods
        /// <summary> Main <see cref="UIElement"/> position on map </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal MapObject SetPosition(in Point pos) {
            this.position = pos;
            return this;
        }

        /// <summary> Do <see href="position + offset"/></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal MapObject SetOffset(in Point offset) {
            this.offset = offset;
            return this;
        }

        /// <summary> Update <see cref="UIElement"/> position by <see href="main position + offset"/> in world map </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal MapObject UpdateLocation() {
            // world rotation apply (in eulers)
            double angleRadians = AirMapPanel.angle * AirMapPanel.TO_RADIANS_MULTIPLIER;

            // resize position by map resize coefficient
            double resizedOffsetX = (this.offset.X) * AirMapPanel.coefficientX;
            double resizedOffsetY = (this.offset.Y) * AirMapPanel.coefficientY;

            // rotate position by world rotation angle
            double resizedRotatedOffsetX = resizedOffsetX * Math.Cos(angleRadians) - resizedOffsetY * Math.Sin(angleRadians);
            double resizedRotatedOffsetY = resizedOffsetX * Math.Sin(angleRadians) + resizedOffsetY * Math.Cos(angleRadians);

            try {
                // update air_plane object position by world parameters
                Canvas.SetLeft(this.uiElement, ((this.position.X * AirMapPanel.coefficientX) + resizedRotatedOffsetX));
                Canvas.SetTop(this.uiElement, ((this.position.Y * AirMapPanel.coefficientY) + resizedRotatedOffsetY));
            }
            catch { }
            return this;
        }

        /// <summary> Apply <see cref="UIElement"/> as reference in <see cref="MapObject.uiElement"/></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal MapObject SetElement(in UIElement uiElement) {
            this.uiElement = uiElement;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal MapObject RotateElement(in double degrees = 0) {

            if (uiElement is null || uiElement.GetType() != typeof(Image)) return this;

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new TranslateTransform(((uiElement as Image).MaxWidth / 2) * (-1), ((uiElement as Image).MaxHeight / 2) * (-1)));
            transformGroup.Children.Add(new RotateTransform(degrees, 0, 0));

            uiElement.RenderTransform = transformGroup;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal MapObject AddToUpdateQueue() {
            isQueueable = true;
            AirMapPanel.mapObjectsUpdatingQueue.Enqueue(this);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal void Free() {
            isQueueable = false;
            AirMapPanel.instance.AirMapContentGrid.Children.Remove(this.uiElement);
            uiElement = null;
            name = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal static MapObject Spawn(in double posX = 0, in double posY = 0, in double sizeX = 32, in double sizeY = 32, in string icon = "NaN", in string name = "undefined") {
            Image imageElement = new Image();
            imageElement.MaxWidth = sizeX;
            imageElement.MaxHeight = sizeY;
            imageElement.Source = icon == "NaN" ? MapObject.defaultIco : new BitmapImage(new Uri($"pack://application:,,,/Images/{icon}"));
            imageElement.HorizontalAlignment = HorizontalAlignment.Center;
            imageElement.VerticalAlignment = VerticalAlignment.Center;

            AirMapPanel.instance.AirMapContentGrid.Children.Add(imageElement);
            MapObject newMapObject = new MapObject().SetElement(imageElement).SetPosition(new Point(posX, posY)).UpdateLocation();
            newMapObject.name = name;
            newMapObject.RotateElement(0);

            return newMapObject;
        }
    }
}
