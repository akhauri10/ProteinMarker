using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service;

namespace ProteinMarker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Input Protein String:");
            var proteinString = Console.ReadLine();
            Console.Write("Input no of partitions:");
            var noOfPartions = Convert.ToInt32(Console.ReadLine());

            Processor process = new Processor();
            process.ProcessProtein(proteinString, noOfPartions);
            Console.ReadLine();
        }        
    }
}
