using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genetic_Algorithms
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            domainUpDown1.DownButton();
            domainUpDown1.DownButton();
            pictureBox1.Image = new Bitmap(@"images\x2y2.PNG");
        }

        public void vChangeTextBox(string sInput)
        {
            textBox2.Text = sInput;
        }

        public int iTextBoxToInt(int iNumberOfChoose)
        {
            string sText = string.Empty;

            switch (iNumberOfChoose)
            {
                case 1: sText = textBox1.Text; break;
                case 2: sText = textBox2.Text; break;
                case 3: sText = textBox3.Text; break;
                case 4: sText = textBox4.Text; break;
                case 5: sText = textBox5.Text; break;
                case 6: sText = textBox6.Text; break;
                case 7: sText = textBox7.Text; break;
                case 8: sText = textBox8.Text; break;
                case 10: sText = textBox10.Text; break;
                case 11: sText = textBox11.Text; break;
                case 12: sText = textBox12.Text; break;
                case 14: sText = textBox14.Text; break;
                case 15: sText = textBox15.Text; break;
                case 17: sText = textBox17.Text; break;
                case 19: sText = textBox19.Text; break;
                case 20: sText = textBox20.Text; break;
                case 99: sText = domainUpDown1.SelectedItem.ToString(); break;
            }

            int iNumValue = Int32.Parse(sText);

            return iNumValue;
        }


        public Double dTextBoxToDouble(int iNumberOfChoose)
        {
            string sText = string.Empty;

            switch (iNumberOfChoose)
            {
                case 1: sText = textBox1.Text; break;
                case 2: sText = textBox2.Text; break;
                case 3: sText = textBox3.Text; break;
                case 4: sText = textBox4.Text; break;
                case 5: sText = textBox5.Text; break;
                case 6: sText = textBox6.Text; break;
                case 7: sText = textBox7.Text; break;
                case 8: sText = textBox8.Text; break;  
                case 10: sText = textBox10.Text; break;
                case 11: sText = textBox11.Text; break;
                case 12: sText = textBox12.Text; break;
                case 14: sText = textBox14.Text; break;
                case 15: sText = textBox15.Text; break;
                case 17: sText = textBox17.Text; break;
                case 19: sText = textBox19.Text; break;
                case 20: sText = textBox20.Text; break;
                case 99: sText = domainUpDown1.SelectedItem.ToString(); break;
            }

            double dNumValue = Double.Parse(sText);

            return dNumValue;
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Test_Functions TestFunctionForm = new Test_Functions(this);
            TestFunctionForm.vMain();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
                    }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(@"images\rosenbrock.png");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Solenoid SolenoidForm = new Solenoid(this);
            SolenoidForm.vMain();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            textBox20.Enabled = true;
            textBox19.Enabled = true;
            label23.Enabled = true;
            label22.Enabled = true;
            textBox17.Enabled = false;
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            textBox20.Enabled = false;
            textBox19.Enabled = false;
            label23.Enabled = false;
            label22.Enabled = false;
            textBox17.Enabled = true;
        }

        private void radioButton7_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(@"images\3-5.PNG");
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(@"images\x2y2.PNG");

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(@"images\fun4.PNG");
        }

        
    }
}
