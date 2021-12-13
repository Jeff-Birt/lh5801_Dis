using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace lh5801_Dis
{

    public class lh5801_Dis
    {
        /// <summary>
        /// SEGMENTS describe the makeup of a section of binary, i.e. @ $1000
        /// there might be CODE, while at $1500 we have data table of BYTEs.
        /// Knowing the type of data lets the dissasembler decode it properly.
        /// </summary>
        enum SEGMENT
        {
            BYTE,
            WORD,
            CODE,
            TEXT,
            SKIP
        }

        #region evil_globals

        private SEGMENT SegmentMode = SEGMENT.CODE; // default to CODE mode
        private ushort SegmentEnd = 0x0000;         // last address of current SegmentMode
        Dictionary<ushort, SEGMENT> SegmentDict;    // Holds address/FRAG type pairs
        Dictionary<ushort, string> CommentDict;     // Holds address/comment pairs

        public bool listFormat = true;      // output in list format "Address hex_val disassembled" 
        public bool disModePC1500 = true;   // disassemble VEJ commands as used on PC-1500
        public bool addressLabels = true;   // display address labels
        public bool libFileEnable = true;   // enable using 'LIB' file to describe binary structure

        public uint tick = 0;   // number of processor clock cyclces
        public ushort P = 0;    // Program Counter
        public byte[] RAM_ME0 = new byte[0xFFFF + 0x01];
        public byte[] RAM_ME1 = new byte[0xFFFF + 0x01];

        private byte[] opBytesP1; // Index of opcode to #bytes used for Page 1
        private byte[] opBytesP2; // Index of opcode to #bytes used for Page 2
        private byte[] vecArgs;   // Index to #arg bytes for $FF page vectors 

        List<Delegate> delegatesTbl1 = new List<Delegate>();    // delegates (function pointers) for page 1 opcodes
        List<Delegate> delegatesTbl2 = new List<Delegate>();    // delegates (function pointers) for page 2 opcodes
        List<Delegate> delegatesVect = new List<Delegate>();    // delegates (function pointers) for vector (FF page) handlers
        StringBuilder disSB = new StringBuilder();              // evil global string builder used in disassembly
        Dictionary<ushort, string> lableDict;                   // holds address, lable text pairs used in disassembly

        System.Text.RegularExpressions.Regex isLabel     = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_-]*$");
        System.Text.RegularExpressions.Regex isDirective = new System.Text.RegularExpressions.Regex("^.[a-zA-Z0-9_-]*$");
        System.Text.RegularExpressions.Regex isHexVal    = new System.Text.RegularExpressions.Regex("^0[xX]?|[$]?[0-9a-fA-F]+[H]?$");
        System.Text.RegularExpressions.Regex isEqu       = new System.Text.RegularExpressions.Regex("^[.]?equ$|^=$");
        System.Text.RegularExpressions.Regex isComment   = new System.Text.RegularExpressions.Regex("^[;]");

        #endregion evil_globals

        /// <summary>
        /// The constructonator
        /// </summary>
        public lh5801_Dis()
        {
            Reset();
            ConfigDelegates();
            ConfigOpcodeBytesTables();
            ReadLibFile();
        }

        #region Configuration 

        /// <summary>
        /// Reset all Flags and Registers
        /// </summary>
        public void Reset (bool resetRAM = false)
        {
            tick = 0;

            if (resetRAM) { ResetRAM(); }
        }

        /// <summary>
        /// Initialize ME0 and ME1 RAM to all zeros
        /// </summary>
        private void ResetRAM()
        {
            Array.Clear(RAM_ME0, 0, RAM_ME0.Length);
            Array.Clear(RAM_ME1, 0, RAM_ME0.Length);
        }

        /// <summary>
        /// Configure Delegate List (function pointers)
        /// </summary>
        private void ConfigDelegates()
        {
            #region Opcode Table 1 0x00-0xFF

            #region Opcodes_0x00-0x0F
            delegatesTbl1.Add((Action)SBC_XL);      // Opcode 0x00, SBC XL
            delegatesTbl1.Add((Action)SBC_X_ME0);   // Opcode 0x01, SBC (X)
            delegatesTbl1.Add((Action)ADC_XL);      // Opcode 0x02, ADC XL
            delegatesTbl1.Add((Action)ADC_X_ME0);   // Opcode 0x03, ADC (X)
            delegatesTbl1.Add((Action)LDA_XL);      // Opcode 0x04, LDA XL
            delegatesTbl1.Add((Action)LDA_X_ME0);   // Opcode 0x05, LDA (X)
            delegatesTbl1.Add((Action)CPA_XL);      // Opcode 0x06, CPA XL
            delegatesTbl1.Add((Action)CPA_X_ME0);   // Opcode 0x07, CPA (X)
            delegatesTbl1.Add((Action)STA_XH);      // Opcode 0x08, STA XH
            delegatesTbl1.Add((Action)AND_X_ME0);   // Opcode 0x09
            delegatesTbl1.Add((Action)STA_XL);      // Opcode 0x0A, STA XL
            delegatesTbl1.Add((Action)ORA_X_ME0);   // Opcode 0x0B, ORA (X)
            delegatesTbl1.Add((Action)DCS_X_ME0);   // Opcode 0x0C, DCS (X)
            delegatesTbl1.Add((Action)EOR_X_ME0);   // Opcode 0x0D, EOR (X)
            delegatesTbl1.Add((Action)STA_X_ME0);   // Opcode 0x0E, STA (X)
            delegatesTbl1.Add((Action)BIT_X_ME0);   // Opcode 0x0F, BIT (X)
            #endregion Opcodes_0x00-0x0F

            #region Opcodes_0x10-0x1F
            delegatesTbl1.Add((Action)SBC_YL);      // Opcode 0x10, SBC YL
            delegatesTbl1.Add((Action)SBC_Y_ME0);   // Opcode 0x11, SBC (Y)
            delegatesTbl1.Add((Action)ADC_YL);      // Opcode 0x12, ADC YL
            delegatesTbl1.Add((Action)ADC_Y_ME0);   // Opcode 0x13, ADC (Y)
            delegatesTbl1.Add((Action)LDA_YL);      // Opcode 0x14, LDA YL
            delegatesTbl1.Add((Action)LDA_Y_ME0);   // Opcode 0x15, LDA (Y)
            delegatesTbl1.Add((Action)CPA_YL);      // Opcode 0x16, CPA YL
            delegatesTbl1.Add((Action)CPA_Y_ME0);   // Opcode 0x17, CPA (Y)
            delegatesTbl1.Add((Action)STA_YH);      // Opcode 0x18, STA YH
            delegatesTbl1.Add((Action)AND_Y_ME0);   // Opcode 0x19
            delegatesTbl1.Add((Action)STA_YL);      // Opcode 0x1A, STA YL
            delegatesTbl1.Add((Action)ORA_Y_ME0);   // Opcode 0x1B, ORA (Y)
            delegatesTbl1.Add((Action)DCS_Y_ME0);   // Opcode 0x1C, DCS (Y)
            delegatesTbl1.Add((Action)EOR_Y_ME0);   // Opcode 0x1D, EOR (Y)
            delegatesTbl1.Add((Action)STA_Y_ME0);   // Opcode 0x1E, STA (Y)
            delegatesTbl1.Add((Action)BIT_Y_ME0);   // Opcode 0x1F, BIT (Y)
            #endregion Opcodes_0x10-0x1F

            #region Opcodes_0x20-0x2F
            delegatesTbl1.Add((Action)SBC_UL);      // Opcode 0x20, SBC UL
            delegatesTbl1.Add((Action)SBC_U_ME0);   // Opcode 0x21, SBC (U)
            delegatesTbl1.Add((Action)ADC_UL);      // Opcode 0x22, ADC UL
            delegatesTbl1.Add((Action)ADC_U_ME0);   // Opcode 0x23, ADC (U)
            delegatesTbl1.Add((Action)LDA_UL);      // Opcode 0x24, LDA UL
            delegatesTbl1.Add((Action)LDA_U_ME0);   // Opcode 0x25, LDA (U)
            delegatesTbl1.Add((Action)CPA_UL);      // Opcode 0x26, CPA UL
            delegatesTbl1.Add((Action)CPA_U_ME0);   // Opcode 0x27, CPA (U)
            delegatesTbl1.Add((Action)STA_UH);      // Opcode 0x28, STA UH
            delegatesTbl1.Add((Action)AND_U_ME0);   // Opcode 0x29
            delegatesTbl1.Add((Action)STA_UL);      // Opcode 0x2A, STA UL
            delegatesTbl1.Add((Action)ORA_U_ME0);   // Opcode 0x2B, ORA (U)
            delegatesTbl1.Add((Action)DCS_U_ME0);   // Opcode 0x2C, DCS (U)
            delegatesTbl1.Add((Action)EOR_U_ME0);   // Opcode 0x2D, EOR (U)
            delegatesTbl1.Add((Action)STA_U_ME0);   // Opcode 0x2E, STA (U)
            delegatesTbl1.Add((Action)BIT_U_ME0);   // Opcode 0x2F, BIT (U)
            #endregion Opcodes_0x20-0x2F

            #region Opcodes_0x30-0x3F
            delegatesTbl1.Add((Action)SBC_VL);      // Opcode 0x30, SBC VL
            delegatesTbl1.Add((Action)SBC_V_ME0);   // Opcode 0x31, SBC (V)
            delegatesTbl1.Add((Action)ADC_VL);      // Opcode 0x32, ADC VL
            delegatesTbl1.Add((Action)ADC_V_ME0);   // Opcode 0x33, ADC (V)
            delegatesTbl1.Add((Action)LDA_VL);      // Opcode 0x34, LDA VL
            delegatesTbl1.Add((Action)LDA_V_ME0);   // Opcode 0x35, LDA (V)
            delegatesTbl1.Add((Action)CPA_VL);      // Opcode 0x36, CPA VL
            delegatesTbl1.Add((Action)CPA_V_ME0);   // Opcode 0x37, CPA (V)
            delegatesTbl1.Add((Action)NOP);         // Opcode 0x38, NOP and STA VH
            delegatesTbl1.Add((Action)AND_V_ME0);   // Opcode 0x39
            delegatesTbl1.Add((Action)STA_VL);      // Opcode 0x3A, STA VL
            delegatesTbl1.Add((Action)ORA_V_ME0);   // Opcode 0x3B, ORA (V)
            delegatesTbl1.Add((Action)DCS_V_ME0);   // Opcode 0x3C, DCS (V)
            delegatesTbl1.Add((Action)EOR_V_ME0);   // Opcode 0x3D, EOR (V)
            delegatesTbl1.Add((Action)STA_V_ME0);   // Opcode 0x3E, STA (V)
            delegatesTbl1.Add((Action)BIT_V_ME0);   // Opcode 0x3F, BIT (V)
            #endregion Opcodes_0x30-0x3F

            #region Opcodes_0x40-0x4F
            delegatesTbl1.Add((Action)INC_XL);      // Opcode 0x40, INC XL
            delegatesTbl1.Add((Action)SIN_X);       // Opcode 0x41, SIN X
            delegatesTbl1.Add((Action)DEC_XL);      // Opcode 0x42, DEC XL
            delegatesTbl1.Add((Action)SDE_X);       // Opcode 0x43, SDE X
            delegatesTbl1.Add((Action)INC_X);       // Opcode 0x44, INC X
            delegatesTbl1.Add((Action)LIN_X);       // Opcode 0x45, LIN X
            delegatesTbl1.Add((Action)DEC_X);       // Opcode 0x46, DEC X
            delegatesTbl1.Add((Action)LDE_X_ME0);   // Opcode 0x47, LDE X
            delegatesTbl1.Add((Action)LDI_XH_n);    // Opcode 0x48, LDI XH
            delegatesTbl1.Add((Action)ANI_X_n_ME0); // Opcode 0x49, ANI (X),n
            delegatesTbl1.Add((Action)LDI_XL_n);    // Opcode 0x4A, LDI XL
            delegatesTbl1.Add((Action)ORI_X_n_ME0); // Opcode 0x4B, ORI (X),n
            delegatesTbl1.Add((Action)CPI_XH_n);    // Opcode 0x4C, CPI XH,n
            delegatesTbl1.Add((Action)BII_X_n_ME0); // Opcode 0x4D, BII (X),n
            delegatesTbl1.Add((Action)CPI_XL_n);    // Opcode 0x4E, CPI XL,n
            delegatesTbl1.Add((Action)ADI_X_n);     // Opcode 0x4F, ADI (X),n
            #endregion Opcodes_0x40-0x4F

            #region Opcodes_0x50-0x5F
            delegatesTbl1.Add((Action)INC_YL);      // Opcode 0x50, INC YL
            delegatesTbl1.Add((Action)SIN_Y);       // Opcode 0x51, SIN Y
            delegatesTbl1.Add((Action)DEC_YL);      // Opcode 0x52, DEC YL
            delegatesTbl1.Add((Action)SDE_Y);       // Opcode 0x53, SDE Y
            delegatesTbl1.Add((Action)INC_Y);       // Opcode 0x54, INC Y
            delegatesTbl1.Add((Action)LIN_Y);       // Opcode 0x55, LIN Y
            delegatesTbl1.Add((Action)DEC_Y);       // Opcode 0x56, DEC Y
            delegatesTbl1.Add((Action)LDE_Y);       // Opcode 0x57, LDE Y
            delegatesTbl1.Add((Action)LDI_YH_n);    // Opcode 0x58, LDI YH
            delegatesTbl1.Add((Action)ANI_Y_n_ME0); // Opcode 0x59, ANI (Y),n
            delegatesTbl1.Add((Action)LDI_YL_n);    // Opcode 0x5A, LDI YL
            delegatesTbl1.Add((Action)ORI_Y_n_ME0); // Opcode 0x5B, ORI (Y),n
            delegatesTbl1.Add((Action)CPI_YH_n);    // Opcode 0x5C, CPI YH,n
            delegatesTbl1.Add((Action)BII_Y_n_ME0); // Opcode 0x5D, BII (Y),n
            delegatesTbl1.Add((Action)CPI_YL_n);    // Opcode 0x5E, CPI YL,n
            delegatesTbl1.Add((Action)ADI_Y_n);     // Opcode 0x5F, ADI (Y),n
            #endregion Opcodes_0x50-0x5F

            #region Opcodes_0x60-0x6F
            delegatesTbl1.Add((Action)INC_UL);      // Opcode 0x60, INC UL
            delegatesTbl1.Add((Action)SIN_U);       // Opcode 0x61, SIN U
            delegatesTbl1.Add((Action)DEC_UL);      // Opcode 0x62, DEC UL
            delegatesTbl1.Add((Action)SDE_U);       // Opcode 0x63, SDE U
            delegatesTbl1.Add((Action)INC_U);       // Opcode 0x64, INC U
            delegatesTbl1.Add((Action)LIN_U);       // Opcode 0x65, LIN U
            delegatesTbl1.Add((Action)DEC_U);       // Opcode 0x66, DEC U
            delegatesTbl1.Add((Action)LDE_U);       // Opcode 0x67, LDE U
            delegatesTbl1.Add((Action)LDI_UH_n);    // Opcode 0x68, LDI UH,n
            delegatesTbl1.Add((Action)ANI_U_n_ME0); // Opcode 0x69, ANI (U),n
            delegatesTbl1.Add((Action)LDI_UL_n);    // Opcode 0x6A, LDI UL,n
            delegatesTbl1.Add((Action)ORI_U_n_ME0); // Opcode 0x6B, ORI (U),n
            delegatesTbl1.Add((Action)CPI_UH_n);    // Opcode 0x6C, CPI UH,n
            delegatesTbl1.Add((Action)BII_U_n_ME0); // Opcode 0x6D, BII (U),n
            delegatesTbl1.Add((Action)CPI_UL_n);    // Opcode 0x6E, CPI UL,n
            delegatesTbl1.Add((Action)ADI_U_n);     // Opcode 0x6F, ADI (U),n
            #endregion Opcodes_0x60-0x6F

            #region Opcodes_0x70-0x7F
            delegatesTbl1.Add((Action)INC_VL);      // Opcode 0x70, INC VL
            delegatesTbl1.Add((Action)SIN_V);       // Opcode 0x71, SIN V
            delegatesTbl1.Add((Action)DEC_VL);      // Opcode 0x72, DEC VL
            delegatesTbl1.Add((Action)SDE_V);       // Opcode 0x73, SDE V
            delegatesTbl1.Add((Action)INC_V);       // Opcode 0x74, INC V 
            delegatesTbl1.Add((Action)LIN_V);       // Opcode 0x75, LIN V
            delegatesTbl1.Add((Action)DEC_V);       // Opcode 0x76, DEC V
            delegatesTbl1.Add((Action)LDE_V_ME0);   // Opcode 0x77, LDE V
            delegatesTbl1.Add((Action)LDI_VH_n);    // Opcode 0x78, LDI VH,n
            delegatesTbl1.Add((Action)ANI_V_n_ME0); // Opcode 0x79, ANI (V),n
            delegatesTbl1.Add((Action)LDI_VL_n);    // Opcode 0x7A, LDI VL,n
            delegatesTbl1.Add((Action)ORI_V_n_ME0); // Opcode 0x7B, ORI (V),n
            delegatesTbl1.Add((Action)CPI_VH_n);    // Opcode 0x7C, CPI VH,n
            delegatesTbl1.Add((Action)BII_V_n_ME0); // Opcode 0x7D, BII (V),n
            delegatesTbl1.Add((Action)CPI_VL_n);    // Opcode 0x7E, CPI VL,n
            delegatesTbl1.Add((Action)ADI_V_n);     // Opcode 0x7F, ADI (V),n
            #endregion Opcodes_0x70-0x7F

            #region Opcodes_0x80-0x8F
            delegatesTbl1.Add((Action)SBC_XH);      // Opcode 0x80, SBC XH
            delegatesTbl1.Add((Action)BCR_n_p);     // Opcode 0x81, BCR+e
            delegatesTbl1.Add((Action)ADC_XH);      // Opcode 0x82, ADC XH
            delegatesTbl1.Add((Action)BCS_n_p);     // Opcode 0x83, BCS+e
            delegatesTbl1.Add((Action)LDA_XH);      // Opcode 0x84, LDA XH
            delegatesTbl1.Add((Action)BHR_n_p);     // Opcode 0x85, BHR+e
            delegatesTbl1.Add((Action)CPA_XH);      // Opcode 0x86, CPA XH
            delegatesTbl1.Add((Action)BHS_n_p);     // Opcode 0x87, BHS+e
            delegatesTbl1.Add((Action)LOP_UL_e);    // Opcode 0x88. LOP UL,e
            delegatesTbl1.Add((Action)BZR_n_p);     // Opcode 0x89, BZR+e
            delegatesTbl1.Add((Action)RTI);         // Opcode 0x8A, RTI
            delegatesTbl1.Add((Action)BZS_n_p);     // Opcode 0x8B, BZS+e
            delegatesTbl1.Add((Action)DCA_X_ME0);   // Opcode 0x8C, DCA (X)
            delegatesTbl1.Add((Action)BVR_n_p);     // Opcode 0x8D, BVR+e
            delegatesTbl1.Add((Action)BCH_n_p);     // Opcode 0x8E, BCH+e
            delegatesTbl1.Add((Action)BVS_n_p);     // Opcode 0x8F, BVS+e
            #endregion Opcodes_0x80-0x8F

            #region Opcodes_0x90-0x9F
            delegatesTbl1.Add((Action)SBC_YH);      // Opcode 0x90, SBC YH
            delegatesTbl1.Add((Action)BCR_n_m);     // Opcode 0x91, BCR-e
            delegatesTbl1.Add((Action)ADC_YH);      // Opcode 0x92, ADC YH
            delegatesTbl1.Add((Action)BCS_n_m);     // Opcode 0x93, BCS-e
            delegatesTbl1.Add((Action)LDA_YH);      // Opcode 0x94, LDA YH
            delegatesTbl1.Add((Action)BHR_n_m);     // Opcode 0x95, BHE-e
            delegatesTbl1.Add((Action)CPA_YH);      // Opcode 0x96, CPA YH
            delegatesTbl1.Add((Action)BHS_n_m);     // Opcode 0x97, BHS-e
            delegatesTbl1.Add((Action)NOP);         // Opcode 0x98
            delegatesTbl1.Add((Action)BZR_n_m);     // Opcode 0x99, BZR-e
            delegatesTbl1.Add((Action)RTN);         // Opcode 0x9A, RTN
            delegatesTbl1.Add((Action)BZS_n_m);     // Opcode 0x9B, BZS-e
            delegatesTbl1.Add((Action)DCA_Y_ME0);   // Opcode 0x9C, DCA (Y)
            delegatesTbl1.Add((Action)BVR_n_m);     // Opcode 0x9D, BVR-e
            delegatesTbl1.Add((Action)BCH_n_m);     // Opcode 0x9E, BCH-e
            delegatesTbl1.Add((Action)BVS_n_m);     // Opcode 0x9F, BVS-e
            #endregion Opcodes_0x90-0x9F

            #region Opcodes_0xA0-0xAF
            delegatesTbl1.Add((Action)SBC_UH);      // Opcode 0xA0, SBC UH
            delegatesTbl1.Add((Action)SBC_pp_ME0);  // Opcode 0xA1, SBC (pp)
            delegatesTbl1.Add((Action)ADC_UH);      // Opcode 0xA2, ADC UH
            delegatesTbl1.Add((Action)ADC_pp_ME0);  // Opcode 0xA3, ADC (pp)
            delegatesTbl1.Add((Action)LDA_UH);      // Opcode 0xA4, LDA XH
            delegatesTbl1.Add((Action)LDA_pp_ME0);  // Opcode 0xA5, LDA (pp)
            delegatesTbl1.Add((Action)CPA_UH);      // Opcode 0xA6, CPA UH
            delegatesTbl1.Add((Action)CPA_pp_ME0);  // Opcode 0xA7, CPA (pp)
            delegatesTbl1.Add((Action)SPV);         // Opcode 0xA8, SPV
            delegatesTbl1.Add((Action)AND_pp_ME0);  // Opcode 0xA9
            delegatesTbl1.Add((Action)LDI_S_pp);    // Opcode 0xAA, LDI S,pp
            delegatesTbl1.Add((Action)ORA_pp_ME0);  // Opcode 0xAB, ORA (pp)
            delegatesTbl1.Add((Action)DCA_U_ME0);   // Opcode 0xAC, DCA (U)
            delegatesTbl1.Add((Action)EOR_pp_ME0);  // Opcode 0xAD, EOR (pp)
            delegatesTbl1.Add((Action)STA_pp_ME0);  // Opcode 0xAE, STA (pp)
            delegatesTbl1.Add((Action)BIT_pp_ME0);  // Opcode 0xAF, BIT (pp)
            #endregion Opcodes_0xA0-0xAF

            #region Opcodes_0xB0-0xBF
            delegatesTbl1.Add((Action)SBC_VH);      // Opcode 0xB0, SBC VH
            delegatesTbl1.Add((Action)SBI_A_n);     // Opcode 0xB1, SBI A,n
            delegatesTbl1.Add((Action)ADC_VH);      // Opcode 0xB2, ADC VH
            delegatesTbl1.Add((Action)ADI_A_n);     // Opcode 0xB3, ADI A,n
            delegatesTbl1.Add((Action)LDA_VH);      // Opcode 0xB4, LDA VH
            delegatesTbl1.Add((Action)LDI_A_n);     // Opcode 0xB5, LDI A,n
            delegatesTbl1.Add((Action)CPA_VH);      // Opcode 0xB6, CPA VH
            delegatesTbl1.Add((Action)CPI_A_n);     // Opcode 0xB7, CPI A,n
            delegatesTbl1.Add((Action)RPV);         // Opcode 0xB8, RPV
            delegatesTbl1.Add((Action)ANI_A_n);     // Opcode 0xB9, ANI A,n
            delegatesTbl1.Add((Action)JMP_pp);      // Opcode 0xBA, JMP pp
            delegatesTbl1.Add((Action)ORI_A_n);     // Opcode 0xBB, ORI A,n
            delegatesTbl1.Add((Action)DCA_V_ME0);   // Opcode 0xBC, DCA (V)
            delegatesTbl1.Add((Action)EAI_n);       // Opcode 0xBD, EAI n
            delegatesTbl1.Add((Action)SJP_pp);      // Opcode 0xBE, SJP pp
            delegatesTbl1.Add((Action)BII_A_n);     // Opcode 0xBF, BII A,n
            #endregion Opcodes_0xB0-0xBF
 
            #region Opcodes_0xC0-0xCF
            delegatesTbl1.Add((Action)VEJ_C0);      // Opcode 0xC0, VEJ (C0)
            delegatesTbl1.Add((Action)VCR_n);       // Opcode 0xC1, VCR n
            delegatesTbl1.Add((Action)VEJ_C2);      // Opcode 0xC2, VEJ (C2)
            delegatesTbl1.Add((Action)VCS_n);       // Opcode 0xC3, VCS n
            delegatesTbl1.Add((Action)VEJ_C4);      // Opcode 0xC4, VEJ (C4)
            delegatesTbl1.Add((Action)VHR_n);       // Opcode 0xC5, VHR n
            delegatesTbl1.Add((Action)VEJ_C6);      // Opcode 0xC6, VEJ (C6)
            delegatesTbl1.Add((Action)VHS_n);       // Opcode 0xC7, VHS n
            delegatesTbl1.Add((Action)VEJ_C8);      // Opcode 0xC8, VEJ (C8)
            delegatesTbl1.Add((Action)VZR_n);       // Opcode 0xC9, VZR n
            delegatesTbl1.Add((Action)VEJ_CA);      // Opcode 0xCA, VEJ (CA)
            delegatesTbl1.Add((Action)VZS_n);       // Opcode 0xCB, VZS n
            delegatesTbl1.Add((Action)VEJ_CC);      // Opcode 0xCC, VEJ (CC)
            delegatesTbl1.Add((Action)VMJ_n);       // Opcode 0xCD, VMJ n
            delegatesTbl1.Add((Action)VEJ_CE);      // Opcode 0xCE, VEJ (CE)
            delegatesTbl1.Add((Action)VVS_n);       // Opcode 0xCF, VVS n
            #endregion Opcodes_0xC0-0xCF

            #region Opcodes_0xD0-0xDF
            delegatesTbl1.Add((Action)VEJ_D0);      // Opcode 0xD0, VEJ (D0)
            delegatesTbl1.Add((Action)ROR);         // Opcode 0xD1, ROR
            delegatesTbl1.Add((Action)VEJ_D2);      // Opcode 0xD2, VEJ (D2)
            delegatesTbl1.Add((Action)DRR_X_ME0);   // Opcode 0xD3, DDR (X)
            delegatesTbl1.Add((Action)VEJ_D4);      // Opcode 0xD4, VEJ (D4)
            delegatesTbl1.Add((Action)SHR);         // Opcode 0xD5, SHR
            delegatesTbl1.Add((Action)VEJ_D6);      // Opcode 0xD6, VEJ (D6)
            delegatesTbl1.Add((Action)DRL_X_ME0);   // Opcode 0xD7, DRL (X)
            delegatesTbl1.Add((Action)VEJ_D8);      // Opcode 0xD8, VEJ (D8)
            delegatesTbl1.Add((Action)SHL);         // Opcode 0xD9, SHL
            delegatesTbl1.Add((Action)VEJ_DA);      // Opcode 0xDA, VEJ (DA)
            delegatesTbl1.Add((Action)ROL);         // Opcode 0xDB, ROL
            delegatesTbl1.Add((Action)VEJ_DC);      // Opcode 0xDC, VEJ (DC)
            delegatesTbl1.Add((Action)INC_A);       // Opcode 0xDD, INC A
            delegatesTbl1.Add((Action)VEJ_DE);      // Opcode 0xDE, VEJ (DE)
            delegatesTbl1.Add((Action)DEC_A);       // Opcode 0xDF,DEC A
            #endregion Opcodes_0xD0-0xDF

            #region Opcodes_0xE0-0xEF
            delegatesTbl1.Add((Action)VEJ_E0);      // Opcode 0xE0, VEJ (E0)
            delegatesTbl1.Add((Action)SPU);         // Opcode 0xE1, SPU
            delegatesTbl1.Add((Action)VEJ_E2);      // Opcode 0xE2, VEJ (E2)
            delegatesTbl1.Add((Action)RPU);         // Opcode 0xE3, RPU
            delegatesTbl1.Add((Action)VEJ_E4);      // Opcode 0xE4, VEJ (E4)
            delegatesTbl1.Add((Action)NOP);         // Opcode 0xE5
            delegatesTbl1.Add((Action)VEJ_E6);      // Opcode 0xE6, VEJ (E6)
            delegatesTbl1.Add((Action)NOP);         // Opcode 0xE7
            delegatesTbl1.Add((Action)VEJ_E8);      // Opcode 0xE8, VEJ (E8)
            delegatesTbl1.Add((Action)ANI_pp_n_ME0);// Opcode 0xE9, ANI (pp),n
            delegatesTbl1.Add((Action)VEJ_EA);      // Opcode 0xEA, VEJ (EA)
            delegatesTbl1.Add((Action)ORI_pp_n_ME0);// Opcode 0xEB, ORI (pp),n
            delegatesTbl1.Add((Action)VEJ_EC);      // Opcode 0xEC, VEJ (EC)
            delegatesTbl1.Add((Action)BII_pp_n_ME0);// Opcode 0xED, BII (pp),n
            delegatesTbl1.Add((Action)VEJ_EE);      // Opcode 0xEE, VEJ (EE)
            delegatesTbl1.Add((Action)ADI_pp_n_ME0);// Opcode 0xEF, ADI (pp),n
            #endregion Opcodes_0xE0-0xEF

            #region Opcodes_0xF0-0xFF
            delegatesTbl1.Add((Action)VEJ_F0);      // Opcode 0xF0, VEJ (F0)
            delegatesTbl1.Add((Action)AEX);         // Opcode 0xF1
            delegatesTbl1.Add((Action)VEJ_F2);      // Opcode 0xF2, VEJ (F2)
            delegatesTbl1.Add((Action)NOP);         // Opcode 0xF3
            delegatesTbl1.Add((Action)VEJ_F4);      // Opcode 0xF4, VEJ (F4)
            delegatesTbl1.Add((Action)TIN);         // Opcode 0xF5, TIN
            delegatesTbl1.Add((Action)VEJ_F6);      // Opcode 0xF6, VEJ (F6)
            delegatesTbl1.Add((Action)CIN);         // Opcode 0xF7, CIN
            delegatesTbl1.Add((Action)VEJ_F8);      // Opcode 0xF8, VEJ (F8)
            delegatesTbl1.Add((Action)REC);         // Opcode 0xF9, REC
            delegatesTbl1.Add((Action)VEJ_FA);      // Opcode 0xFA, VEJ (FA)
            delegatesTbl1.Add((Action)SEC);         // Opcode 0xFB, SEC
            delegatesTbl1.Add((Action)VEJ_FC);      // Opcode 0xFC, VEJ (FC)
            delegatesTbl1.Add((Action)FD_P2);       // Opcode 0xFD, FD xx
            delegatesTbl1.Add((Action)VEJ_FE);      // Opcode 0xFE, VEJ (FE)
            delegatesTbl1.Add((Action)NOP);         // Opcode 0xFF
            #endregion Opcodes_0xF0-0xFF

            #endregion Opcodes Table 1 0x00-0xFF

            #region Opcode Table 2 0xFD00-0xFDFF

            #region Opcodes_0xFD00-0xFD0F
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 00
            delegatesTbl2.Add((Action)SBC_X_ME1);   // Opcode FD 01, SBC #(X)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 02
            delegatesTbl2.Add((Action)ADC_X_ME1);   // Opcode FD 03, ADC #(X)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 04
            delegatesTbl2.Add((Action)LDA_X_ME1);   // Opcode FD 05, LDA #(X)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 06
            delegatesTbl2.Add((Action)CPA_X_ME1);   // Opcode FD 07, CPA #(X)
            delegatesTbl2.Add((Action)LDX_X);       // Opcode FD 08, LDX X
            delegatesTbl2.Add((Action)AND_X_ME1);   // Opcode FD 09
            delegatesTbl2.Add((Action)POP_X);       // Opcode FD 0A, POP X
            delegatesTbl2.Add((Action)ORA_X_ME1);   // Opcode FD 0B, ORA #(X)
            delegatesTbl2.Add((Action)DCS_X_ME1);   // Opcode FD 0C, DCS #(X)
            delegatesTbl2.Add((Action)EOR_X_ME1);   // Opcode FD 0D, EOR #(X)
            delegatesTbl2.Add((Action)STA_X_ME1);   // Opcode FD 0E, STA #(X)
            delegatesTbl2.Add((Action)BIT_X_ME1);   // Opcode FD 0F, BIT #(X)
            #endregion Opcodes_0x00-0x0F

            #region Opcodes_0xFD10-0xFD1F
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 10
            delegatesTbl2.Add((Action)SBC_Y_ME1);   // Opcode FD 11, SBC #(Y)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 12
            delegatesTbl2.Add((Action)ADC_Y_ME1);   // Opcode FD 13, ADC #(Y)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 14
            delegatesTbl2.Add((Action)LDA_Y_ME1);   // Opcode FD 15, LDA #(Y)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 16
            delegatesTbl2.Add((Action)CPA_Y_ME1);   // Opcode FD 17, CPA #(Y)
            delegatesTbl2.Add((Action)LDX_Y);       // Opcode FD 18, LDX Y
            delegatesTbl2.Add((Action)AND_Y_ME1);   // Opcode FD 19
            delegatesTbl2.Add((Action)POP_Y);       // Opcode FD 1A. POP Y
            delegatesTbl2.Add((Action)ORA_Y_ME1);   // Opcode FD 1B, ORA #(Y)
            delegatesTbl2.Add((Action)DCS_Y_ME1);   // Opcode FD 1C, DCS #(Y)
            delegatesTbl2.Add((Action)EOR_Y_ME1);   // Opcode FD 1D, EOR #(Y)
            delegatesTbl2.Add((Action)STA_Y_ME1);   // Opcode FD 1E, STA #(Y)
            delegatesTbl2.Add((Action)BIT_Y_ME1);   // Opcode FD 1F, BIT #(Y)
            #endregion Opcodes_0x10-0x1F

            #region Opcodes_0xFD20-0xFD2F
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 20
            delegatesTbl2.Add((Action)SBC_U_ME1);   // Opcode FD 21, SBC #(U)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 22
            delegatesTbl2.Add((Action)ADC_U_ME1);   // Opcode FD 23, ADC #(U)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 24
            delegatesTbl2.Add((Action)LDA_U_ME1);   // Opcode FD 25, LDA #(U)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 26
            delegatesTbl2.Add((Action)CPA_U_ME1);   // Opcode FD 27, CPA #(U)
            delegatesTbl2.Add((Action)LDX_U);       // Opcode FD 28, LDX U
            delegatesTbl2.Add((Action)AND_U_ME1);   // Opcode FD 29
            delegatesTbl2.Add((Action)POP_U);       // Opcode FD 2A, POP U
            delegatesTbl2.Add((Action)ORA_U_ME1);   // Opcode FD 2B, ORA #(U)
            delegatesTbl2.Add((Action)DCS_U_ME1);   // Opcode FD 2C, DCS #(U)
            delegatesTbl2.Add((Action)EOR_U_ME1);   // Opcode FD 2D, EOR #(U)
            delegatesTbl2.Add((Action)STA_U_ME1);   // Opcode FD 2E, STA #(U)
            delegatesTbl2.Add((Action)BIT_U_ME1);   // Opcode FD 2F, BIT #(U)
            #endregion Opcodes_0x20-0x2F

            #region Opcodes_0xFD30-0xFD3F
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 30
            delegatesTbl2.Add((Action)SBC_V_ME1);   // Opcode FD 31, SBC #(V)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 32
            delegatesTbl2.Add((Action)ADC_V_ME1);   // Opcode FD 33, ADC #(V)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 34
            delegatesTbl2.Add((Action)LDA_V_ME1);   // Opcode FD 35, LDA #(V)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 36
            delegatesTbl2.Add((Action)CPA_V_ME1);   // Opcode FD 37, CPA #(V)
            delegatesTbl2.Add((Action)LDX_V);       // Opcode FD 38, LDX V
            delegatesTbl2.Add((Action)AND_V_ME1);   // Opcode FD 39
            delegatesTbl2.Add((Action)POP_V);       // Opcode FD 3A, POP V
            delegatesTbl2.Add((Action)ORA_V_ME1);   // Opcode FD 3B, ORA #(V)
            delegatesTbl2.Add((Action)DCS_V_ME1);   // Opcode FD 3C, DCS #(V)
            delegatesTbl2.Add((Action)EOR_V_ME1);   // Opcode FD 3D, EOR #(V)
            delegatesTbl2.Add((Action)STA_V_ME1);   // Opcode FD 3E, STA #(X)
            delegatesTbl2.Add((Action)BIT_V_ME1);   // Opcode FD 3F, BIT #(V)
            #endregion Opcodes_0x30-0x3F

            #region Opcodes_0xFD40-0xFD4F
            delegatesTbl2.Add((Action)INC_XH);      // Opcode FD 40, INC XH
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 41
            delegatesTbl2.Add((Action)DEC_XH);      // Opcode FD 42, DEC XH
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 43
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 44
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 45
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 46
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 47
            delegatesTbl2.Add((Action)LDX_S);       // Opcode FD 48, LDX S
            delegatesTbl2.Add((Action)ANI_X_n_ME1); // Opcode FD 49, ANI #(X),n
            delegatesTbl2.Add((Action)STX_X);       // Opcode FD 4A, STX X
            delegatesTbl2.Add((Action)ORI_X_n_ME1); // Opcode FD 4B, ORI #(X),n
            delegatesTbl2.Add((Action)OFF);         // Opcode FD 4C, OFF
            delegatesTbl2.Add((Action)BII_X_n_ME1); // Opcode FD 4D, BII #(X),n
            delegatesTbl2.Add((Action)STX_S);       // Opcode FD 4E, STX S
            delegatesTbl2.Add((Action)ADI_X_n_ME1); // Opcode FD 4F, ADI #(X),n
            #endregion Opcodes_0x40-0x4F

            #region Opcodes_0xFD50-0xFD5F
            delegatesTbl2.Add((Action)INC_YH);      // Opcode FD 50, INC YH
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 51
            delegatesTbl2.Add((Action)DEC_YH);      // Opcode FD 52, DEC YH
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 53
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 54
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 55
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 56
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 57
            delegatesTbl2.Add((Action)LDX_P);       // Opcode FD 58, LDX P
            delegatesTbl2.Add((Action)ANI_Y_n_ME1); // Opcode FD 59, ANI #(Y),n
            delegatesTbl2.Add((Action)STX_Y);       // Opcode FD 5A, STX Y
            delegatesTbl2.Add((Action)ORI_Y_n_ME1); // Opcode FD 5B, ORI #(Y),n
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 5C
            delegatesTbl2.Add((Action)BII_Y_n_ME1); // Opcode FD 5D, BII #(Y),n
            delegatesTbl2.Add((Action)STX_P);       // Opcode FD 5E, STX P
            delegatesTbl2.Add((Action)ADI_Y_n_ME1); // Opcode FD 5F, ADI #(Y),n
            #endregion Opcodes_0x50-0x5F

            #region Opcodes_0xFD60-0xFD6F
            delegatesTbl2.Add((Action)INC_UH);      // Opcode FD 60, INC UH
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 61
            delegatesTbl2.Add((Action)DEC_UH);      // Opcode FD 62, DEC UH
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 63
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 64
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 65
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 66
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 67
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 68
            delegatesTbl2.Add((Action)ANI_U_n_ME1); // Opcode FD 69, ANI #(U),n
            delegatesTbl2.Add((Action)STX_U);       // Opcode FD 6A, STX U
            delegatesTbl2.Add((Action)ORI_U_n_ME1); // Opcode FD 6B, ORI #(U),n
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 6C
            delegatesTbl2.Add((Action)BII_U_n_ME1); // Opcode FD 6D, BII #(U),n
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 6E
            delegatesTbl2.Add((Action)ADI_U_n_ME1); // Opcode FD 6F, ADI #(U),n
            #endregion Opcodes_0x60-0x6F

            #region Opcodes_0xFD70-0xFD7F
            delegatesTbl2.Add((Action)INC_VH);      // Opcode FD 70, INC VH
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 71
            delegatesTbl2.Add((Action)DEC_VH);      // Opcode FD 72, DEC VH
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 73
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 74
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 75
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 76
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 77
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 78
            delegatesTbl2.Add((Action)ANI_V_n_ME1); // Opcode FD 79, ANI #(V),n
            delegatesTbl2.Add((Action)STX_V);       // Opcode FD 7A, STX V
            delegatesTbl2.Add((Action)ORI_V_n_ME1); // Opcode FD 7B, ORI #(V),n
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 7C
            delegatesTbl2.Add((Action)BII_V_n_ME1); // Opcode FD 7D, BII #(V),n
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 7E
            delegatesTbl2.Add((Action)ADI_V_n_ME1); // Opcode FD 7F, ADI #(V),n
            #endregion Opcodes_0x70-0x7F

            #region Opcodes_0xFD80-0xFD8F
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 80
            delegatesTbl2.Add((Action)SIE);         // Opcode FD 81. SIE
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 82
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 83
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 84
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 85
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 86
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 87
            delegatesTbl2.Add((Action)PSH_X);       // Opcode FD 88, PSH X
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 89
            delegatesTbl2.Add((Action)POP_A);       // Opcode FD 8A, POP A
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 8B
            delegatesTbl2.Add((Action)DCA_X_ME1);   // Opcode FD 8C, DCA #(X)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 8D
            delegatesTbl2.Add((Action)CDV);         // Opcode FD 8E, CDV
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 8F
            #endregion Opcodes_0x80-0x8F

            #region Opcodes_0xFD90-0xFD9F
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 90
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 91
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 92
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 93
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 94
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 95
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 96
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 97
            delegatesTbl2.Add((Action)PSH_Y);       // Opcode FD 98, PSH Y
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 99
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 9A
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 9B
            delegatesTbl2.Add((Action)DCA_Y_ME1);   // Opcode FD 9C, DCA #(Y)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 9D
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 9E
            delegatesTbl2.Add((Action)NOP);         // Opcode FD 9F
            #endregion Opcodes_0x90-0x9F

            #region Opcodes_0xFDA0-0xFDAF
            delegatesTbl2.Add((Action)NOP);         // Opcode FD A0
            delegatesTbl2.Add((Action)SBC_pp_ME1);  // Opcode FD A1, SBC #(pp)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD A2
            delegatesTbl2.Add((Action)ADC_pp_ME1);  // Opcode FD A3, ADC #(pp)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD A4
            delegatesTbl2.Add((Action)LDA_pp_ME1);  // Opcode FD A5, LDA #(pp)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD A6
            delegatesTbl2.Add((Action)CPA_pp_ME1);  // Opcode FD A7, CPA #(pp)
            delegatesTbl2.Add((Action)PSH_U);       // Opcode FD A8, PSH U
            delegatesTbl2.Add((Action)AND_pp_ME1);  // Opcode FD A9
            delegatesTbl2.Add((Action)TTA);         // Opcode FD AA, TTA
            delegatesTbl2.Add((Action)ORA_pp_ME1);  // Opcode FD AB, ORA #(pp)
            delegatesTbl2.Add((Action)DCA_U_ME1);   // Opcode FD AC, DCA #(U)
            delegatesTbl2.Add((Action)EOR_pp_ME1);  // Opcode FD AD, EOR #(pp)
            delegatesTbl2.Add((Action)STA_pp_ME1);  // Opcode FD AE, STA #(pp)
            delegatesTbl2.Add((Action)BIT_pp_ME1);  // Opcode FD AF, BIT #(pp)
            #endregion Opcodes_0xA0-0xAF

            #region Opcodes_0xFDB0-0xFDBF
            delegatesTbl2.Add((Action)NOP);         // Opcode FD B0
            delegatesTbl2.Add((Action)HLT);         // Opcode FD B1, HLT
            delegatesTbl2.Add((Action)NOP);         // Opcode FD B2
            delegatesTbl2.Add((Action)NOP);         // Opcode FD B3
            delegatesTbl2.Add((Action)NOP);         // Opcode FD B4
            delegatesTbl2.Add((Action)NOP);         // Opcode FD B5
            delegatesTbl2.Add((Action)NOP);         // Opcode FD B6
            delegatesTbl2.Add((Action)NOP);         // Opcode FD B7
            delegatesTbl2.Add((Action)PSH_V);       // Opcode FD B8, PSH V
            delegatesTbl2.Add((Action)NOP);         // Opcode FD B9
            delegatesTbl2.Add((Action)ITA);         // Opcode FD BA, ITA
            delegatesTbl2.Add((Action)NOP);         // Opcode FD BB
            delegatesTbl2.Add((Action)DCA_V_ME1);   // Opcode FD BC, DCA #S(V)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD BD
            delegatesTbl2.Add((Action)RIE);         // Opcode FD BE, RIE
            delegatesTbl2.Add((Action)NOP);         // Opcode FD BF
            #endregion Opcodes_0xB0-0xBF

            #region Opcodes_0xFDC0-0xFDCF
            delegatesTbl2.Add((Action)RDP);         // Opcode FD C0, RDP
            delegatesTbl2.Add((Action)SDP);         // Opcode FD C1, SDP
            delegatesTbl2.Add((Action)NOP);         // Opcode FD C2
            delegatesTbl2.Add((Action)NOP);         // Opcode FD C3
            delegatesTbl2.Add((Action)NOP);         // Opcode FD C4
            delegatesTbl2.Add((Action)NOP);         // Opcode FD C5
            delegatesTbl2.Add((Action)NOP);         // Opcode FD C6
            delegatesTbl2.Add((Action)NOP);         // Opcode FD C7
            delegatesTbl2.Add((Action)PSH_A);       // Opcode FD C8, PSH A
            delegatesTbl2.Add((Action)NOP);         // Opcode FD C9
            delegatesTbl2.Add((Action)ADR_X);       // Opcode FD CA, ADR X
            delegatesTbl2.Add((Action)NOP);         // Opcode FD CB
            delegatesTbl2.Add((Action)ATP);         // Opcode FD CC, ATP
            delegatesTbl2.Add((Action)NOP);         // Opcode FD CD
            delegatesTbl2.Add((Action)AM0);         // Opcode FD CE
            delegatesTbl2.Add((Action)NOP);         // Opcode FD CF
            #endregion Opcodes_0xC0-0xCF

            #region Opcodes_0xFDD0-0xFDDF
            delegatesTbl2.Add((Action)NOP);         // Opcode FD D0
            delegatesTbl2.Add((Action)NOP);         // Opcode FD D1
            delegatesTbl2.Add((Action)NOP);         // Opcode FD D2
            delegatesTbl2.Add((Action)DRR_X_ME1);   // Opcode FD D3, DDR #(X)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD D4
            delegatesTbl2.Add((Action)NOP);         // Opcode FD D5
            delegatesTbl2.Add((Action)NOP);         // Opcode FD D6
            delegatesTbl2.Add((Action)DRL_X_ME1);   // Opcode FD D7, DRL #(X)
            delegatesTbl2.Add((Action)NOP);         // Opcode FD D8
            delegatesTbl2.Add((Action)NOP);         // Opcode FD D9
            delegatesTbl2.Add((Action)ADR_Y);       // Opcode FD DA, ADR Y
            delegatesTbl2.Add((Action)NOP);         // Opcode FD DB
            delegatesTbl2.Add((Action)NOP);         // Opcode FD DC
            delegatesTbl2.Add((Action)NOP);         // Opcode FD DD
            delegatesTbl2.Add((Action)AM1);         // Opcode FD DE
            delegatesTbl2.Add((Action)NOP);         // Opcode FD DF
            #endregion Opcodes_0xD0-0xDF

            #region Opcodes_0xFDE0-0xFDEF
            delegatesTbl2.Add((Action)NOP);         // Opcode FD E0
            delegatesTbl2.Add((Action)NOP);         // Opcode FD E1
            delegatesTbl2.Add((Action)NOP);         // Opcode FD E2
            delegatesTbl2.Add((Action)NOP);         // Opcode FD E3
            delegatesTbl2.Add((Action)NOP);         // Opcode FD E4
            delegatesTbl2.Add((Action)NOP);         // Opcode FD E5
            delegatesTbl2.Add((Action)NOP);         // Opcode FD E6
            delegatesTbl2.Add((Action)NOP);         // Opcode FD E7
            delegatesTbl2.Add((Action)NOP);         // Opcode FD E8
            delegatesTbl2.Add((Action)ANI_pp_n_ME1);// Opcode FD E9, ANI #(pp),n
            delegatesTbl2.Add((Action)ADR_U);       // Opcode FD EA, ADR U
            delegatesTbl2.Add((Action)ORI_pp_n_ME1);// Opcode FD EB, ORI #(pp),n
            delegatesTbl2.Add((Action)ATT);         // Opcode FD EC, ATT
            delegatesTbl2.Add((Action)BII_pp_n_ME1);// Opcode FD ED, BII #(pp),n
            delegatesTbl2.Add((Action)NOP);         // Opcode FD EE
            delegatesTbl2.Add((Action)ADI_pp_n_ME1);// Opcode FD EF, ADI (pp),n
            #endregion Opcodes_0xE0-0xEF

            #region Opcodes_0xFDF0-0xFDFF
            delegatesTbl2.Add((Action)NOP);         // Opcode FD F0
            delegatesTbl2.Add((Action)NOP);         // Opcode FD F1
            delegatesTbl2.Add((Action)NOP);         // Opcode FD F2
            delegatesTbl2.Add((Action)NOP);         // Opcode FD F3
            delegatesTbl2.Add((Action)NOP);         // Opcode FD F4
            delegatesTbl2.Add((Action)NOP);         // Opcode FD F5
            delegatesTbl2.Add((Action)NOP);         // Opcode FD F6
            delegatesTbl2.Add((Action)NOP);         // Opcode FD F7
            delegatesTbl2.Add((Action)NOP);         // Opcode FD F8
            delegatesTbl2.Add((Action)NOP);         // Opcode FD F9
            delegatesTbl2.Add((Action)ADR_V);       // Opcode FD FA, ADR V
            delegatesTbl2.Add((Action)NOP);         // Opcode FD FB
            delegatesTbl2.Add((Action)NOP);         // Opcode FD FC
            delegatesTbl2.Add((Action)NOP);         // Opcode FD FD
            delegatesTbl2.Add((Action)NOP);         // Opcode FD FE
            delegatesTbl2.Add((Action)NOP);         // Opcode FD FF
            #endregion Opcodes_0xF0-0xFF

            #endregion Opcodes Table 2 0xFD00-0xFDFF

            #region Vector Jump Table

            #region 0xFF00-0xFF3E

            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1_b2_o1);  // 00
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1_b2_o1);  // 02
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_o1);        // 04
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 06 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_o1);        // 08
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 0A n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 0C n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1_o1);     // 0E

            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1);        // 10
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 12 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 14 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 16 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 18 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_o1);        // 1A
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1);        // 1C
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 1E n/a

            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 20 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 22 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 24 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 26 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_o1);        // 28
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1_b2);     // 2A
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_78b1);      // 2C
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_78b1);      // 2E

            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 30 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 32 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1_nb2o1);  // 34 
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 36 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 38 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 3A n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 3C n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default);   // 3E n/a

            #endregion 0xFF00-0xFF3E

            #region 0xFFC0-0xFFFE

            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // C0 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_p_o1);    // C2
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_p_o1);    // C4
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // C6 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_o1);      // C8
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_78b1);    // CA 
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_78b1);    // CC 
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1_o1);   // CE

            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1_o1);   // D0 
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1_b2);   // D2
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1);      // D4
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_b1);      // D6 
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // D8 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // DA n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // DC n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_o1);      // DE

            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // E0 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // E2 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // E4 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // E6 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // E8 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // EA n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // EC n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // EE n/a

            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // F0 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // F2 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_w1);      // F4 
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_w1);      // F6 
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // F8 n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // FA n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // FC n/a
            delegatesVect.Add((Func<Byte, Tuple<String, byte>>)VEJ_Default); // FE n/a

            #endregion 0xFFC0-0xFFFE

            #endregion Vector Jump Table
        }

        /// <summary>
        /// Configure byte arrays used to look up bytes used by each opcode
        /// </summary>
        private void ConfigOpcodeBytesTables()
        {
            opBytesP1 = new byte[] {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                                    1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                                    1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,
                                    1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,
                                    1,2,1,2,1,2,1,2,2,2,1,2,1,2,2,2,1,2,1,2,1,2,1,2,0,2,1,2,1,2,2,2,
                                    1,3,1,3,1,3,1,3,1,3,3,3,1,3,3,3,1,2,1,2,1,2,1,2,1,2,3,2,1,2,3,2,
                                    1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                                    1,1,1,1,1,0,1,0,1,4,1,4,1,4,1,4,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,0};

            opBytesP2 = new byte[] {0,2,0,2,0,2,0,2,2,2,2,2,2,2,2,2,0,2,0,2,0,2,0,2,2,2,2,2,2,2,2,2,
                                    0,2,0,2,0,2,0,2,2,2,2,2,2,2,2,2,0,2,0,2,0,2,0,2,2,2,2,2,2,2,2,2,
                                    2,0,2,0,0,0,0,0,2,3,2,3,2,3,2,3,2,0,2,0,0,0,0,0,2,3,2,3,0,3,2,3,
                                    2,0,2,0,0,0,0,0,0,3,2,3,0,3,0,3,2,0,2,0,0,0,0,0,0,3,2,3,0,3,0,3,
                                    0,2,0,0,0,0,0,0,2,0,2,0,2,0,2,0,0,0,0,0,0,0,0,0,2,0,0,0,2,0,0,0,
                                    0,4,0,4,0,4,0,4,2,4,2,4,2,4,4,4,0,2,0,0,0,0,0,0,2,0,2,0,2,0,2,0,
                                    2,2,0,0,0,0,0,0,2,0,2,0,2,0,2,0,0,0,0,2,0,0,0,2,0,0,2,0,0,0,2,0,
                                    0,0,0,0,0,0,0,0,0,5,2,5,2,5,0,5,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0};

            /// # of arguments for $FF page vectors. i.e. $FF(00)=3 Args, (02)=3 Args, (04)=1 Args
            /// Vector low nibble / 2 = index. i.e. $FF(F4) F4/2 = 7A = Value of 0x02
            vecArgs = new byte[]   {3,3,1,0,1,0,0,2,1,0,0,0,0,1,1,0,0,0,0,0,1,2,1,1,0,0,1,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                    0,2,2,0,1,1,1,2,2,2,1,1,0,0,0,1,1,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0};
        }

        /// <summary>
        /// Read include file add LABLES, SEGMENTS, COMMENTS to matching
        /// dictionary. Comments except in .COMMENT lines ignored
        private void ReadLibFile()
        {
            lableDict = new Dictionary<ushort, string>();
            SegmentDict = new Dictionary<ushort, SEGMENT>();
            CommentDict = new Dictionary<ushort, string>();
            string path = "C:\\Users\\birtj\\OneDrive\\Vintage_Computers\\Sharp_Pocket_Computer\\PC-1500\\PC-1500_DEV\\lh5801_Emu_Project\\lh5801_Dis\\Test_Code\\Include.txt";

            string line;
            StreamReader fileReader = new StreamReader(path);
            line = fileReader.ReadLine();

            // if there is an actual line
            while (line != null)
            {
                // if this line is not just a comment
                if (line != "" & !isComment.IsMatch(line))
                {
                    // We have a valid line, split on any white space
                    string[] eval = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                    // eval[0] is directive, i.e. 'LABLE_1', '.BYTE'
                    if (eval.Count() > 0 & isLabel.IsMatch(eval[0]))
                    {
                        LableHandler(eval);
                    }
                    else if (isDirective.IsMatch(eval[0]))
                    {
                        DirectiveHandler(line);
                    }
                }
                line = fileReader.ReadLine();
            }

        }

        /// <summary>
        ///  Lables should have 3 parts: START	.equ	$1000, '=' or '.equ'
        /// </summary>
        /// <param name="eval"></param>
        private void LableHandler(string[] eval)
        {
            // If we have at least three parts (eval[0-2]) check for '=' 
            if ((eval.Count() > 2) & isEqu.IsMatch(eval[1]))
            {
                // is arg 3 address in hexidecimal?
                if (isHexVal.IsMatch(eval[2]))
                {
                    //remove any hex prefix/suffix
                    eval[2] = System.Text.RegularExpressions.Regex.Replace(eval[2], "[$H]", string.Empty);

                    bool addressOK = true;
                    ushort address = 0;
                    try
                    {
                        address = (ushort)Convert.ToUInt16(eval[2], 16);
                    }
                    catch
                    {
                        addressOK = false;
                        System.Console.WriteLine("Error in line {0} {1} {2}", eval[0], eval[1], eval[2]);
                    }

                    if (addressOK) { lableDict.Add(address, eval[0]); } // save address, lable 
                }
            }
            else
            {
                System.Console.WriteLine("Error in line {0}, too few arguments", eval[0]);
            }
        }

        /// <summary>
        ///  Should have two parts: 'DIRECTIVE ADDRESS', i.e. '.BYTE $1234'
        /// </summary>
        /// <param name="eval"></param>
        private void DirectiveHandler(string line)
        {
            string[] eval = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            // make sure we have at least 'DIRECTIVE ADDRESS' we ignore any trailing comments
            if (eval.Count() > 1)
            {
                // remove '0x' or '$' from address value
                eval[1] = System.Text.RegularExpressions.Regex.Replace(eval[1], "[$H]", string.Empty);

                bool addressOK = true;
                ushort address = 0;
                try
                {
                    address = (ushort)Convert.ToUInt16(eval[1], 16); // need to trap bogus address
                }
                catch
                {
                    addressOK = false;
                    System.Console.WriteLine("Error in line {0}", line);
                }

                if (addressOK)
                {
                    switch (eval[0])
                    {
                        case ".BYTE":
                            SegmentDict.Add(address, SEGMENT.BYTE); // catch address exists
                            break;
                        case ".WORD":
                            SegmentDict.Add(address, SEGMENT.WORD); // catch address exists
                            break;
                        case ".CODE":
                            SegmentDict.Add(address, SEGMENT.CODE); // catch address exists
                            break;
                        case ".TEXT":
                            SegmentDict.Add(address, SEGMENT.TEXT); // catch address exists
                            break;
                        case ".SKIP":
                            SegmentDict.Add(address, SEGMENT.SKIP); // catch address exists
                            break;
                        case ".COMMENT":
                            eval = line.Split(';'); // split at ';' to get comment text
                            string comment = eval[1]; // make sure comment makes sense
                            CommentDict.Add(address, comment);  // catch address exists
                            break;
                    }
                }
            }
            else
            {
                System.Console.WriteLine("Error in line {0}, too few arguments", eval[0]);
            }

        }

        #endregion Configuration

        #region Helpers

        // Look at next byte but don't advance program counter
        private byte PeekByte()
        {
            byte val = RAM_ME0[P];

            return val;
        }

        /// <summary>
        /// Retrive byte from address P
        /// Advance Program Counter
        /// </summary>
        /// <returns></returns>
        private byte GetByte()
        {
            byte val = RAM_ME0[P];
            P += 1; // Advance Program Counter

            return val;
        }

        /// <summary>
        /// Retrive 16bit word (address, value) from address P, P+1
        /// Advance Program Counter
        /// </summary>
        /// <returns>Address</returns>
        private ushort GetWord()
        {
            ushort val = (ushort)((RAM_ME0[P] << 8) | RAM_ME0[P + 1]);
            P += 2; // Advance Program Counter

            return val;
        }

        /// <summary>
        /// Retrive 16bit word from address 
        /// </summary>
        /// <returns>16bit value</returns>
        private ushort GetWord(ushort address)
        {
            return (ushort)((RAM_ME0[address] << 8) | RAM_ME0[address + 1]);
        }

        /// <summary>
        /// Do a hex dump of current line
        /// </summary>
        /// Format: Address opcode_arguments, #### ## ##...
        /// <param name="numBytes"># of bytes in this line</param>
        private void LineDump(ushort start, int numBytes)
        {
            disSB.Append(String.Format("{0:X4}   ", start)); // address

            // max 8 bytes
            for (ushort i = 0; i < 8; i++)
            {
                if (i < numBytes)
                {
                    disSB.Append(String.Format("{0:X2} ", RAM_ME0[start + i]));
                }
                else
                {
                    disSB.Append("   ");
                }
            }
            disSB.Append("   "); // extra padding between raw bytes and disassembly
        }

        /// <summary>
        /// Dump label for this address
        /// </summary>
        /// <param name="lable"></param>
        private void LableDump(string label)
        {
            disSB.Append(String.Format("{1}{0}:{1}", label, Environment.NewLine));
        }

        /// <summary>
        /// Return label for address if exists or hex formatted address
        /// </summary>
        /// <param name="value">address</param>
        /// <returns></returns>
        private string GetAddOrLabel(ushort value)
        {
            string output = "";

            if (addressLabels && lableDict.ContainsKey(value))
            {
                output = lableDict[value];
            }
            else
            {
                output = "$" + value.ToString("X4");
            }

            return output;
        }

        /// <summary>
        /// Finds next SEGMENT definition, if any
        /// </summary>
        /// <param name="address"></param>
        private void FindNextSegment(ushort address)
        {
            bool match = false;
            ushort i = 0;
            SegmentEnd = 65535;

            // use default of CODE segmetn mode if libFile read not enabled
            if (libFileEnable)
            {
                //iterate through frag dictionary, stop iterating when match found
                do
                {
                    if (address >= SegmentDict.ElementAt(i).Key)
                    {
                        SegmentMode = SegmentDict.ElementAt(i).Value;
                        match = true;

                        if (i + 1 < SegmentDict.Count)
                        {
                            // set FragEnd to address of next fragment if one exists
                            SegmentEnd = (ushort)(SegmentDict.ElementAt(i + 1).Key - 1);
                        }
                        //else
                        //{
                        //    SegmentEnd = 65535;
                        //}
                    }

                    i++;

                } while (!match && i < SegmentDict.Count);
            }
        }

        #endregion Helpers

        #region VEJ_Handlers

        /// <summary>
        /// Default hanlder for vector calls with no arguments
        /// </summary>
        /// <param name="numBytes"></param>
        /// <returns></returns>
        private Tuple<String, byte> VEJ_Default(Byte numBytes)
        {
            return Tuple.Create("", (byte)0);
        }

        /// <summary>
        /// String representation of vector jump arguments
        /// with 78b1 format (byte value t0 -> $78b1 address)
        /// </summary>
        /// <returns></returns>
        private Tuple<String, byte> VEJ_78b1(Byte numBytes)
        {
            byte value = GetByte();   // P += 1
            return Tuple.Create(String.Format("$78{0:X2}", value), (byte)1);
        }

        /// <summary>
        /// String representation of vector jump arguments
        /// with b1 format (byte value)
        /// </summary>
        /// <returns></returns>
        private Tuple<String, byte> VEJ_b1(Byte numBytes)
        {
            byte b1 = GetByte();    // P += 1

            return Tuple.Create(String.Format("${0:X2}", b1), (byte)1);
        }

        /// <summary>
        /// String representation of vector jump arguments
        /// with b1_b2 format (byte value, byte value)
        /// </summary>
        /// <returns></returns>
        private Tuple<String, byte> VEJ_b1_b2(Byte numBytes)
        {
            byte b1 = GetByte();    // P += 1
            byte b2 = GetByte();    // P += 1

            return Tuple.Create(String.Format("${0:X2},${1:X2}", b1, b2), (Byte)2);
        }

        /// <summary>
        /// String representation of vector jump arguments
        /// with b1_o1 format (byte value, byte branch offset)
        /// </summary>
        /// <returns></returns>
        private Tuple<String, byte> VEJ_b1_o1(Byte numBytes)
        {
            byte variable = GetByte();    // P += 1
            byte value    = GetByte();    // P += 1
            ushort address = (ushort)(P + value);

            return Tuple.Create(String.Format("${0:X2},{1:X4}", variable, GetAddOrLabel(address)), (Byte)2);
        }

        /// <summary>
        /// String representation of vector jump arguments
        /// with b1_b2_o1 format (byte value, byte value, byte branch offset)
        /// </summary>
        /// <returns></returns>
        private Tuple<String, byte> VEJ_b1_b2_o1(Byte numBytes)
        {
            byte b1    = GetByte(); // P += 1
            byte b2    = GetByte(); // P += 1
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P + value);

            return Tuple.Create(String.Format("${0:X2},${1:X2},{2:X4}", b1, b2, GetAddOrLabel(address)), (Byte)3);
        }

        /// <summary>
        /// String representation of vector jump arguments
        /// with b1_n(b2_o1) format (byte #arg pairs, [byte charecter, byte branch offset])
        /// </summary>
        /// <returns></returns>
        private Tuple<String, byte> VEJ_b1_nb2o1(Byte numBytes)
        {
            StringBuilder sb = new StringBuilder();
            byte numArgs = (byte)(GetByte()* 4);    // (b1) P += 1 
            byte character;                         // (b2)
            byte offset;                            // (o1)

            for (int i = 0; i < numArgs; i+=2)
            {
                character = GetByte();  // P += 1
                offset = GetByte();     // P += 1
                ushort address = (ushort)(P + offset);

                sb.AppendLine(String.Format(",${0:X2},{1:X4}", character, GetAddOrLabel(address)));
                sb.Append("                                         ");
            }

            sb.Remove(0, 1); // remove leading comma
            return Tuple.Create(sb.ToString(), (numArgs += 1) );
        }

        /// <summary>
        /// String representation of vector jump arguments
        /// with o1 format (byte branch offset)
        /// </summary>
        /// <returns></returns>
        private Tuple<String, byte> VEJ_o1(Byte numBytes)
        {
            byte value = GetByte();   // P += 1
            ushort address = (ushort)(P + value);
            return Tuple.Create(String.Format("{0:X4}", GetAddOrLabel(address)), (Byte)1);
        }

        /// <summary>
        /// String representation of vector jump arguments
        /// with p_o1 format (byte/word value, byte branch offset)
        /// </summary>
        /// <returns></returns>
        private Tuple<String, byte> VEJ_p_o1(Byte numBytes)
        {
            if (numBytes == 4) // p is a function
            {
                ushort token = GetWord();   // P += 2
                byte   value = GetByte();   // P += 1
                ushort address = (ushort)(P + value);

                return Tuple.Create(String.Format("${0:X4},{1:X4}", token, GetAddOrLabel(address)), (Byte)4);
            }
            else // p is a character
            {
                byte character = GetByte();   // P += 1
                byte value     = GetByte();   // P += 1
                ushort address = (ushort)(P + value);

                return Tuple.Create(String.Format("${0:X2},{1:X4}", character, GetAddOrLabel(address)), (Byte)3);
            }
        }

        /// <summary>
        /// String representation of vector jump arguments
        /// with w1 format (word address)
        /// </summary>
        /// <returns></returns>
        private Tuple<String, byte> VEJ_w1(Byte numBytes)
        {
            ushort address = GetWord();   // P += 2
            return Tuple.Create(String.Format("{0:X4}", GetAddOrLabel(address)), (Byte)2);
        }

        // Indexed Vector Opcodes: VCR, VCS, VHR, VHS, VMJ, VVS, VZR, VZS
        private string VectorIndex(byte opcode, out byte numArgs)
        {
            String output = "";
            Tuple<String, byte> retValues;// = new Tuple<String, byte>("", 1);
            byte lowNibble = GetByte(); // P += 1, Vector low nibble / 2 = index
            int index = lowNibble / 2;  // index into vector delgate list
            numArgs = vecArgs[index];   // get # args from vector#, i.e. $FF(00)=3 Args (may not need)

            // we skip middle 64 vectors as they take no arguments
            if (index >=0 && index < 32) // 0-31 (32-63,64-95) 96-127
            {
                retValues = (Tuple<String, byte>)(delegatesVect[index].DynamicInvoke(numArgs));
                output = String.Format("(${0:X2}),{01}", lowNibble, retValues.Item1);
                numArgs = (byte)(retValues.Item2 + 1);
            }
            else if (index > 31 && index < 96)
            {
                output = String.Format("(${0:X2})", lowNibble);
                numArgs += 1;
            }
            else if(index > 95 && index < 128)
            {
                // we skipped 32-95 in delegate table, offset index
                retValues = (Tuple<String, byte>)(delegatesVect[index-64].DynamicInvoke(numArgs));
                output = String.Format("(${0:X2}),{01}", lowNibble, retValues.Item1);
                numArgs = (byte)(retValues.Item2 + 1);
            }

            return output;
        }

        #endregion VEJ_Handlers

        #region UI Interface

        #region RAM Read/Write

        /// <summary>
        /// Read value at address from RAM bank ME1
        /// </summary>
        /// <param name="address"></param>
        /// <returns>Byte value at address</returns>
        public byte ReadRAM_ME0(ushort address)
        {
            return RAM_ME0[address];
        }

        /// <summary>
        /// Read value at address from RAM bank ME1
        /// </summary>
        /// <param name="address"></param>
        /// <returns>Byte value at address</returns>
        public byte ReadRAM_ME1(ushort address)
        {
            return RAM_ME1[address];
        }

        /// <summary>
        /// Write new byte value to address in RAM bank ME0
        /// </summary>
        /// <param name="address">Address to write to</param>
        /// <param name="value">Byte value</param>
        public void WriteRAM_ME0(ushort address, byte value)
        {
            RAM_ME0[address] = value;
        }

        /// <summary>
        /// Write new byte values starting at address in RAM bank ME0
        /// </summary>
        /// <param name="address">Starting address</param>
        /// <param name="values">Byte array to write</param>
        public void WriteRAM_ME0(ushort address, byte[] values)
        {
            for (int i=0; i < values.Length; i++)
            {
                RAM_ME0[address + i] = values[i];
            }
        }

        /// <summary>
        /// Write new byte value to address in RAM bank ME0
        /// </summary>
        /// <param name="address">Address to write to</param>
        /// <param name="value">Byte value</param>
        public void WriteRAM_ME1(ushort address, byte value)
        {
            RAM_ME1[address] = value;
        }

        /// <summary>
        /// Write new byte values starting at address in RAM bank ME0
        /// </summary>
        /// <param name="address">Starting address</param>
        /// <param name="values">Byte array to write</param>
        public void WriteRAM_ME1(ushort address, byte[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                RAM_ME1[address + i] = values[i];
            }
        }

        #endregion RAM Read/Write

        /// <summary>
        /// Run code at current progrma counter 
        /// Single step if SingleStep is set
        /// </summary>
        public void Run(ushort start, ushort end)
        {
            disSB.Clear();  // clear the stringbuilder
            P = start;

            FindNextSegment(P);    // find segment for starting address

            do
            {
                // Find next segment if past current segment end.
                if (P > SegmentEnd)
                {
                    FindNextSegment(P);
                    disSB.Append(String.Format("{0}", Environment.NewLine)); // blank line between segments
                }
                ushort stop = Math.Min(SegmentEnd, end);

                // if we have a label for this address output it
                if (addressLabels && lableDict.ContainsKey(P)) { LableDump(lableDict[P]); }

                // Choose handler based on SEGMENT mode
                switch (SegmentMode)
                {
                    case SEGMENT.CODE:
                        CodeSegmentHandler(stop);
                        break;

                    // dump from start/P to lesser of end of fragment or end requested range
                    case SEGMENT.BYTE:
                        ByteSegmentHandler(stop);
                        break;

                    // dump from start/P to lesser of end of fragment or end requested range
                    case SEGMENT.WORD:
                        WordSegmentHandler(stop);
                        break;

                    // dump from start/P to lesser of end of fragment or end requested range
                    case SEGMENT.TEXT:
                        TextSegmentHandler(stop);
                        break;

                    // skip from start/P to lesser of end of fragment or end requested range
                    case SEGMENT.SKIP:
                        SkipSegmentHandler(stop);
                        break;
                }

            } while (P < (end + 1));
        }

        /// <summary>
        /// Simple method to provide a dissasembly
        /// as a large string for display in a textbox
        /// </summary>
        /// <returns></returns>
        public string DisDump()
        {
            return disSB.ToString();
        }

        #endregion UI Interface


        #region SEGMENT handlers

        /// <summary>
        /// Handles CODE segments, disassembles given segment range
        /// </summary>
        /// Listformat handled by individual opcode functions
        /// <param name="end"></param>
        private void CodeSegmentHandler(ushort stop)
        {
            while (P <= stop)
            {
                byte opcode = RAM_ME0[P];   // grab next opcode
                delegatesTbl1[opcode].DynamicInvoke();
            }
        }

        /// <summary>
        /// Handles BYTE segments
        /// </summary>
        /// <param name="end"></param>
        private void ByteSegmentHandler(ushort stop)
        {
            while (P <= stop )
            {
                int bytesLeft = stop - P + 1;
                if (listFormat) { LineDump(P, Math.Min(bytesLeft, 8)); } // dump bytes
                disSB.Append(".BYTE  ");

                // two sets of 8 bytes in hex
                int lineCount = 0;
                while (lineCount < 8 && P <= stop)
                {
                    disSB.Append(String.Format("${0:X2},", RAM_ME0[P]));
                    lineCount++;
                    P++;
                }
                disSB.Replace(",", "", disSB.Length-1, 1);
                disSB.Append(String.Format("{0}", Environment.NewLine));
            }
        }

        /// <summary>
        /// Handles WORD segments
        /// </summary>
        /// <param name="end"></param>
        private void WordSegmentHandler(ushort stop)
        {
            while (P <= stop)
            {
                int bytesLeft = stop - P + 1;
                if (listFormat) { LineDump(P, Math.Min(bytesLeft, 8)); } // dump bytes
                disSB.Append(".WORD  ");

                // two sets of 8 bytes in hex
                int lineCount = 0;
                while (lineCount < 8 && P <= stop)
                {
                    ushort word = (ushort)((RAM_ME0[P] << 8) & RAM_ME0[P + 1]);
                    disSB.Append(String.Format("${0:X4},", word));
                    lineCount++;
                    P++;
                }
                disSB.Replace(",", "", disSB.Length - 1, 1);
                disSB.Append(String.Format("{0}", Environment.NewLine));
            }
        }

        /// <summary>
        /// Handles TEXT segments
        /// </summary>
        /// <param name="end"></param>
        private void TextSegmentHandler(ushort stop)
        {
            if (listFormat) { LineDump(P, stop - P + 1); } // dump bytes
            disSB.Append(".TEXT  \"");

            while (P <= stop)
            {
                disSB.Append(String.Format("{0}", (char)(RAM_ME0[P])));
                P++;
            }

            disSB.Append(String.Format("\"{0}", Environment.NewLine));

        }

        /// <summary>
        /// Handles SKIP segments, shows skip comment, adjust P
        /// </summary>
        /// <param name="end"></param>
        private void SkipSegmentHandler(ushort stop)
        {
            while (P <= stop)
            {
                disSB.Append(String.Format(";${0:X4} to ${1:X4} skipped", P, stop));
                P = (ushort)(stop + 1); // make sure not over ushort range
            }
        }

        #endregion SEGMENT handlers


        #region OPCODES 0x00-0xFF

        #region Opcodes_0x00-0x0F

        /// <summary>
        /// SBC XL
        /// A = A - XL
        /// Opcode 00, Bytes 1
        /// </summary>
        public void SBC_XL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x00]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  XL{0}", Environment.NewLine));
        }

        /// <summary>
        /// SBC (X)
        /// A = A - (X)
        /// Opcode 01, Bytes 1
        /// </summary>
        public void SBC_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x01]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  (X){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// ADC XL
        /// A = A + XL
        /// Opcode 0x02, Bytes 1
        /// </summary>
        private void ADC_XL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x02]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  XL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ADC (X)
        /// A = A + (X)
        /// Opcode 0x03, Bytes 1
        /// </summary>
        private void ADC_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x03]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  (X){0}", Environment.NewLine));
        }

        /// <summary>
        /// LDA XL
        /// A = XL
        /// Opcode 0x04, Bytes 1
        /// </summary>
        private void LDA_XL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x04]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  XL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDA (X)
        /// A = (X)
        /// Opcode 0x05, Bytes 1
        /// </summary>
        private void LDA_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x05]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  (X){0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// CPA XL
        /// Opcode 06, Bytes 1
        /// Compare of A + XL
        /// </summary>
        private void CPA_XL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x06]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  XL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// CPA (X)
        /// Opcode 07, Bytes 1
        /// Compare A + (X)
        /// </summary>
        private void CPA_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x07]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  (X){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// STA XH
        /// XH = A 
        /// Opcode 0x08, Bytes 1
        /// </summary>
        private void STA_XH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x08]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  XH{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// AND (X)
        /// A = A & (X) 
        /// Opcode 0x09, Bytes 1
        /// </summary>
        private void AND_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x09]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("AND  (X){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// STA XL
        /// XL = A 
        /// Opcode 0x0A, Bytes 1
        /// </summary>
        private void STA_XL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x0A]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  XL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// ORA (X)
        /// A = A | (X)
        /// Opcode 0B, Bytes 1
        /// </summary>
        private void ORA_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x0B]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ORA  (X){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// DCS (X)
        /// A = A - (X) BCD Subtraction
        /// Opcode 0C, Bytes 1
        /// </summary>
        private void DCS_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x0C]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCS  (X){0}", Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// EOR (X)
        /// A = A ^ (X)
        /// Opcode 0D, Bytes 1
        /// </summary>
        private void EOR_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x0D]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("EOR  (X){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// STA (X)
        /// (X) = A 
        /// Opcode 0x0E, Bytes 1
        /// </summary>
        private void STA_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x0E]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  (X){0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// BIT (X)
        /// ZFLAG = A & (X)
        /// Opcode 0F, Bytes 1
        /// </summary>
        /// result = A & value at address REG.X.R
        private void BIT_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x0F]); } // dump bytes

            P += 1; // advance Program Counter
             disSB.Append(String.Format("BIT  (X){0}", Environment.NewLine));
            //tick += 7;
        }

        #endregion Opcodes_0x00-0x0F

        #region Opcodes_0x10-0x1F

        /// <summary>
        /// SBC YL
        /// A = A - YL
        /// Opcode 10, Bytes 1
        /// </summary>
        public void SBC_YL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x10]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  YL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// SBC (Y)
        /// A = A - (Y)
        /// Opcode 11, Bytes 1
        /// </summary>
        public void SBC_Y_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x11]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  (Y){0}", Environment.NewLine));
            // tick += 7;
        }

        /// <summary>
        /// ADC YL
        /// A = A + YL
        /// Opcode 12, Bytes 1
        /// </summary>
        private void ADC_YL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x12]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  YL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ADC (Y)
        /// A = A + (Y)
        /// Opcode 13, Bytes 1
        /// </summary>
        private void ADC_Y_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x13]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  (Y){0}", Environment.NewLine));
        }

        /// <summary>
        /// LDA YL
        /// A = YL
        /// Opcode 0x14, Bytes 1
        /// </summary>
        private void LDA_YL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x14]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  YL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDA (Y)
        /// A = (Y)
        /// Opcode 0x15, Bytes 1
        /// </summary>
        private void LDA_Y_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x15]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  (Y){0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// CPA YL
        /// Opcode 16, Bytes 1
        /// Compare of A + YL
        /// </summary>
        private void CPA_YL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x16]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  YL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// CPA (Y)
        /// Opcode 17, Bytes 1
        /// Compare A + (Y)
        /// </summary>
        private void CPA_Y_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x17]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  (Y){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// STA YH
        /// YH = A 
        /// Opcode 0x18, Bytes 1
        /// </summary>
        private void STA_YH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x18]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  YH{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// AND (Y)
        /// A = A & (Y) 
        /// Opcode 0x19, Bytes 1
        /// </summary>
        private void AND_Y_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x19]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("AND  (Y){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// STA YL
        /// YL = A 
        /// Opcode 0x1A, Bytes 1
        /// </summary>
        private void STA_YL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x1A]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  YL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// ORA (Y)
        /// A = A | (Y)
        /// Opcode 1B, Bytes 1
        /// </summary>
        private void ORA_Y_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x1B]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ORA  (Y){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// DCS (Y)
        /// A = A - (Y) BCD Subtraction
        /// Opcode 1C, Bytes 1
        /// </summary>
        private void DCS_Y_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x1C]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCS  (Y){0}", Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// EOR (Y)
        /// A = A ^ (Y)
        /// Opcode 1D, Bytes 1
        /// </summary>
        private void EOR_Y_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x1D]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("EOR  (Y){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// STA (Y)
        /// (Y) = A 
        /// Opcode 0x1E, Bytes 1
        /// </summary>
        private void STA_Y_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x1E]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  (Y){0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// BIT (Y)
        /// ZFLAG = A & (Y)
        /// Opcode 1F, Bytes 1
        /// </summary>
        private void BIT_Y_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x1F]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("BIT  (Y){0}", Environment.NewLine));
            //tick += 7;
        }

        #endregion Opcodes_0x10-0x1F

        #region Opcodes_0x20-0x2F

        /// <summary>
        /// SBC UL
        /// A = A - UL
        /// Opcode 20, Bytes 1
        /// </summary>
        public void SBC_UL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x20]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  UL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// SBC (U)
        /// A = A - (U)
        /// Opcode 21, Bytes 1
        /// </summary>
        public void SBC_U_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x21]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  (U){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// ADC UL
        /// A = A + UL
        /// Opcode 22, Bytes 1
        /// </summary>
        private void ADC_UL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x22]); } // dump b

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  UL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ADC (U)
        /// A = A + (U)
        /// Opcode 23, Bytes 1
        /// </summary>
        private void ADC_U_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x23]); } // dump b

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  (U){0}", Environment.NewLine));
        }

        /// <summary>
        /// LDA UL
        /// A = UL
        /// Opcode 0x24, Bytes 1
        /// </summary>
        private void LDA_UL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x24]); } // dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  UL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDA (U)
        /// A = (U)
        /// Opcode 0x25, Bytes 1
        /// </summary>
        private void LDA_U_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x25]); } // dump b

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  (U){0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// CPA UL
        /// Opcode 26, Bytes 1
        /// Compare of A + UL
        /// </summary>
        private void CPA_UL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x26]); } // dump b

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  UL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// CPA (U)
        /// Opcode 27, Bytes 1
        /// Compare A + (U)
        /// </summary>
        private void CPA_U_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x27]); } // dump b

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  (U){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// STA UH
        /// UH = A 
        /// Opcode 0x28, Bytes 1
        /// </summary>
        private void STA_UH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x28]); } // dump b

            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  UH{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// AND (U)
        /// A = A & (U) 
        /// Opcode 0x29, Bytes 1
        /// </summary>
        private void AND_U_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x29]); } // dump b

            P += 1; // advance Program Counter
            disSB.Append(String.Format("AND  (U){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// STA UL
        /// UL = A 
        /// Opcode 0x2A, Bytes 1
        /// </summary>
        private void STA_UL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x2A]); } // dump b

            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  UL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// ORA (U)
        /// A = A | (U)
        /// Opcode 2B, Bytes 1
        /// </summary>
        private void ORA_U_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x2B]); } // dump b

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ORA  (U){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// DCS (U)
        /// A = A - (U) BCD Subtraction
        /// Opcode 2C, Bytes 1
        /// </summary>
        private void DCS_U_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x2C]); } // dump b

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCS  (U){0}", Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// EOR (U)
        /// A = A ^ (U)
        /// Opcode 2D, Bytes 1
        /// </summary>
        private void EOR_U_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x2D]); } // dump b

            P += 1; // advance Program Counter
            disSB.Append(String.Format("EOR  (U){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// STA (U)
        /// (U) = A 
        /// Opcode 0x2E, Bytes 1
        /// </summary>
        private void STA_U_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x2E]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  (U){0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// BIT (U)
        /// ZFLAG = A & (U)
        /// Opcode 2F, Bytes 1
        /// </summary>
        private void BIT_U_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x2F]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("BIT  (U){0}", Environment.NewLine));
            //tick += 7;
        }

        #endregion Opcodes_0x20-0x2F

        #region Opcodes_0x30-0x3F

        /// <summary>
        /// SBC VL
        /// A = A - VL
        /// Opcode 30, Bytes 1
        /// </summary>
        public void SBC_VL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x30]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  VL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// SBC (V)
        /// A = A - (V)
        /// Opcode 31, Bytes 1
        /// </summary>
        public void SBC_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x31]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  (V){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// ADC VL
        /// A = A + VL
        /// Opcode 32, Bytes 1
        /// </summary>
        private void ADC_VL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x32]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  VL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ADC (V)
        /// A = A + (V)
        /// Opcode 33, Bytes 1
        /// </summary>
        private void ADC_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x33]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  (V){0}", Environment.NewLine));
        }

        /// <summary>
        /// LDA VL
        /// A = VL
        /// Opcode 0x34, Bytes 1
        /// </summary>
        private void LDA_VL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x34]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  VL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDA (V)
        /// A = (V)
        /// Opcode 0x35, Bytes 1
        /// </summary>
        private void LDA_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x35]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  (V){0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// CPA VL
        /// Opcode 36, Bytes 1
        /// Compare of A + VL
        /// </summary>
        private void CPA_VL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x36]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  VL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// CPA (V)
        /// Opcode 37, Bytes 1
        /// Compare A + (V)
        /// </summary>
        /// todo: simplify
        private void CPA_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x37]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  (V){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// NOP
        /// Opcode 38, Bytes 1
        /// </summary>
        private void NOP()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x38]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("NOP{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// AND (V)
        /// A = A & V 
        /// Opcode 0x39, Bytes 1
        /// </summary>
        private void AND_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x39]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("AND  (V){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// STA VL
        /// VL = A 
        /// Opcode 0x3A, Bytes 1
        /// </summary>
        private void STA_VL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x3A]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  VL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// ORA (V)
        /// A = A | (V)
        /// Opcode 3B, Bytes 1
        /// </summary>
        private void ORA_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x3B]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ORA  (V){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// DCS (V)
        /// A = A - (V) BCD Subtraction
        /// Opcode 3C, Bytes 1
        /// </summary>
        /// todo: simplify
        private void DCS_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x3C]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCS  (V){0}", Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// EOR (V)
        /// A = A ^ (V)
        /// Opcode 3D, Bytes 1
        /// </summary>
        /// todo: simplify
        private void EOR_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x3D]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("EOR  (V){0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// STA (V)
        /// (V) = A 
        /// Opcode 0x3E, Bytes 1
        /// </summary>
        private void STA_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x3E]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  (V){0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// BIT (V)
        /// ZFLAG = A & (V)
        /// Opcode 3F, Bytes 1
        /// </summary>
        /// todo: simplify
        private void BIT_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x3F]); } // hex dump

            P += 1; // advance Program Counter
            disSB.Append(String.Format("BIT  (V){0}", Environment.NewLine));
            //tick += 7;
        }

        #endregion Opcodes_0x30-0x3F

        #region Opcodes_0x40-0x4F

        /// <summary>
        /// INC XL
        /// XL = XL + 1
        /// Opcode 40, Bytes 1
        /// </summary>
        private void INC_XL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x40]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  XL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// SIN X
        /// (X) = A then X = X + 1
        /// Opcode 41, Bytes 1
        /// </summary>
        private void SIN_X()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x41]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SIN  X{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// DEC XL
        /// XL = XL - 1
        /// Opcode 42, Bytes 1
        /// </summary>
        private void DEC_XL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x42]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  XL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// SDE X
        /// (X) = A then X = X - 1
        /// Opcode 43, Bytes 1
        /// </summary>
        private void SDE_X()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x43]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SDE  X{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// INC X
        /// X = X + 1
        /// Opcode 44, Bytes 1
        /// </summary>
        private void INC_X()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x44]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  X{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDI X
        /// A = (X) then INC X
        /// Opcode 45, Bytes 1
        /// </summary>
        public void LIN_X()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x45]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LIN  X{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// DEC X
        /// X = X - 1
        /// Opcode 46, Bytes 1
        /// </summary>
        private void DEC_X()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x46]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  X{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDE X
        /// A = (X) then X = X - 1
        /// Opcode 0x47, Bytes 1
        /// </summary>
        private void LDE_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x47]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDE  X{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// LDI XH
        /// XH = n
        /// Opcode 48, Bytes 2
        /// </summary>
        public void LDI_XH_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x48]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("LDI  XH,${0:X2}{1}", value, Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ANI (X),n
        /// (X) = (X) & n  
        /// Opcode 0x49, Bytes 2
        /// </summary>
        private void ANI_X_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x49]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ANI  (X),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// LDI XL
        /// XL = n
        /// Opcode 4A, Bytes 2
        /// </summary>
        public void LDI_XL_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x4A]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P +=1
            disSB.Append(String.Format("LDI  XL,${0}{1}", value.ToString("X2"), Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ORI (X),n
        /// (X) = (X) | n
        /// Opcode 4B, Bytes 2
        /// </summary>
        private void ORI_X_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x4B]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte();
            disSB.Append(String.Format("ORI  (X),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// CPI XH,n
        /// Opcode 4C, Bytes 2
        /// Compare of XH + n
        /// </summary>
        private void CPI_XH_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x4C]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("CPI  XH,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// BII (X),n
        /// FLAGS = (X) & n
        /// Opcode 4D, Bytes 2
        /// </summary>
        private void BII_X_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x4D]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = this.GetByte(); // P += 1
            disSB.Append(String.Format("BII  (X),${0:X2}{1}", value, Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// CPI XL,n
        /// Opcode 4E, Bytes 2
        /// Compare of XL + n
        /// </summary>
        private void CPI_XL_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x4E]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("CPI  XL,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// ADI (X),n
        /// (X) = (X) + n
        /// Opcode 4F, Bytes 2
        /// </summary>
        private void ADI_X_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x4F]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ADI  (X),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        #endregion Opcodes_0x40-0x4F

        #region Opcodes_0x50-0x5F

        /// <summary>
        /// INC YL
        /// YL = YL + 1
        /// Opcode 50, Bytes 1
        /// </summary>
        private void INC_YL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x50]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  YL{0}", Environment.NewLine));
        }

        /// <summary>
        /// SIN Y
        /// (Y) = A then Y = Y + 1
        /// Opcode 51, Bytes 1
        /// </summary>
        private void SIN_Y()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x51]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SIN  Y{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// DEC YL
        /// YL = YL - 1
        /// Opcode 52, Bytes 1
        /// </summary>
        private void DEC_YL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x52]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  YL{0}", Environment.NewLine));
            tick += 5;
        }

        /// <summary>
        /// SDE Y
        /// (Y) = A then Y = Y - 1
        /// Opcode 53, Bytes 1
        /// </summary>
        private void SDE_Y()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x53]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SDE  Y{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// INC Y
        /// Y = Y + 1
        /// Opcode 54, Bytes 1
        /// </summary>
        private void INC_Y()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x54]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  Y{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LIN Y
        /// A = (Y) then INC Y
        /// Opcode 55, Bytes 1
        /// </summary>
        public void LIN_Y()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x55]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LIN  Y{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// DEC Y
        /// Y = Y - 1
        /// Opcode 56, Bytes 1
        /// </summary>
        private void DEC_Y()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x56]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  Y{0}", Environment.NewLine));
            tick += 5;
        }

        /// <summary>
        /// LDE Y
        /// A = (Y) then Y = Y - 1
        /// Opcode 0x57, Bytes 1
        /// </summary>
        private void LDE_Y()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x57]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDE  Y{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// LDI YH
        /// YH = n
        /// Opcode 58 Bytes 2
        /// </summary>
        public void LDI_YH_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x58]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("LDI  YH,${0:X2}{1}", value, Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ANI (Y),n
        /// (Y) = (Y) & n  
        /// Opcode 0x59, Bytes 2
        /// </summary>
        private void ANI_Y_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x59]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ANI  (Y),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// LDI YL,n
        /// YL = n
        /// Opcode 5A, Bytes 2
        /// </summary>
        public void LDI_YL_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x5A]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("LDI  YL,${0:X2}{1}", value, Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ORI (Y),n
        /// (Y) = (Y) | n
        /// Opcode 5B, Bytes 2
        /// </summary>
        private void ORI_Y_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x5B]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte();
            disSB.Append(String.Format("ORI  (Y),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// CPI YH,n
        /// Opcode 5C, Bytes 2
        /// Compare of YH + n
        /// </summary>
        private void CPI_YH_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x5C]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("CPI  YH,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// BII (Y),n
        /// FLAGS = (Y) & n
        /// Opcode 5D, Bytes 2
        /// </summary>
        private void BII_Y_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x5D]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = this.GetByte(); // P += 1
            disSB.Append(String.Format("BII  (Y),${0:X2}{1}", value, Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// CPI YL,n
        /// Opcode 5E, Bytes 2
        /// Compare of YL + n
        /// </summary>
        private void CPI_YL_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x5E]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("CPI  YL,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// ADI (Y),n
        /// (Y) = (Y) + n
        /// Opcode 5F, Bytes 2
        /// </summary>
        private void ADI_Y_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x5F]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ADI  (Y),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        #endregion Opcodes_0x50-0x5

        #region Opcodes_0x60-0x6F

        /// <summary>
        /// INC UL
        /// UL = UL + 1
        /// Opcode 60, Bytes 1
        /// </summary>
        private void INC_UL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x60]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  UL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// SIN U
        /// (U) = A then U = U + 1
        /// Opcode 61, Bytes 1
        /// </summary>
        private void SIN_U()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x61]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SIN  U{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// DEC UL
        /// UL = UL - 1
        /// Opcode 62, Bytes 1
        /// </summary>
        private void DEC_UL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x62]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  UL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// SDE U
        /// (U) = A then U = U - 1
        /// Opcode 63, Bytes 1
        /// </summary>
        private void SDE_U()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x63]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SDE  U{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// INC U
        /// U = U + 1
        /// Opcode 64, Bytes 1
        /// </summary>
        private void INC_U()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x64]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  U{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDI U
        /// A = (U) then INC U
        /// Opcode 65, Bytes 1
        /// </summary>
        public void LIN_U()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x65]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LIN  U{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// DEC U
        /// U = U - 1
        /// Opcode 66, Bytes 1
        /// </summary>
        private void DEC_U()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x66]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  U{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDE U
        /// A = (U) then U = U - 1
        /// Opcode 0x67, Bytes 1
        /// </summary>
        private void LDE_U()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x67]); } // dump bytes

            P  += 1; // advance Program Counter
            disSB.Append(String.Format("LDE  U{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// LDI UH,n
        /// UH = n
        /// Opcode 68, Bytes 2
        /// </summary>
        public void LDI_UH_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x68]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("LDI  UH,${0:X2}{1}", value, Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ANI (U),n
        /// (U) = (U) & n  
        /// Opcode 0x69, Bytes 2
        /// </summary>
        private void ANI_U_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x69]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ANI  (U),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// LDI UL,n
        /// UL = n
        /// Opcode 6A, Bytes 2
        /// </summary>
        public void LDI_UL_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x6A]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("LDI  UL,${0:X2}{1}", value, Environment.NewLine));
            tick += 6;
        }

        /// <summary>
        /// ORI (U),n
        /// (U) = (U) | n
        /// Opcode 6B, Bytes 2
        /// </summary>
        private void ORI_U_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x6B]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte();
            disSB.Append(String.Format("ORI  (U),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// CPI UH,n
        /// Opcode 6C, Bytes 2
        /// Compare of UH + n
        /// </summary>
        private void CPI_UH_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x6C]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("CPI  UH,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// BII (U),n
        /// FLAGS = (U) & n
        /// Opcode 6D, Bytes 2
        /// </summary>
        private void BII_U_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x6D]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("BII  (U),${0:X2}{1}", value, Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// CPI UL,n
        /// Opcode 6E, Bytes 2
        /// Compare of UL + n
        /// </summary>
        private void CPI_UL_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x6E]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("CPI  UL,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// ADI (U),n
        /// (U) = (U) + n
        /// Opcode 6F, Bytes 2
        /// </summary>
        private void ADI_U_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x6F]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ADI  (U),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        #endregion Opcodes_0x60-0x6F

        #region Opcodes_0x70-0x7F

        /// <summary>
        /// INC VL
        /// VL = VL + 1
        /// Opcode 70, Bytes 1
        /// </summary>
        private void INC_VL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x70]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  VL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// SIN V
        /// (V) = A then V = V + 1
        /// Opcode 71, Bytes 1
        /// </summary>
        private void SIN_V()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x71]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SIN  V{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// DEC VL
        /// VL = VL - 1
        /// Opcode 72, Bytes 1
        /// </summary>
        private void DEC_VL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x72]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  VL{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// SDE V
        /// (V) = A then V = V - 1
        /// Opcode 73, Bytes 1
        /// </summary>
        private void SDE_V()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x73]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SDE  V{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// INC V
        /// V = V + 1
        /// Opcode 74, Bytes 1
        /// </summary>
        private void INC_V()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x74]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  V{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDI V
        /// A = (V) then INC V
        /// Opcode 75, Bytes 1
        /// </summary>
        public void LIN_V()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x75]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LIN  V{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// DEC V
        /// V = V - 1
        /// Opcode 76, Bytes 1
        /// </summary>
        private void DEC_V()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x76]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  V{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDE V
        /// A = (V) then V = V - 1
        /// Opcode 77, Bytes 1
        /// </summary>
        /// todo: simplify
        private void LDE_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x77]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDE  V{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// LDI VH,n
        /// VH = n
        /// Opcode 78, Bytes 2
        /// </summary>
        /// todo: simplify
        public void LDI_VH_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x78]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("LDI  VH,${0:X2}{1}", value, Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ANI (V),n
        /// (V) = (V) & n  
        /// Opcode 0x79, Bytes 2
        /// </summary>
        /// todo: simplify
        private void ANI_V_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x79]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ANI  (V),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// LDI VL,n
        /// VL = n
        /// Opcode 7A, Bytes 2
        /// </summary>
        /// todo: simplify
        public void LDI_VL_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x7A]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("LDI  VL,${0:X2}{1}", value, Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ORI (V),n
        /// (V) = (V) | n
        /// Opcode 7B, Bytes 2
        /// </summary>
        /// todo: simplify
        private void ORI_V_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x7B]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte();
            disSB.Append(String.Format("ORI  (V),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// CPI VH,n
        /// Opcode 7C, Bytes 2
        /// Compare of VH + n
        /// </summary>
        /// todo: simplify
        private void CPI_VH_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x7C]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("CPI  VH,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// BII (V),n
        /// FLAGS = (V) & n
        /// Opcode 7D, Bytes 2
        /// </summary>
        /// todo: simplify
        private void BII_V_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x7D]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = this.GetByte(); // P += 1
            disSB.Append(String.Format("BII  (V),${0:X2}{1}", value, Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// CPI VL,n
        /// Opcode 7E, Bytes 2
        /// Compare of VL + n
        /// </summary>
        /// todo: simplify
        private void CPI_VL_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x7E]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("CPI  VL,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// ADI (V),n
        /// (V) = (V) + n
        /// Opcode 7F, Bytes 2
        /// </summary>
        /// todo: simplify
        private void ADI_V_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x7F]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ADI  (V),${0:X2}{1}", value, Environment.NewLine));
            //tick += 13;
        }

        #endregion Opcodes_0x70-0x7F

        #region Opcodes_0x80-0x8F

        /// <summary>
        /// SBC XH
        /// A = A - XH
        /// Opcode 80, Bytes 1
        /// </summary>
        public void SBC_XH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x80]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  XH{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// BCR+e
        /// Branch if Carry reset forward+
        /// Opcode 81, Bytes 2
        /// </summary>
        private void BCR_n_p()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x81]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P + value);
            disSB.Append(String.Format("BCR+ {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// ADC XH
        /// A = A + XH
        /// Opcode 82, Bytes 1
        /// </summary>
        private void ADC_XH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x82]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  XH{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// BCS+e
        /// Branch if Carry set forward+
        /// Opcode 83, Bytes 2
        /// </summary>
        private void BCS_n_p()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x83]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P + value);
            disSB.Append(String.Format("BCS+ {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8; 
        }

        /// <summary>
        /// LDA XH
        /// A = XH
        /// Opcode 0x84, Bytes 1
        /// </summary>
        private void LDA_XH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x84]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  XH{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// BHR+e
        /// Branch if Half Carry reset forward+
        /// Opcode 85, Bytes 2
        /// </summary>
        private void BHR_n_p()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x85]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P + value);
            disSB.Append(String.Format("BHR+ {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// CPA XH
        /// Opcode 86, Bytes 1
        /// Compare of A + XH
        /// </summary>
        private void CPA_XH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x86]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  XH{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// BHS+e
        /// Branch if Half Carry set forward+
        /// Opcode 87, Bytes 2
        /// </summary>
        private void BHS_n_p()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x87]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P + value);
            disSB.Append(String.Format("BHS+ {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// LOP UL,e
        /// UL = UL - 1, loop back 'e' if Borrow/Carry Flag not set
        /// Opcode 88, Bytes 2
        /// </summary>
        private void LOP_UL_e()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x88]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P - value);
            disSB.Append(String.Format("LOP UL,{0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// BZR+e
        /// Branch if Zero reset forward+
        /// Opcode 89, Bytes 2
        /// </summary>
        private void BZR_n_p()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x89]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P + value);
            disSB.Append(String.Format("BZR+ {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// RTI
        /// Return from interrupt
        /// Opcode 8A, Bytes 1
        /// </summary>
        private void RTI()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x8A]); } // dump bytes

            P += 1;  // advance Program Counter
            disSB.Append(String.Format("RTI{0}", Environment.NewLine));
            //tick += 14;
        }

        /// <summary>
        /// BZS+e
        /// Branch if Zero set forward+
        /// Opcode 8B, Bytes 2
        /// </summary>
        private void BZS_n_p()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x8B]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P + value);
            disSB.Append(String.Format("BZS+ {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// DCA (X)
        /// A = A + (X) BCD Addition
        /// Opcode 8C, Bytes 1
        /// </summary>
        private void DCA_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x8C]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCA  (X){0}", Environment.NewLine));
            //tick += 15;
        }

        /// <summary>
        /// BVR+e
        /// Branch if Overflow reset forward+
        /// Opcode 8D, Bytes 2
        /// </summary>
        private void BVR_n_p()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x8D]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P + value);
            disSB.Append(String.Format("BVR+ {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// BCH+e
        /// Branch unconditional forward+
        /// Opcode 8E, Bytes 2
        /// </summary>
        private void BCH_n_p()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x8E]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P + value);
            disSB.Append(String.Format("BCH+ {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// BVS+e
        /// Branch if Overlow set forward+
        /// Opcode 8F, Bytes 2
        /// </summary>
        private void BVS_n_p()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x8F]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P + value);
            disSB.Append(String.Format("BVS+ {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        #endregion Opcodes_0x80-0x8F

        #region Opcodes_0x90-0x9F

        /// <summary>
        /// SBC YH
        /// A = A - YH
        /// Opcode 90, Bytes 1
        /// </summary>
        public void SBC_YH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x90]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  YH{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// BCR-e
        /// Branch if Carry reset back-
        /// Opcode 91, Bytes 2
        /// </summary>
        private void BCR_n_m()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x91]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P - value);
            disSB.Append(String.Format("BCR- {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8; // three extra cycles to take back branch
        }

        /// <summary>
        /// ADC YH
        /// A = A + YH
        /// Opcode 92, Bytes 1
        /// </summary>
        private void ADC_YH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x92]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  YH{0}", Environment.NewLine));
            // tick += 6;
        }

        /// <summary>
        /// BCS-e
        /// Branch if Carry set back-
        /// Opcode 93, Bytes 2
        /// </summary>
        private void BCS_n_m()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x93]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P - value);
            disSB.Append(String.Format("BCS- {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// LDA YH
        /// A = YH
        /// Opcode 0x94, Bytes 1
        /// </summary>
        private void LDA_YH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x94]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  YH{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// BHR-e
        /// Branch if Half Carry reset back-
        /// Opcode 95, Bytes 2
        /// </summary>
        private void BHR_n_m()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x95]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P - value);
            disSB.Append(String.Format("BHR- {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// CPA YH
        /// Opcode 96, Bytes 1
        /// Compare of A + YH
        /// </summary>
        private void CPA_YH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x96]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  YH{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// BHS-e
        /// Branch if Half Carry set back-
        /// Opcode 97, Bytes 2
        /// </summary>
        private void BHS_n_m()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x97]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P - value);
            disSB.Append(String.Format("BHS- {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// BZR-e
        /// Branch if Zero reset back-
        /// Opcode 99, Bytes 2
        /// </summary>
        private void BZR_n_m()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x99]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P - value);
            disSB.Append(String.Format("BZR- {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// RTN
        /// Return from subroutine
        /// Opcode 9A, Bytes 1
        /// </summary>
        private void RTN()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x9A]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("RTN{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// BZS-e
        /// Branch if Zero set back
        /// Opcode 9B, Bytes 2
        /// </summary>
        private void BZS_n_m()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x9B]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P - value);
            disSB.Append(String.Format("BZS- {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// DCA (Y)
        /// A = A + (Y) BCD Addition
        /// Opcode 9C, Bytes 1
        /// </summary>
        private void DCA_Y_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x9C]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCA  (Y){0}", Environment.NewLine));
            //tick += 15;
        }

        /// <summary>
        /// BVR-e
        /// Branch if Overflow reset back-
        /// Opcode 9D, Bytes 2
        /// </summary>
        private void BVR_n_m()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x9D]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P - value);
            disSB.Append(String.Format("BVR- {0}{1}", GetAddOrLabel(address), Environment.NewLine));
           // tick += 8;
        }

        /// <summary>
        /// BCH-e
        /// Branch unconditional back-
        /// Opcode 9E, Bytes 2
        /// </summary>
        private void BCH_n_m()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x9E]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P - value);
            disSB.Append(String.Format("BCH- {0}{1}", GetAddOrLabel(address), Environment.NewLine));
           //tick += 9;
        }

        /// <summary>
        /// BVS-e
        /// Branch if Overlow set back-
        /// Opcode 9F, Bytes 2
        /// </summary>
        private void BVS_n_m()
        {
            if (listFormat) { LineDump(P, opBytesP1[0x9F]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            ushort address = (ushort)(P - value);
            disSB.Append(String.Format("BVS- {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 8;
        }

        #endregion Opcodes_0x90-0x9F

        #region Opcodes_0xA0-0xAF

        /// <summary>
        /// SBC UH
        /// A = A - UH
        /// Opcode A0, Bytes 1
        /// </summary>
        public void SBC_UH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xA0]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  UH{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// SBC (pp)
        /// A = A - (pp)
        /// Opcode A1, Bytes 3
        /// </summary>
        public void SBC_pp_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xA1]); } // dump bytes

            P += 1;                       // advance Program Counter
            ushort address = GetWord();   // P += 2
            disSB.Append(String.Format("SBC  ({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// ADC UH
        /// A = A + UH
        /// Opcode A2, Bytes 1
        /// </summary>
        private void ADC_UH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xA2]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  UH{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ADC (pp)
        /// Opcode A3, Bytes 3
        /// </summary>
        private void ADC_pp_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xA3]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("ADC  ({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// LDA UH
        /// A = UH
        /// Opcode 0xA4, Bytes 1
        /// </summary>
        private void LDA_UH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xA0]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  UH{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDA (pp)
        /// A = (pp)
        /// Opcode 0xA5, Bytes 3
        /// </summary>
        private void LDA_pp_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xA5]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("LDA  ({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 12;
        }

        /// <summary>
        /// CPA UH
        /// Opcode A6, Bytes 1
        /// Compare of A + UH
        /// </summary>
        private void CPA_UH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xA6]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  UH{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// CPA (pp)
        /// Opcode A7, Bytes 3
        /// Compare A + (pp)
        /// </summary>
        private void CPA_pp_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xA7]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("CPA  ({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// SPV
        /// Opcode A8, Bytes 1
        /// Set PV flip-flop. PV controls external device selection
        /// </summary>
        private void SPV()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xA8]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SPV{0}", Environment.NewLine));
            //tick += 4;
        }

        /// <summary>
        /// AND (pp)
        /// A = A & (pp) 
        /// Opcode 0xA9, Bytes 3
        /// </summary>
        private void AND_pp_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xA9]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("AND  ({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// LDI S,pp
        /// S = pp
        /// Opcode AA, Bytes 3
        /// </summary>
        private void LDI_S_pp()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xAA]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("LDI  S,({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 12;
        }

        /// <summary>
        /// ORA (pp)
        /// A = A | (pp)
        /// Opcode AB, Bytes 3
        /// </summary>
        private void ORA_pp_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xAB]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("ORA  ({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// DCA (U)
        /// A = A + (U) BCD Addition
        /// Opcode AC, Bytes 1
        /// </summary>
        private void DCA_U_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xAC]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCA  (U){0}", Environment.NewLine));
            //tick += 15;
        }

        /// <summary>
        /// EOR (pp)
        /// A = A ^ (pp)
        /// Opcode AD, Bytes 3
        /// </summary>
        private void EOR_pp_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xAD]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2;
            disSB.Append(String.Format("EOR  ({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 13;
        }

        /// <summary>
        /// STA pp
        /// (pp) = A 
        /// Opcode 0xAE, Bytes 3
        /// </summary>
        private void STA_pp_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xAE]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("STA  ({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            // tick += 12;
        }

        /// <summary>
        /// BIT (pp)
        /// ZFLAG = A & (pp)
        /// Opcode AF, Bytes 1
        /// </summary>
        private void BIT_pp_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xAF]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("BIT  ({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 13;
        }

        #endregion Opcodes_0xA0-0xAF

        #region Opcodes_0xB0-0xBF

        /// <summary>
        /// SBC VH
        /// A = A - VH
        /// Opcode B0, Bytes 1
        /// </summary>
        public void SBC_VH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xB0]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  VH{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// SBI A,n
        /// A = A - n
        /// Opcode B1, Bytes 1
        /// </summary>
        public void SBI_A_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xB1]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte();
            disSB.Append(String.Format("SBI  A,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// ADC VH
        /// A = A + VH
        /// Opcode B2, Bytes 1
        /// </summary>
        private void ADC_VH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xB2]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  VH{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// ADI A,n
        /// A = A + n
        /// Opcode B3, Bytes 2
        /// </summary>
        private void ADI_A_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xB3]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = this.GetByte(); // P += 1
            disSB.Append(String.Format("ADI  A,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// LDA VH
        /// A = VH
        /// Opcode 0xB4, Bytes 1
        /// </summary>
        private void LDA_VH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xB4]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  VH{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// LDI A,n
        /// A = n
        /// Opcode B5, Bytes 2
        /// </summary>
        /// <param name="value"></param>
        public void LDI_A_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xB5]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("LDI  A,${0:X2}{1}", value, Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// CPA VH
        /// Opcode B6, Bytes 1
        /// Compare of A + VH
        /// </summary>
        private void CPA_VH()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xB6]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  VH{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// CPI A,n
        /// Opcode B7, Bytes 2
        /// Compare of A + n
        /// </summary>
        private void CPI_A_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xB7]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("CPI  A,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// RPV
        /// Opcode B8, Bytes 1
        /// Reset PV Flip Flip, PV controls external device selection
        /// </summary>
        private void RPV()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xB8]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("RPV{0}", Environment.NewLine));
            //tick += 4;
        }

        /// <summary>
        /// ANI A,n
        /// FLAGS = A & n
        /// Opcode B9, Bytes 2
        /// </summary>
        private void ANI_A_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xB9]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = this.GetByte(); // P += 1
            disSB.Append(String.Format("ANI  A,${0:X2}{1}", value, Environment.NewLine));
            tick += 7;
        }

        /// <summary>
        /// JMP pp
        /// Opcode BA, Bytes 2
        /// Jump to address (pp), unconditional.
        /// </summary>
        private void JMP_pp()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xBA]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("JMP  ${0}{1}", address.ToString("X4"), Environment.NewLine));
            //tick += 12;
        }

        /// <summary>
        /// ORI A,n
        /// A = A | n
        /// Opcode BB, Bytes 2
        /// </summary>
        private void ORI_A_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xBB]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte();
            disSB.Append(String.Format("ORI  A,${0:X2}{1}", value, Environment.NewLine));
            tick += 7;
        }

        /// <summary>
        /// DCA (V)
        /// A = A + (V) BCD Addition
        /// Opcode BC, Bytes 1
        /// </summary>
        /// todo: simplify
        private void DCA_V_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xBC]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCA  (V){0}", Environment.NewLine));
            //tick += 15;
        }

        /// <summary>
        /// EAI n
        /// A = A ^ n
        /// Opcode BD, Bytes 2
        /// </summary>
        private void EAI_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xBD]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("EAI  ${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// SJP (pp)
        /// Subroutine Jump (CALL)
        /// Opcode BE, Bytes 2
        /// </summary>
        private void SJP_pp()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xBE]); } // dump bytes

            P += 1;                         // advance Program Counter
            ushort address = GetWord();     // P + =2, now pointing to next instruction
            disSB.Append(String.Format("SJP  {0}{1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 19;
        }

        /// <summary>
        /// BII A,n
        /// FLAGS = A & n
        /// Opcode BF, Bytes 2
        /// </summary>
        private void BII_A_n()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xBF]); } // dump bytes

            P += 1; // advance Program Counter
            byte value = this.GetByte(); // P += 1
            disSB.Append(String.Format("BII  A,${0:X2}{1}", value, Environment.NewLine));
            //tick += 7;
        }

        #endregion Opcodes_0xB0-0xBF

        #region Opcodes_0xC0-0xCF

        /// <summary>
        /// VEJ (C0)
        /// Vectored Call, $FF C0
        /// Opcode C0, Bytes 1
        /// </summary>
        public void VEJ_C0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xC0]); } // dump bytes

            P += 1;                       // advance Program Counter
            disSB.Append(String.Format("VEJ  (C0){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// VCR n
        /// If Carry Reset do Indexed Vectored Call, $FFn, n = ($00-$F6)
        /// Opcode C1, Bytes 2
        /// </summary>
        public void VCR_n()
        {
            P += 1; // advance Program Counter
            byte numBytes = 2;
            //byte numArgs = 0;
            string args = "";

            if (disModePC1500)
            {
                args = VectorIndex(0xC1, out numBytes);
                numBytes += 1;
            }
            else
            {
                byte value = GetByte(); // P += 1
                args = String.Format("${0:X2}", value);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VCR  {0}{1}", args, Environment.NewLine));
        }

        /// <summary>
        /// VEJ (C2)
        /// Vectored Call, $FF C2
        /// Opcode C2, Bytes 1
        /// </summary>
        /// VEJ (C2),*p,b2 - For PC-1500 arguments follow this opcode
        /// If next character/token is not '*p', branch fwd 'b2'. 
        /// *If  first arg >= E0, then '*p' is a function & 2 bytes, i.e. 3 total bytes in argument.
        public void VEJ_C2()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            byte numBytes = opBytesP1[0xC2];
            String args = "";

            if (disModePC1500)
            {
                numBytes += 2;
                if (PeekByte() >= 0xE0) { numBytes += 1; } // +1 bytes if function
                retValues = VEJ_p_o1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (C2){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// VCS n
        /// If Carry Set do Indexed Vectored Call, $FFn, n = ($00-$F6)
        /// Opcode C3, Bytes n
        /// </summary>
        public void VCS_n()
        {
            P += 1; // advance Program Counter
            byte numBytes = 2;
            string args = "";

            if (disModePC1500)
            {
                args = VectorIndex(0xC3, out numBytes);
                numBytes += 1;
            }
            else
            {
                byte value = GetByte(); // P += 1
                args = String.Format("${0:X2}", value);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VCS  {0}{1}", args, Environment.NewLine));
        }

        /// <summary>
        /// VEJ (C4)
        /// Vectored Call, $FF C4
        /// Opcode C4, Bytes 1
        /// </summary>
        /// VEJ (C4),*p,b2 - For PC-1500 arguments follow this opcode
        /// If next character/token in U is not '*p', branch fwd 'b2'. 
        /// *If  first arg >= E0, then '*p' is a function & 2 bytes, i.e. 3 total bytes in argument. 
        public void VEJ_C4()
        {
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            P += 1; // advance Program Counter
            byte numBytes = opBytesP1[0xC4];
            String args = "";

            if (disModePC1500)
            {
                numBytes += 2;
                if (PeekByte() >= 0xE0) { numBytes += 1; } // +1 bytes if function
                retValues = VEJ_p_o1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (C4){0}{1}", args, Environment.NewLine));
            //tick += 17;    
        }

        /// <summary>
        /// VHR n
        /// If Half-Carry Reset do Indexed Vectored Call. $FF n. n = ($00-$F6)
        /// Opcode C5, Bytes 2
        /// </summary>
        public void VHR_n()
        {
            P += 1; // advance Program Counter
            byte numBytes = 2;
            string args = "";

            if (disModePC1500)
            {
                args = VectorIndex(0xC5, out numBytes);
                numBytes += 1;
            }
            else
            {
                byte value = GetByte(); // P += 1
                args = String.Format("${0:X2}", value);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VHR  {0}{1}", args, Environment.NewLine));
        }

        /// <summary>
        /// VEJ (C6)
        /// Vectored Call, $FF C6
        /// Opcode C6, Bytes 1
        /// </summary>
        public void VEJ_C6()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xC6]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (C6){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// VHS n
        /// If Half-Carry Set do Indexed Vectored Call. $FF n. n = ($00-$F6)
        /// Opcode C7, Bytes 2
        /// </summary>
         public void VHS_n()
        {
            P += 1; // advance Program Counter
            byte numBytes = 2;
            string args = "";

            if (disModePC1500)
            {
                args = VectorIndex(0xC7, out numBytes);
                numBytes += 1;
            }
            else
            {
                byte value = GetByte(); // P += 1
                args = String.Format("${0:X2}", value);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VHS  {0}{1}", args, Environment.NewLine));
        }

        /// <summary>
        /// VEJ (C8)
        /// Vectored Call, $FF C8
        /// Opcode C8, Bytes 1
        /// </summary>
        /// VEJ (C8),o1 - For PC-1500 arguments follow this opcode
        /// Syntax check: Jump fwd (o1) if following character doesn’t represent 
        /// end to command sequence or line. C=1 if ":"
        public void VEJ_C8()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            byte numBytes = opBytesP1[0xC8];
            String args = "";

            if (disModePC1500)
            {
                numBytes += 1;
                retValues = VEJ_o1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (C8){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// VZR n
        /// If Z Reset do Indexed Vectored Call. $FF n. n = ($00-$F6)
        /// Opcode C9, Bytes 2
        /// </summary>
        public void VZR_n()
        {
            P += 1; // advance Program Counter
            byte numBytes = 2;
            string args = "";

            if (disModePC1500)
            {
                args = VectorIndex(0xC9, out numBytes);
                numBytes += 1;
            }
            else
            {
                byte value = GetByte(); // P += 1
                args = String.Format("${0:X2}", value);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VZR  {0}{1}", args, Environment.NewLine));
        }

        /// <summary>
        /// VEJ (CA)
        /// Vectored Call, $FF CA
        /// Opcode CA, Bytes 1
        /// </summary>
        /// VEJ (CA),b1 - For PC-1500 arguments follow this opcode
        /// Transfers X to 78(b1), 78(b1+1)
        public void VEJ_CA()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            String args = "";

            byte numBytes = opBytesP1[0xCA];

            if (disModePC1500)
            {
                numBytes += 1;
                retValues = VEJ_78b1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (CA){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// VZS n
        /// If Z Set do Indexed Vectored Call. $FF n. n = ($00-$F6)
        /// Opcode CB, Bytes 2
        /// </summary>
        public void VZS_n()
        {
            P += 1; // advance Program Counter
            byte numBytes = 2;
            string args = "";

            if (disModePC1500)
            {
                args = VectorIndex(0xCB, out numBytes);
                numBytes += 1;
            }
            else
            {
                byte value = GetByte(); // P += 1
                args = String.Format("${0:X2}", value);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VZS  {0}{1}", args, Environment.NewLine));
        }

        /// <summary>
        /// VEJ (CC)
        /// Vectored Call, $FF CC
        /// Opcode CC, Bytes 1
        /// </summary>
        /// VEJ (CC),b1 - For PC-1500 arguments follow this opcode
        /// Transfers X to 78(b1), 78(b1+1)
        public void VEJ_CC()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            String args = "";

            byte numBytes = opBytesP1[0xCC];

            if (disModePC1500)
            {
                numBytes += 1;
                retValues = VEJ_78b1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (CC){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// VMJ n
        /// Unconditional Indexed Vectored Call. $FF n. n = ($00-$F6)
        /// Opcode CD, Bytes 2
        /// </summary>
        public void VMJ_n()
        {
            P += 1; // advance Program Counter
            byte numBytes = 2;
            string args = "";

            if (disModePC1500)
            {
                //VectorIndex appends 
                args = VectorIndex(0xCD, out numBytes);
                numBytes += 1;
            }
            else
            {
                byte value = GetByte(); // P += 1
                args = String.Format("${0:X2}", value);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VMJ  {0}{1}", args, Environment.NewLine));
        }

        /// <summary>
        /// VEJ (CE)
        /// Vectored Call, $FF CE
        /// Opcode CE, Bytes 1
        /// </summary>
        /// VEJ (CE),b1,o2 - For PC-1500 arguments follow this opcode
        /// Determines address of variable 'n1'  if not numeric, branch to P+n2
        public void VEJ_CE()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            String args = "";

            byte numBytes = opBytesP1[0xCE];

            if (disModePC1500)
            {
                numBytes += 2;
                retValues = VEJ_b1_o1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (CE){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// VVS n
        /// If V Set do Indexed Vectored Call.. $FF n. n = ($00-$F6)
        /// Opcode CF, Bytes 2
        /// </summary>
        public void VVS_n()
        {
            P += 1; // advance Program Counter
            byte numBytes = 2;
            string args = "";

            if (disModePC1500)
            {
                args = VectorIndex(0xCF, out numBytes);
                numBytes += 1;
            }
            else
            {
                byte value = GetByte(); // P += 1
                args = String.Format("${0:X2}", value);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VVS  {0}{1}", args, Environment.NewLine));
        }

        #endregion Opcodes_0xC0-0xCF

        #region Opcodes_0xD0-0xDF

        /// <summary>
        /// VEJ (D0)
        /// Vectored Call, $FF D0
        /// Opcode D0, Bytes 1
        /// </summary>
        /// VEJ (D0),p,n - For PC-1500 arguments follow this opcode
        /// Convert AR-X to integer & load to U-Reg.  If range 'p' exceeded, then branch fwd n
        public void VEJ_D0()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            String args = "";

            byte numBytes = opBytesP1[0xD0];

            if (disModePC1500)
            {
                numBytes += 2;
                retValues = VEJ_b1_o1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (D0){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// ROR
        /// A = A >> 1. Rotated right through Carry
        /// Opcode D1, Bytes 1
        /// </summary>
        private void ROR()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xD1]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ROR{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// VEJ (D2)
        /// Vectored Call, $FF D2
        /// Opcode D2, Bytes 1
        /// </summary>
        /// VEJ (D2),n1,n2 - For PC-1500 arguments follow this opcode
        /// Convert AR-X to integer & load to U-Reg.  If range 'p' exceeded, then branch fwd n
        public void VEJ_D2()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            String args = "";

            byte numBytes = opBytesP1[0xD2];

            if (disModePC1500)
            {
                numBytes += 2;
                retValues = VEJ_b1_b2(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (D2){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// DRR (X)
        /// Right rotation between Accumulator and (X)
        /// Opcode D3, Bytes 1
        /// </summary>
        /// (R)->A, RH = AL, RL = RH
        private void DRR_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xD3]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DDR  (X){0}", Environment.NewLine));
            //tick += 12;
        }

        /// <summary>
        /// VEJ (D4)
        /// Vectored Call, $FF D4
        /// Opcode D4, Bytes 1
        /// </summary>
        /// VEJ (D4),n - For PC-1500 arguments follow this opcode
        /// Copies pointer for current Processing status 
        /// of program in memory. A0=Program AC=Break B2=Error
        public void VEJ_D4()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            String args = "";

            byte numBytes = opBytesP1[0xD4];

            if (disModePC1500)
            {
                numBytes += 1;
                retValues = VEJ_b1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (D4){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// SHR
        /// A = A >> 1. Shifted through Carry, 0 into MSB
        /// Opcode D5, Bytes 1
        /// </summary>
        private void SHR()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xD5]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SHR{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// VEJ (D6)
        /// Vectored Call, $FF D6
        /// Opcode D6, Bytes 1
        /// </summary>
        /// VEJ (D6),n - For PC-1500 arguments follow this opcode
        /// Loads address pointer from memory to AR-Y A6=PROGRAM AC=BREAK B8=ON ERROR
        public void VEJ_D6()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            String args = "";
            
            byte numBytes = opBytesP1[0xD6];

            if (disModePC1500)
            {
                numBytes += 1;
                retValues = VEJ_b1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (D6){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// DRL (X)
        /// Left rotation between Accumulator and (X) (ME0)
        /// Opcode D7, Bytes 1
        /// </summary>
        /// (R)->A, RH=RL, RL=AH
        private void DRL_X_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xD1]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DRL  (X){0}", Environment.NewLine));
            //tick += 12;
        }

        /// <summary>
        /// VEJ (D8)
        /// Vectored Call, $FF D8
        /// Opcode D8, Bytes 1
        /// </summary>
        public void VEJ_D8()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xD8]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (D8){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// SHL
        /// AA = A << 1. Shifted through Carry, 0 into LSB
        /// Opcode D9, Bytes 1
        /// </summary>
        private void SHL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xD9]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SHL{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// VEJ (DA)
        /// Vectored Call, $FF DA
        /// Opcode DA, Bytes 1
        /// </summary>
        public void VEJ_DA()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xDA]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (DA){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// ROL
        /// A = A << 1. Rotated left through Carry
        /// Opcode DB, Bytes 1
        /// </summary>
        private void ROL()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xDB]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ROL{0}", Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// VEJ (DC)
        /// Vectored Call, $FF DC
        /// Opcode DC, Bytes 1
        /// </summary>
        public void VEJ_DC()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xDC]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (DC){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// INC A
        /// A = A + 1
        /// Opcode DD, Bytes 1
        /// </summary>
        private void INC_A()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xDD]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  A{0}", Environment.NewLine));
            //tick += 5;
        }

        /// <summary>
        /// VEJ (DE)
        /// Vectored Call, $FF DE
        /// Opcode DE, Bytes 1
        /// </summary>
        /// VEJ (DE),n - For PC-1500 arguments follow this opcode
        /// Calculates formula to which Y-Reg points and passes 
        /// result to AR-X. Jump FWD (n) if error
        public void VEJ_DE()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            String args = "";

            byte numBytes = opBytesP1[0xDE];

            if (disModePC1500)
            {
                numBytes += 1;
                retValues = VEJ_o1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (DE){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// DEC A
        /// A = A - 1
        /// Opcode DF, Bytes 1
        /// </summary>
        private void DEC_A()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xDF]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  A{0}", Environment.NewLine));
            //tick += 5;
        }

        #endregion Opcodes_0xD0-0xDF

        #region Opcodes_0xE0-0xEF

        /// <summary>
        /// VEJ (E0)
        /// Vectored Call, $FF E0
        /// Opcode E0, Bytes 1
        /// </summary>
        public void VEJ_E0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xE0]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (E0){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// SPU
        /// Opcode E1, Bytes 1
        /// Set PU Flip Flip, PU controls banking of external ROMs
        /// </summary>
        private void SPU()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xE1]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SPU{0}", Environment.NewLine));
            //tick += 4;
        }

        /// <summary>
        /// VEJ (E2)
        /// Vectored Call, $FF E2
        /// Opcode E2, Bytes 1
        /// </summary>
        public void VEJ_E2()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xE2]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (E2){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// RPU
        /// Opcode E3, Bytes 1
        /// Reset PU Flip Flip, PU controls banking of external ROMs
        /// </summary>
        private void RPU()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xE3]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("RPU{0}", Environment.NewLine));
            //tick += 4;
        }

        /// <summary>
        /// VEJ (E4)
        /// Vectored Call, $FF E4
        /// Opcode E4, Bytes 1
        /// </summary>
        public void VEJ_E4()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xE4]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (E4){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// VEJ (E6)
        /// Vectored Call, $FF E6
        /// Opcode E6, Bytes 1
        /// </summary>
        public void VEJ_E6()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xE6]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (E6){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// VEJ (E8)
        /// Vectored Call, $FF E8
        /// Opcode E8, Bytes 1
        /// </summary>
        public void VEJ_E8()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xE8]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (E8){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// ANI (pp),n
        /// (pp) = (pp) & n (ME0) 
        /// Opcode 0xE9, Bytes 4
        /// </summary>
        private void ANI_pp_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xE9]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            byte value = GetByte();     // P += 1
            disSB.Append(String.Format("ANI  ({0}),${1:X2}{2}", GetAddOrLabel(address), value, Environment.NewLine));
            //tick += 19;
        }

        /// <summary>
        /// VEJ (EA)
        /// Vectored Call, $FF EA
        /// Opcode EA, Bytes 1
        /// </summary>
        public void VEJ_EA()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xEA]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (EA){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// ORI (pp),n
        /// (pp) = (pp) | n
        /// Opcode EB, Bytes 4
        /// </summary>
        private void ORI_pp_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xEB]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord();
            byte value = GetByte();
            disSB.Append(String.Format("ORI  ({0}),${1:X2}{2}", GetAddOrLabel(address), value, Environment.NewLine));
            //tick += 19;
        }

        /// <summary>
        /// VEJ (EC)
        /// Vectored Call, $FF EC
        /// Opcode EC, Bytes 1
        /// </summary>
        public void VEJ_EC()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xEC]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (EC){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// BII (pp),n
        /// FLAGS = (pp) & n
        /// Opcode ED, Bytes 4
        /// </summary>
        private void BII_pp_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xED]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            byte value = this.GetByte(); // P += 1
            disSB.Append(String.Format("BII  ({0}),${1:X2}{2}", GetAddOrLabel(address), value, Environment.NewLine));
            //tick += 16;
        }

        /// <summary>
        /// VEJ (EE)
        /// Vectored Call, $FF EE
        /// Opcode EE, Bytes 1
        /// </summary>
        public void VEJ_EE()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xEE]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (EE){0}" ,Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// ADI (pp),n
        /// (pp) = (pp) + n
        /// Opcode EF, Bytes 4
        /// </summary>
        private void ADI_pp_n_ME0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xEF]); } // dump bytes

            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            byte value = GetByte();     // P += 1
            disSB.Append(String.Format("ADI  ({0}),${1:X2}{2}", GetAddOrLabel(address), value, Environment.NewLine));
            //tick += 19;
        }

        #endregion Opcodes_0xF0-0xFF

        #region Opcodes_0xF0-0xFF

        /// <summary>
        /// VEJ (F0)
        /// Vectored Call, $FF F0
        /// Opcode F0, Bytes 1
        /// </summary>
        public void VEJ_F0()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xF0]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (F0){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// AEX
        /// Opcode F1, Bytes 1
        /// Swap Accumulator High nibble & low nibble
        /// </summary>
        private void AEX()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xF1]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("AEX{0}", Environment.NewLine));
            //tick += 6;
        }

        /// <summary>
        /// VEJ (F2)
        /// Vectored Call, $FF F2
        /// Opcode F2, Bytes 1
        /// </summary>
        public void VEJ_F2()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xF2]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (F2){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// VEJ (F4)
        /// Vectored Call, $FF F4
        /// Opcode F4, Bytes 1
        /// </summary>
        /// VEJ (F4),pp - For PC-1500 arguments follow this opcode
        /// Loads U-Reg with 16-bit value from address of (pp)
        public void VEJ_F4()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            String args = "";

            byte numBytes = opBytesP1[0xF4];

            if (disModePC1500)
            {
                numBytes += 2;
                retValues = VEJ_w1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (F4){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// TIN
        /// Opcode F5, Bytes 1
        /// (Y) = (X) then X = X + 1, Y = Y + 1
        /// </summary>
        private void TIN()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xF5]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("TIN{0}", Environment.NewLine));
            //tick += 7;
        }

        /// <summary>
        /// VEJ (F6)
        /// Vectored Call, $FF F6
        /// Opcode F6, Bytes 1
        /// </summary>
        /// VEJ (F6),pp - For PC-1500 arguments follow this opcode
        /// Transfers U to pp, pp+1
        public void VEJ_F6()
        {
            P += 1; // advance Program Counter
            Tuple<String, byte> retValues = new Tuple<String, byte>("", 1);
            String args = "";

            byte numBytes = opBytesP1[0xF6];

            if (disModePC1500)
            {
                numBytes += 2;
                retValues = VEJ_w1(numBytes);
                args = String.Format(",{0}", retValues.Item1);
            }

            if (listFormat) { LineDump((ushort)(P - numBytes), numBytes); } // dump bytes
            disSB.Append(String.Format("VEJ  (F6){0}{1}", args, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// CIN
        /// Opcode F7, Bytes 1
        /// FLAGS = A compared to (X) register, then INC X
        /// </summary>
        private void CIN()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xF7]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("CIN{0}", Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// VEJ (F8)
        /// Vectored Call, $FF F8
        /// Opcode F8, Bytes 1
        /// </summary>
        public void VEJ_F8()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xF8]); } // dump bytes

            P += 1;  // advance Program Counter
            disSB.Append(String.Format("VEJ  (F8){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// REC
        /// Opcode F9, Bytes 1
        /// Reset Carry Flag
        /// </summary>
        private void REC()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xF9]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("REC{0}", Environment.NewLine));
            //tick += 4;
        }

        /// <summary>
        /// VEJ (FA)
        /// Vectored Call, $FF FA
        /// Opcode FA, Bytes 1
        /// </summary>
        public void VEJ_FA()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xFA]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (FA){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// SEC
        /// Opcode FB, Bytes 1
        /// Set Carry Flag
        /// </summary>
        private void SEC()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xFB]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("SEC{0}", Environment.NewLine));
            //tick += 4;
        }

        /// <summary>
        /// VEJ (FC)
        /// Vectored Call, $FF FC
        /// Opcode FC, Bytes 1
        /// </summary>
        public void VEJ_FC()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xFC]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("VEJ  (FC){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// FD xx
        /// FD signals use of Opcode Table 2
        /// </summary>
        private void FD_P2()
        {
            P += 1; // advance Program Counter
            byte opcode = RAM_ME0[P];
            delegatesTbl2[opcode].DynamicInvoke();
            // read next opcode and index into table 2
        }

        /// <summary>
        /// VEJ (FE)
        /// Vectored Call, $FF FE
        /// Opcode FE, Bytes 1
        /// </summary>
        public void VEJ_FE()
        {
            if (listFormat) { LineDump(P, opBytesP1[0xF1]); } // dump bytes

            P += 1;  // advance Program Counter
            disSB.Append(String.Format("VEJ  (FE){0}", Environment.NewLine));
            //tick += 17;
        }

        #endregion Opcodes_0xF0-0xFF

        #endregion OPCODES 0x00-0xFF

        #region OPCODES 0xFD00-0xFDFF

        #region Opcodes_0xFD00-0xFD0F

        /// <summary>
        /// SBC #(X)
        /// A = A - #(X)
        /// Opcode FD 01, Bytes 2
        /// </summary>
        public void SBC_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P-1), opBytesP2[0x01]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  #(X){0}", Environment.NewLine));
            //tick += 11;
        }
        
        /// <summary>
        /// ADC #(X)
        /// Opcode FD 03, Bytes 2
        /// </summary>
        private void ADC_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x03]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  #(X){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// LDA #(X)
        /// A = #(X)
        /// Opcode 0xFD 05, Bytes 2
        /// </summary>
        private void LDA_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x05]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  #(X){0}", Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// CPA #(X)
        /// Opcode FD 07, Bytes 2
        /// Compare A + #(X)
        /// </summary>
        private void CPA_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x07]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  #(X){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// LDX X
        /// X = X
        /// Opcode FD 08, Bytes 2
        /// </summary>
        public void LDX_X()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x08]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDX  X{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// AND #(X)
        /// A = A & #(X) 
        /// Opcode 0xFD 09, Bytes 2
        /// </summary>
        private void AND_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x09]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("AND  #(X){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// POP X
        /// Stack -> X
        /// Opcode FD 0A, Bytes 2
        /// </summary>
        private void POP_X()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x0A]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("POP  X{0}", Environment.NewLine));
            //tick += 15;
        }

        /// <summary>
        /// ORA #(X)
        /// A = A | #(X)
        /// Opcode FD 0B, Bytes 2
        /// </summary>
        private void ORA_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x0B]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ORA  #(X){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// DCS #(X)
        /// A = A - #(X) BCD Subtraction
        /// Opcode FD 0C, Bytes 2
        /// </summary>
        private void DCS_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x0C]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCS  #(X){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// EOR #(X)
        /// A = A ^ #(X)
        /// Opcode FD 0D, Bytes 2
        /// </summary>
        private void EOR_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x0D]); } // dump bytes
            
            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("EOR  #(X){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// STA #(X)
        /// #(X) = A 
        /// Opcode FD 0E, Bytes 2
        /// </summary>
        private void STA_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x0E]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  #(X){0}", Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// BIT #(X)
        /// ZFLAG = A & #(X)
        /// Opcode FD 0F, Bytes 2
        /// </summary>
        private void BIT_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x0F]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("BIT  #(X){0}", Environment.NewLine));
            //tick += 11;
        }

        #endregion Opcodes_0xFD00-0xFD0F

        #region Opcodes_0xFD10-0xFD1F

        /// <summary>
        /// SBC #(Y)
        /// A = A - #(Y)
        /// Opcode FD 11, Bytes 2
        /// </summary>
        public void SBC_Y_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x11]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  #(Y){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ADC #(Y)
        /// Opcode FD 13, Bytes 2
        /// </summary>
        private void ADC_Y_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x13]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  #(Y){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// LDA #(Y)
        /// A = #(Y)
        /// Opcode 0xFD 15, Bytes 2
        /// </summary>
        private void LDA_Y_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x15]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  #(Y){0}", Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// CPA #(Y)
        /// Opcode FD 17, Bytes 2
        /// Compare A + #(Y)
        /// </summary>
        private void CPA_Y_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x17]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  #(Y){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// LDX Y
        /// X = Y
        /// Opcode FD 18, Bytes 2
        /// </summary>
        public void LDX_Y()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x18]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDX  Y{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// AND #(Y)
        /// A = A & #(Y) 
        /// Opcode FD 19, Bytes 2
        /// </summary>
        private void AND_Y_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x19]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("AND  #(Y){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// POP Y
        /// Stack -> Y
        /// Opcode FD 1A, Bytes 2
        /// </summary>
        private void POP_Y()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x1A]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("POP  Y{0}", Environment.NewLine));
            //tick += 15;
        }

        /// <summary>
        /// ORA #(Y)
        /// A = A | #(Y)
        /// Opcode FD 1B, Bytes 2
        /// </summary>
        private void ORA_Y_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x1B]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ORA  #(Y){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// DCS #(Y)
        /// A = A - #(Y) BCD Subtraction
        /// Opcode FD 1C, Bytes 2
        /// </summary>
        private void DCS_Y_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x1C]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCS  #(Y){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// EOR #(Y)
        /// A = A ^ #(Y)
        /// Opcode FD 1D, Bytes 2
        /// </summary>
        private void EOR_Y_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x1D]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("EOR  #(Y){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// STA #(Y)
        /// #(Y) = A 
        /// Opcode FD 1E, Bytes 2
        /// </summary>
        private void STA_Y_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x1E]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  #(Y){0}", Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// BIT #(Y)
        /// ZFLAG = A & #(Y)
        /// Opcode FD 1F, Bytes 2
        /// </summary>
        private void BIT_Y_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x1F]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("BIT  #(Y){0}", Environment.NewLine));
            //tick += 11;
        }

        #endregion Opcodes_0xFD10-0xFD1F

        #region Opcodes_0xFD20-0xFD2F

        /// <summary>
        /// SBC #(U)
        /// A = A - #(U)
        /// Opcode FD 21, Bytes 2
        /// </summary>
        public void SBC_U_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x21]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  #(U){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ADC #(U)
        /// Opcode FD 23, Bytes 2
        /// </summary>
        private void ADC_U_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x23]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  #(U){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// LDA #(U)
        /// A = #(U)
        /// Opcode 0xFD 25, Bytes 2
        /// </summary>
        private void LDA_U_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x25]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  #(U){0}", Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// CPA #(U)
        /// Opcode FD 27, Bytes 2
        /// Compare A + #(X)
        /// </summary>
        private void CPA_U_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x27]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  #(U){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// LDX U
        /// X = U
        /// Opcode FD 28, Bytes 2
        /// </summary>
        public void LDX_U()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x28]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDX  U{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// AND #(U)
        /// A = A & #(U) 
        /// Opcode 0xFD 29, Bytes 2
        /// </summary>
        private void AND_U_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x29]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("AND  #(U){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// POP U
        /// Stack -> U
        /// Opcode FD 2A, Bytes 2
        /// </summary>
        private void POP_U()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x2A]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("POP  U{0}", Environment.NewLine));
            //tick += 15;
        }

        /// <summary>
        /// ORA #(U)
        /// A = A | #(U)
        /// Opcode FD 2B, Bytes 2
        /// </summary>
        private void ORA_U_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x2B]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ORA  #(U){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// DCS #(U)
        /// A = A - #(U) BCD Subtraction
        /// Opcode FD 2C, Bytes 2
        /// </summary>
        private void DCS_U_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x2C]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCS  #(U){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// EOR #(U)
        /// A = A ^ #(U)
        /// Opcode FD 2D, Bytes 2
        /// </summary>
        private void EOR_U_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x2D]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("EOR  #(U){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// STA #(U)
        /// #(U) = A 
        /// Opcode FD 2E, Bytes 2
        /// </summary>
        private void STA_U_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x2E]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  #(U){0}", Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// BIT #(U)
        /// ZFLAG = A & #(U)
        /// Opcode FD 2F, Bytes 1
        /// </summary>
        private void BIT_U_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x2F]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("BIT  #(U){0}", Environment.NewLine));
            tick += 11;
        }

        #endregion Opcodes_0xFD20-0xFD2F

        #region Opcodes_0xFD30-0xFD3F

        /// <summary>
        /// SBC #(V)
        /// A = A - #(V)
        /// Opcode FD 31, Bytes 2
        /// </summary>
        public void SBC_V_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x31]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("SBC  #(V){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ADC #(V)
        /// Opcode FD 33, Bytes 2
        /// </summary>
        private void ADC_V_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x33]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADC  #(V){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// LDA #(V)
        /// A = #(V)
        /// Opcode 0xFD 35, Bytes 2
        /// </summary>
        private void LDA_V_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x35]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDA  #(V){0}", Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// CPA #(V)
        /// Opcode FD 37, Bytes 2
        /// Compare A + #(V)
        /// </summary>
        private void CPA_V_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x37]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("CPA  #(V){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// LDX V
        /// X = V
        /// Opcode FD 38, Bytes 2
        /// </summary>
        public void LDX_V()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x38]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDX  V{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// AND #(V)
        /// A = A & #(V) 
        /// Opcode FD 39, Bytes 2
        /// </summary>
        private void AND_V_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x39]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("AND  #(V){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// POP V
        /// Stack -> V
        /// Opcode FD 3A, Bytes 2
        /// </summary>
        private void POP_V()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x3A]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("POP  V{0}", Environment.NewLine));
            //tick += 15;
        }

        /// <summary>
        /// ORA #(V)
        /// A = A | #(V)
        /// Opcode FD 3B, Bytes 2
        /// </summary>
        private void ORA_V_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x3B]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ORA  #(V){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// DCS #(V)
        /// A = A - #(V) BCD Subtraction
        /// Opcode FD 3C, Bytes 2
        /// </summary>
        private void DCS_V_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x3C]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCS  #(V){0}", Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// EOR #(V)
        /// A = A ^ #(V)
        /// Opcode FD 3D, Bytes 2
        /// </summary>
        private void EOR_V_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x3D]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("EOR  #(V){0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// STA #(V)
        /// #(V) = A 
        /// Opcode FD 3E, Bytes 2
        /// </summary>
        private void STA_V_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x3E]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("STA  #(V){0}", Environment.NewLine));
            //tick += 10;
        }

        /// <summary>
        /// BIT #(V)
        /// ZFLAG = A & #(V)
        /// Opcode FD 3F, Bytes 1
        /// </summary>
        private void BIT_V_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x3F]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("BIT  #(V){0}", Environment.NewLine));
            //tick += 11;
        }

        #endregion Opcodes_0xFD30-0xFD3F

        #region Opcodes_0xFD40-0xFD4F

        /// <summary>
        /// INC XH
        /// XH = XH + 1
        /// Opcode FD 40, Bytes 1
        /// </summary>
        private void INC_XH()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x40]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  XH{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// DEC XH
        /// XH = XH - 1
        /// Opcode FD 42, Bytes 1
        /// </summary>
        private void DEC_XH()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x42]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  XH{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// LDX S
        /// X = S
        /// Opcode FD 48, Bytes 2
        /// </summary>
        public void LDX_S()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x48]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDX  S{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ANI #(X),n
        /// #(X) = #(X) & n  
        /// Opcode FD 49, Bytes 3
        /// </summary>
        private void ANI_X_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x49]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ANI  #(X),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// STX X
        /// X = X
        /// Opcode FD 4A, Bytes 2
        /// </summary>
        private void STX_X()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x4A]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("STX  X{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ORI #(X),n
        /// #(X) = #(X) | n
        /// Opcode FD 4B, Bytes 3
        /// </summary>
        private void ORI_X_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x4B]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte();
            disSB.Append(String.Format("ORI  #(X),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// OFF
        /// BF Flip-Flop reset, Not sure what BF flip-flop is
        /// Opcode FD 4C, Bytes 2
        /// </summary>
        private void OFF()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x4C]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("OFF{0}", Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// BII #(X),n
        /// FLAGS = #(X) & n
        /// Opcode FD 4D, Bytes 3
        /// </summary>
        private void BII_X_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x4D]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = this.GetByte(); // P += 1
            disSB.Append(String.Format("BII  #(X),${0:X2}{1}", value, Environment.NewLine));
            //tick += 14;
        }

        /// <summary>
        /// STX S
        /// S = X
        /// Opcode FD 4E, Bytes 2
        /// </summary>
        private void STX_S()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x4E]); } // dump bytes

            // FD handled before hand P += 1
            P += 1;
            disSB.Append(String.Format("STX  S{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ADI #(X),n
        /// Opcode FD 4F, Bytes 3
        /// </summary>
        private void ADI_X_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x4F]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ADI  #(X),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        #endregion Opcodes_0xFD40-0xFD4F

        #region Opcodes_0xFD50-0xFD5F

        /// <summary>
        /// INC YH
        /// YH = YH + 1
        /// Opcode FD 50, Bytes 1
        /// </summary>
        private void INC_YH()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x50]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  YH{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// DEC YH
        /// YH = YH - 1
        /// Opcode FD 52, Bytes 1
        /// </summary>
        private void DEC_YH()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x52]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  YH{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// LDX P
        /// X = P
        /// Opcode FD 58, Bytes 2
        /// </summary>
        public void LDX_P()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x58]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("LDX  P{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ANI #(Y),n
        /// #(Y) = #(Y) & n  
        /// Opcode FD 59, Bytes 3
        /// </summary>
        private void ANI_Y_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x59]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ANI  #(Y),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// STX Y
        /// Y = X
        /// Opcode FD 5A, Bytes 2
        /// </summary>
        private void STX_Y()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x5A]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("STX  Y{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ORI #(Y),n
        /// #(Y) = #(Y) | n
        /// Opcode FD 5B, Bytes 3
        /// </summary>
        private void ORI_Y_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x5B]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte();
            disSB.Append(String.Format("ORI  #(Y),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// BII #(Y),n
        /// FLAGS = #(Y) & n
        /// Opcode FD 5D, Bytes 3
        /// </summary>
        private void BII_Y_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x5D]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = this.GetByte(); // P += 1
            disSB.Append(String.Format("BII  #(Y),${0:X2}{1}", value, Environment.NewLine));
            //tick += 14;
        }

        /// <summary>
        /// STX P
        /// P = X.  Program_Counter = X
        /// Opcode FD 5E, Bytes 2
        /// </summary>
        private void STX_P()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x5E]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("STX  P{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ADI #(Y),n
        /// Opcode FD 5F, Bytes 3
        /// </summary>
        private void ADI_Y_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x5F]); } // dump bytes
            
            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ADI  #(Y),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        #endregion Opcodes_0xFD50-0xFD5F

        #region Opcodes_0xFD60-0xFD6F

        /// <summary>
        /// INC UH
        /// UH = UH + 1
        /// Opcode FD 60, Bytes 1
        /// </summary>
        private void INC_UH()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x60]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  UH{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// DEC UH
        /// UH = UH - 1
        /// Opcode FD 62, Bytes 1
        /// </summary>
        private void DEC_UH()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x62]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  UH{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// ANI #(U),n
        /// #(U) = #(U) & n  
        /// Opcode FD 69, Bytes 3
        /// </summary>
        private void ANI_U_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x69]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ANI  #(U),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// STX U
        /// U = X
        /// Opcode FD 6A, Bytes 2
        /// </summary>
        private void STX_U()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x6A]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("STX  U{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ORI #(U),n
        /// #(U) = #(U) | n
        /// Opcode FD 6B, Bytes 3
        /// </summary>
        private void ORI_U_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x6B]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte();
            disSB.Append(String.Format("ORI  #(U),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// BII #(U),n
        /// FLAGS = #(U) & n
        /// Opcode FD 6D, Bytes 3
        /// </summary>
        private void BII_U_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x6D]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = this.GetByte(); // P += 1
            disSB.Append(String.Format("BII  #(U),${0:X2}{1}", value, Environment.NewLine));
            //tick += 14;
        }

        /// <summary>
        /// ADI #(U),n
        /// Opcode FD 6F, Bytes 3
        /// </summary>
        private void ADI_U_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x6F]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ADI  #(U),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        #endregion Opcodes_0xFD60-0xFD6F

        #region Opcodes_0xFD70-0xFD7F

        /// <summary>
        /// INC VH
        /// VH = VH + 1
        /// Opcode FD 70, Bytes 1
        /// </summary>
        private void INC_VH()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x70]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("INC  VH{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// DEC VH
        /// VH = VH - 1
        /// Opcode FD 72, Bytes 1
        /// </summary>
        private void DEC_VH()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x72]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("DEC  VH{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// ANI #(V),n
        /// #(V) = #(V) & n  
        /// Opcode FD 79, Bytes 2
        /// </summary>
        private void ANI_V_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x79]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ANI  #(V),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// STX V
        /// V = X
        /// Opcode FD 7A, Bytes 2
        /// </summary>
        private void STX_V()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x7A]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("STX  V{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ORI #(V),n
        /// #(V) = #(V) | n
        /// Opcode FD 7B, Bytes 3
        /// </summary>
        private void ORI_V_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x7B]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte();
            disSB.Append(String.Format("ORI  #(V),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// BII #(V),n
        /// FLAGS = #(V) & n
        /// Opcode FD 7D, Bytes 3
        /// </summary>
        /// todo: simplify
        private void BII_V_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x7D]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = this.GetByte(); // P += 1
            disSB.Append(String.Format("BII  #(V),${0:X2}{1}", value, Environment.NewLine));
            //tick += 14;
        }

        /// <summary>
        /// ADI #(V),n
        /// Opcode FD 7F, Bytes 3
        /// </summary>
        private void ADI_V_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x7F]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            byte value = GetByte(); // P += 1
            disSB.Append(String.Format("ADI  #(V),${0:X2}{1}", value, Environment.NewLine));
            //tick += 17;
        }

        #endregion Opcodes_0xFD70-0xFD7F

        #region Opcodes_0xFD80-0xFD8F

        /// <summary>
        /// SIE
        /// Set Interrupt Enable Flag
        /// Opcode FD 81, Bytes 1
        /// </summary>
        private void SIE()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x81]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("SIE{0}", Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// PSH X
        /// X -> Stack
        /// Opcode FD 88, Bytes 2
        /// </summary>
        private void PSH_X()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x88]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("PSH  X{0}", Environment.NewLine));
            //tick += 14;
        }

        /// <summary>
        /// POP A
        /// Stack -> A
        /// Opcode FD 8A, Bytes 2
        /// </summary>
        private void POP_A()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x8A]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("POP  A{0}", Environment.NewLine));
            //tick += 12;
        }

        /// <summary>
        /// DCA #(X)
        /// A = A + (X) BCD Addition
        /// Opcode FD 8C, Bytes 2
        /// </summary>
        private void DCA_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x8C]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCA  #(X){0}", Environment.NewLine));
            //tick += 19;
        }

        /// <summary>
        /// CDV
        /// Clear CPU clock divider, resets CPU 
        /// Opcode FD 8E, Bytes 1
        /// </summary>
        private void CDV()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x8E]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("CDV{0}", Environment.NewLine));
            //tick += 8; 
        }

        #endregion Opcodes_0xFD80-0xFD8F

        #region Opcodes_0xFD90-0xFD9F

        /// <summary>
        /// PSH Y
        /// Y -> Stack
        /// Opcode FD 98, Bytes 2
        /// </summary>
        private void PSH_Y()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x98]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("PSH  Y{0}", Environment.NewLine));
            //tick += 14;
        }

        /// <summary>
        /// DCA #(Y)
        /// A = A + (Y) BCD Addition
        /// Opcode FD 9C, Bytes 2
        /// </summary>
        private void DCA_Y_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0x9C]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCA  #(Y){0}", Environment.NewLine));
            //tick += 19;
        }

        #endregion Opcodes_0xFD90-0xFD9F

        #region Opcodes_0xFDA0-0xFDAF

        /// <summary>
        /// SBC #(pp)
        /// A = A - #(pp)
        /// Opcode FD A1, Bytes 4
        /// </summary>
        public void SBC_pp_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xA1]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("SBC  #({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// ADC #(pp)
        /// Opcode FD A3, Bytes 4
        /// </summary>
        private void ADC_pp_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xA3]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("ADC  #({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// LDA #(pp)
        /// A = #(pp)
        /// Opcode FD A5, Bytes 4
        /// </summary>
        private void LDA_pp_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xA5]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("LDA  #({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 16;
        }

        /// <summary>
        /// CPA #(pp)
        /// Opcode FD A7, Bytes 3
        /// Compare A + #(pp)
        /// </summary>
        private void CPA_pp_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xA7]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("CPA  #({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// PSH U
        /// U -> Stack
        /// Opcode FD A8, Bytes 2
        /// </summary>
        private void PSH_U()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xA8]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("PSH  U{0}", Environment.NewLine));
            //tick += 14;
        }

        /// <summary>
        /// AND #(pp)
        /// A = A & #(pp) 
        /// Opcode FD A9, Bytes 4
        /// </summary>
        private void AND_pp_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xA9]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("AND  #({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// TTA
        /// A = T (flags) 
        /// Opcode FD AA, Bytes 2
        /// </summary>
        private void TTA()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xAA]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("TTA{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// ORA #(pp)
        /// A = A | #(pp)
        /// Opcode FD AB, Bytes 4
        /// </summary>
        private void ORA_pp_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xAB]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("ORA  #({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 17;
        }

        /// <summary>
        /// DCA #(U)
        /// A = A + (U) BCD Addition
        /// Opcode FD AC, Bytes 2
        /// </summary>
        private void DCA_U_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xAC]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCA  #(U){0}", Environment.NewLine));
            //tick += 19;
        }

        /// <summary>
        /// EOR #(pp)
        /// A = A ^ #(pp)
        /// Opcode FD AD, Bytes 4
        /// </summary>
        private void EOR_pp_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xAD]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2;
            disSB.Append(String.Format("EOR  #({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// STA #(pp)
        /// (pp) = A 
        /// Opcode FD AE, Bytes 4
        /// </summary>
        private void STA_pp_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xAE]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("STA  #({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 16;
        }

        /// <summary>
        /// BIT #(pp)
        /// ZFLAG = A & #(pp)
        /// Opcode FD AF, Bytes 1
        /// </summary>
        private void BIT_pp_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xAF]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            disSB.Append(String.Format("BIT  #({0}){1}", GetAddOrLabel(address), Environment.NewLine));
            //tick += 17;
        }

        #endregion Opcodes_0xFDA0-0xFDAF

        #region Opcodes_0xFDB0-0xFDBF

        /// <summary>
        /// HLT
        /// Stop CPU
        /// Opcode FD B1, Bytes 1
        /// </summary>
        private void HLT()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xB1]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("HLT{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// PSH V
        /// V -> Stack
        /// Opcode FD B8, Bytes 2
        /// </summary>
        private void PSH_V()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xB8]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("PSH  V{0}", Environment.NewLine));
            //tick += 14;
        }

        /// <summary>
        /// ITA
        /// Input Port IN0-IN7 -> Accumulator
        /// Opcode FD BA, Bytes 2
        /// </summary>
        private void ITA()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xBA]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ITA{0}", Environment.NewLine));
            tick += 9;
        }

        /// <summary>
        /// DCA #(V)
        /// A = A + (V) BCD Addition
        /// Opcode FD BC, Bytes 2
        /// </summary>
        private void DCA_V_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xBC]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DCA  #(V){0}", Environment.NewLine));
            //tick += 19;
        }

        /// <summary>
        /// RIE
        /// Opcode FD BE, Bytes 2
        /// Reset Interrupt Enable Flag
        /// </summary>
        private void RIE()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xBE]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("RIE{0}", Environment.NewLine));
            //tick += 8;
        }

        #endregion Opcodes_0xFDB0-0xFDBF

        #region Opcodes_0xFDC0-0xFDCF

        /// <summary>
        /// RDP
        /// Resets LCD ON/OFF flip-flop
        /// Opcode FD C0, Bytes 2
        /// </summary>
        private void RDP()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xC0]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("RDP{0}", Environment.NewLine));
            //tick += 8;
        }

        /// <summary>
        /// SDP
        /// Opcode FD C1, Bytes 2
        /// Sets LCD ON/OFF control flip-flop
        /// </summary>
        private void SDP()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xC1]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("SDP{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// PSH A
        /// A -> Stack
        /// Opcode FD C8, Bytes 2
        /// </summary>
        private void PSH_A()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xC8]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("PSH  A{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ADR X
        /// X = X + A
        /// Opcode FD CA, Bytes 2
        /// </summary>
        private void ADR_X()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xCA]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADR  X{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ATP
        /// Opcode FD CC, Bytes 2
        /// A -> Data Bus
        /// </summary>
        private void ATP()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xCC]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ATP{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// AM0
        /// Opcode FD CE, Bytes 2
        /// A -> Timer register, bit 9 padded with 0
        /// </summary>
        private void AM0()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xCE]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("AM0{0}", Environment.NewLine));
            //tick += 9;
        }

        #endregion Opcodes_0xFDC0-0xFDCF

        #region Opcodes_0xFDD0-0xFDDF

        /// <summary>
        /// DDR #(X)
        /// Right rotation between Accumulator and #(X)
        /// Opcode FD D3, Bytes 2
        /// </summary>
        /// (X)->A, XH=XL, XL=AL
        private void DRR_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xD3]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DRR  #(X){0}", Environment.NewLine));
            //tick += 16;
        }

        /// <summary>
        /// DRL #(X)
        /// Left rotation between Accumulator and #(X)
        /// Opcode FD D7, Bytes 2
        /// </summary>
        /// (R)->A, RH=RL, RL=AH
        private void DRL_X_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xD7]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("DRL  #(X){0}", Environment.NewLine));
            //tick += 16;
        }

        /// <summary>
        /// ADR Y
        /// Y = Y + A
        /// Opcode FD DA, Bytes 2
        /// </summary>
        private void ADR_Y()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xDA]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADR  Y{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// AM1
        /// Opcode FD DE, Bytes 2
        /// A -> Timer register, bit 9 padded with 1
        /// </summary>
        private void AM1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xDE]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("AM1{0}", Environment.NewLine));
            //tick += 9;
        }

        #endregion Opcodes_0xFDD0-0xFDDF

        #region Opcodes_0xFDE0-0xFDEF

        /// <summary>
        /// ANI #(pp),n
        /// #(pp) = #(pp) & n (ME1)
        /// Opcode FD E9, Bytes 5
        /// </summary>
        private void ANI_pp_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xE9]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            byte value = GetByte();     // P += 1
            disSB.Append(String.Format("ANI  #({0}),${1:X2}{2}", GetAddOrLabel(address), value, Environment.NewLine));
            //tick += 23;
        }

        /// <summary>
        /// ADR U
        /// U = U + A
        /// Opcode FD EA, Bytes 2
        /// </summary>
        private void ADR_U()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xEA]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADR  U{0}", Environment.NewLine));
            //tick += 11;
        }

        /// <summary>
        /// ORI #(pp),n
        /// #(pp) = #(pp) | n
        /// Opcode FD EB, Bytes 5
        /// </summary>
        private void ORI_pp_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xEB]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            byte value = GetByte();     // P += 1
            disSB.Append(String.Format("ORI  #({0}),${1:X2}{2}", GetAddOrLabel(address), value, Environment.NewLine));
            //tick += 23;
        }

        /// <summary>
        /// ATT
        /// Opcode FD EC, Bytes 2
        /// A -> T Register
        /// </summary>
        private void ATT()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xEC]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            disSB.Append(String.Format("ATT{0}", Environment.NewLine));
            //tick += 9;
        }

        /// <summary>
        /// BII #(pp),n
        /// FLAGS = #(pp) & n
        /// Opcode FD ED, Bytes 4
        /// </summary>
        private void BII_pp_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xED]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            byte value = GetByte();     // P += 1
            disSB.Append(String.Format("BII  #({0}),${1:X2}{2}", GetAddOrLabel(address), value, Environment.NewLine));
            //tick += 20;
        }

        /// <summary>
        /// ADI #(pp),n
        /// Opcode FD EF, Bytes 5
        /// </summary>
        private void ADI_pp_n_ME1()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xEF]); } // dump bytes

            // FD handled before hand P += 1
            P += 1; // advance Program Counter
            ushort address = GetWord(); // P += 2
            byte value = GetByte();     // P += 1
            disSB.Append(String.Format("ADI  #({0}),${1:X2}{2}", GetAddOrLabel(address), value, Environment.NewLine));
            //tick += 23;
        }

        #endregion Opcodes_0xFDE0-0xFDEF

        #region Opcodes_0xFDF0-0xFDFF

        /// <summary>
        /// ADR V
        /// V = V + A
        /// Opcode FD FA, Bytes 2
        /// </summary>
        private void ADR_V()
        {
            if (listFormat) { LineDump((ushort)(P - 1), opBytesP2[0xFA]); } // dump bytes

            P += 1; // advance Program Counter
            disSB.Append(String.Format("ADR  V{0}", Environment.NewLine));
            //tick += 11;
        }


        #endregion Opcodes_0xFDF0-0xFDFF

        #endregion OPCODES 0xFD00-0xFDFF

    }
}
