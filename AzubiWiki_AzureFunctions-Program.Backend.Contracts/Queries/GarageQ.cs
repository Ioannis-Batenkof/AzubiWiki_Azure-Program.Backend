using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries
{
    public class GarageQ
    {
        public Guid ID { get; set; }
        public string BelongsTo { get; set; }
    }
}
