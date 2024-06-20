using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Windows.Forms;

namespace GameBoyEmu
{
    internal class Cartridge
    {
        public static int MAX_CART_SIZE = 0x200000;

        public static int TITLE_ADDR_START = 0x0134;
        public static int TITLE_ADDR_END = 0x0143;
        public static int MFR_ADDR_START = 0x0134;
        public static int MFR_ADDR_END = 0x0143;
        
        public static int NEW_LIC_CODE_ADDR = 0x0144;
        public static int OLD_LIC_CIDE_ADDR = 0x014B;

        public static int CGB_FLAG_ADDR = 0x0143;
        public static int SGB_FLAG_ADDR = 0x0147;

        public static Dictionary<string, string> NEW_LICENCEE_PUBLISHERS = new Dictionary<string, string>() {
            { "00", "None" },
            { "01", "Nintendo Research & Development 1" },
            { "08", "Capcom" },
            { "13", "EA (Electronic Arts)" },
            { "18", "Hudson Soft" },
            { "19", "B-AI" },
            { "20", "KSS" },
            { "22", "Planning Office WADA" },
            { "24", "PCM Complete" },
            { "25", "San-X" },
            { "28", "Kemco" },
            { "29", "SETA Corporation" },
            { "30", "Viacom" },
            { "31", "Nintendo" },
            { "32", "Bandai" },
            { "33", "Ocean Software/Acclaim Entertainment" },
            { "34", "Konami" },
            { "35", "HectorSoft" },
            { "37", "Taito" },
            { "38", "Hudson Soft" },
            { "39", "Banpresto" },
            { "41", "Ubi Soft1" },
            { "42", "Atlus" },
            { "44", "Malibu Interactive" },
            { "46", "Angel" },
            { "47", "Bullet-Proof Software2" },
            { "49", "Irem" },
            { "50", "Absolute" },
            { "51", "Acclaim Entertainment" },
            { "52", "Activision" },
            { "53", "Sammy USA Corporation" },
            { "54", "Konami" },
            { "55", "Hi Tech Expressions" },
            { "56", "LJN" },
            { "57", "Matchbox" },
            { "58", "Mattel" },
            { "59", "Milton Bradley Company" },
            { "60", "Titus Interactive" },
            { "61", "Virgin Games Ltd.3" },
            { "64", "Lucasfilm Games4" },
            { "67", "Ocean Software" },
            { "69", "EA (Electronic Arts)" },
            { "70", "Infogrames5" },
            { "71", "Interplay Entertainment" },
            { "72", "Broderbund" },
            { "73", "Sculptured Software6" },
            { "75", "The Sales Curve Limited7" },
            { "78", "THQ" },
            { "79", "Accolade" },
            { "80", "Misawa Entertainment" },
            { "83", "lozc" },
            { "86", "Tokuma Shoten" },
            { "87", "Tsukuda Original" },
            { "91", "Chunsoft Co.8" },
            { "92", "Video System" },
            { "93", "Ocean Software/Acclaim Entertainment" },
            { "95", "Varie" },
            { "96", "Yonezawa/s’pal" },
            { "97", "Kaneko" },
            { "99", "Pack-In-Video" },
            { "9H", "Bottom Up" },
            { "A4", "Konami (Yu-Gi-Oh!)" },
            { "BL", "MTO" },
            { "DK", "Kodansha" },
        };
        public static Dictionary<string, string> OLD_LICENCEE_PUBLISHERS = new Dictionary<string, string>()
        {
            { "00", "None" },
            { "01", "Nintendo" },
            { "08", "Capcom" },
            { "09", "HOT-B" },
            { "0A", "Jaleco" },
            { "0B", "Coconuts Japan" },
            { "0C", "Elite Systems" },
            { "13", "EA (Electronic Arts)" },
            { "18", "Hudson Soft" },
            { "19", "ITC Entertainment" },
            { "1A", "Yanoman" },
            { "1D", "Japan Clary" },
            { "1F", "Virgin Games Ltd.3" },
            { "24", "PCM Complete" },
            { "25", "San-X" },
            { "28", "Kemco" },
            { "29", "SETA Corporation" },
            { "30", "Infogrames5" },
            { "31", "Nintendo" },
            { "32", "Bandai" },
            { "33", "Indicates that the New licensee code should be used instead." },
            { "34", "Konami" },
            { "35", "HectorSoft" },
            { "38", "Capcom" },
            { "39", "Banpresto" },
            { "3C", ".Entertainment i" },
            { "3E", "Gremlin" },
            { "41", "Ubi Soft1" },
            { "42", "Atlus" },
            { "44", "Malibu Interactive" },
            { "46", "Angel" },
            { "47", "Spectrum Holoby" },
            { "49", "Irem" },
            { "4A", "Virgin Games Ltd.3" },
            { "4D", "Malibu Interactive" },
            { "4F", "U.S. Gold" },
            { "50", "Absolute" },
            { "51", "Acclaim Entertainment" },
            { "52", "Activision" },
            { "53", "Sammy USA Corporation" },
            { "54", "GameTek" },
            { "55", "Park Place" },
            { "56", "LJN" },
            { "57", "Matchbox" },
            { "59", "Milton Bradley Company" },
            { "5A", "Mindscape" },
            { "5B", "Romstar" },
            { "5C", "Naxat Soft13" },
            { "5D", "Tradewest" },
            { "60", "Titus Interactive" },
            { "61", "Virgin Games Ltd.3" },
            { "67", "Ocean Software" },
            { "69", "EA (Electronic Arts)" },
            { "6E", "Elite Systems" },
            { "6F", "Electro Brain" },
            { "70", "Infogrames5" },
            { "71", "Interplay Entertainment" },
            { "72", "Broderbund" },
            { "73", "Sculptured Software6" },
            { "75", "The Sales Curve Limited7" },
            { "78", "THQ" },
            { "79", "Accolade" },
            { "7A", "Triffix Entertainment" },
            { "7C", "Microprose" },
            { "7F", "Kemco" },
            { "80", "Misawa Entertainment" },
            { "83", "Lozc" },
            { "86", "Tokuma Shoten" },
            { "8B", "Bullet-Proof Software2" },
            { "8C", "Vic Tokai" },
            { "8E", "Ape" },
            { "8F", "I’Max" },
            { "91", "Chunsoft Co.8" },
            { "92", "Video System" },
            { "93", "Tsubaraya Productions" },
            { "95", "Varie" },
            { "96", "Yonezawa/S’Pal" },
            { "97", "Kemco" },
            { "99", "Arc" },
            { "9A", "Nihon Bussan" },
            { "9B", "Tecmo" },
            { "9C", "Imagineer" },
            { "9D", "Banpresto" },
            { "9F", "Nova" },
            { "A1", "Hori Electric" },
            { "A2", "Bandai" },
            { "A4", "Konami" },
            { "A6", "Kawada" },
            { "A7", "Takara" },
            { "A9", "Technos Japan" },
            { "AA", "Broderbund" },
            { "AC", "Toei Animation" },
            { "AD", "Toho" },
            { "AF", "Namco" },
            { "B0", "Acclaim Entertainment" },
            { "B1", "ASCII Corporation or Nexsoft" },
            { "B2", "Bandai" },
            { "B4", "Square Enix" },
            { "B6", "HAL Laboratory" },
            { "B7", "SNK" },
            { "B9", "Pony Canyon" },
            { "BA", "Culture Brain" },
            { "BB", "Sunsoft" },
            { "BD", "Sony Imagesoft" },
            { "BF", "Sammy Corporation" },
            { "C0", "Taito" },
            { "C2", "Kemco" },
            { "C3", "Square" },
            { "C4", "Tokuma Shoten" },
            { "C5", "Data East" },
            { "C6", "Tonkinhouse" },
            { "C8", "Koei" },
            { "C9", "UFL" },
            { "CA", "Ultra" },
            { "CB", "Vap" },
            { "CC", "Use Corporation" },
            { "CD", "Meldac" },
            { "CE", "Pony Canyon" },
            { "CF", "Angel" },
            { "D0", "Taito" },
            { "D1", "Sofel" },
            { "D2", "Quest" },
            { "D3", "Sigma Enterprises" },
            { "D4", "ASK Kodansha Co." },
            { "D6", "Naxat Soft13" },
            { "D7", "Copya System" },
            { "D9", "Banpresto" },
            { "DA", "Tomy" },
            { "DB", "LJN" },
            { "DD", "NCS" },
            { "DE", "Human" },
            { "DF", "Altron" },
            { "E0", "Jaleco" },
            { "E1", "Towa Chiki" },
            { "E2", "Yutaka" },
            { "E3", "Varie" },
            { "E5", "Epcoh" },
            { "E7", "Athena" },
            { "E8", "Asmik Ace Entertainment" },
            { "E9", "Natsume" },
            { "EA", "King Records" },
            { "EB", "Atlus" },
            { "EC", "Epic/Sony Records" },
            { "EE", "IGS" },
            { "F0", "A Wave" },
            { "F3", "Extreme Entertainment" },
            { "FF", "LJN" }
        };

