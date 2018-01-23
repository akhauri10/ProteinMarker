using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Peptide
    {
        public Peptide()
        {
            PermutedSplitString = new List<string>();
            PeptibaseResponse = new List<ServerResponse>();
        }
        public string PermutedString { get; set; }
        public string PermutedStringFileName { get; set; }
        public List<string> PermutedSplitString { get; set; }
        public List<ServerResponse> PeptibaseResponse { get; set; }
        public int Score { get; set; }
    }
}
