using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using lh5801_Dis;

namespace lh5801_Dis_Test
{
    [TestClass]
    public class UnitTest_P1
    {
        lh5801_Dis.lh5801_Dis CPU = new lh5801_Dis.lh5801_Dis();

        [TestMethod]
        public void Opcodes_0n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                                        0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   00                         SBC  XL\r\n";
            Assert.AreEqual(match, output, "SBC  XL");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   01                         SBC  (X)\r\n";
            Assert.AreEqual(match, output, "SBC  (X)");

            CPU.Run((ushort)(address + 2), (ushort)(address + 2));
            output = CPU.DisDump();
            match = "1002   02                         ADC  XL\r\n";
            Assert.AreEqual(match, output, "ADC  XL");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   03                         ADC  (X)\r\n";
            Assert.AreEqual(match, output, "ADC  (X)");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   04                         LDA  XL\r\n";
            Assert.AreEqual(match, output, "LDA  XL");

            CPU.Run((ushort)(address + 5), (ushort)(address + 5));
            output = CPU.DisDump();
            match = "1005   05                         LDA  (X)\r\n";
            Assert.AreEqual(match, output, "LDA  (X)");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   06                         CPA  XL\r\n";
            Assert.AreEqual(match, output, "CPA  XL");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   07                         CPA  (X)\r\n";
            Assert.AreEqual(match, output, "CPA  (X)");

            CPU.Run((ushort)(address + 8), (ushort)(address + 8));
            output = CPU.DisDump();
            match = "1008   08                         STA  XH\r\n";
            Assert.AreEqual(match, output, "STA  XH");

            CPU.Run((ushort)(address + 9), (ushort)(address + 9));
            output = CPU.DisDump();
            match = "1009   09                         AND  (X)\r\n";
            Assert.AreEqual(match, output, "AND  (X)");

            CPU.Run((ushort)(address + 0xA), (ushort)(address + 0xA));
            output = CPU.DisDump();
            match = "100A   0A                         STA  XL\r\n";
            Assert.AreEqual(match, output, "STA  XL");

            CPU.Run((ushort)(address + 0xB), (ushort)(address + 0xB));
            output = CPU.DisDump();
            match = "100B   0B                         ORA  (X)\r\n";
            Assert.AreEqual(match, output, "ORA  (X)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   0C                         DCS  (X)\r\n";
            Assert.AreEqual(match, output, "DCS  (X)");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   0D                         EOR  (X)\r\n";
            Assert.AreEqual(match, output, "EOR  (X)");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   0E                         STA  (X)\r\n";
            Assert.AreEqual(match, output, "STA  (X)");

            CPU.Run((ushort)(address + 0x0F), (ushort)(address + 0x0F));
            output = CPU.DisDump();
            match = "100F   0F                         BIT  (X)\r\n";
            Assert.AreEqual(match, output, "BIT  (X)");
        }

        [TestMethod]
        public void Opcodes_1n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17,
                                        0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   10                         SBC  YL\r\n";
            Assert.AreEqual(match, output, "SBC  YL");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   11                         SBC  (Y)\r\n";
            Assert.AreEqual(match, output, "SBC  (Y)");

            CPU.Run((ushort)(address + 2), (ushort)(address + 2));
            output = CPU.DisDump();
            match = "1002   12                         ADC  YL\r\n";
            Assert.AreEqual(match, output, "ADC  YL");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   13                         ADC  (Y)\r\n";
            Assert.AreEqual(match, output, "ADC  (Y)");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   14                         LDA  YL\r\n";
            Assert.AreEqual(match, output, "LDA  YL");

            CPU.Run((ushort)(address + 5), (ushort)(address + 5));
            output = CPU.DisDump();
            match = "1005   15                         LDA  (Y)\r\n";
            Assert.AreEqual(match, output, "LDA  (Y)");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   16                         CPA  YL\r\n";
            Assert.AreEqual(match, output, "CPA  YL");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   17                         CPA  (Y)\r\n";
            Assert.AreEqual(match, output, "CPA  (Y)");

            CPU.Run((ushort)(address + 8), (ushort)(address + 8));
            output = CPU.DisDump();
            match = "1008   18                         STA  YH\r\n";
            Assert.AreEqual(match, output, "STA  YH");

            CPU.Run((ushort)(address + 9), (ushort)(address + 9));
            output = CPU.DisDump();
            match = "1009   19                         AND  (Y)\r\n";
            Assert.AreEqual(match, output, "AND  (Y)");

            CPU.Run((ushort)(address + 0xA), (ushort)(address + 0xA));
            output = CPU.DisDump();
            match = "100A   1A                         STA  YL\r\n";
            Assert.AreEqual(match, output, "STA  YL");

            CPU.Run((ushort)(address + 0xB), (ushort)(address + 0xB));
            output = CPU.DisDump();
            match = "100B   1B                         ORA  (Y)\r\n";
            Assert.AreEqual(match, output, "ORA  (Y)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   1C                         DCS  (Y)\r\n";
            Assert.AreEqual(match, output, "DCS  (Y)");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   1D                         EOR  (Y)\r\n";
            Assert.AreEqual(match, output, "EOR  (Y)");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   1E                         STA  (Y)\r\n";
            Assert.AreEqual(match, output, "STA  (Y)");

            CPU.Run((ushort)(address + 0x0F), (ushort)(address + 0x0F));
            output = CPU.DisDump();
            match = "100F   1F                         BIT  (Y)\r\n";
            Assert.AreEqual(match, output, "BIT  (Y)");
        }

        [TestMethod]
        public void Opcodes_2n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
                                        0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   20                         SBC  UL\r\n";
            Assert.AreEqual(match, output, "SBC  UL");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   21                         SBC  (U)\r\n";
            Assert.AreEqual(match, output, "SBC  (U)");

            CPU.Run((ushort)(address + 2), (ushort)(address + 2));
            output = CPU.DisDump();
            match = "1002   22                         ADC  UL\r\n";
            Assert.AreEqual(match, output, "ADC  UL");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   23                         ADC  (U)\r\n";
            Assert.AreEqual(match, output, "ADC  (U)");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   24                         LDA  UL\r\n";
            Assert.AreEqual(match, output, "LDA  UL");

            CPU.Run((ushort)(address + 5), (ushort)(address + 5));
            output = CPU.DisDump();
            match = "1005   25                         LDA  (U)\r\n";
            Assert.AreEqual(match, output, "LDA  (U)");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   26                         CPA  UL\r\n";
            Assert.AreEqual(match, output, "CPA  UL");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   27                         CPA  (U)\r\n";
            Assert.AreEqual(match, output, "CPA  (U)");

            CPU.Run((ushort)(address + 8), (ushort)(address + 8));
            output = CPU.DisDump();
            match = "1008   28                         STA  UH\r\n";
            Assert.AreEqual(match, output, "STA  UH");

            CPU.Run((ushort)(address + 9), (ushort)(address + 9));
            output = CPU.DisDump();
            match = "1009   29                         AND  (U)\r\n";
            Assert.AreEqual(match, output, "AND  (U)");

            CPU.Run((ushort)(address + 0xA), (ushort)(address + 0xA));
            output = CPU.DisDump();
            match = "100A   2A                         STA  UL\r\n";
            Assert.AreEqual(match, output, "STA  UL");

            CPU.Run((ushort)(address + 0xB), (ushort)(address + 0xB));
            output = CPU.DisDump();
            match = "100B   2B                         ORA  (U)\r\n";
            Assert.AreEqual(match, output, "ORA  (U)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   2C                         DCS  (U)\r\n";
            Assert.AreEqual(match, output, "DCS  (U)");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   2D                         EOR  (U)\r\n";
            Assert.AreEqual(match, output, "EOR  (U)");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   2E                         STA  (U)\r\n";
            Assert.AreEqual(match, output, "STA  (U)");

            CPU.Run((ushort)(address + 0x0F), (ushort)(address + 0x0F));
            output = CPU.DisDump();
            match = "100F   2F                         BIT  (U)\r\n";
            Assert.AreEqual(match, output, "BIT  (U)");
        }

        [TestMethod]
        public void Opcodes_3n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
                                        0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   30                         SBC  VL\r\n";
            Assert.AreEqual(match, output, "SBC  VL");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   31                         SBC  (V)\r\n";
            Assert.AreEqual(match, output, "SBC  (V)");

            CPU.Run((ushort)(address + 2), (ushort)(address + 2));
            output = CPU.DisDump();
            match = "1002   32                         ADC  VL\r\n";
            Assert.AreEqual(match, output, "ADC  VL");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   33                         ADC  (V)\r\n";
            Assert.AreEqual(match, output, "ADC  (V)");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   34                         LDA  VL\r\n";
            Assert.AreEqual(match, output, "LDA  VL");

            CPU.Run((ushort)(address + 5), (ushort)(address + 5));
            output = CPU.DisDump();
            match = "1005   35                         LDA  (V)\r\n";
            Assert.AreEqual(match, output, "LDA  (V)");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   36                         CPA  VL\r\n";
            Assert.AreEqual(match, output, "CPA  VL");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   37                         CPA  (V)\r\n";
            Assert.AreEqual(match, output, "CPA  (V)");

            CPU.Run((ushort)(address + 8), (ushort)(address + 8));
            output = CPU.DisDump();
            match = "1008   38                         NOP\r\n";
            Assert.AreEqual(match, output, "NOP");

            CPU.Run((ushort)(address + 9), (ushort)(address + 9));
            output = CPU.DisDump();
            match = "1009   39                         AND  (V)\r\n";
            Assert.AreEqual(match, output, "AND  (V)");

            CPU.Run((ushort)(address + 0xA), (ushort)(address + 0xA));
            output = CPU.DisDump();
            match = "100A   3A                         STA  VL\r\n";
            Assert.AreEqual(match, output, "STA  VL");

            CPU.Run((ushort)(address + 0xB), (ushort)(address + 0xB));
            output = CPU.DisDump();
            match = "100B   3B                         ORA  (V)\r\n";
            Assert.AreEqual(match, output, "ORA  (V)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   3C                         DCS  (V)\r\n";
            Assert.AreEqual(match, output, "DCS  (V)");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   3D                         EOR  (V)\r\n";
            Assert.AreEqual(match, output, "EOR  (V)");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   3E                         STA  (V)\r\n";
            Assert.AreEqual(match, output, "STA  (V)");

            CPU.Run((ushort)(address + 0x0F), (ushort)(address + 0x0F));
            output = CPU.DisDump();
            match = "100F   3F                         BIT  (V)\r\n";
            Assert.AreEqual(match, output, "BIT  (V)");
        }