        private byte[] cart = new byte[Cartridge.MAX_CART_SIZE];

        public string Title = "";
        public string ManufacturerCode = "";
        public string LicenseeCode = "";
        public string Publisher = "";
        public byte CgbFlag = 0;

        public Cartridge()
        {
            this.Init();
        }

        public void Init()
        {
            Array.Clear(this.cart, 0, this.cart.Length);

            this.Title = "";
            this.ManufacturerCode = "";
            this.CgbFlag = 0;
            this.LicenseeCode = "";
        }

        public void LoadCartridge(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var bytesRead = fs.Read(this.cart, 0, Cartridge.MAX_CART_SIZE);

                fs.Close();

                this.CgbFlag = this.cart[Cartridge.CGB_FLAG_ADDR];

                this.getTitle();
                this.getManufacturer();
                this.getPublisher();
            }
        }

        public void Write(UInt16 address)
        {

        }

        public byte Read(UInt16 address)
        {
            return this.cart[address];
        }

        public byte[] ReadMany(UInt16 startAddr, UInt16 endAddr)
        {
            int len = (int)(endAddr - startAddr);
            byte[] readData = new byte[len];

            Array.Copy(this.cart, startAddr, readData, 0, len);

            return readData;
        }

        private void getPublisher()
        {
            byte[] newLicCode = { this.cart[Cartridge.NEW_LIC_CODE_ADDR],
                this.cart[Cartridge.NEW_LIC_CODE_ADDR + 1] };
            byte oldLicCode = this.cart[Cartridge.OLD_LIC_CIDE_ADDR];

            if (oldLicCode == 0x33)
            {
                // New Licensee Code will be used
                this.LicenseeCode = Encoding.UTF8.GetString(newLicCode);
                this.Publisher = Cartridge.NEW_LICENCEE_PUBLISHERS[this.LicenseeCode];
            }

            else
            {
                // We will be using the old licensee code
                this.LicenseeCode = oldLicCode.ToString("X2");
                this.Publisher = Cartridge.OLD_LICENCEE_PUBLISHERS[this.LicenseeCode];
            }
        }

        private void getTitle()
        {
            byte[] title = new byte[16];

            for (int i = TITLE_ADDR_START; i <= TITLE_ADDR_END; i++)
            {
                title[i - TITLE_ADDR_START] = this.cart[i];
            }

            this.Title = Encoding.UTF8.GetString(title);
        }

        private void getManufacturer()
        {
            byte[] manufacturerCode = new byte[4];

            for (int i = MFR_ADDR_START; i <= MFR_ADDR_END; i++)
            {
                manufacturerCode[i - MFR_ADDR_START] = this.cart[i];
            }

            this.ManufacturerCode = Encoding.UTF8.GetString(manufacturerCode);
        }
    }
}
