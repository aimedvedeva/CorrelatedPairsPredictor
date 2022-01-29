using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace predictor_v1._1
{
    class Program
    {
        class correlationPair: IComparable {
            private int matrixRow;
            private int matrixCol;
            private double correlationValue;

            public correlationPair() 
            {
                matrixRow = 0;
                matrixCol = 0;
                correlationValue = 0;
            }

            public correlationPair(int row, int col, double correlationValue) 
            {
                matrixRow = row;
                matrixCol = col;
                this.correlationValue = correlationValue;
            }

            public void setRow(int row) 
            {
                matrixRow = row;
            }

            public void setCol(int col) 
            {
                matrixCol = col;
            }

            public void setCorrelationVal(double value) 
            {
                correlationValue = value;
            }

            public double getCorrelationVal() 
            {
                return correlationValue;
            }

            public int getRow() 
            {
                return matrixRow;
            }

            public int getCol() 
            {
                return matrixCol;
            }

            public int CompareTo(Object obj) {
                correlationPair pair = (correlationPair)obj;

                if (this.correlationValue < pair.getCorrelationVal())
                    return 1;
                else if (this.correlationValue > pair.getCorrelationVal())
                    return -1;
                return 0;
            }
        }

        private static double[][] readDataMatrix(System.IO.StreamReader inputFile, int rows, int cols)
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

        static double getMedian(double[] sample, int size)
        {
            int index = 0;

            if (size % 2 == 0)
            {
                index = size / 2;
                return sample[index];
            }
            index = size / 2;
            return sample[index] + sample[index + 1];
        }

        static double[][] getCorrelationMatrix(double[][] samples, int samplesNum, int samplesSize, double[] medians)
        {
            double[][] correlationMatrix = new double[samplesNum - 1][];
            int colsShift = 1;
            
            for (int i = 0; i < samplesNum - 1; i++){
                correlationMatrix[i] = new double[samplesNum - colsShift];
                colsShift++;
            }
  
            colsShift = 1;

            for (int i = 0; i < samplesNum; i++){
                for (int j = i + 1, rIndex = 0; (j < samplesSize && rIndex < samplesNum - colsShift); j++, rIndex++){
                correlationMatrix[i][rIndex] = 0;

                for (int k = 0; k < samplesSize; k++){
                    correlationMatrix[i][rIndex] += Math.Sign(samples[i][k] - medians[i]) * Math.Sign(samples[j][k] - medians[j]);
                }
                correlationMatrix[i][rIndex] /= samplesSize;
                }
                colsShift++;
            }

            return correlationMatrix;
        }
         
        static void Main(string[] args)
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
            if(outputFile == null)
            {
                Console.WriteLine("InputFile can not be opened !");
            }

            
            int cols = 0;
            int rows = 0;

            string[] metrics = inputFile.ReadLine().Split(' ');
            rows = int.Parse(metrics[0]);
            cols = int.Parse(metrics[1]);

            double[][] matrix = readDataMatrix(inputFile, rows, cols);

            int tmp = rows;
            rows = cols;
            cols = tmp;

            inputFile.Close();

            for (int i = 0; i < rows; i++){
                Array.Sort(matrix[i]);
            }

            double[] med = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                med[i] = getMedian(matrix[i], cols);
            }

            double[][] correlationMatrix = getCorrelationMatrix(matrix, rows, cols, med);

            int correlationPairsNum = rows * (rows - 1) / 2;

            correlationPair[] pairs = new correlationPair[correlationPairsNum];

            int currentPair = 0;
            int colShift = 1;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0, index = i + 1; j < rows - colShift; j++, index++)
                {
                    pairs[currentPair] = new correlationPair(i, index, correlationMatrix[i][j]);
                    currentPair++;
                }
                colShift++;
            }

            Array.Sort(pairs);

            outputFile.WriteLine(correlationPairsNum.ToString() + '\n');

            for (int i = 0; i < correlationPairsNum; i++ )
            {
                outputFile.WriteLine(pairs[i].getCorrelationVal().ToString() + " " + pairs[i].getRow().ToString() + " " + pairs[i].getCol().ToString() + '\n');
            }

            outputFile.Close();
        }
    }
}
