using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using InstructorShell;
using InstructorShell.DataClasses;

namespace CustomDesktopShell {
    /// <summary>
    /// Логика взаимодействия для InstuctorTestPanel.xaml
    /// </summary>
    public partial class InstuctorTestPanel : Page {


        // constructor
        public InstuctorTestPanel() {
            InitializeComponent();
            StartUpdate();
        }

        // methods
        public void CreateArrowOn(in Grid gridBlock, out ImageFields transform, int arrowType = 0, Thickness margin = default) {

            arrowType = Math.Clamp(arrowType, 0, 5);

            string baseUri = "pack://application:,,,/Images/stat_panel/accelerometer_arrow";

            if (arrowType <= 1) {
                baseUri += @$".png";
            }
            else {
                baseUri += @$"{arrowType}.png";
            }

            // initialize new image
            Image arrow = new Image();
            arrow.Source = new BitmapImage(new Uri(baseUri));
            arrow.RenderTransformOrigin = new System.Windows.Point(0.506f, 0.50f);

            // create new transform group
            TransformGroup arrowsTransform = new TransformGroup();

            // create translate transform
            TranslateTransform _translate = new TranslateTransform(-1.5f, -116.94f);
            arrowsTransform.Children.Add(_translate);

            // create scale transform
            ScaleTransform _scale = new ScaleTransform(0.5f, 0.5f);
            arrowsTransform.Children.Add(new ScaleTransform(0.5f, 0.5f));

            // create rotation transform
            RotateTransform _rotation = new RotateTransform(0);
            arrowsTransform.Children.Add(_rotation);

            // create refs container
            transform = new ImageFields() {
                rotation = _rotation,
                translate = _translate,
                scale = _scale,
                image = arrow,
            };

            // apply transform group
            arrow.RenderTransform = arrowsTransform;
            arrow.Margin = margin;
            // add as children
            gridBlock.Children.Add(arrow);
        }
        public void StartUpdate() {
            CreateArrowOn(block_speed, out ImageFields speed_transform);
            CreateArrowOn(block_variometer, out ImageFields variometer_transform);
            CreateArrowOn(block_oborots, out ImageFields oborots_kontur_1_transform, 4);
            CreateArrowOn(block_oborots, out ImageFields oborots_kontur_2_transform, 3);
            CreateArrowOn(block_gastemperatures, out ImageFields gastemperatures_transform, 5);
            CreateArrowOn(block_radioheight, out ImageFields radioheight_transform);
            CreateArrowOn(block_radioheight_700, out ImageFields radioheight700_transform, 5);
            CreateArrowOn(block_radiocompass, out ImageFields radiocompass_transform);
            CreateArrowOn(block_fuel, out ImageFields fuel_transform, 5);
            CreateArrowOn(block_barometer, out ImageFields barometer_transform);

            oborots_kontur_1_transform.rotation.Angle = 25;
            oborots_kontur_2_transform.rotation.Angle = 95;

            // speed
            ToolTipText speedToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_speed, speedToolTip);

            // tangaj kren
            ToolTipText krenTangajToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_tangajkren, krenTangajToolTip);

