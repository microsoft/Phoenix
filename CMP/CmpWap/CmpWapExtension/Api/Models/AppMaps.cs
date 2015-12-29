namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AppMap")]
    public partial class AppMap
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string AppIdCode { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string Config { get; set; }

        public string TagData { get; set; }

        public int? TagId { get; set; }

        public bool? IsActive { get; set; }
    }
}
