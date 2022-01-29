using System;
using System.Collections.Generic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.SOM;
using Encog.Neural.SOM.Training.Neighborhood;
using Encog.ML.Train;
using Encog.ML.Data.Basic;
using Encog;

namespace predictor_v3
{
    internal class Program
    {
        private static double [][] readDataMatrix(System.IO.StreamReader inputFile, int rows, int cols)
        {   
            double[][] matrix = new double[cols][];

            
            for (int i = 0; i < cols; i++)
            {
                matrix[i] = new double[rows];
            }

            for (int i = 0; i < rows; i++)
            {
                string[] currentRow = inputFile.ReadLine().Split(' ');

                for (int j = 0; j < cols; j++)
                {
                    matrix[j][i] = double.Parse(currentRow[j]);
                }
            }

            inputFile.Close();

            return matrix;
        }

        private static void normalize(double[][] matrix, int rows, int cols) {
            for (int i = 0; i < rows; i++) 
            {
                double maxValue = 0;

                for (int j = 0; j < cols; j++)
                {
                    if(matrix[i][j] > maxValue)
                    {
                        maxValue = matrix[i][j];
                    }
                }

                for (int j = 0; j < cols; j++)
                {
                    matrix[i][j] /= maxValue;
                }
            }
        }

        private static void Main(string[] args)
        {
            string inputFileName;
            string outputFileName;

            System.IO.StreamReader inputFile;
            System.IO.StreamWriter outputFile;

            if (args.Length == 0)
            {
                Console.WriteLine("Please, write input and output file names with their absolute pathes !");
                string[] fileNames = Console.ReadLine().Split(' ');
                inputFileName = fileNames[0];
                outputFileName = fileNames[1];
            }
            else
            {
                inputFileName = args[0];
                outputFileName = args[1];
            }

            inputFile = new System.IO.StreamReader(inputFileName);
            outputFile = new System.IO.StreamWriter(outputFileName);

            if(inputFile == null)
            {
                Console.WriteLine("InputFile can not be opened !");
            }
            if (outputFile == null)
            {
                Console.WriteLine("InputFile can not be opened !");
            }

            int cols = 0;
            int rows = 0;

            string[] metrics = inputFile.ReadLine().Split(' ');
            rows = int.Parse(metrics[0]);
            cols = int.Parse(metrics[1]);

            double[][] data = readDataMatrix(inputFile, rows, cols);

            // readDataMatrix reads data in transposed form. Thus, we should swap row and cols
            int tmp = rows;
            rows = cols;
            cols = tmp;

            normalize(data, rows, cols);

            IMLDataSet trainingData = new BasicMLDataSet(data, null);

            //cols - dimension of input data, rows - number of output neurons
            SOMNetwork network = new SOMNetwork(cols, rows);
            network.Reset();

            BasicTrainSOM train = new BasicTrainSOM(network, 0.7, trainingData, new NeighborhoodSingle());
            train.AutoDecay();

            for(int i = 0; i < trainingData.Count ; i++)
            {
                train.Iteration();
            }

            List<IMLData> samples = new List<IMLData>();

            for(int i = 0; i < data.Length; i++)
            {
                samples.Add(new BasicMLData(data[i]));
            }

            string [] outputData = new string[samples.Count];

            for(int i = 0; i < samples.Count; i++)
            {
                int index = network.Classify(samples[i]);
                outputData[index] += i.ToString() + " ";
            }


            int counter = 0;

            for (int i = 0; i < outputData.Length; i++)
            {
                if (outputData[i] != null)
                {
                    string[] str = outputData[i].Split(' ');
                    if (str.Length > 2)
                    {
                        counter++;
                    }
                }

            }

            outputFile.WriteLine(counter);

            for (int i = 0; i < outputData.Length; i++)
            {
                if(outputData[i] != null){
                    string[] str = outputData[i].Split(' ');
                    if(str.Length > 2)
                    {
                        outputFile.WriteLine(outputData[i]);
                    }
                }
                
            }

            inputFile.Close();
            outputFile.Close();
        }
    }
}
