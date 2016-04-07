using Org.XmlUnit.Diff;
using System;
using System.Linq;
using System.Xml;
using Org.XmlUnit.Builder;
using System.Globalization;

namespace Comparator
{
    class Program
    {
        public static void Compare(string pathToXml1, string pathToXml2)
        {
            Console.WriteLine("\n--------------------\nComparing '{0}' and '{1}':\n", pathToXml1, pathToXml2);
            XmlDocument file1Xml = new XmlDocument();
            file1Xml.Load(pathToXml1);
            XmlDocument file2Xml = new XmlDocument();
            file2Xml.Load(pathToXml2);

            var source1 = Input.From(file1Xml).Build();
            var source2 = Input.From(file2Xml).Build();

            Predicate<XmlNode> exceptedElementFilter = (XmlNode node) => { return node.Name != "userDefinedFields"; };

            Diff diff = DiffBuilder
               .Compare(source1)
               .WithTest(source2)
               .CheckForSimilar()
               .WithNodeFilter(exceptedElementFilter)
               .Build();

            var differences = diff.Differences.ToList();

            foreach (Difference difference in differences)
            {

                bool isAmount = false;
                Decimal amount1 = 0;
                Decimal amount2 = 0;
                if (difference.Comparison.TestDetails.Value != null && difference.Comparison.ControlDetails.Value != null)
                {
                    isAmount = Decimal.TryParse(difference.Comparison.TestDetails.Value.ToString(),
                        NumberStyles.Any, new CultureInfo("en-US"), out amount2);
                    isAmount = Decimal.TryParse(difference.Comparison.ControlDetails.Value.ToString(),
                        NumberStyles.Any, new CultureInfo("en-US"), out amount1);
                }


                //if we compare digits, then we should print only the ones that differ for more than 1%
                if (isAmount
                    && difference.Comparison.ControlDetails.Target.NodeType.Equals(XmlNodeType.Text)
                    && difference.Comparison.TestDetails.Target.NodeType.Equals(XmlNodeType.Text))
                {
                    if ((Math.Abs(amount1 - amount2) / amount2) > (decimal)0.01)
                    {
                        Console.WriteLine(difference.ToString());
                    }
                }
                else //if it is not a digit, then we print the difference anyway
                {
                    Console.WriteLine(difference.ToString() + "\n");
                }
            }
        }

        static void Main(string[] args)
        {
            Compare("sample1a.xml", "sample1b.xml");
            Compare("sample2a.xml", "sample2b.xml");
            Compare("sample3a.xml", "sample3b.xml");
            Compare("sample4a.xml", "sample4b.xml");
            Compare("sample5a.xml", "sample5b.xml");
            Compare("sample6a.xml", "sample6b.xml");
        }
    }
}
