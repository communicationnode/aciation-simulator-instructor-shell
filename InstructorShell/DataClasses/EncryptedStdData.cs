/*
 =============================================================================

README: it's simintec packet

output y[44];
y[0] = 203;
y[1] = TUMBLERY_KRAN_SHASSI;
y[2] = KNOPKI_ZAKRYLKI;
y[3] = KNOPKI_TRIM_TANGAZH;
y[4] = SISTEMY_TURBO_ZAPUSK;
y[5] = SISTEMY_DVIGATEL_ZAPUSK;
y[6] = round(SISTEMY_OBOROTY_RVD_DVIG);
y[7] = round(SISTEMY_OBOROTY_RND_DVIG);

//+8 DOUBLES IN DATA BASE (TOP ROW IN PANEL)
y[8] = PRIBORS_AIRSPEED;
y[9] = PRIBORS_TANGAJ;
y[10] = PRIBORS_KREN;
y[11] = PRIBORS_SKOLJEN;
y[12] = PRIBORS_VY;
y[13] = PRIBORS_N_1;
y[14] = PRIBORS_N_2;
y[15] = PRIBORS_TEMPERATURE;

//+13 DOUBLES IN DATA BASE (CENTER ROW IN PANEL)
y[16] = PRIBORS_BAROM_HEIGHT;
y[17] = PRIBORS_BAROM_DAVLEN;
y[18] = PRIBORS_BAROM_CUR_DAVLEN;
y[19] = PRIBORS_KURS;
y[20] = PRIBORS_KURS_DEFAULT;
y[21] = PRIBORS_KURS_ANGLE_RADIOSTATION;
y[22] = PRIBORS_BLANKER_K;
y[23] = PRIBORS_BLANKER_G;
y[24] = PRIBORS_RADIOCOMPASS;
y[25] = PRIBORS_DAVLEN_FUEL;
y[26] = PRIBORS_DAVLEN_OIL;
y[27] = PRIBORS_TEMPERATURE_OIL;
y[28] = PRIBORS_FUEL_VALUE;

//+15 DOUBLES IN DATA BASE (BOTTOM ROW IN PANEL)
y[29] = PRIBORS_RV_HEIGHT;
y[30] = PRIBORS_RV_DANGER_HEIGHT;
y[31] = PRIBORS_PPD_DISTANCE;
y[32] = PRIBORS_TTEK_HOUR;
y[33] = PRIBORS_TTEK_MIN;
y[34] = PRIBORS_TTEK_SEC;
y[35] = PRIBORS_TPOL_HOUR;
y[36] = PRIBORS_TPOL_MIN;
y[37] = PRIBORS_TSEK_MIN;
y[38] = PRIBORS_TSEK_SEC;
y[39] = PRIBORS_KABINE_HEIGHT;
y[40] = PRIBORS_KABINE_DAVLEN_CHANGES;
y[41] = PRIBORS_VIBROSPEED;
y[42] = PRIBORS_VOLTS;
y[43] = PRIBORS_AMPERS;

// ADDITIONAL DOUBLES 01.09.2025
y[44] = PRIBORS_KURS_VERTICAL_G_OFFSET;
y[45] = PRIBORS_KURS_HORIZONT_K_OFFSET;
 =============================================================================
 */


