using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.MachineLearning.VectorMachines;

namespace lecture_6_svm_example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private static Dictionary<string, double> dicVocabulary = new Dictionary<string, double>();

        private void btnConstruct_Click(object sender, RoutedEventArgs e)
        {
            initVocabulary();
            initFeatureVectors();
            // printFeatureVectors();
            doSVM();
        }

        private void doSVM()
        {
            double[][] inputs =
{
    new double[] { 0, 0 }, // the XOR function takes two booleans
    new double[] { 0, 1 }, // and computes their exclusive or: the
    new double[] { 1, 0 }, // output is true only if the two booleans
    new double[] { 1, 1 }  // are different
};
            inputs = new double[1100][];

            var vrNegativeTrainingSet = dicDocumentsFeatures.Where(pr => pr.Value.dblDocumentClass == 0).Take(550);

            var vrNegativeTestSet = dicDocumentsFeatures.Where(pr => pr.Value.dblDocumentClass == 0).Skip(550);

            var vrPositiveTrainingSet = dicDocumentsFeatures.Where(pr => pr.Value.dblDocumentClass == 1).Take(550);

            var vrPositiveTestSet = dicDocumentsFeatures.Where(pr => pr.Value.dblDocumentClass == 1).Skip(550);

            int irInputIndex = 0;

            double[] outputClass = new double[1100];

            foreach (var vrPerDocument in vrNegativeTrainingSet)
            {
                inputs[irInputIndex] = vrPerDocument.Value.featureWeights.ToArray();
                outputClass[irInputIndex] = 0;
                irInputIndex++;
            }

            foreach (var vrPerDocument in vrPositiveTrainingSet)
            {
                inputs[irInputIndex] = vrPerDocument.Value.featureWeights.ToArray();
                outputClass[irInputIndex] = 1;
                irInputIndex++;
            }

            // Now, we can create the sequential minimal optimization teacher
            var learn = new SequentialMinimalOptimization()
            {
                UseComplexityHeuristic = true,
                UseKernelEstimation = false
              
            };

            // And then we can obtain a trained SVM by calling its Learn method
            SupportVectorMachine svm = learn.Learn(inputs, outputClass);

            // Finally, we can obtain the decisions predicted by the machine:
            bool[] prediction = svm.Decide(inputs);
            int irTrueCount = 0;
            for (int i = 0; i < 1100; i++)
            {
                if (prediction[i] == Convert.ToBoolean(outputClass[i]))
                {
                    irTrueCount++;
                }
            }

            MessageBox.Show($"the trained model can correctly predict {irTrueCount} and fails {1100 - irTrueCount} training data - success: {successPercent(irTrueCount, 1100 - irTrueCount)}");

            int irCount = vrNegativeTestSet.ToList().Count + vrPositiveTestSet.ToList().Count;
            inputs = new double[irCount][];

            irInputIndex = 0;

            outputClass = new double[irCount];

            foreach (var vrPerDocument in vrNegativeTestSet)
            {
                inputs[irInputIndex] = vrPerDocument.Value.featureWeights.ToArray();
                outputClass[irInputIndex] = 0;
                irInputIndex++;
            }

            foreach (var vrPerDocument in vrPositiveTestSet)
            {
                inputs[irInputIndex] = vrPerDocument.Value.featureWeights.ToArray();
                outputClass[irInputIndex] = 1;
                irInputIndex++;
            }

            // Finally, we can obtain the decisions predicted by the machine:
            prediction = svm.Decide(inputs);
            irTrueCount = 0;
            for (int i = 0; i < irCount; i++)
            {
                if (prediction[i] == Convert.ToBoolean(outputClass[i]))
                {
                    irTrueCount++;
                }
            }

            MessageBox.Show($"the trained model can correctly predict {irTrueCount} and fails {irCount - irTrueCount} training data - success: {(successPercent(irTrueCount,(irCount - irTrueCount)))}");

        }

        private static string successPercent(int irTrue, int irFalse)
        {
            double dblTotal = Convert.ToDouble(irTrue) + Convert.ToDouble(irFalse);
            return (Convert.ToDouble(irTrue) / dblTotal * 100.0).ToString("N0") + "%";
        }

        private void printFeatureVectors()
        {
            File.WriteAllLines("positive_processed.txt", dicDocumentsFeatures.Where(pr => pr.Value.dblDocumentClass == 1).Select(pr => pr.Key.ToString() + " ; " + string.Join(", ", pr.Value.featureWeights)));

            File.WriteAllLines("negative_processed.txt", dicDocumentsFeatures.Where(pr => pr.Value.dblDocumentClass == 0).Select(pr => pr.Key.ToString() + " ; " + string.Join(", ", pr.Value.featureWeights)));
        }

        private Dictionary<double, stPerDocument> dicDocumentsFeatures = new Dictionary<double, stPerDocument>();

        public struct stPerDocument
        {
            public double dblDocId;
            public string srDocumentText;
            public double[] featureWeights;
            public double dblDocumentClass;//0 = negative, 1 = positive
        }

        private void initFeatureVectors()
        {
            double dblDocClass = 0;
            double dblDocId = 0;
            foreach (var vrFileName in new List<string> { "Negative_raw.txt", "Positive_raw.txt" })
            {
                foreach (var vrLine in File.ReadLines(vrFileName))
                {
                    dblDocId++;
                    stPerDocument stPerDocument = new stPerDocument();
                    stPerDocument.dblDocId = dblDocId;
                    stPerDocument.dblDocumentClass = dblDocClass;
                    stPerDocument.featureWeights = new double[dicVocabulary.Count];

                    foreach (var vrWord in vrLine.Split(' '))
                    {
                        stPerDocument.featureWeights[(Int64)dicVocabulary[vrWord]] = 1;
                    }

                    dicDocumentsFeatures.Add(dblDocId, stPerDocument);
                }

                dblDocClass++;
            }
        }

        private static void initVocabulary()
        {
            double dblWordIndex = 0;
            foreach (var vrFileName in new List<string> { "Negative_raw.txt", "Positive_raw.txt" })
            {
                foreach (var vrLine in File.ReadLines(vrFileName))
                {
                    foreach (var vrWord in vrLine.Split(' '))
                    {
                        if (dicVocabulary.ContainsKey(vrWord))
                        {
                            continue;
                        }

                        dicVocabulary.Add(vrWord, dblWordIndex);
                        dblWordIndex++;
                    }
                }
            }
        }
    }
}