        [TestMethod]
        public void Opcodes_4n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47,
                                        0x48, 0x1A, 0x49, 0x1B, 0x4A, 0x1C, 0x4B, 0x1D,
                                        0x4C, 0x1E, 0x4D, 0x1F, 0x4E, 0x20, 0x4F, 0x21};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   40                         INC  XL\r\n";
            Assert.AreEqual(match, output, "INC  XL");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   41                         SIN  X\r\n";
            Assert.AreEqual(match, output, "SIN  X");

            CPU.Run((ushort)(address + 2), (ushort)(address + 2));
            output = CPU.DisDump();
            match = "1002   42                         DEC  XL\r\n";
            Assert.AreEqual(match, output, "ADC  VL");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   43                         SDE  X\r\n";
            Assert.AreEqual(match, output, "SDE  X");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   44                         INC  X\r\n";
            Assert.AreEqual(match, output, "INC  X");

            CPU.Run((ushort)(address + 5), (ushort)(address + 5));
            output = CPU.DisDump();
            match = "1005   45                         LIN  X\r\n";
            Assert.AreEqual(match, output, "LIN  X");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   46                         DEC  X\r\n";
            Assert.AreEqual(match, output, "DEC  X");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   47                         LDE  X\r\n";
            Assert.AreEqual(match, output, "LDE  X");

            CPU.Run((ushort)(address + 8), (ushort)(address + 8));
            output = CPU.DisDump();
            match = "1008   48 1A                      LDI  XH,$1A\r\n";
            Assert.AreEqual(match, output, "LDI  XH,$1A");

            CPU.Run((ushort)(address + 0xA), (ushort)(address + 0xA));
            output = CPU.DisDump();
            match = "100A   49 1B                      ANI  (X),$1B\r\n";
            Assert.AreEqual(match, output, "ANI  (X),$1B");

            CPU.Run((ushort)(address + 0xC), (ushort)(address + 0xC));
            output = CPU.DisDump();
            match = "100C   4A 1C                      LDI  XL,$1C\r\n";
            Assert.AreEqual(match, output, "LDI  XL,$1C");

