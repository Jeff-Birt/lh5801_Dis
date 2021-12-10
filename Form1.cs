using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.IO;

namespace lh5801_Dis
{
    
    public partial class Form1 : Form
    {
        lh5801_Emu.Dis_Window DumpWin = new lh5801_Emu.Dis_Window();

        lh5801_Dis CPU = new lh5801_Dis();
        private System.ComponentModel.Design.ByteViewer byteviewer;
        System.Text.RegularExpressions.Regex isHexSpc = new System.Text.RegularExpressions.Regex("^[a-fA-F0-9\\s]+$");
        System.Text.RegularExpressions.Regex isHexSpcKey = new System.Text.RegularExpressions.Regex("^[a-fA-F0-9\\s\\b\\cA\\cC\\cV\\cX]+$");
        System.Text.RegularExpressions.Regex isHex = new System.Text.RegularExpressions.Regex("^[a-fA-F0-9]+$");
        System.Text.RegularExpressions.Regex isHexKey = new System.Text.RegularExpressions.Regex("^[a-fA-F0-9\\cA\\cC\\cV\\cX\\b]+$");

        private readonly SynchronizationContext synchronizationContext;
        private DateTime previousTime = DateTime.Now;
        uint lastTick = 0; // used to keep track of clock speed

        /// <summary>
        /// You guessed it, the constructor!
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;

            byteviewer = new ByteViewer();
            byteviewer.Location = new Point(215, 39);
            byteviewer.Size = new Size(410, 410);
            byteviewer.SetBytes(CPU.RAM_ME0);
            byteviewer.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.Controls.Add(byteviewer);

            updateUI();
        }


        /// <summary>
        /// Update Text Boxes and Check Boxes
        /// </summary>
        private void updateUI(int value = 0)
        {
            tbDisStart.Text = tbDisStart.Text.ToUpper();
            tbDisEnd.Text = tbDisEnd.Text.ToUpper();
            tbAddress.Text = tbAddress.Text.ToUpper();
            byteviewer.Refresh();
        }

        #region RUN Controls

        /// <summary>
        /// Single step next opcode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStep_Click(object sender, EventArgs e)
        {
            //CPU.Run();
            //updateUI();
        }

        /// <summary>
        /// Run code until paused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;
            //CPU.tick = 0;

            ushort start = (ushort)Convert.ToUInt16(tbDisStart.Text, 16); 
            ushort end = (ushort)Convert.ToUInt16(tbDisEnd.Text, 16);  

            CPU.Run(start, end);
            DumpWin.SetDump(CPU.DisDump());
            DumpWin.ShowDialog();

