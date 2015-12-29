using System;
using System.Collections.Generic;
using System.Linq;
using CmpInterfaceModel;

namespace AzureAdminClientLib
{
    public class AzureAdminTaskStatus
    {
        public enum ResultEnum {Failed, Success, Succeeded };
        public string ID {get; set;}
        public string Status {get; set;}
        public string HttpStatusCode {get; set;}
        public string ErrorCode {get; set;}
        public string ErrorMessage { get; set; }
        public ResultEnum Result {get; set;}

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="operationText"></param>
        /// <param name="requestType"></param>
        ///  
        //*********************************************************************
        public AzureAdminTaskStatus(string operationText, string requestType)
        {
            if (operationText[0].Equals('{'))
            {
                switch (requestType)
                {
                    case "NewVM":
                        AzureArmTaskStatus(operationText);
                        break;
                    //It's a START, STOP OR RESTART command
                    default:
                        //AzureArmOperationStatus(operationText, requestType);
                        AzureArmOperationStatus(operationText);
                        break;
                }
            }
            else
                AzureRDFETaskStatus(operationText);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public AzureAdminTaskStatus()
        {
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationText"></param>
        /// 
        //*********************************************************************

        private void AzureArmTaskStatus(string operationText)
        {
            List<AzureEventOps.AzureException> azureExceptionList;

            if (AzureEventOps.IsComplete(
                operationText, "Microsoft.Compute", out azureExceptionList))
            {
                Status = "Complete";
                ErrorCode = null;
                ErrorMessage = null;
            }
            else if (null == azureExceptionList)
            {
                Status = "InProgress";
                ErrorCode = null;
                ErrorMessage = null;
            }
            else
            {
                Status = "Failed";
                ErrorCode = azureExceptionList[0].ErrorCode;
                ErrorMessage = azureExceptionList[0].ErrorMessage;
            }

            ID = "";
            HttpStatusCode = "";

            switch (Status)
            {
                case "Failed":
                    Result = ResultEnum.Failed;
                    break;
                default:
                    Result = ResultEnum.Success;
                    break;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationText"></param>
        /// 
        /// 
        //*********************************************************************

        private void AzureArmOperationStatus(string operationText)
        {
            try
            {
                Status = Utilities.FetchJsonValue(operationText, "status") as string;
            }
            catch (Exception e)
            {
                Status = "Failed";
                ErrorCode = e.HResult.ToString();
                ErrorMessage = e.Message;
            }
            finally
            {
                switch (Status)
                {
                    case "Failed":
                        Result = ResultEnum.Failed;
                        break;
                    default:
                        Result = ResultEnum.Success;
                        break;
                }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationText"></param>
        /// <param name="requestType"></param>
        /// 
        //*********************************************************************

        private void AzureArmOperationStatus(string operationText, string requestType)
        {
            try
            {
                var statusCollection = DataContracts.Status.DeserializeJsonInstanceViewStatus(operationText);
                DataContracts.Status s = statusCollection.Statuses.FirstOrDefault(x => x.Code.IndexOf("PowerState", StringComparison.InvariantCultureIgnoreCase) >= 0);
                if (s == null)
                {
                    Status = "Failed";
                }
                else
                {
                    Status = s.Code;
                    switch (requestType)
                    {
                        case "START":
                            Status = s.Code.IndexOf("running", StringComparison.InvariantCultureIgnoreCase) >= 0 ? "Complete" : "InProgress";
                            break;
                        case "STOP":
                            Status = s.Code.IndexOf("stop", StringComparison.InvariantCultureIgnoreCase) >= 0 ? "Complete" : "InProgress";
                            break;
                        case "RESTART":
                            Status = s.Code.IndexOf("running", StringComparison.InvariantCultureIgnoreCase) >= 0 ? "Complete" : "InProgress";
                            break;
                        case "DEALLOCATE":
                            Status = s.Code.IndexOf("deallocating", StringComparison.InvariantCultureIgnoreCase) >= 0 ? "Complete" : "InProgress";
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Status = "Failed";
                ErrorCode = e.HResult.ToString();
                ErrorMessage = e.Message;
            }
            finally
            {
                switch (Status)
                {
                    case "Failed":
                        Result = ResultEnum.Failed;
                        break;
                    default:
                        Result = ResultEnum.Success;
                        break;
                }
            }
            
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationText"></param>
        /// 
        //*********************************************************************

        private void AzureRDFETaskStatus(string operationText)
        {
            ID = Utilities.GetXmlInnerText(operationText, "ID");
            Status = Utilities.GetXmlInnerText(operationText, "Status");
            HttpStatusCode = Utilities.GetXmlInnerText(operationText, "HttpStatusCode");
            ErrorCode = Utilities.GetXmlInnerText(operationText, "Code");
            ErrorMessage = Utilities.GetXmlInnerText(operationText, "Message");

            switch (Status)
            {
                case "Failed":
                    Result = ResultEnum.Failed;
                    break;
                default:
                    Result = ResultEnum.Success;
                    break;
            }
        }
    }

}
