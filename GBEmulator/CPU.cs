﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace GBEmulator
{
    internal class CPU
    {
        /* Useful documentation:
         * http://marc.rawer.de/Gameboy/Docs/GBCPUman.pdf
         * http://gameboy.mongenel.com/dmg/opcodes.html
         * https://cturt.github.io/cinoop.html 
         * https://rylev.github.io/DMG-01/public/book/memory_map.html
         * https://gb-archive.github.io/salvage/decoding_gbz80_opcodes/Decoding%20Gamboy%20Z80%20Opcodes.html
         * https://github.com/CTurt/Cinoop
         */


        // Define the registers; A-L: 8bit; SP & PC: 16bit;
        private byte A = 0;
        private byte B = 0;
        private byte C = 0;
        private byte D = 0;
        private byte E = 0;
        private byte H = 0;
        private byte L = 0;
        private byte flag;

        private ushort SP; // Stack
        private ushort PC; // Program counter

        // Combination registers; All 16bit
        private ushort AF = 0;
        private ushort BC = 0;
        private ushort DE = 0;
        private ushort HL = 0;
        // Main Memory Manager
        MMU mmu = new MMU();

        //DEBUG STUFF
        public string debugRegisters;

        private string ReturnRegisters()
        {
            string registers = "";
            registers += A.ToString("X4") + "\n" + B.ToString("X4") + "\n"+ C.ToString("X4") + "\n" + D.ToString("X4") + "\n" + E.ToString("X4") + "\n" + H.ToString("X4") + "\n" + L.ToString("X4") + "\n" + A.ToString("X4") + "\n";
            return registers;
        }
            
        

        public void Step() {
            var opcode = (ushort)(mmu.getOpcode(PC));
            ushort nibble = (ushort)(opcode & 0xF000);

            string opcodeString = opcode.ToString("X");

            Console.WriteLine("Opcode: " + opcodeString);

            PC += 1; // Increment program count by 2, one for each character in opcode

            /*
             * OPCODE STUFF; (https://www.pastraiser.com/cpu/gameboy/gameboy_opcodes.html)
             * !!!!!SHITS FUCKED AND DOESNT WORK!!!!!
             */



            switch (opcode)
            {
                // Opcodes at offset 0
                case 0x00: Console.WriteLine("NOP"); break;
                case 0x01: Console.WriteLine("LD BC"); break;
                case 0x02: Console.WriteLine("LD (BC),A"); break;
                case 0x03: Console.WriteLine("INC BC"); BC++; break;
                case 0x04: Console.WriteLine("INC B"); B++;  break;
                case 0x05: Console.WriteLine("DEC B"); B--; break;
                case 0x06: Console.WriteLine("LD B"); mmu.LD8(B, PC); break;
                case 0x07: Console.WriteLine("RLCA"); break;
                case 0x08: Console.WriteLine("LD SP"); break;
                case 0x09: Console.WriteLine("ADD HL,BC"); HL = (ushort)(HL + BC); break;
                case 0x0A: Console.WriteLine("LD A,(BC)"); break;
                case 0x0B: Console.WriteLine("DEC BC"); BC--; break;
                case 0x0C: Console.WriteLine("INC C"); C++;  break;
                case 0x0D: Console.WriteLine("DEC C"); C--; break;
                case 0x0E: Console.WriteLine("LD C"); mmu.LD8(C, PC); break;
                case 0x0F: Console.WriteLine("RRCA"); break;

                // Opcodes at offset 1
                case 0x10: Console.WriteLine("STOP"); break;
                case 0x11: Console.WriteLine("LD DE"); break;
                case 0x12: Console.WriteLine("LD DE,(A)"); break;
                case 0x13: Console.WriteLine("INC DE"); DE++; break;
                case 0x14: Console.WriteLine("INC D"); D++; break;
                case 0x15: Console.WriteLine("DEC D"); D--; break;
                case 0x16: Console.WriteLine("LD D"); mmu.LD8(D, PC); break;
                case 0x17: Console.WriteLine("RLA"); break;
                case 0x18: Console.WriteLine("JR"); break;
                case 0x19: Console.WriteLine("ADD HL,DE"); HL = (ushort)(HL + DE); break;
                case 0x1A: Console.WriteLine("LD A, (DE)"); break;
                case 0x1B: Console.WriteLine("DEC DE"); DE--; break;
                case 0x1C: Console.WriteLine("INC E"); E++; break;
                case 0x1D: Console.WriteLine("DEC E"); E--; break;
                case 0x1E: Console.WriteLine("LD E"); mmu.LD8(E, PC); break;
                case 0x1F: Console.WriteLine("RRA"); break;

                // Opcodes at offset 2
                case 0x20: Console.WriteLine("JR NZ"); break;
                case 0x21: Console.WriteLine("LD HL"); break;
                case 0x22: Console.WriteLine("LD (HL+),A"); break;
                case 0x23: Console.WriteLine("INC HL"); HL++; break;
                case 0x24: Console.WriteLine("INC H"); H++; break;
                case 0x25: Console.WriteLine("DEC H"); H--; break;
                case 0x26: Console.WriteLine("LD H"); mmu.LD8(H, PC); break;
                case 0x27: Console.WriteLine("DAA"); break;
                case 0x28: Console.WriteLine("JR Z"); break;
                case 0x29: Console.WriteLine("ADD HL,HL"); HL = (ushort)(HL + HL); break;
                case 0x2A: Console.WriteLine("LD A,(HL+)"); break;
                case 0x2B: Console.WriteLine("DEC HL"); HL--; break;
                case 0x2C: Console.WriteLine("INC L"); L++; break;
                case 0x2D: Console.WriteLine("DEC L"); L--; break;
                case 0x2E: Console.WriteLine("LD L"); mmu.LD8(L, PC); break;
                case 0x2F: Console.WriteLine("CPL"); break;

                // Opcodes at offset 3
                case 0x30: Console.WriteLine("JR NC"); break;
                case 0x31: Console.WriteLine("LD SP"); break;
                case 0x32: Console.WriteLine("LD (HL-),A"); break;
                case 0x33: Console.WriteLine("INC SP"); SP++; break;
                case 0x34: Console.WriteLine("INC HL"); HL++; break;
                case 0x35: Console.WriteLine("DEC HL"); HL--; break;
                case 0x36: Console.WriteLine("LD HL"); break;
                case 0x37: Console.WriteLine("SCF"); break;
                case 0x38: Console.WriteLine("JR C"); break;
                case 0x39: Console.WriteLine("ADD HL SP"); HL = (ushort)(HL+SP); break;
                case 0x3A: Console.WriteLine("LD A,(HL-)"); break;
                case 0x3B: Console.WriteLine("DEC SP"); SP--; break;
                case 0x3C: Console.WriteLine("INC A"); A++; break;
                case 0x3D: Console.WriteLine("DEC A"); A--; break;
                case 0x3E: Console.WriteLine("LD A"); mmu.LD8(A, PC); break;
                case 0x3F: Console.WriteLine("CCF"); break;

                // Opcodes at offset 4
                case 0x40: Console.WriteLine("LD B,B"); break;
                case 0x41: Console.WriteLine("LD B,C"); break;
                case 0x42: Console.WriteLine("LD B,D"); break;
                case 0x43: Console.WriteLine("LD B,E"); break;
                case 0x44: Console.WriteLine("LD B,H"); break;
                case 0x45: Console.WriteLine("LD B,L"); break;
                case 0x46: Console.WriteLine("LD B,HL"); break;
                case 0x47: Console.WriteLine("LD B,A"); break;
                case 0x48: Console.WriteLine("LD C,B"); break;
                case 0x49: Console.WriteLine("LD C,C"); break;
                case 0x4A: Console.WriteLine("LD C,D"); break;
                case 0x4B: Console.WriteLine("LD C,E"); break;
                case 0x4C: Console.WriteLine("LD C,H"); break;
                case 0x4D: Console.WriteLine("LD C,L"); break;
                case 0x4E: Console.WriteLine("LD C,HL"); break;
                case 0x4F: Console.WriteLine("LD C,A"); break;

                // Opcodes at offset 5
                case 0x50: Console.WriteLine("LD D,B"); break;
                case 0x51: Console.WriteLine("LD D,C"); break;
                case 0x52: Console.WriteLine("LD D,D"); break;
                case 0x53: Console.WriteLine("LD D,E"); break;
                case 0x54: Console.WriteLine("LD D,H"); break;
                case 0x55: Console.WriteLine("LD D,L"); break;
                case 0x56: Console.WriteLine("LD D,HL"); break;
                case 0x57: Console.WriteLine("LD D,A"); break;
                case 0x58: Console.WriteLine("LD E,B"); break;
                case 0x59: Console.WriteLine("LD E,C"); break;
                case 0x5A: Console.WriteLine("LD E,D"); break;
                case 0x5B: Console.WriteLine("LD E,E"); break;
                case 0x5C: Console.WriteLine("LD E,H"); break;
                case 0x5D: Console.WriteLine("LD E,L"); break;
                case 0x5E: Console.WriteLine("LD E,L"); break;
                case 0x5F: Console.WriteLine("LD E,HL"); break;

                // Opcodes at offset 6
                case 0x60: Console.WriteLine("LD H,B"); break;
                case 0x61: Console.WriteLine("LD H,C"); break;
                case 0x62: Console.WriteLine("LD H,D"); break;
                case 0x63: Console.WriteLine("LD H,E"); break;
                case 0x64: Console.WriteLine("LD H,H"); break;
                case 0x65: Console.WriteLine("LD H,L"); break;
                case 0x66: Console.WriteLine("LD H,HL"); break;
                case 0x67: Console.WriteLine("LD H,A"); break;
                case 0x68: Console.WriteLine("LD H,B"); break;
                case 0x69: Console.WriteLine("LD L,C"); break;
                case 0x6A: Console.WriteLine("LD L,D"); break;
                case 0x6B: Console.WriteLine("LD L,E"); break;
                case 0x6C: Console.WriteLine("LD L,H"); break;
                case 0x6D: Console.WriteLine("LD L,L"); break;
                case 0x6E: Console.WriteLine("LD L,HL"); break;
                case 0x6F: Console.WriteLine("LD L,A"); break;

                // Opcodes at offset 7
                case 0x70: Console.WriteLine("LD HL,B"); break;
                case 0x71: Console.WriteLine("LD HL,C"); break;
                case 0x72: Console.WriteLine("LD HL,D"); break;
                case 0x73: Console.WriteLine("LD HL,E"); break;
                case 0x74: Console.WriteLine("LD HL,H"); break;
                case 0x75: Console.WriteLine("LD HL,L"); break;
                case 0x76: Console.WriteLine("HALT"); break;
                case 0x77: Console.WriteLine("LD HL,A"); break;
                case 0x78: Console.WriteLine("LD HL,B"); break;
                case 0x79: Console.WriteLine("LD A,C"); break;
                case 0x7A: Console.WriteLine("LD A,D"); break;
                case 0x7B: Console.WriteLine("LD A,E"); break;
                case 0x7C: Console.WriteLine("LD A,H"); break;
                case 0x7D: Console.WriteLine("LD A,L"); break;
                case 0x7E: Console.WriteLine("LD A,HL"); break;
                case 0x7F: Console.WriteLine("LD A,A"); break;

                // Opcodes at offset 8
                case 0x80: Console.WriteLine("ADD A,B"); A = (byte)(A + B); break;
                case 0x81: Console.WriteLine("ADD A,C"); A = (byte)(A + C); break;
                case 0x82: Console.WriteLine("ADD A,D"); A = (byte)(A + D); break;
                case 0x83: Console.WriteLine("ADD A,E"); A = (byte)(A + E); break;
                case 0x84: Console.WriteLine("ADD A,H"); A = (byte)(A + H); break;
                case 0x85: Console.WriteLine("ADD A,L"); A = (byte)(A + L); break;
                case 0x86: Console.WriteLine("ADD A,HL"); A = (byte)(A + HL); break;
                case 0x87: Console.WriteLine("ADD A,A"); A = (byte)(A + A); break;
                case 0x88: Console.WriteLine("ADC A,B"); break;
                case 0x89: Console.WriteLine("ADC A,C"); break;
                case 0x8A: Console.WriteLine("ADC A,D"); break;
                case 0x8B: Console.WriteLine("ADC A,E"); break;
                case 0x8C: Console.WriteLine("ADC A,H"); break;
                case 0x8D: Console.WriteLine("ADC A,L"); break;
                case 0x8E: Console.WriteLine("ADC A,HL");  break;
                case 0x8F: Console.WriteLine("ADC A,C");  break;

                // Opcodes at offset 9
                case 0x90: Console.WriteLine("SUB B"); break;
                case 0x91: Console.WriteLine("SUB C"); break;
                case 0x92: Console.WriteLine("SUB D"); break;
                case 0x93: Console.WriteLine("SUB E"); break;
                case 0x94: Console.WriteLine("SUB H"); break;
                case 0x95: Console.WriteLine("SUB L"); break;
                case 0x96: Console.WriteLine("SUB HL"); break;
                case 0x97: Console.WriteLine("SUB A"); break;
                case 0x98: Console.WriteLine("SBC A,B"); break;
                case 0x99: Console.WriteLine("SBC A,C"); break;
                case 0x9A: Console.WriteLine("SBC A,D"); break;
                case 0x9B: Console.WriteLine("SBC A,E"); break;
                case 0x9C: Console.WriteLine("SBC A,H"); break;
                case 0x9D: Console.WriteLine("SBC A,L"); break;
                case 0x9E: Console.WriteLine("SBC A,HL"); break;
                case 0x9F: Console.WriteLine("SBC A,A"); break;

                // Opcodes at offset A
                case 0xA0: Console.WriteLine("AND B"); break;
                case 0xA1: Console.WriteLine("AND C"); break;
                case 0xA2: Console.WriteLine("AND D"); break;
                case 0xA3: Console.WriteLine("AND E"); break;
                case 0xA4: Console.WriteLine("AND H"); break;
                case 0xA5: Console.WriteLine("AND L"); break;
                case 0xA6: Console.WriteLine("AND HL"); break;
                case 0xA7: Console.WriteLine("AND A"); break;
                case 0xA8: Console.WriteLine("XOR B"); break;
                case 0xA9: Console.WriteLine("XOR C"); break;
                case 0xAA: Console.WriteLine("XOR D"); break;
                case 0xAB: Console.WriteLine("XOR E"); break;
                case 0xAC: Console.WriteLine("XOR H"); break;
                case 0xAD: Console.WriteLine("XOR L"); break;
                case 0xAE: Console.WriteLine("XOR HL"); break;
                case 0xAF: Console.WriteLine("XOR A"); break;

                // Opcodes at offset B
                case 0xB0: Console.WriteLine("OR B"); break;
                case 0xB1: Console.WriteLine("OR C"); break;
                case 0xB2: Console.WriteLine("OR D"); break;
                case 0xB3: Console.WriteLine("OR E"); break;
                case 0xB4: Console.WriteLine("OR H"); break;
                case 0xB5: Console.WriteLine("OR L"); break;
                case 0xB6: Console.WriteLine("OR HL"); break;
                case 0xB7: Console.WriteLine("OR A"); break;
                case 0xB8: Console.WriteLine("CP B"); break;
                case 0xB9: Console.WriteLine("CP C"); break;
                case 0xBA: Console.WriteLine("CP D"); break;
                case 0xBB: Console.WriteLine("CP E"); break;
                case 0xBC: Console.WriteLine("CP H"); break;
                case 0xBD: Console.WriteLine("CP L"); break;
                case 0xBE: Console.WriteLine("CP HL"); break;
                case 0xBF: Console.WriteLine("CP A"); break;
                
                // Opcodes at offset C
                case 0xC0: Console.WriteLine("RET NZ"); break;
                case 0xC1: Console.WriteLine("POP BC"); break;
                case 0xC2: Console.WriteLine("JP NZ"); break;
                case 0xC3: Console.WriteLine("JP"); PC = (ushort)mmu.JP(PC); break;
                case 0xC4: Console.WriteLine("CALL NZ"); break;
                case 0xC5: Console.WriteLine("PUSH BC"); break;
                case 0xC6: Console.WriteLine("ADD A"); break;
                case 0xC7: Console.WriteLine("RST 00H"); break;
                case 0xC8: Console.WriteLine("RET Z"); break;
                case 0xC9: Console.WriteLine("RET"); break;
                case 0xCA: Console.WriteLine("JP Z"); break;
                case 0xCB: Console.WriteLine("PREFIX"); break;
                case 0xCC: Console.WriteLine("CALL Z"); break;
                case 0xCD: Console.WriteLine("CALL"); break;
                case 0xCE: Console.WriteLine("ADC A"); break;
                case 0xCF: Console.WriteLine("RST"); break;

                // Opcodes at offset D
                case 0xD0: Console.WriteLine("NOP"); break;
                case 0xD1: Console.WriteLine("LD BC"); break;
                case 0xD2: Console.WriteLine("LD (BC),A"); break;
                case 0xD3: Console.WriteLine("INC BC"); break;
                case 0xD4: Console.WriteLine("INC B"); break;
                case 0xD5: Console.WriteLine("DEC B"); break;
                case 0xD6: Console.WriteLine("LD B"); break;
                case 0xD7: Console.WriteLine("RLCA"); break;
                case 0xD8: Console.WriteLine("LD SP"); break;
                case 0xD9: Console.WriteLine("ADD HL,BC"); break;
                case 0xDA: Console.WriteLine("LD A,(BC)"); break;
                case 0xDB: Console.WriteLine("DEC BC"); BC++; break;
                case 0xDC: Console.WriteLine("INC C"); C++; break;
                case 0xDD: Console.WriteLine("DEC C"); C--; break;
                case 0xDE: Console.WriteLine("LD C"); break;
                case 0xDF: Console.WriteLine("RRCA"); break;
                
                // Opcodes at offset E
                case 0xE0: Console.WriteLine("NOP"); break;
                case 0xE1: Console.WriteLine("LD BC"); break;
                case 0xE2: Console.WriteLine("LD (BC),A"); break;
                case 0xE3: Console.WriteLine("INC BC"); BC++; break;
                case 0xE4: Console.WriteLine("INC B"); B++; break;
                case 0xE5: Console.WriteLine("DEC B"); B--; break;
                case 0xE6: Console.WriteLine("LD B"); break;
                case 0xE7: Console.WriteLine("RLCA"); break;
                case 0xE8: Console.WriteLine("LD SP"); break;
                case 0xE9: Console.WriteLine("ADD HL,BC"); break;
                case 0xEA: Console.WriteLine("LD A,(BC)"); break;
                case 0xEB: Console.WriteLine("DEC BC"); BC--; break;
                case 0xEC: Console.WriteLine("INC C"); C++; break;
                case 0xED: Console.WriteLine("DEC C"); C--; break;
                case 0xEE: Console.WriteLine("LD C"); mmu.LD8(C, PC); break;
                case 0xEF: Console.WriteLine("RRCA"); break;

                // Opcodes at offset F
                case 0xF0: Console.WriteLine("NOP"); break;
                case 0xF1: Console.WriteLine("LD BC"); break;
                case 0xF2: Console.WriteLine("LD (BC),A"); break;
                case 0xF3: Console.WriteLine("INC BC"); BC++; break;
                case 0xF4: Console.WriteLine("INC B"); B++; break;
                case 0xF5: Console.WriteLine("DEC B"); B--; break;
                case 0xF6: Console.WriteLine("LD B"); break;
                case 0xF7: Console.WriteLine("RLCA"); break;
                case 0xF8: Console.WriteLine("LD SP"); break;
                case 0xF9: Console.WriteLine("ADD HL,BC"); break;
                case 0xFA: Console.WriteLine("LD A,(BC)"); break;
                case 0xFB: Console.WriteLine("DEC BC"); BC--; break;
                case 0xFC: Console.WriteLine("INC C"); C++; break;
                case 0xFD: Console.WriteLine("DEC C"); C--; break;
                case 0xFE: Console.WriteLine("LD C"); mmu.LD8(C, PC); break;
                case 0xFF: Console.WriteLine("RRCA"); break;

            }
        }

        /*
         * INSTRUCTION SET
         *

        private void LDAdd8(byte reg) {
            ushort address = (ushort)(RAM[PC + 1] << 8 | RAM[PC]);
            RAM[address] = reg;
        }

        
        private void JP(byte Flag)
        {
            ushort address = (ushort)(RAM[PC + 1] << 8 | RAM[PC]);
            flag = Flag;

            Console.WriteLine("Address: " + address.ToString("X4")); // Concantenate the next 2 bytes and print as string
            PC = address;
        }
        */
        public void Main(string path)
        {
            Console.WriteLine("Running...");
            Cartridge cart = new Cartridge();
             
            List<byte> gameData = cart.ReadProgram(path); // Load cartridge data
            mmu.LDRom(gameData);
            

            for (int i = 0; i < 500; i++){
                Step();
                debugRegisters = ReturnRegisters();
            }
            

        }

    }
}
