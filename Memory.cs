using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyEmu
{
    internal class Memory
    {
        /**
         * 
         * MEMORY MAP
         * 
         * 0x0000-0x3FFF - 16KiB ROM Bank 00 - From Cartridge
         * 0x4000-0x7FFF - 16KiB ROM Bank 01-NN - From Cartridge, switchable bank via mapper
         * 0x8000-0x9FFF - 8KiB Video RAM (VRAM) - In CGB mode, switchable bank 0/1
         * 0xA000-0xBFFF - 8KiB External RAM - From Cartridge, switchable bank if any
         * 0xC000-0xCFFF - 4KiB Work RAM
         * 0xD000-0xDFFF - 4Kib Work RAM - In CGB mode, switchable bank 1-7
         * 0xE000-0xFDFF - Echo RAM (mirror of 0xC000-0xDDFF) - Use of this area is prohibited
         * 0xFE00-0xFE9F - Object Attribute Memory (OAM)
         * 0xFEA0-0xFEFF - Not Usable - Restricted
         * 0xFF00-0xFF7F - I/O Registers
         * 0xFF80-0xFFFE - High RAM (HRAM)
         * 0xFFFF00xFFFF - Interrupt Enable Register (IE)
         * 
         */
        public static int MAX_CART_SIZE = 0x200000;
        public static int MAX_MEM_SIZE = 0x10000;
        public static int RAM_BANK_SIZE = 0x2000;
        public static int MAX_RAM_BANK_SIZE = 0x8000;

        public static int ROM_MODE_ADDR = 0x147;

        private byte[] rom = new byte[Memory.MAX_MEM_SIZE];
        private byte[] ram = new byte[Memory.MAX_RAM_BANK_SIZE];

        private Cartridge cart;

        private bool MBC1 = false;
        private bool MBC2 = false;
        private int currentRomBank = 1;
        private int currentRamBank = 0;

        public Memory(Cartridge cart)
        {
            this.cart = cart;
            
            this.init();
            this.initRom();
        }

        private void init()
        {
            // Initialization of the memory
            Array.Clear(this.rom, 0, this.rom.Length);
            Array.Clear(this.ram, 0, this.ram.Length);

            this.cart.Init();

            this.currentRomBank = 1;
            this.currentRamBank = 0;
        }

        private void initRom()
        {
            // This initializes the ROM's state
            this.rom[0xFF05] = 0x00;
            this.rom[0xFF06] = 0x00;
            this.rom[0xFF07] = 0x00;
            this.rom[0xFF10] = 0x80;
            this.rom[0xFF11] = 0xBF;
            this.rom[0xFF12] = 0xF3;
            this.rom[0xFF14] = 0xBF;
            this.rom[0xFF16] = 0x3F;
            this.rom[0xFF17] = 0x00;
            this.rom[0xFF19] = 0xBF;
            this.rom[0xFF1A] = 0x7F;
            this.rom[0xFF1B] = 0xFF;
            this.rom[0xFF1C] = 0x9F;
            this.rom[0xFF1E] = 0xBF;
            this.rom[0xFF20] = 0xFF;
            this.rom[0xFF21] = 0x00;
            this.rom[0xFF22] = 0x00;
            this.rom[0xFF23] = 0xBF;
            this.rom[0xFF24] = 0x77;
            this.rom[0xFF25] = 0xF3;
            this.rom[0xFF26] = 0xF1;
            this.rom[0xFF40] = 0x91;
            this.rom[0xFF42] = 0x00;
            this.rom[0xFF43] = 0x00;
            this.rom[0xFF45] = 0x00;
            this.rom[0xFF47] = 0xFC;
            this.rom[0xFF48] = 0xFF;
            this.rom[0xFF49] = 0xFF;
            this.rom[0xFF4A] = 0x00;
            this.rom[0xFF4B] = 0x00;
            this.rom[0xFFFF] = 0x00;
        }

        public void Write(UInt16 address, byte value)
        {
            if (address < 0x8000)
            {
                // This memory address is not available for writing
                // This space is readonly memory
            }

            else if ((address >= 0xE000) && (address < 0xFE00))
            {
                // This is the echo memory area, we can write here
                this.rom[address] = value;

                // Also write to the main memory area
                this.Write((UInt16)(address - 0x2000), value);
            }

            else if ((address >= 0xFEA0) && (address <= 0xFEFF))
            {
                // NO OP
                // This is a restricted memory area
            }

            else
            {
                // We can write here, yay!
                this.rom[address] = value;
            }
        }

        public byte Read(UInt16 address)
        {
            if ((address >= 0x4000) && (address <=0x7FFF))
            {
                // We are reading from the Cart's memory bank

                UInt16 newAddress = (UInt16)(address - 0x4000);

                return this.cart.Read((UInt16)(newAddress + (this.currentRomBank * 0x4000)));
            }

            else if ((address >= 0xA000) && (address <= 0xBFFF))
            {
                // We are reading from the RAM's memory bank

                UInt16 newAddress = (UInt16)(address - 0xA000);

                return this.ram[newAddress + (this.currentRamBank * 0x2000)];
            }

            else
            {
                // We are in the ROM

                return this.rom[address];
            }
        }

        public void LoadMBCs()
        {
            this.MBC1 = ((int[])[1, 2, 3]).Contains(this.cart.Read((UInt16)Memory.ROM_MODE_ADDR));
            this.MBC2 = ((int[])[5, 6]).Contains(this.cart.Read((UInt16)Memory.ROM_MODE_ADDR));
        }
    }
}