using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace InstructorShell.DataClasses {

    [SkipLocalsInit]
    public static class EncryptedSTData {

        // fields

        public static DataEntity Data = new DataEntity();


        //methods

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        public static void UpdateDataBySpecificDatagram(in byte[] datagram) {
            //+8 DOUBLES IN DATA BASE (TOP ROW IN PANEL)
            EncryptedSTData.Data.PRIBORS_AIRSPEED = BitConverter.ToSingle(datagram, 8);
            EncryptedSTData.Data.PRIBORS_TANGAJ = BitConverter.ToSingle(datagram, 12);
            EncryptedSTData.Data.PRIBORS_KREN = BitConverter.ToSingle(datagram, 16);
            EncryptedSTData.Data.PRIBORS_SKOLJEN = 0;//BitConverter.ToSingle(datagram, 20);
            EncryptedSTData.Data.PRIBORS_VY = BitConverter.ToSingle(datagram, 24);
            EncryptedSTData.Data.PRIBORS_N_1 = BitConverter.ToSingle(datagram, 28);
            EncryptedSTData.Data.PRIBORS_N_2 = BitConverter.ToSingle(datagram, 32);
            EncryptedSTData.Data.PRIBORS_TEMPERATURE = BitConverter.ToSingle(datagram, 36);

            //+13 DOUBLES IN DATA BASE (CENTER ROW IN PANEL)
            EncryptedSTData.Data.PRIBORS_BAROM_HEIGHT = BitConverter.ToSingle(datagram, 40);
            EncryptedSTData.Data.PRIBORS_BAROM_DAVLEN = BitConverter.ToSingle(datagram, 44);
            EncryptedSTData.Data.PRIBORS_BAROM_CUR_DAVLEN = BitConverter.ToSingle(datagram, 48);
            EncryptedSTData.Data.PRIBORS_KURS = BitConverter.ToSingle(datagram, 52);
            EncryptedSTData.Data.PRIBORS_KURS_DEFAULT = BitConverter.ToSingle(datagram, 56);
            EncryptedSTData.Data.PRIBORS_KURS_ANGLE_RADIOSTATION = BitConverter.ToSingle(datagram, 60);
            EncryptedSTData.Data.PRIBORS_BLANKER_K = BitConverter.ToSingle(datagram, 64);
            EncryptedSTData.Data.PRIBORS_BLANKER_G = BitConverter.ToSingle(datagram, 68);
            EncryptedSTData.Data.PRIBORS_RADIOCOMPASS = BitConverter.ToSingle(datagram, 72);
            EncryptedSTData.Data.PRIBORS_DAVLEN_FUEL = BitConverter.ToSingle(datagram, 76);
            EncryptedSTData.Data.PRIBORS_DAVLEN_OIL = BitConverter.ToSingle(datagram, 80);
            EncryptedSTData.Data.PRIBORS_TEMPERATURE_OIL = BitConverter.ToSingle(datagram, 84);
            EncryptedSTData.Data.PRIBORS_FUEL_VALUE = BitConverter.ToSingle(datagram, 88);

            //+15 DOUBLES IN DATA BASE (BOTTOM ROW IN PANEL)
            EncryptedSTData.Data.PRIBORS_RV_HEIGHT = BitConverter.ToSingle(datagram, 92);
            EncryptedSTData.Data.PRIBORS_RV_DANGER_HEIGHT = BitConverter.ToSingle(datagram, 96);
            EncryptedSTData.Data.PRIBORS_PPD_DISTANCE = BitConverter.ToSingle(datagram, 100);
            EncryptedSTData.Data.PRIBORS_TTEK_HOUR = BitConverter.ToSingle(datagram, 104);
            EncryptedSTData.Data.PRIBORS_TTEK_MIN = BitConverter.ToSingle(datagram, 108);
            EncryptedSTData.Data.PRIBORS_TTEK_SEC = BitConverter.ToSingle(datagram, 112);
            EncryptedSTData.Data.PRIBORS_TPOL_HOUR = BitConverter.ToSingle(datagram, 116);
            EncryptedSTData.Data.PRIBORS_TPOL_MIN = BitConverter.ToSingle(datagram, 120);
            EncryptedSTData.Data.PRIBORS_TSEK_MIN = BitConverter.ToSingle(datagram, 124);
            EncryptedSTData.Data.PRIBORS_TSEK_SEC = BitConverter.ToSingle(datagram, 128);
            EncryptedSTData.Data.PRIBORS_KABINE_HEIGHT = BitConverter.ToSingle(datagram, 132);
            EncryptedSTData.Data.PRIBORS_KABINE_DAVLEN_CHANGES = BitConverter.ToSingle(datagram, 136);
            EncryptedSTData.Data.PRIBORS_VIBROSPEED = BitConverter.ToSingle(datagram, 140);
            EncryptedSTData.Data.PRIBORS_VOLTS = BitConverter.ToSingle(datagram, 144);
            EncryptedSTData.Data.PRIBORS_AMPERS = BitConverter.ToSingle(datagram, 148);

            // ADDITIONAL DOUBLES 01.09.2025
            EncryptedSTData.Data.PRIBORS_KURS_VERTICAL_G_OFFSET = BitConverter.ToSingle(datagram, 152);
            EncryptedSTData.Data.PRIBORS_KURS_HORIZONT_K_OFFSET = BitConverter.ToSingle(datagram, 156);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        public static void AllRandomize() {

            Random random = new Random();

            //+8 DOUBLES IN DATA BASE (TOP ROW IN PANEL)
            EncryptedSTData.Data.PRIBORS_AIRSPEED = random.NextSingle() * 1200f;
            EncryptedSTData.Data.PRIBORS_TANGAJ = -90f + (random.NextSingle() * 180f);
            EncryptedSTData.Data.PRIBORS_KREN = -180 + (random.NextSingle() * 360f);
            EncryptedSTData.Data.PRIBORS_SKOLJEN = random.NextSingle();
            EncryptedSTData.Data.PRIBORS_VY = -80 + (random.NextSingle() * 160f);
            EncryptedSTData.Data.PRIBORS_N_1 = random.NextSingle() * 100f;
            EncryptedSTData.Data.PRIBORS_N_2 = random.NextSingle() * 100f;
            EncryptedSTData.Data.PRIBORS_TEMPERATURE = random.NextSingle() * 900f;

            //+13 DOUBLES IN DATA BASE (CENTER ROW IN PANEL)
            EncryptedSTData.Data.PRIBORS_BAROM_HEIGHT = random.NextSingle() * 5000f;
            EncryptedSTData.Data.PRIBORS_BAROM_DAVLEN = 670f + random.NextSingle() * 110f;
            EncryptedSTData.Data.PRIBORS_BAROM_CUR_DAVLEN = random.NextSingle();
            EncryptedSTData.Data.PRIBORS_KURS = random.NextSingle() * 360;
            EncryptedSTData.Data.PRIBORS_KURS_DEFAULT = random.NextSingle() * 360;
            EncryptedSTData.Data.PRIBORS_KURS_ANGLE_RADIOSTATION = random.NextSingle() * 360;
            EncryptedSTData.Data.PRIBORS_BLANKER_K = (float)Math.Round(random.NextSingle() * 2f);
            EncryptedSTData.Data.PRIBORS_BLANKER_G = (float)Math.Round(random.NextSingle() * 2f);
            EncryptedSTData.Data.PRIBORS_RADIOCOMPASS = -180 + (random.NextSingle() * 360f);
            EncryptedSTData.Data.PRIBORS_DAVLEN_FUEL = random.NextSingle() * 100f;
            EncryptedSTData.Data.PRIBORS_DAVLEN_OIL = -0.3f + random.NextSingle() * 6.3f;
            EncryptedSTData.Data.PRIBORS_TEMPERATURE_OIL = -70 + (random.NextSingle() * 220f);
            EncryptedSTData.Data.PRIBORS_FUEL_VALUE = random.NextSingle() * 825f;

            //+15 DOUBLES IN DATA BASE (BOTTOM ROW IN PANEL)
            EncryptedSTData.Data.PRIBORS_RV_HEIGHT = random.NextSingle() * 750f;
            EncryptedSTData.Data.PRIBORS_RV_DANGER_HEIGHT = random.NextSingle();
            EncryptedSTData.Data.PRIBORS_PPD_DISTANCE = random.NextSingle() * 10f;
            EncryptedSTData.Data.PRIBORS_TTEK_HOUR = random.NextSingle();
            EncryptedSTData.Data.PRIBORS_TTEK_MIN = random.NextSingle();
            EncryptedSTData.Data.PRIBORS_TTEK_SEC = random.NextSingle();
            EncryptedSTData.Data.PRIBORS_TPOL_HOUR = random.NextSingle();
            EncryptedSTData.Data.PRIBORS_TPOL_MIN = random.NextSingle();
            EncryptedSTData.Data.PRIBORS_TSEK_MIN = random.NextSingle();
            EncryptedSTData.Data.PRIBORS_TSEK_SEC = random.NextSingle();
            EncryptedSTData.Data.PRIBORS_KABINE_HEIGHT = random.NextSingle();
            EncryptedSTData.Data.PRIBORS_KABINE_DAVLEN_CHANGES = -0.4f + random.NextSingle();
            EncryptedSTData.Data.PRIBORS_VIBROSPEED = random.NextSingle() * 100f;
            EncryptedSTData.Data.PRIBORS_VOLTS = random.NextSingle() * 40f;
            EncryptedSTData.Data.PRIBORS_AMPERS = random.NextSingle() * 40f;
            EncryptedSTData.Data.PRIBORS_KURS_VERTICAL_G_OFFSET = -1f + random.NextSingle() * 2f;
            EncryptedSTData.Data.PRIBORS_KURS_HORIZONT_K_OFFSET = -1f + random.NextSingle() * 2f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [SkipLocalsInit]
        public static DataEntity WriteEntity() {
            return Data;
        }

    }
    // structs
    [StructLayout(LayoutKind.Auto)]
    [SkipLocalsInit]
    public struct DataEntity {
        #region Fields
        // 8 FLOATS IN DATA BASE (TOP ROW IN PANEL)
        public int recorded_second { get; set; }
        public float PRIBORS_AIRSPEED { get; set; }
        public float PRIBORS_TANGAJ { get; set; }
        public float PRIBORS_KREN { get; set; }
        public float PRIBORS_SKOLJEN { get; set; }
        /// <summary> Variometer vertical speed </summary>
        public float PRIBORS_VY { get; set; }
        public float PRIBORS_N_1 { get; set; }
        public float PRIBORS_N_2 { get; set; }
        public float PRIBORS_TEMPERATURE { get; set; }

        // 13 FLOATS IN DATA BASE (CENTER ROW IN PANEL)
        public float PRIBORS_BAROM_HEIGHT { get; set; }
        public float PRIBORS_BAROM_DAVLEN { get; set; }
        public float PRIBORS_BAROM_CUR_DAVLEN { get; set; }
        /// <summary> NPP.kurs </summary>
        public float PRIBORS_KURS { get; set; }
        /// <summary> NPP.kurs_zad; </summary>
        public float PRIBORS_KURS_DEFAULT { get; set; }
        /// <summary> NPP.kur; </summary>
        public float PRIBORS_KURS_ANGLE_RADIOSTATION { get; set; }
        public float PRIBORS_BLANKER_K { get; set; }
        public float PRIBORS_BLANKER_G { get; set; }
        public float PRIBORS_RADIOCOMPASS { get; set; }
        public float PRIBORS_DAVLEN_FUEL { get; set; }
        public float PRIBORS_DAVLEN_OIL { get; set; }
        public float PRIBORS_TEMPERATURE_OIL { get; set; }
        public float PRIBORS_FUEL_VALUE { get; set; }

        // 15 FLOATS IN DATA BASE (BOTTOM ROW IN PANEL)
        public float PRIBORS_RV_HEIGHT { get; set; }
        public float PRIBORS_RV_DANGER_HEIGHT { get; set; }
        public float PRIBORS_PPD_DISTANCE { get; set; }
        public float PRIBORS_TTEK_HOUR { get; set; }
        public float PRIBORS_TTEK_MIN { get; set; }
        public float PRIBORS_TTEK_SEC { get; set; }
        public float PRIBORS_TPOL_HOUR { get; set; }
        public float PRIBORS_TPOL_MIN { get; set; }
        public float PRIBORS_TSEK_MIN { get; set; }
        public float PRIBORS_TSEK_SEC { get; set; }
        public float PRIBORS_KABINE_HEIGHT { get; set; }
        public float PRIBORS_KABINE_DAVLEN_CHANGES { get; set; }
        public float PRIBORS_VIBROSPEED { get; set; }
        public float PRIBORS_VOLTS { get; set; }
        public float PRIBORS_AMPERS { get; set; }

        // ADDITIONAL DOUBLES 01.09.2025
        public float PRIBORS_KURS_VERTICAL_G_OFFSET { get; set; }
        public float PRIBORS_KURS_HORIZONT_K_OFFSET { get; set; }

        #endregion
    }
}
