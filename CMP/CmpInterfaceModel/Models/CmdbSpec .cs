using System;

namespace CmpInterfaceModel.Models
{
    public class CmdbSpec
    {
        public bool UpdateCmdb { set; get; }
        public string Config { set; get; }
        public string TagData { set; get; }
        public string Request { set; get; }
        public string Result { set; get; }


        /// <summary>
        /// This is the property that is used both as the unique identifier and the name in ITSM. It's a bit of a misnomer in that
        /// it doesn't actually have to be unique and will still work but oh well.
        /// </summary>
        public string CIUniqueIdentifier { get; set; }

        /// <summary>
        /// The verbatim name of an Azure region.
        /// </summary>
        public string AzureRegion { get; set; }

        /// <summary>
        /// ITSM defined class name. Examples are Dev, Prod. 
        /// This is EnvironmentName attribute in the tag data
        /// </summary>
        public string EnvironmentClass { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string FinancialAssetOwner { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string ChargebackGroup { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string OrgID { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string ResponsibleOwner { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string AccountableOwner { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string CIOwner { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string MSITMonitored { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string L1DomainSupport { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string L2DomainSupport { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string L3DomainSupport { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string ParentApp { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string ServiceCategory { get; set; }

        /// <summary>
        /// ITSM defined value.
        /// </summary>
        public string ParentSubscriptionID { get; set; }

        /// <summary>
        /// records the initial record id for migration
        /// </summary>
        public string RfcNmuber { get; set; }

        public string OrgDivison { get; set; }
        public string OrgDomain { get; set; }
        public string Organization { get; set; }
        public string Team { get; set; }
        public string OrgId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationId { get; set; }

        public string ServerName { get; set; }

        public string AzureComputeSku { get; set; }



        //***************************

        //private string OrgID;
        //private string OrgDomain;
        public string OrgFinancialAssetOwner { get; set; }
        public string OrgChargebackGroup { get; set; }
        public string OrgDivision { get; set; }
        public string OrgOrg { get; set; }
        public string OrgTeam { get; set; }
        public string AppID { get; set; }
        public string AppName { get; set; }
        public string ITSMServiceCategory { get; set; }
        public string ITSMServiceWindow { get; set; }
        public string ITSMResponsibleOwner { get; set; }
        public string ITSMAccountableOwner { get; set; }
        public string ITSMCIOwner { get; set; }
        public string ITSML1SupportTeam { get; set; }
        public string ITSML2SupportTeam { get; set; }
        public string ITSML3SupportTeam { get; set; }
        public string ITSMMonitoredFlag { get; set; }
        public string ITSMNameOverride { get; set; }
        public string Nic1 { get; set; }




           /*,[OrgID]
		  ,[OrgDomain]
		  ,[OrgFinancialAssetOwner]
		  ,[OrgChargebackGroup]
		  ,[OrgDivision]
		  ,[OrgOrg]
		  ,[OrgTeam]
		  ,[AppID]
		  ,[AppName]
		  ,[ITSMServiceCategory]
		  ,[ITSMServiceWindow]
		  ,[ITSMResponsibleOwner]
		  ,[ITSMAccountableOwner]
		  ,[ITSMCIOwner]
		  ,[ITSML1SupportTeam]
		  ,[ITSML2SupportTeam]
		  ,[ITSML3SupportTeam]
		  ,[ITSMMonitoredFlag]
		  ,[ITSMNameOverride]*/


        //***************************
        
        /// <summary>
        /// Used temperorly to get the old server name to rename the itsm record associated
        /// Future will be used to get the old server name and do rename process in cmp
        //sourceservername will be used then
        /// </summary>
        public string OldItsmName { get; set; }

        public string Serialize()
        {
            return Utilities.Serialize(typeof(CmdbSpec), this);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static CmdbSpec Deserialize(string input)
        {
            try
            {
                return Utilities.DeSerialize(typeof(CmdbSpec), input, true) as CmdbSpec;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmdbSpec.Deserialize() : Unable to deserialize given VM Config structure, may be malformed : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }
    }
}
