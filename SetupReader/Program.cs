using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SetupReader
{
    class Program
    {
        struct Comparison
        {
            public byte Value1;
            public byte Value2;
            public bool Match;
        }

        static void Main(string[] args)
        {
            var file1Bytes = File.ReadAllBytes(@"C:\Users\Stuart\Downloads\VBinDiff-3.0_beta4\Test1.rds");
            var file2Bytes = File.ReadAllBytes(@"C:\Users\Stuart\Downloads\VBinDiff-3.0_beta4\Test2.rds");

            var matches = new List<Comparison>(file1Bytes.Length);
            var differences = new List<int>();

            var file1Enum = file1Bytes.GetEnumerator();
            var file2Enum = file2Bytes.GetEnumerator();

            for(int i = 0; i <file1Bytes.Length; i++)
            {
                byte value1 = file1Bytes[i];
                byte value2 = file2Bytes[i];

                matches.Add(new Comparison 
                { 
                    Value1 = value1,
                    Value2 = value2,
                    Match = value1 == value2
                });

                if (value1 != value2)
                    differences.Add(i);
            }

            Console.WriteLine("Length: {0}", file1Bytes.Length);
            Console.WriteLine("Differences: {0}", differences.Count);
            var first = differences[0];
            var last = differences[differences.Count - 2];
            Console.WriteLine("First diff: {0}, last diff -1: {1}, length: {2}", first, last, last - first + 1);
            
            Console.ReadKey();

            foreach (var diff in differences)
            {
                Console.WriteLine("{2}:\t{0}\t{1}",
                    matches[diff].Value1,
                    matches[diff].Value2,
                    diff);
            }

            Console.WriteLine("END");

            Console.ReadKey();
        }
    }
}

