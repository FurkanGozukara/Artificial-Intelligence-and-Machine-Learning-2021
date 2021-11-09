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
            printFeatureVectors();
        }

        private void printFeatureVectors()
        {
            File.WriteAllLines("positive_processed.txt", dicDocumentsFeatures.Where(pr => pr.Value.dblDocumentClass==1).Select(pr => pr.Key.ToString() + " ; " + string.Join(", ", pr.Value.featureWeights)));

            File.WriteAllLines("negative_processed.txt", dicDocumentsFeatures.Where(pr => pr.Value.dblDocumentClass == 0).Select(pr => pr.Key.ToString() +" ; "+ string.Join(", ", pr.Value.featureWeights)));
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
