using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml;
using System.IO;
using InstructorShell.DataClasses;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.IO.Packaging;


namespace InstructorShell.DataOutputXML {

    [SkipLocalsInit]
    public sealed class XMLOutputEntity {

        public const byte TIME_COL = 2;
        public const byte SPEED_COL = 3;
        public const byte KREN_COL = 4;
        public const byte TANGAJ_COL = 5;
        public const byte VARIOMETER_COL = 6;
        public const byte RADHEIGHT_COL = 7;


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        public static void DrawGraph(in DataEntity[] dataEntity) {

            // Установка лицензии

            ExcelPackage.License.SetNonCommercialPersonal("By Communication Node");
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage? package = new ExcelPackage()) {
                // add list
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"Recorded session {Guid.NewGuid()}");
                ExcelWorksheet worksheetData = package.Workbook.Worksheets.Add($"Recorded data {Guid.NewGuid()}");

                worksheet.PrinterSettings.FitToPage = true;
                worksheet.PrinterSettings.VerticalCentered = true;
                worksheet.PrinterSettings.HorizontalCentered = true;
                worksheet.PrinterSettings.BottomMargin = 0.64f * 0.4f;
                worksheet.PrinterSettings.TopMargin = 0.64f * 0.4f;
                worksheet.PrinterSettings.LeftMargin = 0.64f * 0.4f;
                worksheet.PrinterSettings.RightMargin = 0.64f * 0.4f;
                worksheet.PrinterSettings.FooterMargin = 0.15f * 0.4f;
                worksheet.PrinterSettings.HeaderMargin = 0.15f * 0.4f;

                worksheetData.PrinterSettings.FitToPage = true;
                worksheetData.PrinterSettings.VerticalCentered = true;
                worksheetData.PrinterSettings.HorizontalCentered = true;
                worksheetData.PrinterSettings.BottomMargin = 0.64f * 0.4f;
                worksheetData.PrinterSettings.TopMargin = 0.64f * 0.4f;
                worksheetData.PrinterSettings.LeftMargin = 0.64f * 0.4f;
                worksheetData.PrinterSettings.RightMargin = 0.64f * 0.4f;
                worksheetData.PrinterSettings.FooterMargin = 0.15f * 0.4f;
                worksheetData.PrinterSettings.HeaderMargin = 0.15f * 0.4f;

                // write titles
                worksheetData.Cells[1, TIME_COL].Value = "Время сессии";
                worksheetData.Cells[1, SPEED_COL].Value = "Скорость";
                worksheetData.Cells[1, KREN_COL].Value = "Крен";
                worksheetData.Cells[1, TANGAJ_COL].Value = "Тангаж";
                worksheetData.Cells[1, VARIOMETER_COL].Value = "Вариометр";
                worksheetData.Cells[1, RADHEIGHT_COL].Value = "Баром. высота";

                // write data
                for (ushort row = 0; row < dataEntity.Length; row++) {
                    worksheetData.Cells[row + 2, TIME_COL].Value = dataEntity[row].recorded_second * 1.0f; // X - time
                    worksheetData.Cells[row + 2, SPEED_COL].Value = dataEntity[row].PRIBORS_AIRSPEED; // speed
                    worksheetData.Cells[row + 2, KREN_COL].Value = dataEntity[row].PRIBORS_KREN; // kren
                    worksheetData.Cells[row + 2, TANGAJ_COL].Value = dataEntity[row].PRIBORS_TANGAJ; // tangaj
                    worksheetData.Cells[row + 2, VARIOMETER_COL].Value = dataEntity[row].PRIBORS_VY; // variometer
                    worksheetData.Cells[row + 2, RADHEIGHT_COL].Value = dataEntity[row].PRIBORS_BAROM_HEIGHT; // variometer
                }

                ExcelChart speedChart = AddChart(worksheet, worksheetData, 0, SPEED_COL, dataEntity, "Speed_Chart", "Скорость");
                ExcelChart krenChart = AddChart(worksheet, worksheetData, 36, KREN_COL, dataEntity, "Kren_Chart", "Крен");
                ExcelChart tangajChart = AddChart(worksheet, worksheetData, 72, TANGAJ_COL, dataEntity, "Tangaj_Chart", "Тангаж");
                ExcelChart variometerChart = AddChart(worksheet, worksheetData, 108, VARIOMETER_COL, dataEntity, "Variometer_Chart", "Вариометр");
                ExcelChart baromHeightChart = AddChart(worksheet, worksheetData, 144, RADHEIGHT_COL, dataEntity, "BaromHeight_Chart", "Баром. высота");

                Save(package);

                worksheet.Dispose();
                worksheetData.Dispose();
                speedChart.Dispose();
                krenChart.Dispose();
                tangajChart.Dispose();
                variometerChart.Dispose();
                baromHeightChart.Dispose();
            }
            //MainWindow.instance.Dispatcher.Invoke(() => { MainWindow.instance.panel_name.Text = dataEntity.Length.ToString(); });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        static ExcelChart AddChart(in ExcelWorksheet worksheet, in ExcelWorksheet worksheetData, in ushort Y, in ushort column, in DataEntity[] dataEntity, in string chartName = "undefined", in string valueName = "undefined") {
            // create chart
            var chart = worksheet.Drawings.AddChart(chartName, eChartType.XYScatterSmooth);

            chart.SetPosition(Y, 0, 0, 0);
            chart.SetSize(1800, 700);
            chart.Title.Text = $"Запись полета: результаты симуляции: {valueName}";

            chart.XAxis.Format = "0.0";
            chart.XAxis.Title.Text = $"Время: {dataEntity.Length} секунд";

            chart.YAxis.Title.Text = "Значение";

            chart.XAxis.Font.Size = 12;
            chart.YAxis.Font.Size = 12;


            AddSeries(chart, worksheetData, column, valueName, dataEntity.Length);
            return chart;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        static void AddSeries(in ExcelChart chart, in ExcelWorksheet worksheet, in int column, in string name, in int dataLength) {


            ExcelChartSerie series = chart.Series.Add(
                //Y part of chart
                worksheet.Cells[2, column, 2 + (dataLength - 1), column],
                //X part of chart
                worksheet.Cells[2, TIME_COL, 2 + (dataLength - 1), TIME_COL]);

            series.Header = name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        static void Save(in ExcelPackage? package) {
            // Сохраняем файл

            string dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), "Объективный контроль");
            if (!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }

            string path = Path.Combine(dirPath, $"Objective_Control-{DateTime.Now.ToString().Replace(' ', '-').Replace(':', '.')}.xlsx");
            FileInfo fileInfo = new FileInfo(path);
            package.SaveAs(fileInfo);
        }
    }
}
