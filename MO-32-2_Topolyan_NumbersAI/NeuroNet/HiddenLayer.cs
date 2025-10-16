using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO_32_2_Topolyan_NumbersAI.NeuroNet
{
    internal class HiddenLayer : Layer
    {

        public HiddenLayer(int non, int nopn, NeuronType nt, string nm_Layer):base(non, nopn, nt, nm_Layer)
        {
            
        }
        //прямой проход
        public override void Recognize(Network net, Layer nextLayer)
        {
            double[] hidden_out = new double[numofneurons];
            for (int i = 0; i < numofneurons; i++)
                hidden_out[i] = neurons[i].Output;
            nextLayer.Data = hidden_out;//передача выходного сигнала на вход следующего слоя
        }

        public override double[] BackwardPass(double[] gr_sums)
        {
            double[] gr_sum = new double[numofprevneurons];
            return gr_sum;
        }
    }
}
