using System;

namespace AzureAdminClientLib
{
    public class ImageInfo
    {
        public enum OsEnum { Windows, Linux };
        public enum RecommendedSizeEnum { Small, Large, Medium, ExtraLarge };
        public string label;
        public string MediaLink;
        public string ImageName;
        public OsEnum OS;
        public string EULA;
        public string Description;
        public string Family;
        public DateTime PublishedDate;
        public bool IsPremium;
        public bool IsGallery;
        public RecommendedSizeEnum RemommendedSize;
        public string Language;
    }

    public class ImageOps
    {
        const string URLTEMPLATE_GETOSIMAGES = "https://management.core.windows.net/{0}/services/images";
        const string URLTEMPLATE_CREATEIMAGE = "https://management.core.windows.net/{0}/services/images";
        const string BODYTEMPLATE_CREATEIMAGE =
            @"<?xml version=""1.0"" encoding=""utf-8""?><OSImage xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
            <Label>{label}</Label>
            <MediaLink>{MediaLink}</MediaLink>
            <Name>{ImageName}</Name>
            <OS>{OS}</OS>
            </OSImage>";

        const string BODYTEMPLATE_CREATEIMAGEY =
            @"<?xml version=""1.0"" encoding=""utf-8""?><OSImage xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
            <Label>{label}</Label>
            <MediaLink>{MediaLink}</MediaLink>
            <Name>{ImageName}</Name>
            <OS>{OS}</OS>
            <Eula>{EULA}</Eula>
            <Description>{Description}</Description>
            <ImageFamily>{Family}</ImageFamily>
            <PublishedDate>{PublishedDate}</PublishedDate>
            <IsPremium>{IsPremium}</IsPremium>
            <ShowInGui>{IsGallery}</ShowInGui>
            <RecommendedVMSize>{RecommendedSize}</RecommendedVMSize>
            <Language>{Language}</Language>
            </OSImage>";

        private readonly Connection _Connection = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// 
        //*********************************************************************

        public ImageOps(Connection connection)
        {
            _Connection = connection;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public AzureAdminClientLib.HttpResponse GetOsImageList()
        {
            var url = string.Format(URLTEMPLATE_GETOSIMAGES, _Connection.SubcriptionID);
            var hi = new HttpInterface(_Connection);
            return hi.PerformRequest(HttpInterface.RequestType_Enum.GET, url);
        }

        public AzureAdminClientLib.HttpResponse CreateImage(ImageInfo imageInfo)
        {
            var url = string.Format(URLTEMPLATE_CREATEIMAGE, _Connection.SubcriptionID);
            var body = string.Copy(BODYTEMPLATE_CREATEIMAGE);

            body = body.Replace("{Description}", imageInfo.Description);
            body = body.Replace("{EULA}", imageInfo.EULA);
            body = body.Replace("{Family}", imageInfo.Family);
            body = body.Replace("{ImageName}", imageInfo.ImageName);
            body = body.Replace("{IsGallery}", imageInfo.IsGallery ? "true" : "false");
            body = body.Replace("{IsPremium}", imageInfo.IsPremium ? "true" : "false");
            body = body.Replace("{label}", imageInfo.label);
            body = body.Replace("{Language}", imageInfo.Language);
            body = body.Replace("{MediaLink}", imageInfo.MediaLink);
            body = body.Replace("{OS}", imageInfo.OS.ToString());
            body = body.Replace("{PublishedDate}", imageInfo.PublishedDate.ToShortTimeString());
            body = body.Replace("{RecommendedSize}", imageInfo.RemommendedSize.ToString());

            var hi = new HttpInterface(_Connection);
            return hi.PerformRequest(HttpInterface.RequestType_Enum.POST, url, body);
        }
    }
}
