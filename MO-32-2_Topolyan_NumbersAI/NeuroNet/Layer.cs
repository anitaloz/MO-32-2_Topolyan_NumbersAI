using System;
using System.IO;
using System.Windows.Forms;
using static System.Math;

namespace MO_32_2_Topolyan_NumbersAI.NeuroNet
{
    abstract class Layer
    {
        protected string name_Layer;
        string pathDirWeights;
        string pathFileWeights;
        protected int numofneurons;
        protected int numofprevneurons;
        protected const double learningrate = 0.60;
        protected const double momentum = 0.050d;
        protected double[,] lastdeltaweights;
        protected double[,] temporaryWeights;
        protected Neuron[] neurons;

        public Neuron[] Neurons { get => neurons; set=>neurons = value; }
        public double[] Data
        {
            set
            {
                for(int i = 0; i<numofneurons;++i)
                {
                    Neurons[i].Activator(value);
                }
            }
        }

        protected Layer(int non, int nopn, NeuronType nt, string nm_Layer)
        {
            int i, j;
            numofneurons = non;
            numofprevneurons = nopn;
            Neurons = new Neuron[non];
            name_Layer= nm_Layer;
            pathDirWeights = AppDomain.CurrentDomain.BaseDirectory+"memory\\";
            pathFileWeights = pathDirWeights + name_Layer + "memory.csv";

            lastdeltaweights = new double[non, nopn + 1];
            //double[,] weights;//временный массив синаптических весов текущего слоя
            temporaryWeights=WeightsInitializer(MemoryMode.INIT, pathDirWeights + name_Layer + "memory.csv");
            WeightsInitializer(MemoryMode.SET, pathDirWeights + name_Layer + "memory.csv"); 
            
        }
       

        private double[,] WeightsInitializer(MemoryMode mm, string path)//может лучше тогда передавать сюда массив? бред какой-то
        {
            double[,] weights = new double[numofneurons, numofprevneurons + 1];
           
            switch (mm)
            {
                case MemoryMode.GET:

                    break;

                case MemoryMode.SET:
                    string SETpath=path;
                    string strSET="";
                    for (int i = 0; i < numofneurons; i++)
                    {
                        strSET+= temporaryWeights[i, 0].ToString();//временные
                        for(int j=1; j<numofprevneurons+1; j++)
                        {
                            strSET += ";" + temporaryWeights[i, j];//временные 
                        }
                        strSET += "\n";
               
                    }
                    File.WriteAllText(SETpath, strSET);
                    break;

                case MemoryMode.INIT:
                    weights = RandomInit(numofneurons, numofprevneurons + 1);
                    weights = SrChanger(numofneurons, numofprevneurons + 1, weights);
                    break;
                   

            }
            return weights;
        }

        private double[,] RandomInit(int a, int b)
        {
            double[,] weights = new double[a, b];
            Random random = new Random();
            for (int i = 0; i < a; i++)
            {

                for (int j = 0; j < b; j++)
                {
                    weights[i, j] = random.NextDouble();

                }
            }
            return weights;
        }

        private double[,] SrChanger(int a, int b, double[,] cweights)//создает отклонение=1 и мат ожидание 0
        {
            double[,] weights = cweights;
            for (int i = 0; i < a; i++)
            {
                double sr = 0;
                for (int j = 0; j < b; j++)
                {
                    sr += weights[i, j];
                }
                sr /= (double)b;
                double s = 0;
                for (int j = 0; j < b; j++)
                {
                    weights[i, j] = weights[i, j] - sr;
                    s += weights[i, j];
                }
                s /= (double)b;//перестраховка 
                double disp = 0;
                for (int j = 0; j < b; j++)
                {
                    disp += Pow(weights[i, j] - s, 2);
                }
                disp /= (double)b;
                for (int j = 0; j < b; j++)
                {
                    weights[i, j] = weights[i, j] / Sqrt(disp);
                }

            }
            return weights;
        }


    }
}
