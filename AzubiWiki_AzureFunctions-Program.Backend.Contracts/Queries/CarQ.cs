using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries
{
    public class CarQ
    {
        public Guid ID { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string Horsepower { get; set; }
        public bool Available { get; set; }
    }
}
