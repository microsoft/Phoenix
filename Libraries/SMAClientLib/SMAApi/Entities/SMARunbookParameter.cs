using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAApi.Entities
{
    public class SMARunbookParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }
        public Guid RunbookVersionID { get; set; }

        public string Type { get; set; }

      
    }
}
