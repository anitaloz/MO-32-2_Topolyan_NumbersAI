using MO_32_2_Topolyan_NumbersAI.NeuroNet;
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
using System.Windows.Forms.DataVisualization.Charting;
namespace MO_32_2_Topolyan_NumbersAI
{
    public partial class FormMain : Form
    {
        private double[] inputPixels;
        private Network network;
        public FormMain()
        {
            InitializeComponent();
            inputPixels = new double[15];

            network = new Network();
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


        private void button_Recognize_Click(object sender, EventArgs e)
        {
            network.ForwardPass(network, inputPixels);
            label_out.Text = network.Fact.ToList().IndexOf(network.Fact.Max()).ToString();
            label_probability.Text = (100 * network.Fact.Max()).ToString("0.00")+ " %";
        }

        private void button_training_Click(object sender, EventArgs e)
        {
            network.Train(network);

            //if (chart_Eavr.Series.Count < 2)
            //{
            //    chart_Eavr.Series.Add("Точность");
            //    chart_Eavr.Series[1].ChartType = SeriesChartType.Line;
            //    chart_Eavr.Series[1].Color = Color.Red;
            //}
            for (int i=0; i<network.E_error_avr.Length; i++)
            {
                chart_Eavr.Series[0].Points.AddY(network.E_error_avr[i]);
                //chart_Eavr.Series[1].Points.AddY(network.Accuracy[i]);
            }
            for (int i = 0; i < network.Accuracy.Length; i++)
            {
                //chart_Eavr.Series[0].Points.AddY(network.E_error_avr[i]);
                chart_accuracy.Series[0].Points.AddY(network.Accuracy[i]);
            }

            //MessageBox.Show("Обучение успешно завершеною", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            network.Test(network);
            //if (chart_Eavr.Series.Count < 2)
            //{
            //    chart_Eavr.Series.Add("Точность");
            //    chart_Eavr.Series[1].ChartType = SeriesChartType.Line;
            //    chart_Eavr.Series[1].Color = Color.Red;
            //}
           
            for (int i = 0; i < network.E_error_avr.Length; i++)
            {
                chart_Eavr.Series[0].Points.AddY(network.E_error_avr[i]);
                //chart_Eavr.Series[1].Points.AddY(network.Accuracy[i]);
            }
            for (int i = 0; i < network.Accuracy.Length; i++)
            {
                //chart_Eavr.Series[0].Points.AddY(network.E_error_avr[i]);
                chart_accuracy.Series[0].Points.AddY(network.Accuracy[i]);
            }

            //MessageBox.Show("Тестирование успешно завершеною", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chart_Eavr_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {
            network.Dropout();
        }
    }
}
