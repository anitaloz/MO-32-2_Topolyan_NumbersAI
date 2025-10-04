using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MO_32_2_Topolyan_NumbersAI.NeuroNet;
namespace MO_32_2_Topolyan_NumbersAI
{
    public partial class FormMain : Form
    {
        private double[] inputPixels;
        public FormMain()
        {
            InitializeComponent();
            inputPixels = new double[15];
        }

        private void Changing_State_Pixel_Button_Click(object sender, EventArgs e)
        {
            if (((Button)sender).BackColor == Color.White)
            {
                ((Button)sender).BackColor = Color.DodgerBlue;
                inputPixels[((Button)sender).TabIndex] = 1d;
            }
            else
            {
                ((Button)sender).BackColor = Color.White;
               inputPixels[((Button)sender).TabIndex] = 0d;
            }
        }

        private void button_SaveTrainSample_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "train.txt";
            string tmpStr = numericUpDown1.Value.ToString();
            for (int i = 0; i < inputPixels.Length; i++)
            {
                tmpStr+=" "+inputPixels[i].ToString();
            }
            tmpStr += "\n";

            File.AppendAllText(path, tmpStr);
        }

        private void button_SaveTestSample_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "test.txt";
            string tmpStr = numericUpDown1.Value.ToString();
            for (int i = 0; i < inputPixels.Length; i++)
            {
                tmpStr += " " + inputPixels[i].ToString();
            }
            tmpStr += "\n";

            File.AppendAllText(path, tmpStr);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            HiddenLayer hiddenLayer1 = new HiddenLayer(5, 7, NeuronType.Hidden, nameof(hiddenLayer1));
        }
    }
}