            btnRun.Enabled = true;
            tmrUpdate.Enabled = true;
        }

        /// <summary>
        /// Set SingleStep to pause after current opcode is done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPause_Click(object sender, EventArgs e)
        {
            //updateUI();
        }

        /// <summary>
        /// Resets CPU but does not clear ME0 or ME1 RAM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            //CPU.Reset();
            //updateUI();
        }

        /// <summary>
        /// Resets CPU and clears ME0 and ME1 RAM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetAll_Click(object sender, EventArgs e)
        {
            //CPU.Reset(true);
            //updateUI();
        }

        /// <summary>
        /// Crude way to get an UI update after a single step forced stop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            updateUI();
            tmrUpdate.Enabled = false;
        }

        #endregion RUN Controls

        #region Registers and HEX Dump

        /// <summary>
        /// Handles all 8 and 16 bit register inputs and validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbRegHL_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (c == (char)Keys.Enter)
            {
                byte value = (byte)Convert.ToUInt16(((TextBox)sender).Text, 16);
                ushort valWord = (ushort)Convert.ToUInt16(((TextBox)sender).Text, 16);

                //switch (((TextBox)sender).Name)
                //{
                //    case ("tbXH"):
                //        //CPU.REG.X.RH = (value);
                //        break;
                //}

                e.Handled = true;
                updateUI();
            }
            else if (!isHexKey.IsMatch(c.ToString()))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Set address code that tBValue is poked into
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (c == (char)Keys.Enter)
            {
                e.Handled = true;
                byteviewer.SetStartLine(Convert.ToUInt16(tbAddress.Text, 16) / 0x10);
                updateUI();
            }
            else if (!isHexKey.IsMatch(c.ToString()))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Validates any texted pasted into text box
        /// Only hex charecters allowed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbRegHL_TextChanged(object sender, EventArgs e)
        {
            string txt = ((TextBox)sender).Text;
            bool isValid = false;

            if (string.IsNullOrEmpty(txt))
            {
                isValid = false;
            }
            else
            {
                isValid = isHex.IsMatch(txt);
            }

            if (!isValid) { ((TextBox)sender).Text = ""; }
        }

        /// <summary>
        /// Parse data entered into tbValue and poke into RAM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool isValid = true;
            char c = e.KeyChar;
    
            if (c == (char)Keys.Enter)
            {
                ushort address = (ushort)(Convert.ToInt16(tbAddress.Text, 16));
                string[] values = tbValue.Text.Split(' ');
                byte[] memVal = new byte[values.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i].Length < 3)
                    {
                        memVal[i] = (byte)(Convert.ToInt16(values[i], 16));
                    }
                    else
                    {
                        string message = values[i] + " is > $FF";
                        MessageBox.Show(message, "Oops!");
                        i = values.Length;
                        isValid = false;
                    }
                }

                // write into selected RAM area if input valid
                if (isValid)
                {
                    if (rbME0.Checked)
                    {
                        CPU.WriteRAM_ME0(address, memVal);
                    }
                    else
                    {
                        CPU.WriteRAM_ME1(address, memVal);
                    }

                    byteviewer.SetStartLine(address / 0x10);

                    tbValue.Text = tbValue.Text.ToUpper();
                }

                e.Handled = true;

                updateUI();
            }
            else if (!isHexSpcKey.IsMatch(c.ToString()))
            {
                e.Handled = true;
            }

        }

        /// <summary>
        /// Validates any texted pasted into text box
        /// Only hex characters and space allowed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbValue_TextChanged(object sender, EventArgs e)
        {
            string txt = tbValue.Text;
            bool isValid = false;

            if (string.IsNullOrEmpty(txt))
            {
                isValid = false;
            }
            else
            {
                isValid = isHexSpc.IsMatch(txt);
            }

            if (!isValid) { tbValue.Text = ""; }
        }

        /// <summary>
        /// Toggle between ME0 and ME1 hex dump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbME0_CheckedChanged(object sender, EventArgs e)
        {
            byteviewer.SetBytes(CPU.RAM_ME0);
            updateUI();
        }

        /// <summary>
        /// Toggle between ME0 and ME1 hex dump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbME1_CheckedChanged(object sender, EventArgs e)
        {
            byteviewer.SetBytes(CPU.RAM_ME1);
            updateUI();
        }

        #endregion Registers and HEX Dump

        #region Load / Save

        /// <summary>
        /// Load a binary file into selected RAM bank
        /// starting at 'Address'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1;
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();

            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.CheckFileExists == true)
            {
                string inputFile = openFileDialog1.FileName;

                FileStream inputfs = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
                BinaryReader fileReader = new BinaryReader(inputfs);
                long fileSize = inputfs.Length;
                ushort targetAddress = (ushort)Convert.ToInt16(tbAddress.Text, 16);
                ushort spaceLeft = (ushort)(0xFFFF - targetAddress);

                if (fileSize > spaceLeft)
                {
                    string message = "File too large";
                    string title = "Oops!";
                    MessageBox.Show(message, title);
                }
                else
                {
                    for (long i = 0; i < fileSize; i++)
                    {
                        if (rbME0.Checked)
                        {
                            CPU.RAM_ME0[targetAddress + i] = fileReader.ReadByte();
                        }
                        else
                        {
                            CPU.RAM_ME1[targetAddress + i] = fileReader.ReadByte();
                        }

                    }
                    tmrUpdate.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Save entire ME0 or ME1 to binary file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog1;
            saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();

            if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.CheckFileExists == true)
            {
                string outputFile = saveFileDialog1.FileName;
                long fileSize = 0xFFFF;

                FileStream outputfs = new FileStream(outputFile, FileMode.Create);
                BinaryWriter fileWriter = new BinaryWriter(outputfs);

                for (long i = 0; i < fileSize; i++)
                {
                    if (rbME0.Checked)
                    {
                        fileWriter.Write((byte)CPU.RAM_ME0[i]);
                    }
                    else
                    {
                        fileWriter.Write((byte)CPU.RAM_ME1[i]);
                    }
                }

                tmrUpdate.Enabled = true;
            }
        }

        #endregion Load / Save


    }
}