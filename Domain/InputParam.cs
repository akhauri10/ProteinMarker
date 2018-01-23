using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class InputParam
    {
        public InputParam(string inputString, int noOfDivisions)
        {
            InputString = inputString;
            NoOfDivisions = noOfDivisions;
        }
        public string InputString { get; private set; }

        public int NoOfDivisions { get; private set; }
        
    }
}
