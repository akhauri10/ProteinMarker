using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Peptides
    {
        public Peptides()
        {
            PeptideList = new Dictionary<string, Peptide>();
        }
        Dictionary<string, Peptide> PeptideList { get; set; }
    }
}
