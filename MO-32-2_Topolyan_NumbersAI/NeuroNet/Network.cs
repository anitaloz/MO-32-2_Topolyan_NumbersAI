namespace MO_32_2_Topolyan_NumbersAI.NeuroNet
{
    class Network
    {
        private InputLayer input_layer = null;
        private HiddenLayer hidden_layer1 = new HiddenLayer(71, 15, NeuronType.Hidden, nameof(hidden_layer1));
        private HiddenLayer hidden_layer2 = new HiddenLayer(34, 71, NeuronType.Hidden, nameof(hidden_layer2));
        private OutputLayer output_layer = new OutputLayer(10, 34, NeuronType.Output, nameof(output_layer));

        private double[] fact = new double[10];//массив фактического выхода
        private double[] e_error_avr;//среднее значение энергии ошибки 
        public double[] Fact { get => fact; }
        public double[] E_error_avr { get => e_error_avr; set => e_error_avr = value; }

        public Network() { }

        public void ForwardPass(Network net, double[] netInput)
        {
            net.hidden_layer1.Data = netInput;
            net.hidden_layer1.Recognize(null, net.hidden_layer2);
            net.hidden_layer2.Recognize(null, net.output_layer);
            net.output_layer.Recognize(net, null);
        }

        // непосредственно обучение
        public void Train(Network net) //backprogration method
        {
            net.input_layer = new InputLayer(NetworkMode.Train);
            int epoches = 40; //кол-во эпох обучения
            double tmpSumError; // временная переменная суммы ошибок
            double[] errors; //вектор (массив) сигнала ошибки выходного слоя
            double[] temp_gsums1; // вектора градиента 1-го скрытого слоя
            double[] temp_gsums2;// вектора градиента 2-го скрытого слоя

            e_error_avr = new double[epoches];
            for (int k = 0; k < epoches; k++) // перебор эпох
            {
                e_error_avr[k] = 0; // вначале каждой эпохи ошибка = 0
                net.input_layer.Shuffling_Array_Rows(net.input_layer.Trainset); //перетасовка
                for (int i = 0; i < net.input_layer.Trainset.GetLength(0); i++)
                {
                    double[] tmpTrain = new double[15];
                    for (int j = 0; j < tmpTrain.Length; j++)
                    {
                        tmpTrain[j] = net.input_layer.Trainset[i, j + 1];
                    }
                    //прямой проход
                    ForwardPass(net, tmpTrain);

                    //вычисление ошибки по итерации
                    tmpSumError = 0; // обнуляем для каждого обучающего образа
                    errors = new double[net.fact.Length]; // массив ошибок выходного слоя
                    for (int x = 0; x < errors.Length; x++)
                    {
                        if (x == net.input_layer.Trainset[i, 0]) // если номер выходного слоя совпадает с желаемым откликом
                        {
                            errors[x] = 1.0 - net.fact[x];
                        }
                        else
                        {
                            errors[x] = -net.fact[x]; //0.0 - net.fact[x];
                        }
                        tmpSumError += errors[x] * errors[x] / 2;
                    }
                    e_error_avr[k] += tmpSumError / errors.Length; //суммарное значение энергии ошибки k-ой эпохи

                    // обратный проход и коррекиця весов!!!!!!!
                    temp_gsums2 = net.output_layer.BackwardPass(errors);
                    temp_gsums1 = net.hidden_layer2.BackwardPass(temp_gsums2);
                    net.hidden_layer1.BackwardPass(temp_gsums1);
                }
                e_error_avr[k] /= net.input_layer.Trainset.GetLength(0); // среднее значение энергии ошибки одной эпохи
            }

            net.input_layer = null; // обнуление (уборка) слоя

            //запись скорректированных вексов в память
            net.hidden_layer1.WeightsInitializer(MemoryMode.SET, "memory\\"+nameof(hidden_layer1) + "_memory.csv");
            net.hidden_layer2.WeightsInitializer(MemoryMode.SET, "memory\\"+nameof(hidden_layer2) + "_memory.csv");
            net.output_layer.WeightsInitializer(MemoryMode.SET, "memory\\"+nameof(output_layer) + "_memory.csv");
        }
    }
}
