using GBEmulator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;

using System.Runtime.InteropServices;

namespace GBEmulator
{
    internal class Cartridge
    {
        public List<byte> ReadProgram(string romPath) {

            using (BinaryReader reader = new BinaryReader(new FileStream(romPath, FileMode.Open)))  // Open the file
            {
                List<byte> rom = new List<byte>();                                                  // Create list for the bytes in the file

                while (reader.BaseStream.Position < reader.BaseStream.Length)                       // While we are still reading the file
                {
                    rom.Add(reader.ReadByte());                                                     // Add bytes to the list
                }

                return rom;
            } 
            
        }
        

    }
}
