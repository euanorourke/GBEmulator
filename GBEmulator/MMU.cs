using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBEmulator
{
    internal class MMU
    {
        public byte[] romBank0 = new byte[0x3FFF];
        public byte[] romBank1 = new byte[0x3FFF];
        private byte[] VRAM = new byte[0x2000];
        private byte[] ERAM = new byte[0x2000];
        private byte[] WRAM = new byte[0x1000];
        private byte[] WRAM1 = new byte[0x1000];
        private byte[] OAM = new byte[0x9F];
        private byte[] IORegisters = new byte[0x7F];
        private byte[] HRAM = new byte[0x7E];

        public void LDRom(List<byte> rom) { 
            //Load into first rombank
            for (int i=0; i < romBank0.Length; i++)
            {
                Console.WriteLine("Writing to RomBank0: " + rom[i].ToString("X"));
                romBank0[i] = rom[i];
            }
            for (int i = 0; i < romBank1.Length; i++)
            {
                Console.WriteLine("Writing to RomBank1: " + rom[romBank0.Length + i].ToString("X"));
                romBank1[i] = rom[romBank0.Length + i];
            }
            Console.WriteLine("Finished Writing to ROM.");
        }
        // Return the opcode of the given address at the program counter, switch between rombanks as neccesary
        public byte getOpcode(int PC) { 
            if (PC < 0x3FFF)
            {
                return romBank0[PC];
            }
            else
            {
                return romBank1[PC];
            }
        }
        // Instructions:
        public void LD8(byte source, int PC) { //Source being the register contents to copy from, and destination being the memory location to copy to
                                               // Find the initial 
            byte destination;
            //Finding the memory address
            if (PC < 0x3FFF)
            {
                
                destination = romBank0[PC];
            }
            else
            {
                destination = romBank1[PC];
            }
            // Copying register to location
            if ((int)destination < 0x3FFF)
            {
                romBank0[destination] = source;
                Console.WriteLine("Writing " + source.ToString("X2") + " to " + destination.ToString("X2"));
            }
            else
            {
                romBank1[destination] = source;
                Console.WriteLine("Writing " + source.ToString("X2") + " to " + destination.ToString("X2"));
            }
            

        }
        public int JP(int PC) {
            byte jumpLocation;
            if (PC < 0x3FFF)
            {

                jumpLocation = romBank0[PC];
            }
            else
            {
                jumpLocation = romBank1[PC];
            }
            // Return the location to change PC to
            Console.WriteLine("Jumping to: " + jumpLocation.ToString("X4"));
            return jumpLocation;
        }
    }
}