            // variometer
            ToolTipText variometerToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_variometer, variometerToolTip);

            // oborots
            ToolTipText oborotsToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_oborots, oborotsToolTip);

            // gastemperatures
            ToolTipText gastemperaturesToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_gastemperatures, gastemperaturesToolTip);

            // radioheight
            ToolTipText radioheightToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_radioheight, radioheightToolTip);

            // kurso glisada
            ToolTipText kursoglisadaToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_kursoglisada, kursoglisadaToolTip);

            // radiocompass
            ToolTipText radiocompassToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_radiocompass, radiocompassToolTip);

            // oil fuel pressures & temperatures
            ToolTipText oilfuelpressuresToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_fueloilbarometer, oilfuelpressuresToolTip);

            // fuel
            ToolTipText fuelToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_fuel, fuelToolTip);

            // radioheight700
            ToolTipText radioheight700ToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_radioheight_700, radioheight700ToolTip);

            // pressure
            ToolTipText pressureToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_barometer, pressureToolTip);

            // vibrometer
            ToolTipText vibrometerToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_enginespeed, vibrometerToolTip);

            // volts & ampers
            ToolTipText voltsampersToolTip = new ToolTipText("debug");
            MainWindow.instance.SetTooltip(block_voltamper, voltsampersToolTip);

            Task.Run(async () => {

                int testValue = 0;

                while (true) {
                    await Task.Delay(5);
                    await Dispatcher.BeginInvoke(() => {

                        testValue++;

                        UpdateAirSpeed(EncryptedSTData.Data.PRIBORS_AIRSPEED, speed_transform.rotation);
                        speedToolTip.text = EncryptedSTData.Data.PRIBORS_AIRSPEED.ToString("0.00");

                        UpdateKrenTangaj(EncryptedSTData.Data.PRIBORS_KREN, EncryptedSTData.Data.PRIBORS_TANGAJ, kren_angle, tangaj_angle);
                        krenTangajToolTip.text = $"" +
                        $"kren: {EncryptedSTData.Data.PRIBORS_KREN.ToString("0.00")}, \n" +
                        $"tangaj: {EncryptedSTData.Data.PRIBORS_TANGAJ.ToString("0.00")}";

                        UpdateVariometer(EncryptedSTData.Data.PRIBORS_VY, variometer_transform.rotation);
                        variometerToolTip.text = EncryptedSTData.Data.PRIBORS_VY.ToString("0.00");

                        UpdateOborots(EncryptedSTData.Data.PRIBORS_N_1, EncryptedSTData.Data.PRIBORS_N_2, oborots_kontur_1_transform.rotation, oborots_kontur_2_transform.rotation);
                        oborotsToolTip.text = $"" +
                        $"n1: {EncryptedSTData.Data.PRIBORS_N_1.ToString("0.00")}, \n" +
                        $"n2: {EncryptedSTData.Data.PRIBORS_N_2.ToString("0.00")}";

                        UpdateGasTemperatures(EncryptedSTData.Data.PRIBORS_TEMPERATURE, gastemperatures_transform.rotation);
                        gastemperaturesToolTip.text = EncryptedSTData.Data.PRIBORS_TEMPERATURE.ToString("0.00");

                        UpdateRadiocompass(EncryptedSTData.Data.PRIBORS_RADIOCOMPASS, radiocompass_transform.rotation);
                        radiocompassToolTip.text = EncryptedSTData.Data.PRIBORS_RADIOCOMPASS.ToString("0.00");

                        UpdateOilFuelPressureTemperature(
                            EncryptedSTData.Data.PRIBORS_DAVLEN_FUEL,
                            EncryptedSTData.Data.PRIBORS_DAVLEN_OIL,
                            EncryptedSTData.Data.PRIBORS_TEMPERATURE_OIL,
                            fuel_pressure_angle,
                            oil_pressure_angle,
                            oil_temperature_angle);
                        oilfuelpressuresToolTip.text = $"" +
                        $"fuel pressure: {EncryptedSTData.Data.PRIBORS_DAVLEN_FUEL.ToString("0.00")}, \n" +
                        $"oil pressure: {EncryptedSTData.Data.PRIBORS_DAVLEN_OIL.ToString("0.00")}, \n" +
                        $"oil temperature: {EncryptedSTData.Data.PRIBORS_TEMPERATURE_OIL.ToString("0.00")}";

                        UpdateFuel(EncryptedSTData.Data.PRIBORS_FUEL_VALUE, fuel_transform.rotation);
                        fuelToolTip.text = EncryptedSTData.Data.PRIBORS_FUEL_VALUE.ToString("0.00");

                        UpdateRadioheight700(EncryptedSTData.Data.PRIBORS_RV_HEIGHT, radioheight700_transform.rotation);
                        radioheight700ToolTip.text = EncryptedSTData.Data.PRIBORS_RV_HEIGHT.ToString("0.00");

                        UpdateDistance(EncryptedSTData.Data.PRIBORS_PPD_DISTANCE, distance_label);

                        UpdateHeightPressure(EncryptedSTData.Data.PRIBORS_KABINE_DAVLEN_CHANGES, barometer_transform.rotation);
                        pressureToolTip.text = EncryptedSTData.Data.PRIBORS_KABINE_DAVLEN_CHANGES.ToString("0.00");

                        UpdateRadioheight(EncryptedSTData.Data.PRIBORS_BAROM_DAVLEN, EncryptedSTData.Data.PRIBORS_BAROM_HEIGHT, radio_height_bg_angle, radioheight_transform.rotation, radio_height_arrow_mini_angle);
                        radioheightToolTip.text = $"" +
                        $"PRIBORS_BAROM_DAVLEN: {EncryptedSTData.Data.PRIBORS_BAROM_DAVLEN.ToString("0.00")}, \n" +
                        $"PRIBORS_BAROM_HEIGHT: {EncryptedSTData.Data.PRIBORS_BAROM_HEIGHT.ToString("0.00")}";

                        UpdateEnginespeed(EncryptedSTData.Data.PRIBORS_VIBROSPEED, enginespeed_angle);
                        vibrometerToolTip.text = EncryptedSTData.Data.PRIBORS_VIBROSPEED.ToString("0.00");

                        UpdateVoltAmper(EncryptedSTData.Data.PRIBORS_VOLTS, EncryptedSTData.Data.PRIBORS_AMPERS, volt_angle, amper_angle);
                        voltsampersToolTip.text = $"" +
                        $"V: {EncryptedSTData.Data.PRIBORS_VOLTS.ToString("0.00")}, \n" +
                        $"A: {EncryptedSTData.Data.PRIBORS_AMPERS.ToString("0.00")}";

                        UpdateKursoGlisada();
                        kursoglisadaToolTip.text = $"" +
                        $"KURS: {EncryptedSTData.Data.PRIBORS_KURS.ToString("0.00")}, \n" +
                        $"KURS_DEFAULT: {EncryptedSTData.Data.PRIBORS_KURS_DEFAULT.ToString("0.00")}, \n" +
                        $"KURS_ANGLE_RADIOSTATION: {EncryptedSTData.Data.PRIBORS_KURS_ANGLE_RADIOSTATION.ToString("0.00")}, \n" +
                        $"BLANKER_K: {EncryptedSTData.Data.PRIBORS_BLANKER_K.ToString("0.00")}, \n" +
                        $"BLANKER_G: {EncryptedSTData.Data.PRIBORS_BLANKER_G.ToString("0.00")}, \n" +
                        $"PRIBORS_KURS_VERTICAL_G_OFFSET: {EncryptedSTData.Data.PRIBORS_KURS_VERTICAL_G_OFFSET.ToString("0.00")}, \n" +
                        $"PRIBORS_KURS_HORIZONT_K_OFFSET: {EncryptedSTData.Data.PRIBORS_KURS_HORIZONT_K_OFFSET.ToString("0.00")}, \n";
                    });
                }
            });
        }


        // update instuments
        public void UpdateOilFuelPressureTemperature(in float value_pressure_fuel, in float value_pressure_oil, in float value_temperature_oil, in RotateTransform rotateTransform_fuel_pressure, in RotateTransform rotateTransform_oil_pressure, in RotateTransform rotateTransform_oil_temperature) {

            // fuel pressure
            if (value_pressure_fuel <= 20) {
                fuel_pressure_angle.Angle = MathAdvanced.GetFromDiapazone(value_pressure_fuel, 0, 20, -109, -84);
            }
            else if (value_pressure_fuel > 20 && value_pressure_fuel < 40) {
                fuel_pressure_angle.Angle = MathAdvanced.GetFromDiapazone(value_pressure_fuel, 20, 40, -84, -47);
            }
            else if (value_pressure_fuel > 40 && value_pressure_fuel < 60) {
                fuel_pressure_angle.Angle = MathAdvanced.GetFromDiapazone(value_pressure_fuel, 40, 60, -47, 6);
            }
            else if (value_pressure_fuel > 60 && value_pressure_fuel < 80) {
                fuel_pressure_angle.Angle = MathAdvanced.GetFromDiapazone(value_pressure_fuel, 60, 80, 6, 70);
            }
            else if (value_pressure_fuel > 80 && value_pressure_fuel < 100) {
                fuel_pressure_angle.Angle = MathAdvanced.GetFromDiapazone(value_pressure_fuel, 80, 100, 70, 117);
            }

            // oil pressure
            if (value_pressure_oil >= -0.3f && value_pressure_oil < 0) {
                oil_pressure_angle.Angle = MathAdvanced.GetFromDiapazone(value_pressure_oil, -0.3f, 0, 120, 115);
            }
            else if (value_pressure_oil >= 0 && value_pressure_oil < 2) {
                oil_pressure_angle.Angle = MathAdvanced.GetFromDiapazone(value_pressure_oil, 0, 2, 115, 73);
            }
            else if (value_pressure_oil >= 2 && value_pressure_oil < 4) {
                oil_pressure_angle.Angle = MathAdvanced.GetFromDiapazone(value_pressure_oil, 2, 4, 73, 29);
            }
            else if (value_pressure_oil >= 4 && value_pressure_oil < 6) {
                oil_pressure_angle.Angle = MathAdvanced.GetFromDiapazone(value_pressure_oil, 4, 6, 29, -25);
            }

            // oil temperature
            if (value_temperature_oil >= -70f && value_temperature_oil < -50) {
                oil_temperature_angle.Angle = MathAdvanced.GetFromDiapazone(value_temperature_oil, -70, -50, -119, -111);
            }
            else if (value_temperature_oil >= -50 && value_temperature_oil < 0) {
                oil_temperature_angle.Angle = MathAdvanced.GetFromDiapazone(value_temperature_oil, -50, 0, -111, -87);
            }
            else if (value_temperature_oil >= 0 && value_temperature_oil < 100) {
                oil_temperature_angle.Angle = MathAdvanced.GetFromDiapazone(value_temperature_oil, 0, 100, -87, -19);
            }
            else if (value_temperature_oil >= 100 && value_temperature_oil < 150) {
                oil_temperature_angle.Angle = MathAdvanced.GetFromDiapazone(value_temperature_oil, 100, 150, -19, 17);
            }
        }
        public void UpdateGasTemperatures(in float value, in RotateTransform rotateTransform) {
            rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 0, 900, -125, 125);
        }
        public void UpdateRadioheight700(in float value, in RotateTransform rotateTransform) {
            if (value < 20) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 0, 20, 0, 34);
            }
            else if (value >= 20 && value < 100) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 20, 100, 34, 152);
            }
            else if (value >= 100 && value < 700) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 100, 700, 152, 307);
            }
            else if (value >= 700 && value < 750) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 700, 750, 307, 320);
            }
        }
        public void UpdateHeightPressure(in float value, in RotateTransform rotateTransform) {
            if (value >= -0.4f && value < -0.04f) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, -0.4f, -0.04f, 312, 302);
            }
            else if (value >= -0.04f && value < -0.02f) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, -0.04f, -0.02f, 302, 293);
            }
            else if (value >= -0.02f && value < 0f) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, -0.02f, 0f, 293, 248);
            }
            else if (value >= 0f && value < 0.04f) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 0f, 0.04f, 248, 228);
            }
            else if (value >= 0.04f && value < 0.06f) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 0.04f, 0.06f, 228, 217);
            }
            else if (value >= 0.06f && value < 0.1f) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 0.06f, 0.1f, 217, 199);
            }
            else if (value >= 0.1f && value < 0.2f) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 0.1f, 0.2f, 199, 166);
            }
            else if (value >= 0.2f && value < 0.4f) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 0.2f, 0.4f, 166, 105);
            }
            else if (value >= 0.4f && value < 0.6f) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 0.4f, 0.6f, 105, 51);
            }
        }
        public void UpdateKursoGlisada() {
            blanker_G.Visibility = EncryptedSTData.Data.PRIBORS_BLANKER_G <= 0 ? Visibility.Visible : Visibility.Collapsed;
            blanker_K.Visibility = EncryptedSTData.Data.PRIBORS_BLANKER_K <= 0 ? Visibility.Visible : Visibility.Collapsed;

            kursoglisada_horizontal_line.Y = MathAdvanced.GetFromDiapazone(EncryptedSTData.Data.PRIBORS_KURS_VERTICAL_G_OFFSET, -1, 1, -62, 62);
            kursoglisada_vertical_line.X = MathAdvanced.GetFromDiapazone(EncryptedSTData.Data.PRIBORS_KURS_HORIZONT_K_OFFSET, -1, 1, -62, 62);

            kurso_glisada_filled_arrow.Angle = EncryptedSTData.Data.PRIBORS_KURS_ANGLE_RADIOSTATION;
            kurso_glisada_transparent_arrow.Angle = kusroglisada_valuemeter_angle.Angle - EncryptedSTData.Data.PRIBORS_KURS_DEFAULT;
            kusroglisada_valuemeter_angle.Angle = EncryptedSTData.Data.PRIBORS_KURS * -1;
        }
        public void UpdateRadiocompass(in float value, in RotateTransform rotateTransform) {
            rotateTransform.Angle = -180 + MathAdvanced.GetFromDiapazone(value, -180, 180, 0, 360);
        }
        public void UpdateRadioheight(in float value_pressure, in float value_barom_height, in RotateTransform pressureRotateTransform, in RotateTransform heightMetersRotateTransform, in RotateTransform heightKilometersRotateTransform) {
            // convert pressure to angles
            pressureRotateTransform.Angle = MathAdvanced.GetFromDiapazone(value_pressure, 670, 780, 265, 0);

            // convert meters to angles
            heightMetersRotateTransform.Angle = value_barom_height / 2.76497695f;

            // convert kilometers to angles
            radio_height_arrow_mini_angle.Angle = (value_barom_height / 1000f) / 0.05555555f;
        }
        public void UpdateEnginespeed(in float value, in RotateTransform rotateTransform) {
            rotateTransform.Angle = MathAdvanced.GetFromDiapazone(Math.Clamp(value, 0, 100), 0, 100, -27, 27);
        }
        public void UpdateVariometer(in float value, in RotateTransform rotateTransform) {
            // +
            if (value >= 0 && value < 10) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 0, 10, -90, -16);
            }
            else if (value >= 10 && value < 20) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 10, 20, -16, 42);
            }
            else if (value >= 20 && value < 50) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 20, 50, 42, 60);
            }
            else if (value >= 50 && value < 80) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 50, 80, 60, 82);
            }

            // - 
            if (value < 0 && value >= -10) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, -10, 0, -178, -90);
            }
            else if (value < -10 && value >= -20) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, -20, -10, -222, -178);
            }
            else if (value < -20 && value >= -50) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, -50, -20, -241, -222);
            }
            else if (value < -50 && value >= -80) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, -80, -50, -263, -241);
            }
        }
        public void UpdateKrenTangaj(in float kren, in float tangaj, in RotateTransform rotateTransform, in TranslateTransform translateTransform) {
            kren_angle.Angle = kren;
            tangaj_angle.Y = MathAdvanced.Lerp(tangaj_angle.Y, MathAdvanced.GetFromDiapazone(Math.Clamp(tangaj, -90, 90), -90, 90, -260, 260), 0.8f);
        }
        public void UpdateVoltAmper(in float value_volt, in float value_amper, in RotateTransform voltRotateTransform, in RotateTransform amperRotateTransform) {
            voltRotateTransform.Angle = MathAdvanced.GetFromDiapazone(Math.Clamp(value_volt, 0, 40), 0, 40, -180, 0);
            amperRotateTransform.Angle = MathAdvanced.GetFromDiapazone(Math.Clamp(value_amper, 0, 40), 0, 40, 180, 0);
        }
        public void UpdateAirSpeed(in float value, in RotateTransform rotateTransform) {

            if (value <= 100) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 0, 100, 5, 32);
            }
            if (value > 100 && value <= 150) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 100, 150, 32, 70);
            }
            if (value > 150 && value <= 200) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 150, 200, 70, 107);
            }
            if (value > 200 && value <= 300) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 200, 300, 107, 157);
            }
            if (value > 300 && value <= 400) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 300, 400, 157, 197);
            }
            if (value > 400 && value <= 600) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 400, 600, 197, 252);
            }
            if (value > 600 && value <= 800) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 600, 800, 252, 294);
            }
            if (value > 800 && value <= 1000) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 800, 1000, 294, 334);
            }
            if (value > 1000 && value <= 1200) {
                rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 1000, 1200, 334, 365);
            }
        }
        public void UpdateDistance(in float value, in Label label) {
            label.Content = value.ToString("000.0");
        }
        public void UpdateOborots(in float value, in float value2, in RotateTransform rotateTransform, in RotateTransform rotateTransform2) {
            rotateTransform.Angle = MathAdvanced.GetFromDiapazone(value, 0, 110, 50, 390);
            rotateTransform2.Angle = MathAdvanced.GetFromDiapazone(value2, 0, 110, 50, 390);
        }
        public void UpdateFuel(in float value, in RotateTransform rotateTransform) {
            rotateTransform.Angle = MathAdvanced.GetFromDiapazone(Math.Clamp(value, 0, 825), 0, 825, -131, 135);
        }

        // advanced classes
        public sealed class ImageFields {
            public TranslateTransform translate = null;
            public RotateTransform rotation = null;
            public ScaleTransform scale = null;
            public Image image = null;
        }
    }
}
