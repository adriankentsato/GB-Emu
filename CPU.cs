using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyEmu
{
    [StructLayout(LayoutKind.Explicit)]
    struct Register
    {
        [FieldOffset(0)] public UInt16 reg;
        [FieldOffset(0)] public byte lo;
        [FieldOffset(1)] public byte hi;
    }

    internal class CPU
    {
        public const int FLAG_Z = 7;
        public const int FLAG_N = 6;
        public const int FLAG_H = 5;
        public const int FLAG_C = 4;

        private Register AF = new Register();
        private Register BC = new Register();
        private Register DE = new Register();
        private Register HL = new Register();

        private UInt16 StackPointer = 0;
        private UInt16 programCounter = 0;

        public CPU()
        {
            this.Init();
        }

        public void Init()
        {
            this.StackPointer = 0xFFFE;
            this.programCounter = 0x100;
            this.AF.reg = 0x01B0;
            this.BC.reg = 0x0013;
            this.DE.reg = 0x00D8;
            this.HL.reg = 0x014D;
        }
    }
}
