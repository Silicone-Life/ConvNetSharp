using System;
using System.IO;
using System.Net;
using MnistDemo.GPU;

namespace MnistDemo.Flow.GPU
{
    internal class DataSets
    {
        private const string urlMnist = @"http://yann.lecun.com/exdb/mnist/";
        private const string mnistFolder = @"..\Mnist\";
        private const string trainingLabelFile = "train-labels-idx1-ubyte.gz";
        private const string trainingImageFile = "train-images-idx3-ubyte.gz";
        private const string testingLabelFile = "t10k-labels-idx1-ubyte.gz";
        private const string testingImageFile = "t10k-images-idx3-ubyte.gz";

        public DataSet Train { get; set; }

        public DataSet Validation { get; set; }

        public DataSet Test { get; set; }

        private void DownloadFile(string urlFile, string destFilepath)
        {
            if (!File.Exists(destFilepath))
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(urlFile, destFilepath);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed downloading " + urlFile);
                    Console.WriteLine(e.Message);
                }
            }
        }

        public bool Load(int validationSize = 1000)
        {
            Directory.CreateDirectory(mnistFolder);

            var trainingLabelFilePath = Path.Combine(mnistFolder, trainingLabelFile);
            var trainingImageFilePath = Path.Combine(mnistFolder, trainingImageFile);
            var testingLabelFilePath = Path.Combine(mnistFolder, testingLabelFile);
            var testingImageFilePath = Path.Combine(mnistFolder, testingImageFile);

            // Download Mnist files if needed
            Console.WriteLine("Downloading Mnist training files...");
            this.DownloadFile(urlMnist + trainingLabelFile, trainingLabelFilePath);
            this.DownloadFile(urlMnist + trainingImageFile, trainingImageFilePath);
            Console.WriteLine("Downloading Mnist testing files...");
            this.DownloadFile(urlMnist + testingLabelFile, testingLabelFilePath);
            this.DownloadFile(urlMnist + testingImageFile, testingImageFilePath);

            // Load data
            Console.WriteLine("Loading the datasets...");
            var trainImages = MnistReader.Load(trainingLabelFilePath, trainingImageFilePath);
            var testingImages = MnistReader.Load(testingLabelFilePath, testingImageFilePath);

            var valiationImages = trainImages.GetRange(trainImages.Count - validationSize, validationSize);
            trainImages = trainImages.GetRange(0, trainImages.Count - validationSize);

            if (trainImages.Count == 0 || valiationImages.Count == 0 || testingImages.Count == 0)
            {
                Console.WriteLine("Missing Mnist training/testing files.");
                Console.ReadKey();
                return false;
            }

            this.Train = new DataSet(trainImages);
            this.Validation = new DataSet(valiationImages);
            this.Test = new DataSet(testingImages);

            return true;
        }
    }
}