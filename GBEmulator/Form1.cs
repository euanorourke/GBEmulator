using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace GBEmulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cartridge cart = new Cartridge(); // Create instance of cartridge

            string path = gamePath.Text.ToString(); // Get the path in the textbox
            List<byte> byteList = cart.ReadProgram(path); // Get bytes from cartridge class
            byteOutput.Text = "ROM Information: \n";

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < byteList.Count; i++) { // Iterate over every byte in byteList
                string strBytes = ((byteList[i].ToString("X"))); // Convert to hex and store in variable
                sb.Append((strBytes) + " "); // Print out the ROM dump
                
            }
            byteOutput.Text = "ROM Information: \n " + sb;
            CPU cpu = new CPU();
            cpu.Main(path); // Run cpu

            registerRtxt.Text = cpu.debugRegisters;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
