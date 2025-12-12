namespace MO_32_2_Topolyan_NumbersAI.NeuroNet
{
    class Network
    {
        private InputLayer input_layer = null;
        private HiddenLayer hidden_layer1 = new HiddenLayer(71, 15, NeuronType.Hidden, nameof(hidden_layer1));
        private HiddenLayer hidden_layer2 = new HiddenLayer(34, 71, NeuronType.Hidden, nameof(hidden_layer2));
        private OutputLayer output_layer = new OutputLayer(10, 34, NeuronType.Output, nameof(output_layer));

        private double[] fact = new double[10];//массив фактического выхода
        private double[] e_error_avr;//среднее значение энергии ошибки  (cумма квадратов ошибок
        private double[] accuracy;
        public double[] Fact { get => fact; }
        public double[] E_error_avr { get => e_error_avr; set => e_error_avr = value; } //средняя энергия ошибки
        public double[] Accuracy { get => accuracy; set => accuracy = value; }
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
            int epoches = 12; //кол-во эпох обучения
            double tmpSumError; // временная переменная суммы ошибок
            double[] errors; //вектор (массив) сигнала ошибки выходного слоя
            double[] temp_gsums1; // вектора градиента 1-го скрытого слоя
            double[] temp_gsums2;// вектора градиента 2-го скрытого слоя

           
           
            e_error_avr = new double[epoches];
            accuracy= new double[epoches];
            for (int k = 0; k < epoches; k++) // перебор эпох
            {
                //double correctAns = 0.0; //Правильные ответы
                e_error_avr[k] = 0; // вначале каждой эпохи ошибка = 0    Е(n)
                accuracy[k] = 0;
                net.input_layer.Shuffling_Array_Rows(net.input_layer.Trainset); //перетасовка
                for (int i = 0; i < net.input_layer.Trainset.GetLength(0); i++)//n
                {
                    double[] tmpTrain = new double[15];
                    for (int j = 0; j < tmpTrain.Length; j++)
                    {
                        tmpTrain[j] = net.input_layer.Trainset[i, j + 1];
                    }
                    //прямой проход
                    ForwardPass(net, tmpTrain);

                    //вычисление ошибки по итерации
                    tmpSumError = 0; // обнуляем для каждого обучающего образа   e(n)
                   
                    int curAns = 0;
                    double maxOutput = -100;
                    errors = new double[net.fact.Length]; // массив ошибок выходного слоя ej(n)
                    for (int x = 0; x < errors.Length; x++)
                    {
                        if (net.fact[x] > maxOutput) //условие для точности
                        {
                            maxOutput = net.fact[x];
                            curAns = x;
                        }
                        if (x == net.input_layer.Trainset[i, 0]) // если номер выходного слоя совпадает с желаемым откликом
                        {
                            errors[x] = 1.0 - net.fact[x]; //(dj(n)-yj(n)) где net.fact[x] это вероятность получения числа которая должна в идеале равняться 1(то есть dj(n))
                       
                        }
                        else
                        {
                            errors[x] = -net.fact[x]; //0.0 - net.fact[x];
                        }
                        tmpSumError += errors[x] * errors[x] / 2; //сумма энергии ошибки каждого нейрона выходного слоя
                    }
                    if (curAns == (int)net.input_layer.Trainset[i, 0])
                        accuracy[k] += 1;

                    e_error_avr[k] += tmpSumError / errors.Length; //суммарное значение энергии ошибки k-ой эпохи  Eav=1/N SUM(E(n))

                    // обратный проход и коррекиця весов!!!!!!!
                    temp_gsums2 = net.output_layer.BackwardPass(errors);
                    temp_gsums1 = net.hidden_layer2.BackwardPass(temp_gsums2);
                    net.hidden_layer1.BackwardPass(temp_gsums1);
                }
                e_error_avr[k] /= net.input_layer.Trainset.GetLength(0); // среднее значение энергии ошибки одной эпохи
                accuracy[k] /= net.input_layer.Trainset.GetLength(0);
            }

            net.input_layer = null; // обнуление (уборка) слоя

            //запись скорректированных весов в память
            net.hidden_layer1.WeightsInitializer(MemoryMode.SET, nameof(hidden_layer1) + "_memory.csv");
            net.hidden_layer2.WeightsInitializer(MemoryMode.SET, nameof(hidden_layer2) + "_memory.csv");
            net.output_layer.WeightsInitializer(MemoryMode.SET, nameof(output_layer) + "_memory.csv");
        }

        public void Test(Network net) //backprogration method
        {
            net.input_layer = new InputLayer(NetworkMode.Test);
            int epoches = 5; //кол-во эпох обучения
            double tmpSumError; // временная переменная суммы ошибок
            double[] errors; //вектор (массив) сигнала ошибки выходного слоя
            double[] temp_gsums1; // вектора градиента 1-го скрытого слоя
            double[] temp_gsums2;// вектора градиента 2-го скрытого слоя

      

            e_error_avr = new double[epoches];
            accuracy = new double[epoches];
            for (int k = 0; k < epoches; k++) // перебор эпох
            {
                e_error_avr[k] = 0; // вначале каждой эпохи ошибка = 0
                accuracy[k] = 0;
                //double correctAns = 0.0; //Правильные ответы
                net.input_layer.Shuffling_Array_Rows(net.input_layer.Testset); //перетасовка
                for (int i = 0; i < net.input_layer.Testset.GetLength(0); i++)
                {

                    double[] tmpTest = new double[15];
                    for (int j = 0; j < tmpTest.Length; j++)
                    {
                        tmpTest[j] = net.input_layer.Testset[i, j + 1];
                    }
                    //прямой проход
                    ForwardPass(net, tmpTest);

                   
                    int curAns = 0;
                    double maxOutput = 0;
                    //вычисление ошибки по итерации
                    tmpSumError = 0; // обнуляем для каждого обучающего образа
                    errors = new double[net.fact.Length]; // массив ошибок выходного слоя
                    for (int x = 0; x < errors.Length; x++)
                    {
                        if (net.fact[x] > maxOutput) //условие для точности
                        {
                            maxOutput = net.fact[x];
                            curAns = x;
                        }
                        if (x == net.input_layer.Testset[i, 0]) // если номер выходного слоя совпадает с желаемым откликом
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

                    if (curAns == net.input_layer.Testset[i, 0])
                        accuracy[k] += 1;
                    // обратный проход и коррекиця весов!!!!!!!
                    //temp_gsums2 = net.output_layer.BackwardPass(errors);
                    //temp_gsums1 = net.hidden_layer2.BackwardPass(temp_gsums2);
                    //net.hidden_layer1.BackwardPass(temp_gsums1);
                }
                e_error_avr[k] /= net.input_layer.Testset.GetLength(0); // среднее значение энергии ошибки одной эпохи
                accuracy[k]/=net.input_layer.Testset.GetLength(0);
            }

            net.input_layer = null; // обнуление (уборка) слоя
           
        }


        public void Dropout()
        {
            hidden_layer1.Dropout();
            hidden_layer2.Dropout();
            output_layer.Dropout();
        }


    }
}
