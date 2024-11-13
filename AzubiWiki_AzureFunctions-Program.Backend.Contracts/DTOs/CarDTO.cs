using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Contracts.DTOs
{
    public class CarDTO
    {
        public Guid ID { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Horsepower { get; set; }
        public bool Available { get; set; }
    }
}
