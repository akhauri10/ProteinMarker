using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ServerResponse
    {
        public string PeptideString { get; set; }

        public int PeptideSequence { get; set; }

        public double Score { get; set; }
    }
}
