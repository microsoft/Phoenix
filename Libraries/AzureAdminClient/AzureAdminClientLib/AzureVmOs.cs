//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Serialization;

//namespace AzureAdminClientLib
//{
//    public class AzureVmOs
//    {
//        [XmlElement]
//        public string Category { get; set; }
//        [XmlElement]
//        public string Label { get; set; }
//        [XmlElement]
//        public string Location { get; set; }
//        [XmlElement]
//        public int LogicalSizeInFB { get; set; }
//        [XmlElement]
//        public string Name { get; set; }
//        [XmlElement]
//        public string OS { get; set; }
//        [XmlElement]
//        public string Eula { get; set; }
//        [XmlElement]
//        public string Description { get; set; }
//        [XmlElement]
//        public string ImageFamily { get; set; }
//        [XmlElement]
//        public DateTime PublishedDate { get; set; }
//        [XmlElement]
//        public Boolean IsPremium { get; set; }
//        [XmlElement]
//        public string IconUri { get; set; }
//        [XmlElement]
//        public string PrivacyUri { get; set; }
//        [XmlElement]
//        public string RecommendedVMSize { get; set; }
//        [XmlElement]
//        public string PublisherName { get; set; }
//        [XmlElement]
//        public string SmallIconUri { get; set; }
//    }

//    [XmlRoot(Namespace = "http://schemas.microsoft.com/windowsazure", ElementName = "Images", IsNullable = true)]
//    public class AzureVmOsDataSet
//    {
//        [XmlElement(ElementName = "OSImage")]
//        public List<AzureVmOs> OSImages { get; set; }

//        public AzureVmOsDataSet()
//        {
//            OSImages = new List<AzureVmOs>();
//        }
//    }
//}