            CPU.Run((ushort)(address + 0xE), (ushort)(address + 0xE));
            output = CPU.DisDump();
            match = "100E   4B 1D                      ORI  (X),$1D\r\n";
            Assert.AreEqual(match, output, "ORI  (X),$1D");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   4C 1E                      CPI  XH,$1E\r\n";
            Assert.AreEqual(match, output, "CPI  XH,$1E");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   4D 1F                      BII  (X),$1F\r\n";
            Assert.AreEqual(match, output, "BII  (X),$1F");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   4E 20                      CPI  XL,$20\r\n";
            Assert.AreEqual(match, output, "CPI  XL,$20");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   4F 21                      ADI  (X),$21\r\n";
            Assert.AreEqual(match, output, "ADI  (X),$21");
        }

        [TestMethod]
        public void Opcodes_5n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57,
                                        0x58, 0x1A, 0x59, 0x1B, 0x5A, 0x1C, 0x5B, 0x1D,
                                        0x5C, 0x1E, 0x5D, 0x1F, 0x5E, 0x20, 0x5F, 0x21};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   50                         INC  YL\r\n";
            Assert.AreEqual(match, output, "INC  YL");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   51                         SIN  Y\r\n";
            Assert.AreEqual(match, output, "SIN  Y");

            CPU.Run((ushort)(address + 2), (ushort)(address + 2));
            output = CPU.DisDump();
            match = "1002   52                         DEC  YL\r\n";
            Assert.AreEqual(match, output, "ADC  YL");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   53                         SDE  Y\r\n";
            Assert.AreEqual(match, output, "SDE  Y");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   54                         INC  Y\r\n";
            Assert.AreEqual(match, output, "INC  Y");

            CPU.Run((ushort)(address + 5), (ushort)(address + 5));
            output = CPU.DisDump();
            match = "1005   55                         LIN  Y\r\n";
            Assert.AreEqual(match, output, "LIN  Y");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   56                         DEC  Y\r\n";
            Assert.AreEqual(match, output, "DEC  Y");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   57                         LDE  Y\r\n";
            Assert.AreEqual(match, output, "LDE  Y");

            CPU.Run((ushort)(address + 8), (ushort)(address + 8));
            output = CPU.DisDump();
            match = "1008   58 1A                      LDI  YH,$1A\r\n";
            Assert.AreEqual(match, output, "LDI  YH,$1A");

            CPU.Run((ushort)(address + 0xA), (ushort)(address + 0xA));
            output = CPU.DisDump();
            match = "100A   59 1B                      ANI  (Y),$1B\r\n";
            Assert.AreEqual(match, output, "ANI  (Y),$1B");

            CPU.Run((ushort)(address + 0xC), (ushort)(address + 0xC));
            output = CPU.DisDump();
            match = "100C   5A 1C                      LDI  YL,$1C\r\n";
            Assert.AreEqual(match, output, "LDI  YL,$1C");

            CPU.Run((ushort)(address + 0xE), (ushort)(address + 0xE));
            output = CPU.DisDump();
            match = "100E   5B 1D                      ORI  (Y),$1D\r\n";
            Assert.AreEqual(match, output, "ORI  (Y),$1D");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   5C 1E                      CPI  YH,$1E\r\n";
            Assert.AreEqual(match, output, "CPI  YH,$1E");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   5D 1F                      BII  (Y),$1F\r\n";
            Assert.AreEqual(match, output, "BII  (Y),$1F");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   5E 20                      CPI  YL,$20\r\n";
            Assert.AreEqual(match, output, "CPI  YL,$20");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   5F 21                      ADI  (Y),$21\r\n";
            Assert.AreEqual(match, output, "ADI  (Y),$21");
        }

        [TestMethod]
        public void Opcodes_6n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67,
                                        0x68, 0x1A, 0x69, 0x1B, 0x6A, 0x1C, 0x6B, 0x1D,
                                        0x6C, 0x1E, 0x6D, 0x1F, 0x6E, 0x20, 0x6F, 0x21};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   60                         INC  UL\r\n";
            Assert.AreEqual(match, output, "INC  UL");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   61                         SIN  U\r\n";
            Assert.AreEqual(match, output, "SIN  U");

            CPU.Run((ushort)(address + 2), (ushort)(address + 2));
            output = CPU.DisDump();
            match = "1002   62                         DEC  UL\r\n";
            Assert.AreEqual(match, output, "ADC  UL");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   63                         SDE  U\r\n";
            Assert.AreEqual(match, output, "SDE  U");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   64                         INC  U\r\n";
            Assert.AreEqual(match, output, "INC  U");

            CPU.Run((ushort)(address + 5), (ushort)(address + 5));
            output = CPU.DisDump();
            match = "1005   65                         LIN  U\r\n";
            Assert.AreEqual(match, output, "LIN  U");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   66                         DEC  U\r\n";
            Assert.AreEqual(match, output, "DEC  U");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   67                         LDE  U\r\n";
            Assert.AreEqual(match, output, "LDE  U");

            CPU.Run((ushort)(address + 8), (ushort)(address + 8));
            output = CPU.DisDump();
            match = "1008   68 1A                      LDI  UH,$1A\r\n";
            Assert.AreEqual(match, output, "LDI  UH,$1A");

            CPU.Run((ushort)(address + 0xA), (ushort)(address + 0xA));
            output = CPU.DisDump();
            match = "100A   69 1B                      ANI  (U),$1B\r\n";
            Assert.AreEqual(match, output, "ANI  (U),$1B");

            CPU.Run((ushort)(address + 0xC), (ushort)(address + 0xC));
            output = CPU.DisDump();
            match = "100C   6A 1C                      LDI  UL,$1C\r\n";
            Assert.AreEqual(match, output, "LDI  UL,$1C");

            CPU.Run((ushort)(address + 0xE), (ushort)(address + 0xE));
            output = CPU.DisDump();
            match = "100E   6B 1D                      ORI  (U),$1D\r\n";
            Assert.AreEqual(match, output, "ORI  (U),$1D");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   6C 1E                      CPI  UH,$1E\r\n";
            Assert.AreEqual(match, output, "CPI  UH,$1E");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   6D 1F                      BII  (U),$1F\r\n";
            Assert.AreEqual(match, output, "BII  (U),$1F");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   6E 20                      CPI  UL,$20\r\n";
            Assert.AreEqual(match, output, "CPI  UL,$20");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   6F 21                      ADI  (U),$21\r\n";
            Assert.AreEqual(match, output, "ADI  (U),$21");
        }

        [TestMethod]
        public void Opcodes_7n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77,
                                        0x78, 0x1A, 0x79, 0x1B, 0x7A, 0x1C, 0x7B, 0x1D,
                                        0x7C, 0x1E, 0x7D, 0x1F, 0x7E, 0x20, 0x7F, 0x21};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   70                         INC  VL\r\n";
            Assert.AreEqual(match, output, "INC  VL");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   71                         SIN  V\r\n";
            Assert.AreEqual(match, output, "SIN  V");

            CPU.Run((ushort)(address + 2), (ushort)(address + 2));
            output = CPU.DisDump();
            match = "1002   72                         DEC  VL\r\n";
            Assert.AreEqual(match, output, "ADC  VL");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   73                         SDE  V\r\n";
            Assert.AreEqual(match, output, "SDE  V");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   74                         INC  V\r\n";
            Assert.AreEqual(match, output, "INC  V");

            CPU.Run((ushort)(address + 5), (ushort)(address + 5));
            output = CPU.DisDump();
            match = "1005   75                         LIN  V\r\n";
            Assert.AreEqual(match, output, "LIN  V");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   76                         DEC  V\r\n";
            Assert.AreEqual(match, output, "DEC  V");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   77                         LDE  V\r\n";
            Assert.AreEqual(match, output, "LDE  V");

            CPU.Run((ushort)(address + 8), (ushort)(address + 8));
            output = CPU.DisDump();
            match = "1008   78 1A                      LDI  VH,$1A\r\n";
            Assert.AreEqual(match, output, "LDI  VH,$1A");

            CPU.Run((ushort)(address + 0xA), (ushort)(address + 0xA));
            output = CPU.DisDump();
            match = "100A   79 1B                      ANI  (V),$1B\r\n";
            Assert.AreEqual(match, output, "ANI  (V),$1B");

            CPU.Run((ushort)(address + 0xC), (ushort)(address + 0xC));
            output = CPU.DisDump();
            match = "100C   7A 1C                      LDI  VL,$1C\r\n";
            Assert.AreEqual(match, output, "LDI  VL,$1C");

            CPU.Run((ushort)(address + 0xE), (ushort)(address + 0xE));
            output = CPU.DisDump();
            match = "100E   7B 1D                      ORI  (V),$1D\r\n";
            Assert.AreEqual(match, output, "ORI  (V),$1D");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   7C 1E                      CPI  VH,$1E\r\n";
            Assert.AreEqual(match, output, "CPI  VH,$1E");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   7D 1F                      BII  (V),$1F\r\n";
            Assert.AreEqual(match, output, "BII  (V),$1F");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   7E 20                      CPI  VL,$20\r\n";
            Assert.AreEqual(match, output, "CPI  VL,$20");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   7F 21                      ADI  (V),$21\r\n";
            Assert.AreEqual(match, output, "ADI  (V),$21");
        }

        [TestMethod]
        public void Opcodes_8n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0x80, 0x81, 0x1A, 0x82, 0x83, 0x1B, 0x84, 0x85,
                                        0x1C, 0x86, 0x87, 0x1D, 0x88, 0x1E, 0x89, 0x1F,
                                        0x8A, 0x8B, 0x20, 0x8C, 0x8D, 0x21, 0x8E, 0x22,
                                        0x8F, 0x23};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   80                         SBC  XH\r\n";
            Assert.AreEqual(match, output, "SBC  XH");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   81 1A                      BCR+ $101D\r\n";
            Assert.AreEqual(match, output, "BCR+ $101D");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   82                         ADC  XH\r\n";
            Assert.AreEqual(match, output, "ADC  XH");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   83 1B                      BCS+ $1021\r\n";
            Assert.AreEqual(match, output, "BCS+ $1B");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   84                         LDA  XH\r\n";
            Assert.AreEqual(match, output, "LDA  XH");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   85 1C                      BHR+ $1025\r\n";
            Assert.AreEqual(match, output, "BHR+ $1025");

            CPU.Run((ushort)(address + 9), (ushort)(address + 9));
            output = CPU.DisDump();
            match = "1009   86                         CPA  XH\r\n";
            Assert.AreEqual(match, output, "CPA  XH");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   87 1D                      BHS+ $1029\r\n";
            Assert.AreEqual(match, output, "BHS+ $1029");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   88 1E                      LOP UL,$0FF0\r\n";
            Assert.AreEqual(match, output, "LOP UL,$0FF0");

            CPU.Run((ushort)(address + 0xE), (ushort)(address + 0xE));
            output = CPU.DisDump();
            match = "100E   89 1F                      BZR+ $102F\r\n";
            Assert.AreEqual(match, output, "BZR+ $102F");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   8A                         RTI\r\n";
            Assert.AreEqual(match, output, "RTI");

            CPU.Run((ushort)(address + 0x11), (ushort)(address + 0x11));
            output = CPU.DisDump();
            match = "1011   8B 20                      BZS+ $1033\r\n";
            Assert.AreEqual(match, output, "BZS+ $1033");

            CPU.Run((ushort)(address + 0x13), (ushort)(address + 0x13));
            output = CPU.DisDump();
            match = "1013   8C                         DCA  (X)\r\n";
            Assert.AreEqual(match, output, "DCA  (X)");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   8D 21                      BVR+ $1037\r\n";
            Assert.AreEqual(match, output, "BVR+ $1037");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   8E 22                      BCH+ $103A\r\n";
            Assert.AreEqual(match, output, "BCH+ $103A");

            CPU.Run((ushort)(address + 0x18), (ushort)(address + 0x18));
            output = CPU.DisDump();
            match = "1018   8F 23                      BVS+ $103D\r\n";
            Assert.AreEqual(match, output, "BVS+ $103D");
        }

        [TestMethod]
        public void Opcodes_9n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0x90, 0x91, 0x1A, 0x92, 0x93, 0x1B, 0x94, 0x95,
                                        0x1C, 0x96, 0x97, 0x1D, 0x99, 0x1E, 0x9A, 0x9B,
                                        0x1F, 0x9C, 0x9D, 0x20, 0x9E, 0x21, 0x9F, 0x22};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   90                         SBC  YH\r\n";
            Assert.AreEqual(match, output, "SBC  YH");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   91 1A                      BCR- $0FE9\r\n";
            Assert.AreEqual(match, output, "BCR- $0FE9");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   92                         ADC  YH\r\n";
            Assert.AreEqual(match, output, "ADC  YH");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   93 1B                      BCS- $0FEB\r\n";
            Assert.AreEqual(match, output, "BCS- $0FEB");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   94                         LDA  YH\r\n";
            Assert.AreEqual(match, output, "LDA  YH");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   95 1C                      BHR- $0FED\r\n";
            Assert.AreEqual(match, output, "BHR- $0FED");

            CPU.Run((ushort)(address + 9), (ushort)(address + 9));
            output = CPU.DisDump();
            match = "1009   96                         CPA  YH\r\n";
            Assert.AreEqual(match, output, "CPA  YH");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   97 1D                      BHS- $0FEF\r\n";
            Assert.AreEqual(match, output, "BHS- $0FEF");

            CPU.Run((ushort)(address + 0xC), (ushort)(address + 0xC));
            output = CPU.DisDump();
            match = "100C   99 1E                      BZR- $0FF0\r\n";
            Assert.AreEqual(match, output, "BZR- $0FF0");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   9A                         RTN\r\n";
            Assert.AreEqual(match, output, "RTN");

            CPU.Run((ushort)(address + 0x0F), (ushort)(address + 0x0F));
            output = CPU.DisDump();
            match = "100F   9B 1F                      BZS- $0FF2\r\n";
            Assert.AreEqual(match, output, "BZS- $0FF2");

            CPU.Run((ushort)(address + 0x11), (ushort)(address + 0x11));
            output = CPU.DisDump();
            match = "1011   9C                         DCA  (Y)\r\n";
            Assert.AreEqual(match, output, "DCA  (Y)");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   9D 20                      BVR- $0FF4\r\n";
            Assert.AreEqual(match, output, "BVR- $0FF4");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   9E 21                      BCH- $0FF5\r\n";
            Assert.AreEqual(match, output, "BCH- $0FF5");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   9F 22                      BVS- $0FF6\r\n";
            Assert.AreEqual(match, output, "BVS- $0FF6");
        }

        [TestMethod]
        public void Opcodes_An()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xA0, 0xA1, 0x1A, 0x55, 0xA2, 0xA3, 0x1B, 0x55,
                                        0xA4, 0xA5, 0x1C, 0x55, 0xA6, 0xA7, 0x1D, 0x55,
                                        0xA8, 0xA9, 0x1E, 0x55, 0xAA, 0x1F, 0x55, 0xAB,
                                        0x20, 0x55, 0xAC, 0xAD, 0x21, 0x55, 0xAE, 0x22,
                                        0x55, 0xAF, 0x23, 0x55};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   A0                         SBC  UH\r\n";
            Assert.AreEqual(match, output, "SBC  UH");

            CPU.Run((ushort)(address + 0x01), (ushort)(address + 0x01));
            output = CPU.DisDump();
            match = "1001   A1 1A 55                   SBC  ($1A55)\r\n";
            Assert.AreEqual(match, output, "SBC  ($1A55)");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   A2                         ADC  UH\r\n";
            Assert.AreEqual(match, output, "ADC  UH");

            CPU.Run((ushort)(address + 0x05), (ushort)(address + 0x05));
            output = CPU.DisDump();
            match = "1005   A3 1B 55                   ADC  ($1B55)\r\n";
            Assert.AreEqual(match, output, "ADC  ($1B55)");

            CPU.Run((ushort)(address + 0x08), (ushort)(address + 0x08));
            output = CPU.DisDump();
            match = "1008   A4                         LDA  UH\r\n";
            Assert.AreEqual(match, output, "LDA  UH");

            CPU.Run((ushort)(address + 0x09), (ushort)(address + 0x09));
            output = CPU.DisDump();
            match = "1009   A5 1C 55                   LDA  ($1C55)\r\n";
            Assert.AreEqual(match, output, "LDA  ($1C55)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   A6                         CPA  UH\r\n";
            Assert.AreEqual(match, output, "CPA  UH");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   A7 1D 55                   CPA  ($1D55)\r\n";
            Assert.AreEqual(match, output, "CPA  ($1D55)");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   A8                         SPV\r\n";
            Assert.AreEqual(match, output, "SPV");

            CPU.Run((ushort)(address + 0x11), (ushort)(address + 0x11));
            output = CPU.DisDump();
            match = "1011   A9 1E 55                   AND  ($1E55)\r\n";
            Assert.AreEqual(match, output, "AND  ($1E55)");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   AA 1F 55                   LDI  S,($1F55)\r\n";
            Assert.AreEqual(match, output, "LDI  S,($1F55)");

            CPU.Run((ushort)(address + 0x17), (ushort)(address + 0x17));
            output = CPU.DisDump();
            match = "1017   AB 20 55                   ORA  ($2055)\r\n";
            Assert.AreEqual(match, output, "ORA  ($2055)");

            CPU.Run((ushort)(address + 0x1A), (ushort)(address + 0x1A));
            output = CPU.DisDump();
            match = "101A   AC                         DCA  (U)\r\n";
            Assert.AreEqual(match, output, "DCA  (U)");

            CPU.Run((ushort)(address + 0x1B), (ushort)(address + 0x1B));
            output = CPU.DisDump();
            match = "101B   AD 21 55                   EOR  ($2155)\r\n";
            Assert.AreEqual(match, output, "EOR  ($2155)");

            CPU.Run((ushort)(address + 0x1E), (ushort)(address + 0x1E));
            output = CPU.DisDump();
            match = "101E   AE 22 55                   STA  ($2255)\r\n";
            Assert.AreEqual(match, output, "STA  ($2255)");

            CPU.Run((ushort)(address + 0x21), (ushort)(address + 0x21));
            output = CPU.DisDump();
            match = "1021   AF 23 55                   BIT  ($2355)\r\n";
            Assert.AreEqual(match, output, "BIT  ($2355)");
        }

        [TestMethod]
        public void Opcodes_Bn()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xB0, 0xB1, 0x1A, 0xB2, 0xB3, 0x1B, 0xB4, 0xB5,
                                        0x1C, 0xB6, 0xB7, 0x1D, 0xB8, 0xB9, 0x1E, 0xBA,
                                        0x1F, 0x55, 0xBB, 0x20, 0xBC, 0xBD, 0x21, 0xBE,
                                        0x22, 0x55, 0xBF, 0x23};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   B0                         SBC  VH\r\n";
            Assert.AreEqual(match, output, "SBC  VH");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   B1 1A                      SBI  A,$1A\r\n";
            Assert.AreEqual(match, output, "SBI  A,$1A");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   B2                         ADC  VH\r\n";
            Assert.AreEqual(match, output, "ADC  VH");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   B3 1B                      ADI  A,$1B\r\n";
            Assert.AreEqual(match, output, "ADI  A,$1B");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   B4                         LDA  VH\r\n";
            Assert.AreEqual(match, output, "LDA  VH");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   B5 1C                      LDI  A,$1C\r\n";
            Assert.AreEqual(match, output, "LDI  A,$1C");

            CPU.Run((ushort)(address + 9), (ushort)(address + 9));
            output = CPU.DisDump();
            match = "1009   B6                         CPA  VH\r\n";
            Assert.AreEqual(match, output, "CPA  VH");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   B7 1D                      CPI  A,$1D\r\n";
            Assert.AreEqual(match, output, "CPI  A,$1D");

            CPU.Run((ushort)(address + 0xC), (ushort)(address + 0xC));
            output = CPU.DisDump();
            match = "100C   B8                         RPV\r\n";
            Assert.AreEqual(match, output, "RPV");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   B9 1E                      ANI  A,$1E\r\n";
            Assert.AreEqual(match, output, "ANI  A,$1E");

            CPU.Run((ushort)(address + 0x0F), (ushort)(address + 0x0F));
            output = CPU.DisDump();
            match = "100F   BA 1F 55                   JMP  $1F55\r\n";
            Assert.AreEqual(match, output, "JMP  $1F55");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   BB 20                      ORI  A,$20\r\n";
            Assert.AreEqual(match, output, "ORI  A,$20");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   BC                         DCA  (V)\r\n";
            Assert.AreEqual(match, output, "DCA  (V)");

            CPU.Run((ushort)(address + 0x15), (ushort)(address + 0x15));
            output = CPU.DisDump();
            match = "1015   BD 21                      EAI  $21\r\n";
            Assert.AreEqual(match, output, "EAI  $21");

            CPU.Run((ushort)(address + 0x17), (ushort)(address + 0x17));
            output = CPU.DisDump();
            match = "1017   BE 22 55                   SJP  $2255\r\n";
            Assert.AreEqual(match, output, "SJP  $2255");

            CPU.Run((ushort)(address + 0x1A), (ushort)(address + 0x1A));
            output = CPU.DisDump();
            match = "101A   BF 23                      BII  A,$23\r\n";
            Assert.AreEqual(match, output, "BII  A,$23");
        }

        [TestMethod]
        public void Opcodes_Cn()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xC0, 0xC1, 0x1A, 0xC2, 0xC3, 0x1B, 0xC4, 0xC5,
                                        0x1C, 0xC6, 0xC7, 0x1D, 0xC8, 0xC9, 0x1E, 0xCA,
                                        0xCB, 0x1F, 0xCC, 0xCD, 0x20, 0xCE, 0xCF, 0x21 };

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   C0                         VEJ  (C0)\r\n";
            Assert.AreEqual(match, output, "VEJ  (C0)");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   C1 1A                      VCR  $1A\r\n";
            Assert.AreEqual(match, output, "VCR  $1A");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   C2                         VEJ  (C2)\r\n";
            Assert.AreEqual(match, output, "VEJ  (C2)");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   C3 1B                      VCS  $1B\r\n";
            Assert.AreEqual(match, output, "VCS  $1B");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   C4                         VEJ  (C4)\r\n";
            Assert.AreEqual(match, output, "VEJ  (C4)");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   C5 1C                      VHR  $1C\r\n";
            Assert.AreEqual(match, output, "VHR  $1C");

            CPU.Run((ushort)(address + 9), (ushort)(address + 9));
            output = CPU.DisDump();
            match = "1009   C6                         VEJ  (C6)\r\n";
            Assert.AreEqual(match, output, "VEJ  (C6)");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   C7 1D                      VHS  $1D\r\n";
            Assert.AreEqual(match, output, "VHS  $1D");

            CPU.Run((ushort)(address + 0xC), (ushort)(address + 0xC));
            output = CPU.DisDump();
            match = "100C   C8                         VEJ  (C8)\r\n";
            Assert.AreEqual(match, output, "VEJ  (C8)");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   C9 1E                      VZR  $1E\r\n";
            Assert.AreEqual(match, output, "VZR  $1E");

            CPU.Run((ushort)(address + 0x0F), (ushort)(address + 0x0F));
            output = CPU.DisDump();
            match = "100F   CA                         VEJ  (CA)\r\n";
            Assert.AreEqual(match, output, "VEJ  (CA)");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   CB 1F                      VZS  $1F\r\n";
            Assert.AreEqual(match, output, "VZS  $1F");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   CC                         VEJ  (CC)\r\n";
            Assert.AreEqual(match, output, "VEJ  (CC)");

            CPU.Run((ushort)(address + 0x13), (ushort)(address + 0x13));
            output = CPU.DisDump();
            match = "1013   CD 20                      VMJ  $20\r\n";
            Assert.AreEqual(match, output, "VMJ  $20");

            CPU.Run((ushort)(address + 0x15), (ushort)(address + 0x15));
            output = CPU.DisDump();
            match = "1015   CE                         VEJ  (CE)\r\n";
            Assert.AreEqual(match, output, "VEJ  (CE)");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   CF 21                      VVS  $21\r\n";
            Assert.AreEqual(match, output, "VVS  $21");
        }

        [TestMethod]
        public void Opcodes_Cn_1500()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xC0, 0xC1, 0x00, 0x01, 0x08, 0x11, 0xC2, 0x30,
                                        0x31, 0xC2, 0xF1, 0xAE, 0x07, 0xC3, 0x04, 0xAA,
                                        0xC4, 0x2C, 0x1A, 0xC4, 0xF1, 0x94, 0x2F, 0xC5,
                                        0x0E, 0xAA, 0x21, 0xC6, 0xC7, 0x10, 0x55, 0xC8,
                                        0x04, 0xC9, 0x2A, 0x55, 0xAA, 0xCA, 0x65, 0xCB,
                                        0x2C, 0x1A, 0xCC, 0x33, 0xCD, 0x34, 0x01, 0x20,
                                        0x10, 0x21, 0x20, 0xCE, 0x58, 0x7A, 0xCF, 0xF4,
                                        0x55, 0xAA};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = true;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   C0                         VEJ  (C0)\r\n";
            Assert.AreEqual(match, output, "VEJ  (C0)");

            CPU.Run((ushort)(address + 0x01), (ushort)(address + 0x01));
            output = CPU.DisDump();
            match = "1001   C1 00 01 08 11             VCR  ($00),$01,$08,$1017\r\n";
            Assert.AreEqual(match, output, "VCR  ($00),$01,$08,$1017");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   C2 30 31                   VEJ  (C2),$30,$103A\r\n";
            Assert.AreEqual(match, output, "VEJ  (C2),$30,$103A");

            CPU.Run((ushort)(address + 0x09), (ushort)(address + 0x09));
            output = CPU.DisDump();
            match = "1009   C2 F1 AE 07                VEJ  (C2),$F1AE,$1014\r\n";
            Assert.AreEqual(match, output, "VEJ  (C2),$F1AE,$07");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   C3 04 AA                   VCS  ($04),$10BA\r\n";
            Assert.AreEqual(match, output, "VCS  ($04),$10BA");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   C4 2C 1A                   VEJ  (C4),$2C,$102D\r\n";
            Assert.AreEqual(match, output, "VEJ  (C4),$2C,$102D");

            CPU.Run((ushort)(address + 0x13), (ushort)(address + 0x13));
            output = CPU.DisDump();
            match = "1013   C4 F1 94 2F                VEJ  (C4),$F194,$1046\r\n";
            Assert.AreEqual(match, output, "VEJ  (C4),$F194,$1046");

            CPU.Run((ushort)(address + 0x17), (ushort)(address + 0x17));
            output = CPU.DisDump();
            match = "1017   C5 0E AA 21                VHR  ($0E),$AA,$103C\r\n";
            Assert.AreEqual(match, output, "VHR  ($0E),$AA,$103C");

            CPU.Run((ushort)(address + 0x1B), (ushort)(address + 0x1B));
            output = CPU.DisDump();
            match = "101B   C6                         VEJ  (C6)\r\n";
            Assert.AreEqual(match, output, "VEJ  (C6)");

            CPU.Run((ushort)(address + 0x1C), (ushort)(address + 0x1C));
            output = CPU.DisDump();
            match = "101C   C7 10 55                   VHS  ($10),$55\r\n";
            Assert.AreEqual(match, output, "VHS  ($10),$55");

            CPU.Run((ushort)(address + 0x1F), (ushort)(address + 0x1F));
            output = CPU.DisDump();
            match = "101F   C8 04                      VEJ  (C8),$1025\r\n";
            Assert.AreEqual(match, output, "VEJ  (C8),$1025");

            CPU.Run((ushort)(address + 0x21), (ushort)(address + 0x21));
            output = CPU.DisDump();
            match = "1021   C9 2A 55 AA                VZR  ($2A),$55,$AA\r\n";
            Assert.AreEqual(match, output, "VZR  ($2A),$55,$AA");

            CPU.Run((ushort)(address + 0x25), (ushort)(address + 0x25));
            output = CPU.DisDump();
            match = "1025   CA 65                      VEJ  (CA),$7865\r\n";
            Assert.AreEqual(match, output, "VEJ  (CA),$7865");

            CPU.Run((ushort)(address + 0x27), (ushort)(address + 0x27));
            output = CPU.DisDump();
            match = "1027   CB 2C 1A                   VZS  ($2C),$781A\r\n";
            Assert.AreEqual(match, output, "VZS  ($2C),$781A");

            CPU.Run((ushort)(address + 0x2A), (ushort)(address + 0x2A));
            output = CPU.DisDump();
            match = "102A   CC 33                      VEJ  (CC),$7833\r\n";
            Assert.AreEqual(match, output, "VEJ  (CC),$7833");

            CPU.Run((ushort)(address + 0x2C), (ushort)(address + 0x2C));
            output = CPU.DisDump();
            match = "102C   CD 34 01 20 10 21 20       VMJ  ($34),$20,$1041\r\n";
            match += "                                         ,$21,$1053\r\n";
            match += "                                         \r\n";
            Assert.AreEqual(match, output, "VMJ ($34)");

            CPU.Run((ushort)(address + 0x33), (ushort)(address + 0x33));
            output = CPU.DisDump();
            match = "1033   CE 58 7A                   VEJ  (CE),$58,$10B0\r\n";
            Assert.AreEqual(match, output, "VEJ  (CE),$58,$10B0");

            CPU.Run((ushort)(address + 0x36), (ushort)(address + 0x36));
            output = CPU.DisDump();
            match = "1036   CF F4 55 AA                VVS  ($F4),$55AA\r\n";
            Assert.AreEqual(match, output, "VVS  ($F4),$55AA");
        }

        [TestMethod]
        public void Opcodes_Dn()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7,
                                        0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF };

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   D0                         VEJ  (D0)\r\n";
            Assert.AreEqual(match, output, "VEJ  (D0)");

            CPU.Run((ushort)(address + 0x01), (ushort)(address + 0x01));
            output = CPU.DisDump();
            match = "1001   D1                         ROR\r\n";
            Assert.AreEqual(match, output, "ROR");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   D2                         VEJ  (D2)\r\n";
            Assert.AreEqual(match, output, "VEJ  (D2)");

            CPU.Run((ushort)(address + 0x03), (ushort)(address + 0x03));
            output = CPU.DisDump();
            match = "1003   D3                         DDR  (X)\r\n";
            Assert.AreEqual(match, output, "DDR  (X)");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   D4                         VEJ  (D4)\r\n";
            Assert.AreEqual(match, output, "VEJ  (D4)");

            CPU.Run((ushort)(address + 0x05), (ushort)(address + 0x05));
            output = CPU.DisDump();
            match = "1005   D5                         SHR\r\n";
            Assert.AreEqual(match, output, "SHR");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   D6                         VEJ  (D6)\r\n";
            Assert.AreEqual(match, output, "VEJ  (D6)");

            CPU.Run((ushort)(address + 0x07), (ushort)(address + 0x07));
            output = CPU.DisDump();
            match = "1007   D7                         DRL  (X)\r\n";
            Assert.AreEqual(match, output, "DRL  (X)");

            CPU.Run((ushort)(address + 0x08), (ushort)(address + 0x08));
            output = CPU.DisDump();
            match = "1008   D8                         VEJ  (D8)\r\n";
            Assert.AreEqual(match, output, "VEJ  (D8)");

            CPU.Run((ushort)(address + 0x09), (ushort)(address + 0x09));
            output = CPU.DisDump();
            match = "1009   D9                         SHL\r\n";
            Assert.AreEqual(match, output, "SHL");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   DA                         VEJ  (DA)\r\n";
            Assert.AreEqual(match, output, "VEJ  (DA)");

            CPU.Run((ushort)(address + 0x0B), (ushort)(address + 0x0B));
            output = CPU.DisDump();
            match = "100B   DB                         ROL\r\n";
            Assert.AreEqual(match, output, "ROL");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   DC                         VEJ  (DC)\r\n";
            Assert.AreEqual(match, output, "VEJ  (DC)");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   DD                         INC  A\r\n";
            Assert.AreEqual(match, output, "INC  A");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   DE                         VEJ  (DE)\r\n";
            Assert.AreEqual(match, output, "VEJ  (DE)");

            CPU.Run((ushort)(address + 0x0F), (ushort)(address + 0x0F));
            output = CPU.DisDump();
            match = "100F   DF                         DEC  A\r\n";
            Assert.AreEqual(match, output, "DEC  A");
        }

        [TestMethod]
        public void Opcodes_Dn_1500()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xD0, 0x04, 0x69, 0xD1, 0xD2, 0x0C, 0x80, 0xD3,
                                        0xD4, 0xA0, 0xD5, 0xD6, 0xA6, 0xD7, 0xD8, 0xD9,
                                        0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0x5A, 0xDF };

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = true;   // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   D0 04 69                   VEJ  (D0),$04,$106C\r\n";
            Assert.AreEqual(match, output, "VEJ  (D0),$04,$106C");

            CPU.Run((ushort)(address + 0x03), (ushort)(address + 0x03));
            output = CPU.DisDump();
            match = "1003   D1                         ROR\r\n";
            Assert.AreEqual(match, output, "ROR");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   D2 0C 80                   VEJ  (D2),$0C,$80\r\n";
            Assert.AreEqual(match, output, "VEJ  (D2),$0C,$80");

            CPU.Run((ushort)(address + 0x07), (ushort)(address + 0x07));
            output = CPU.DisDump();
            match = "1007   D3                         DDR  (X)\r\n";
            Assert.AreEqual(match, output, "DDR  (X)");

            CPU.Run((ushort)(address + 0x08), (ushort)(address + 0x08));
            output = CPU.DisDump();
            match = "1008   D4 A0                      VEJ  (D4),$A0\r\n";
            Assert.AreEqual(match, output, "VEJ  (D4),$A0");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   D5                         SHR\r\n";
            Assert.AreEqual(match, output, "SHR");

            CPU.Run((ushort)(address + 0x0B), (ushort)(address + 0x0B));
            output = CPU.DisDump();
            match = "100B   D6 A6                      VEJ  (D6),$A6\r\n";
            Assert.AreEqual(match, output, "VEJ  (D6),$A6");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   D7                         DRL  (X)\r\n";
            Assert.AreEqual(match, output, "DRL  (X)");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   D8                         VEJ  (D8)\r\n";
            Assert.AreEqual(match, output, "VEJ  (D8)");

            CPU.Run((ushort)(address + 0x0F), (ushort)(address + 0x0F));
            output = CPU.DisDump();
            match = "100F   D9                         SHL\r\n";
            Assert.AreEqual(match, output, "SHL");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   DA                         VEJ  (DA)\r\n";
            Assert.AreEqual(match, output, "VEJ  (DA)");

            CPU.Run((ushort)(address + 0x11), (ushort)(address + 0x11));
            output = CPU.DisDump();
            match = "1011   DB                         ROL\r\n";
            Assert.AreEqual(match, output, "ROL");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   DC                         VEJ  (DC)\r\n";
            Assert.AreEqual(match, output, "VEJ  (DC)");

            CPU.Run((ushort)(address + 0x13), (ushort)(address + 0x13));
            output = CPU.DisDump();
            match = "1013   DD                         INC  A\r\n";
            Assert.AreEqual(match, output, "INC  A");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   DE 5A                      VEJ  (DE),$1070\r\n";
            Assert.AreEqual(match, output, "VEJ  (DE),$1070");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   DF                         DEC  A\r\n";
            Assert.AreEqual(match, output, "DEC  A");
        }

        [TestMethod]
        public void Opcodes_En()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE6, 0xE8, 0xE9,
                                        0x1A, 0x55, 0x2A, 0xEA, 0xEB, 0x1B, 0x55, 0x2B,
                                        0xEC, 0xED, 0x1C, 0x55, 0x2C, 0xEE, 0xEF, 0x1D,
                                        0x55, 0x2D};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   E0                         VEJ  (E0)\r\n";
            Assert.AreEqual(match, output, "VEJ  (E0)");

            CPU.Run((ushort)(address + 0x01), (ushort)(address + 0x01));
            output = CPU.DisDump();
            match = "1001   E1                         SPU\r\n";
            Assert.AreEqual(match, output, "SPU");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   E2                         VEJ  (E2)\r\n";
            Assert.AreEqual(match, output, "VEJ  (E2)");

            CPU.Run((ushort)(address + 0x03), (ushort)(address + 0x03));
            output = CPU.DisDump();
            match = "1003   E3                         RPU\r\n";
            Assert.AreEqual(match, output, "RPU");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   E4                         VEJ  (E4)\r\n";
            Assert.AreEqual(match, output, "VEJ  (E4)");

            CPU.Run((ushort)(address + 0x05), (ushort)(address + 0x05));
            output = CPU.DisDump();
            match = "1005   E6                         VEJ  (E6)\r\n";
            Assert.AreEqual(match, output, "VEJ  (E6)");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   E8                         VEJ  (E8)\r\n";
            Assert.AreEqual(match, output, "VEJ  (E8)");

            CPU.Run((ushort)(address + 0x07), (ushort)(address + 0x07));
            output = CPU.DisDump();
            match = "1007   E9 1A 55 2A                ANI  ($1A55),$2A\r\n";
            Assert.AreEqual(match, output, "ANI  ($1A55),$2A");

            CPU.Run((ushort)(address + 0x0B), (ushort)(address + 0x0B));
            output = CPU.DisDump();
            match = "100B   EA                         VEJ  (EA)\r\n";
            Assert.AreEqual(match, output, "VEJ  (EA)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   EB 1B 55 2B                ORI  ($1B55),$2B\r\n";
            Assert.AreEqual(match, output, "ORI  ($1B55),$2B");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   EC                         VEJ  (EC)\r\n";
            Assert.AreEqual(match, output, "VEJ  (EC)");

            CPU.Run((ushort)(address + 0x11), (ushort)(address + 0x11));
            output = CPU.DisDump();
            match = "1011   ED 1C 55 2C                BII  ($1C55),$2C\r\n";
            Assert.AreEqual(match, output, "BII  ($1C55),$2C");

            CPU.Run((ushort)(address + 0x15), (ushort)(address + 0x15));
            output = CPU.DisDump();
            match = "1015   EE                         VEJ  (EE)\r\n";
            Assert.AreEqual(match, output, "VEJ  (EE)");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   EF 1D 55 2D                ADI  ($1D55),$2D\r\n";
            Assert.AreEqual(match, output, "ADI  ($1D55),$2D");
        }

        [TestMethod]
        public void Opcodes_Fn()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xF0, 0xF1, 0xF2, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8,
                                        0xF9, 0xFA, 0xFB, 0xFC, 0xFE};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   F0                         VEJ  (F0)\r\n";
            Assert.AreEqual(match, output, "VEJ  (F0)");

            CPU.Run((ushort)(address + 1), (ushort)(address + 1));
            output = CPU.DisDump();
            match = "1001   F1                         AEX\r\n";
            Assert.AreEqual(match, output, "AEX");

            CPU.Run((ushort)(address + 2), (ushort)(address + 2));
            output = CPU.DisDump();
            match = "1002   F2                         VEJ  (F2)\r\n";
            Assert.AreEqual(match, output, "VEJ  (F2)");

            CPU.Run((ushort)(address + 3), (ushort)(address + 3));
            output = CPU.DisDump();
            match = "1003   F4                         VEJ  (F4)\r\n";
            Assert.AreEqual(match, output, "VEJ  (F4)");

            CPU.Run((ushort)(address + 4), (ushort)(address + 4));
            output = CPU.DisDump();
            match = "1004   F5                         TIN\r\n";
            Assert.AreEqual(match, output, "TIN");

            CPU.Run((ushort)(address + 5), (ushort)(address + 5));
            output = CPU.DisDump();
            match = "1005   F6                         VEJ  (F6)\r\n";
            Assert.AreEqual(match, output, "VEJ  (F6)");

            CPU.Run((ushort)(address + 6), (ushort)(address + 6));
            output = CPU.DisDump();
            match = "1006   F7                         CIN\r\n";
            Assert.AreEqual(match, output, "CIN");

            CPU.Run((ushort)(address + 7), (ushort)(address + 7));
            output = CPU.DisDump();
            match = "1007   F8                         VEJ  (F8)\r\n";
            Assert.AreEqual(match, output, "VEJ  (F8)");

            CPU.Run((ushort)(address + 8), (ushort)(address + 8));
            output = CPU.DisDump();
            match = "1008   F9                         REC\r\n";
            Assert.AreEqual(match, output, "REC");

            CPU.Run((ushort)(address + 9), (ushort)(address + 9));
            output = CPU.DisDump();
            match = "1009   FA                         VEJ  (FA)\r\n";
            Assert.AreEqual(match, output, "VEJ  (FA)");

            CPU.Run((ushort)(address + 0xA), (ushort)(address + 0xA));
            output = CPU.DisDump();
            match = "100A   FB                         SEC\r\n";
            Assert.AreEqual(match, output, "SEC");

            CPU.Run((ushort)(address + 0xB), (ushort)(address + 0xB));
            output = CPU.DisDump();
            match = "100B   FC                         VEJ  (FC)\r\n";
            Assert.AreEqual(match, output, "VEJ  (FC)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   FE                         VEJ  (FE)\r\n";
            Assert.AreEqual(match, output, "VEJ  (FE)");
        }

        [TestMethod]
        public void Opcodes_Fn_PC1500()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xF0, 0xF1, 0xF2, 0xF4, 0x78, 0x99, 0xF5, 0xF6,
                                        0x78, 0x86, 0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC,
                                        0xFE };

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = true;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   F0                         VEJ  (F0)\r\n";
            Assert.AreEqual(match, output, "VEJ  (F0)");

            CPU.Run((ushort)(address + 0x01), (ushort)(address + 0x01));
            output = CPU.DisDump();
            match = "1001   F1                         AEX\r\n";
            Assert.AreEqual(match, output, "AEX");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   F2                         VEJ  (F2)\r\n";
            Assert.AreEqual(match, output, "VEJ  (F2)");

            CPU.Run((ushort)(address + 0x03), (ushort)(address + 0x03));
            output = CPU.DisDump();
            match = "1003   F4 78 99                   VEJ  (F4),$7899\r\n";
            Assert.AreEqual(match, output, "VEJ  (F4),$7899");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   F5                         TIN\r\n";
            Assert.AreEqual(match, output, "TIN");

            CPU.Run((ushort)(address + 0x07), (ushort)(address + 0x07));
            output = CPU.DisDump();
            match = "1007   F6 78 86                   VEJ  (F6),$7886\r\n";
            Assert.AreEqual(match, output, "VEJ  (F6),7886");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   F7                         CIN\r\n";
            Assert.AreEqual(match, output, "CIN");

            CPU.Run((ushort)(address + 0x0B), (ushort)(address + 0x0B));
            output = CPU.DisDump();
            match = "100B   F8                         VEJ  (F8)\r\n";
            Assert.AreEqual(match, output, "VEJ  (F8)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   F9                         REC\r\n";
            Assert.AreEqual(match, output, "REC");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   FA                         VEJ  (FA)\r\n";
            Assert.AreEqual(match, output, "VEJ  (FA)");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   FB                         SEC\r\n";
            Assert.AreEqual(match, output, "SEC");

            CPU.Run((ushort)(address + 0x0F), (ushort)(address + 0x0F));
            output = CPU.DisDump();
            match = "100F   FC                         VEJ  (FC)\r\n";
            Assert.AreEqual(match, output, "VEJ  (FC)");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   FE                         VEJ  (FE)\r\n";
            Assert.AreEqual(match, output, "VEJ  (FE)");
        }
    }

    [TestClass]
    public class UnitTest_P2
    {
        lh5801_Dis.lh5801_Dis CPU = new lh5801_Dis.lh5801_Dis();

        [TestMethod]
        public void Opcodes_FD0n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xFD, 0x01, 0xFD, 0x03, 0xFD, 0x05, 0xFD, 0x07,
                                        0xFD, 0x08, 0xFD, 0x09, 0xFD, 0x0A, 0xFD, 0x0B,
                                        0xFD, 0x0C, 0xFD, 0x0D, 0xFD, 0x0E, 0xFD, 0x0F};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD 01                      SBC  #(X)\r\n";
            Assert.AreEqual(match, output, "SBC  #(X)");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD 03                      ADC  #(X)\r\n";
            Assert.AreEqual(match, output, "#(X)");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD 05                      LDA  #(X)\r\n";
            Assert.AreEqual(match, output, "LDA  #(X)");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   FD 07                      CPA  #(X)\r\n";
            Assert.AreEqual(match, output, "CPA  #(X)");

            CPU.Run((ushort)(address + 0x08), (ushort)(address + 0x08));
            output = CPU.DisDump();
            match = "1008   FD 08                      LDX  X\r\n";
            Assert.AreEqual(match, output, "LDX  X");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   FD 09                      AND  #(X)\r\n";
            Assert.AreEqual(match, output, "AND  #(X)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   FD 0A                      POP  X\r\n";
            Assert.AreEqual(match, output, "POP  X");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   FD 0B                      ORA  #(X)\r\n";
            Assert.AreEqual(match, output, "ORA  #(X)");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   FD 0C                      DCS  #(X)\r\n";
            Assert.AreEqual(match, output, "DCS  #(X)");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   FD 0D                      EOR  #(X)\r\n";
            Assert.AreEqual(match, output, "EOR  #(X)");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   FD 0E                      STA  #(X)\r\n";
            Assert.AreEqual(match, output, "STA  #(X)");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   FD 0F                      BIT  #(X)\r\n";
            Assert.AreEqual(match, output, "BIT  #(X)");
        }

        [TestMethod]
        public void Opcodes_FD1n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xFD, 0x11, 0xFD, 0x13, 0xFD, 0x15, 0xFD, 0x17,
                                        0xFD, 0x18, 0xFD, 0x19, 0xFD, 0x1A, 0xFD, 0x1B,
                                        0xFD, 0x1C, 0xFD, 0x1D, 0xFD, 0x1E, 0xFD, 0x1F};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD 11                      SBC  #(Y)\r\n";
            Assert.AreEqual(match, output, "SBC  #(Y)");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD 13                      ADC  #(Y)\r\n";
            Assert.AreEqual(match, output, "#(Y)");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD 15                      LDA  #(Y)\r\n";
            Assert.AreEqual(match, output, "LDA  #(Y)");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   FD 17                      CPA  #(Y)\r\n";
            Assert.AreEqual(match, output, "CPA  #(Y)");

            CPU.Run((ushort)(address + 0x08), (ushort)(address + 0x08));
            output = CPU.DisDump();
            match = "1008   FD 18                      LDX  Y\r\n";
            Assert.AreEqual(match, output, "LDX  Y");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   FD 19                      AND  #(Y)\r\n";
            Assert.AreEqual(match, output, "AND  #(Y)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   FD 1A                      POP  Y\r\n";
            Assert.AreEqual(match, output, "POP  Y");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   FD 1B                      ORA  #(Y)\r\n";
            Assert.AreEqual(match, output, "ORA  #(Y)");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   FD 1C                      DCS  #(Y)\r\n";
            Assert.AreEqual(match, output, "DCS  #(Y)");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   FD 1D                      EOR  #(Y)\r\n";
            Assert.AreEqual(match, output, "EOR  #(Y)");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   FD 1E                      STA  #(Y)\r\n";
            Assert.AreEqual(match, output, "STA  #(Y)");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   FD 1F                      BIT  #(Y)\r\n";
            Assert.AreEqual(match, output, "BIT  #(Y)");
        }

        [TestMethod]
        public void Opcodes_FD2n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xFD, 0x21, 0xFD, 0x23, 0xFD, 0x25, 0xFD, 0x27,
                                        0xFD, 0x28, 0xFD, 0x29, 0xFD, 0x2A, 0xFD, 0x2B,
                                        0xFD, 0x2C, 0xFD, 0x2D, 0xFD, 0x2E, 0xFD, 0x2F};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD 21                      SBC  #(U)\r\n";
            Assert.AreEqual(match, output, "SBC  #(U)");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD 23                      ADC  #(U)\r\n";
            Assert.AreEqual(match, output, "#(U)");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD 25                      LDA  #(U)\r\n";
            Assert.AreEqual(match, output, "LDA  #(U)");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   FD 27                      CPA  #(U)\r\n";
            Assert.AreEqual(match, output, "CPA  #(U)");

            CPU.Run((ushort)(address + 0x08), (ushort)(address + 0x08));
            output = CPU.DisDump();
            match = "1008   FD 28                      LDX  U\r\n";
            Assert.AreEqual(match, output, "LDX  U");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   FD 29                      AND  #(U)\r\n";
            Assert.AreEqual(match, output, "AND  #(U)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   FD 2A                      POP  U\r\n";
            Assert.AreEqual(match, output, "POP  U");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   FD 2B                      ORA  #(U)\r\n";
            Assert.AreEqual(match, output, "ORA  #(U)");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   FD 2C                      DCS  #(U)\r\n";
            Assert.AreEqual(match, output, "DCS  #(U)");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   FD 2D                      EOR  #(U)\r\n";
            Assert.AreEqual(match, output, "EOR  #(U)");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   FD 2E                      STA  #(U)\r\n";
            Assert.AreEqual(match, output, "STA  #(U)");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   FD 2F                      BIT  #(U)\r\n";
            Assert.AreEqual(match, output, "BIT  #(U)");
        }

        [TestMethod]
        public void Opcodes_FD3n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xFD, 0x31, 0xFD, 0x33, 0xFD, 0x35, 0xFD, 0x37,
                                        0xFD, 0x38, 0xFD, 0x39, 0xFD, 0x3A, 0xFD, 0x3B,
                                        0xFD, 0x3C, 0xFD, 0x3D, 0xFD, 0x3E, 0xFD, 0x3F};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD 31                      SBC  #(V)\r\n";
            Assert.AreEqual(match, output, "SBC  #(V)");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD 33                      ADC  #(V)\r\n";
            Assert.AreEqual(match, output, "#(V)");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD 35                      LDA  #(V)\r\n";
            Assert.AreEqual(match, output, "LDA  #(V)");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   FD 37                      CPA  #(V)\r\n";
            Assert.AreEqual(match, output, "CPA  #(V)");

            CPU.Run((ushort)(address + 0x08), (ushort)(address + 0x08));
            output = CPU.DisDump();
            match = "1008   FD 38                      LDX  V\r\n";
            Assert.AreEqual(match, output, "LDX  V");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   FD 39                      AND  #(V)\r\n";
            Assert.AreEqual(match, output, "AND  #(V)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   FD 3A                      POP  V\r\n";
            Assert.AreEqual(match, output, "POP  V");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   FD 3B                      ORA  #(V)\r\n";
            Assert.AreEqual(match, output, "ORA  #(V)");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   FD 3C                      DCS  #(V)\r\n";
            Assert.AreEqual(match, output, "DCS  #(V)");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   FD 3D                      EOR  #(V)\r\n";
            Assert.AreEqual(match, output, "EOR  #(V)");

            CPU.Run((ushort)(address + 0x14), (ushort)(address + 0x14));
            output = CPU.DisDump();
            match = "1014   FD 3E                      STA  #(V)\r\n";
            Assert.AreEqual(match, output, "STA  #(V)");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   FD 3F                      BIT  #(V)\r\n";
            Assert.AreEqual(match, output, "BIT  #(V)");
        }

        [TestMethod]
        public void Opcodes_FD4n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xFD, 0x40, 0xFD, 0x42, 0xFD, 0x48, 0xFD, 0x49,
                                        0x1A, 0xFD, 0x4A, 0xFD, 0x4B, 0x1B, 0xFD, 0x4C,
                                        0xFD, 0x4D, 0x1C, 0xFD, 0x4E, 0xFD, 0x4F, 0x1D};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD 40                      INC  XH\r\n";
            Assert.AreEqual(match, output, "INC  XH");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD 42                      DEC  XH\r\n";
            Assert.AreEqual(match, output, "DEC  XH");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD 48                      LDX  S\r\n";
            Assert.AreEqual(match, output, "LDX  S");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   FD 49 1A                   ANI  #(X),$1A\r\n";
            Assert.AreEqual(match, output, "ANI  #(X),$1A");

            CPU.Run((ushort)(address + 0x09), (ushort)(address + 0x09));
            output = CPU.DisDump();
            match = "1009   FD 4A                      STX  X\r\n";
            Assert.AreEqual(match, output, "STX  X");

            CPU.Run((ushort)(address + 0x0B), (ushort)(address + 0x0B));
            output = CPU.DisDump();
            match = "100B   FD 4B 1B                   ORI  #(X),$1B\r\n";
            Assert.AreEqual(match, output, "ORI  #(X),$1B");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   FD 4C                      OFF\r\n";
            Assert.AreEqual(match, output, "OFF");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   FD 4D 1C                   BII  #(X),$1C\r\n";
            Assert.AreEqual(match, output, "BII  #(X),$1C");

            CPU.Run((ushort)(address + 0x13), (ushort)(address + 0x13));
            output = CPU.DisDump();
            match = "1013   FD 4E                      STX  S\r\n";
            Assert.AreEqual(match, output, "STX  S");

            CPU.Run((ushort)(address + 0x15), (ushort)(address + 0x15));
            output = CPU.DisDump();
            match = "1015   FD 4F 1D                   ADI  #(X),$1D\r\n";
            Assert.AreEqual(match, output, "ADI  #(X),$1D");
        }

        [TestMethod]
        public void Opcodes_FD5n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xFD, 0x50, 0xFD, 0x52, 0xFD, 0x58, 0xFD, 0x59,
                                        0x1A, 0xFD, 0x5A, 0xFD, 0x5B, 0x1B, 0xFD, 0x5D,
                                        0x1C, 0xFD, 0x5E, 0xFD, 0x5F, 0x1D};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD 50                      INC  YH\r\n";
            Assert.AreEqual(match, output, "INC  YH");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD 52                      DEC  YH\r\n";
            Assert.AreEqual(match, output, "DEC  YH");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD 58                      LDX  P\r\n";
            Assert.AreEqual(match, output, "LDX  P");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   FD 59 1A                   ANI  #(Y),$1A\r\n";
            Assert.AreEqual(match, output, "ANI  #(Y),$1A");

            CPU.Run((ushort)(address + 0x09), (ushort)(address + 0x09));
            output = CPU.DisDump();
            match = "1009   FD 5A                      STX  Y\r\n";
            Assert.AreEqual(match, output, "STX  Y");

            CPU.Run((ushort)(address + 0x0B), (ushort)(address + 0x0B));
            output = CPU.DisDump();
            match = "100B   FD 5B 1B                   ORI  #(Y),$1B\r\n";
            Assert.AreEqual(match, output, "ORI  #(Y),$1B");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   FD 5D 1C                   BII  #(Y),$1C\r\n";
            Assert.AreEqual(match, output, "BII  #(Y),$1C");

            CPU.Run((ushort)(address + 0x11), (ushort)(address + 0x11));
            output = CPU.DisDump();
            match = "1011   FD 5E                      STX  P\r\n";
            Assert.AreEqual(match, output, "STX  P");

            CPU.Run((ushort)(address + 0x13), (ushort)(address + 0x13));
            output = CPU.DisDump();
            match = "1013   FD 5F 1D                   ADI  #(Y),$1D\r\n";
            Assert.AreEqual(match, output, "ADI  #(Y),$1D");
        }

        [TestMethod]
        public void Opcodes_FD6n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xFD, 0x60, 0xFD, 0x62, 0xFD, 0x69, 0x1A, 0xFD,
                                        0x6B, 0x1B, 0xFD, 0x6D, 0x1C, 0xFD, 0x6F, 0x1D};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD 60                      INC  UH\r\n";
            Assert.AreEqual(match, output, "INC  UH");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD 62                      DEC  UH\r\n";
            Assert.AreEqual(match, output, "DEC  UH");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD 69 1A                   ANI  #(U),$1A\r\n";
            Assert.AreEqual(match, output, "ANI  #(U),$1A");

            CPU.Run((ushort)(address + 0x07), (ushort)(address + 0x07));
            output = CPU.DisDump();
            match = "1007   FD 6B 1B                   ORI  #(U),$1B\r\n";
            Assert.AreEqual(match, output, "ORI  #(U),$1B");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   FD 6D 1C                   BII  #(U),$1C\r\n";
            Assert.AreEqual(match, output, "BII  #(U),$1C");

            CPU.Run((ushort)(address + 0x0D), (ushort)(address + 0x0D));
            output = CPU.DisDump();
            match = "100D   FD 6F 1D                   ADI  #(U),$1D\r\n";
            Assert.AreEqual(match, output, "ADI  #(U),$1D");
        }

        [TestMethod]
        public void Opcodes_FD7n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xFD, 0x70, 0xFD, 0x72, 0xFD, 0x79, 0x1A, 0xFD,
                                        0x7A, 0xFD, 0x7B, 0x1B, 0xFD, 0x7D, 0x1C, 0xFD,
                                        0x7F, 0x1D};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD 70                      INC  VH\r\n";
            Assert.AreEqual(match, output, "INC  VH");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD 72                      DEC  VH\r\n";
            Assert.AreEqual(match, output, "DEC  VH");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD 79 1A                   ANI  #(V),$1A\r\n";
            Assert.AreEqual(match, output, "ANI  #(V),$1A");

            CPU.Run((ushort)(address + 0x07), (ushort)(address + 0x07));
            output = CPU.DisDump();
            match = "1007   FD 7A                      STX  V\r\n";
            Assert.AreEqual(match, output, "STX  V");

            CPU.Run((ushort)(address + 0x09), (ushort)(address + 0x09));
            output = CPU.DisDump();
            match = "1009   FD 7B 1B                   ORI  #(V),$1B\r\n";
            Assert.AreEqual(match, output, "ORI  #(V),$1B");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   FD 7D 1C                   BII  #(V),$1C\r\n";
            Assert.AreEqual(match, output, "BII  #(V),$1C");

            CPU.Run((ushort)(address + 0x0F), (ushort)(address + 0x0F));
            output = CPU.DisDump();
            match = "100F   FD 7F 1D                   ADI  #(V),$1D\r\n";
            Assert.AreEqual(match, output, "ADI  #(V),$1D");
        }

        [TestMethod]
        public void Opcodes_FD8n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xFD, 0x81, 0xFD, 0x88, 0xFD, 0x8A, 0xFD, 0x8C,
                                        0xFD, 0x8E};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD 81                      SIE\r\n";
            Assert.AreEqual(match, output, "SIE");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD 88                      PSH  X\r\n";
            Assert.AreEqual(match, output, "PSH  X");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD 8A                      POP  A\r\n";
            Assert.AreEqual(match, output, "POP  A");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   FD 8C                      DCA  #(X)\r\n";
            Assert.AreEqual(match, output, "DCA  #(X)");

            CPU.Run((ushort)(address + 0x08), (ushort)(address + 0x08));
            output = CPU.DisDump();
            match = "1008   FD 8E                      CDV\r\n";
            Assert.AreEqual(match, output, "CDV");

        }

        [TestMethod]
        public void Opcodes_FD9n()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xFD, 0x98, 0xFD, 0x9C};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD 98                      PSH  Y\r\n";
            Assert.AreEqual(match, output, "PSH  Y");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD 9C                      DCA  #(Y)\r\n";
            Assert.AreEqual(match, output, "DCA  #(Y)");
        }

        [TestMethod]
        public void Opcodes_FDAn()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] {0xFD, 0xA1, 0x1A, 0x55, 0xFD, 0xA3, 0x1B, 0x55,
                                        0xFD, 0xA5, 0x1C, 0x55, 0xFD, 0xA7, 0x1D, 0x55,
                                        0xFD, 0xA8, 0xFD, 0xA9, 0x1E, 0x55, 0xFD, 0xAA,
                                        0xFD, 0xAB, 0x1F, 0x55, 0xFD, 0xAC, 0xFD, 0xAD,
                                        0x2A, 0x55, 0xFD, 0xAE, 0x2B, 0x55, 0xFD, 0xAF,
                                        0x2C, 0x55};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD A1 1A 55                SBC  #($1A55)\r\n";
            Assert.AreEqual(match, output, "SBC  #($1A55)");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD A3 1B 55                ADC  #($1B55)\r\n";
            Assert.AreEqual(match, output, "ADC  #($1B55)");

            CPU.Run((ushort)(address + 0x08), (ushort)(address + 0x08));
            output = CPU.DisDump();
            match = "1008   FD A5 1C 55                LDA  #($1C55)\r\n";
            Assert.AreEqual(match, output, "LDA  #($1C55)");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   FD A7 1D 55                CPA  #($1D55)\r\n";
            Assert.AreEqual(match, output, "CPA  #($1D55)");

            CPU.Run((ushort)(address + 0x10), (ushort)(address + 0x10));
            output = CPU.DisDump();
            match = "1010   FD A8                      PSH  U\r\n";
            Assert.AreEqual(match, output, "PSH  U");

            CPU.Run((ushort)(address + 0x12), (ushort)(address + 0x12));
            output = CPU.DisDump();
            match = "1012   FD A9 1E 55                AND  #($1E55)\r\n";
            Assert.AreEqual(match, output, "AND  #($1E55)");

            CPU.Run((ushort)(address + 0x16), (ushort)(address + 0x16));
            output = CPU.DisDump();
            match = "1016   FD AA                      TTA\r\n";
            Assert.AreEqual(match, output, "TTA");

            CPU.Run((ushort)(address + 0x18), (ushort)(address + 0x18));
            output = CPU.DisDump();
            match = "1018   FD AB 1F 55                ORA  #($1F55)\r\n";
            Assert.AreEqual(match, output, "ORA  #($1F55)");

            CPU.Run((ushort)(address + 0x1C), (ushort)(address + 0x1C));
            output = CPU.DisDump();
            match = "101C   FD AC                      DCA  #(U)\r\n";
            Assert.AreEqual(match, output, "DCA  #(U)");

            CPU.Run((ushort)(address + 0x1E), (ushort)(address + 0x1E));
            output = CPU.DisDump();
            match = "101E   FD AD 2A 55                EOR  #($2A55)\r\n";
            Assert.AreEqual(match, output, "EOR  #($2A55)");

            CPU.Run((ushort)(address + 0x22), (ushort)(address + 0x22));
            output = CPU.DisDump();
            match = "1022   FD AE 2B 55                STA  #($2B55)\r\n";
            Assert.AreEqual(match, output, "STA  #($2B55)");

            CPU.Run((ushort)(address + 0x26), (ushort)(address + 0x26));
            output = CPU.DisDump();
            match = "1026   FD AF 2C 55                BIT  #($2C55)\r\n";
            Assert.AreEqual(match, output, "BIT  #($2C55)");
        }

        [TestMethod]
        public void Opcodes_FDBn()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] { 0xFD, 0xB1, 0xFD, 0xB8, 0xFD, 0xBA, 0xFD, 0xBC,
                                         0xFD, 0xBE};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD B1                      HLT\r\n";
            Assert.AreEqual(match, output, "HLT");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD B8                      PSH  V\r\n";
            Assert.AreEqual(match, output, "PSH  V");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD BA                      ITA\r\n";
            Assert.AreEqual(match, output, "ITA");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   FD BC                      DCA  #(V)\r\n";
            Assert.AreEqual(match, output, "DCA  #(V)");

            CPU.Run((ushort)(address + 0x08), (ushort)(address + 0x08));
            output = CPU.DisDump();
            match = "1008   FD BE                      RIE\r\n";
            Assert.AreEqual(match, output, "RIE");
        }

        [TestMethod]
        public void Opcodes_FDCn()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] { 0xFD, 0xC0, 0xFD, 0xC1, 0xFD, 0xC8, 0xFD, 0xCA,
                                         0xFD, 0xCC, 0xFD, 0xCE};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD C0                      RDP\r\n";
            Assert.AreEqual(match, output, "RDP");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD C1                      SDP\r\n";
            Assert.AreEqual(match, output, "SDP");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD C8                      PSH  A\r\n";
            Assert.AreEqual(match, output, "ITA");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   FD CA                      ADR  X\r\n";
            Assert.AreEqual(match, output, "ADR  X");

            CPU.Run((ushort)(address + 0x08), (ushort)(address + 0x08));
            output = CPU.DisDump();
            match = "1008   FD CC                      ATP\r\n";
            Assert.AreEqual(match, output, "ATP");

            CPU.Run((ushort)(address + 0x0A), (ushort)(address + 0x0A));
            output = CPU.DisDump();
            match = "100A   FD CE                      AM0\r\n";
            Assert.AreEqual(match, output, "AM0");
        }

        [TestMethod]
        public void Opcodes_FDDn()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] { 0xFD, 0xD3, 0xFD, 0xD7, 0xFD, 0xDA, 0xFD, 0xDE };

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD D3                      DRR  #(X)\r\n";
            Assert.AreEqual(match, output, "DRR  #(X)");

            CPU.Run((ushort)(address + 0x02), (ushort)(address + 0x02));
            output = CPU.DisDump();
            match = "1002   FD D7                      DRL  #(X)\r\n";
            Assert.AreEqual(match, output, "DRL #(X)");

            CPU.Run((ushort)(address + 0x04), (ushort)(address + 0x04));
            output = CPU.DisDump();
            match = "1004   FD DA                      ADR  Y\r\n";
            Assert.AreEqual(match, output, "ADR  Y");

            CPU.Run((ushort)(address + 0x06), (ushort)(address + 0x06));
            output = CPU.DisDump();
            match = "1006   FD DE                      AM1\r\n";
            Assert.AreEqual(match, output, "AM1");
        }

        [TestMethod]
        public void Opcodes_FDEn()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] { 0xFD, 0xE9, 0x1A, 0x55, 0xAA, 0xFD, 0xEA, 0xFD,
                                         0xEB, 0x2A, 0x55, 0xAA, 0xFD, 0xEC, 0xFD, 0xED,
                                         0x3A, 0x55, 0xAA, 0xFD, 0xEF, 0x4A, 0x55, 0xAA};

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD E9 1A 55 AA             ANI  #($1A55),$AA\r\n";
            Assert.AreEqual(match, output, "ANI  #($1A55),$AA");

            CPU.Run((ushort)(address + 0x05), (ushort)(address + 0x05));
            output = CPU.DisDump();
            match = "1005   FD EA                      ADR  U\r\n";
            Assert.AreEqual(match, output, "ADR  U");

            CPU.Run((ushort)(address + 0x07), (ushort)(address + 0x07));
            output = CPU.DisDump();
            match = "1007   FD EB 2A 55 AA             ORI  #($2A55),$AA\r\n";
            Assert.AreEqual(match, output, "ORI  #($2A55),$AA");

            CPU.Run((ushort)(address + 0x0C), (ushort)(address + 0x0C));
            output = CPU.DisDump();
            match = "100C   FD EC                      ATT\r\n";
            Assert.AreEqual(match, output, "ATT");

            CPU.Run((ushort)(address + 0x0E), (ushort)(address + 0x0E));
            output = CPU.DisDump();
            match = "100E   FD ED 3A 55 AA             BII  #($3A55),$AA\r\n";
            Assert.AreEqual(match, output, "BII  #($3A55),$AA");

            CPU.Run((ushort)(address + 0x13), (ushort)(address + 0x13));
            output = CPU.DisDump();
            match = "1013   FD EF 4A 55 AA             ADI  #($4A55),$AA\r\n";
            Assert.AreEqual(match, output, "ADI  #($4A55),$AA");
        }

        [TestMethod]
        public void Opcodes_FDFn()
        {
            ushort address = 0x1000;
            byte[] memVal = new byte[] { 0xFD, 0xFA };

            CPU.addressLabels = false;  // don't use address lables
            CPU.disModePC1500 = false;  // don't use PC-1500 disassembly mode
            CPU.libFileEnable = false;  // don't use lib file / segments
            CPU.WriteRAM_ME0(address, memVal);

            CPU.Run(address, (ushort)(address));
            String output = CPU.DisDump();
            String match = "1000   FD FA                      ADR  V\r\n";
            Assert.AreEqual(match, output, "ADR  V");
        }

    }

}
