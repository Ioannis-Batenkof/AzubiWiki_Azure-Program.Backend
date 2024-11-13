using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Model
{
    public class Garage
    {
        public Guid ID { get; set; }
        public string BelongsTo { get; set; }
        public List<Car> Cars { get; set; }
        public const int Limit = 4;
    }
}
