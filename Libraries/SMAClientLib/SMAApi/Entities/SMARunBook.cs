using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAApi.Entities
{

    #region Properties
    public class SMARunbook
    {
       public string Description { get; set; }
       public Guid Id { get; set; }
       public string Name { get; set; }
       public string Tags { get; set; }
       public Guid PublishedRunbookVersionID { get; set; }

       public IQueryable<SMARunbookParameter> Parameters { get; set; }
    }

    #endregion
}
