using System;
using System.Collections.Generic;
using System.Linq;
using CmpCommon;
using Constants = CmpInterfaceModel.Constants;

namespace CmpServiceLib
{
    class AftsStatus
    {
        public int ID;
        public string StatusCode;
        public string ResultDescription;
        public string Source;
        public string Destination;
    }
    public class AftsDb
    {
        private readonly string _ConnectionString = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// 
        //*********************************************************************

        public AftsDb(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public Models.Request FetchRequest(int reqId)
        {
            try
            {
                using (var db = new Models.AzureFileTransferContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReq = (from rb in db.Requests
                                    where rb.RequestID == reqId
                                    select rb).First();

                    return foundReq;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in AftsDb.FetchRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<Models.Request> FetchRequestByTagId(int tagId)
        {
            var outList = new List<Models.Request>();

            try
            {
                using (var db = new Models.AzureFileTransferContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReqList = (from rb in db.Requests
                                        where rb.TagID == tagId
                                        select rb);

                    outList.AddRange(foundReqList);
                    return outList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in AftsDb.FetchRequestByTagID() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagId"></param>
        ///
        //*********************************************************************

        public void DeleteRequestByTagId(int tagId)
        {
            var outList = new List<Models.Request>();

            try
            {
                using (var db = new Models.AzureFileTransferContext())
                {
                    db.Database.Connection.ConnectionString = _ConnectionString;

                    var foundReqList = from rb in db.Requests
                                        where rb.TagID == tagId
                                        select rb;

                    foreach (var req in foundReqList)
                        db.Requests.Remove(req);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in AftsDb.DeleteRequestByTagId() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferRequest"></param>
        /// <param name="db"></param>
        /// 
        //*********************************************************************

        public void InsertTransferRequest(Models.Request transferRequest,
            Models.AzureFileTransferContext db)
        {
            try
            {
                db.Database.Connection.ConnectionString = _ConnectionString;
                db.Requests.Add(transferRequest);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in AftsDb.InsertTransferRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferRequest"></param>
        /// <param name="db"></param>
        /// 
        //*********************************************************************

        public void UpdateTransferRequest(Models.Request transferRequest, 
            Models.AzureFileTransferContext db)
        {
            try
            {
                db.Database.Connection.ConnectionString = _ConnectionString;

                var foundReq = (from rb in db.Requests
                                where rb.RequestID == transferRequest.RequestID
                                select rb).First();

                foundReq.ResultStatusCode = transferRequest.ResultStatusCode;
                //foundReq.AgentName = null;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in AftsDb.UpdateTransferRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transferRequest"></param>
        /// <param name="db"></param>
        /// 
        //*********************************************************************

        public void ResubmitTransferRequest(Models.Request transferRequest, 
            Models.AzureFileTransferContext db)
        {
            try
            {
                db.Database.Connection.ConnectionString = _ConnectionString;

                var foundReq = (from rb in db.Requests
                                where rb.RequestID == transferRequest.RequestID
                                select rb).First();

                foundReq.ResultStatusCode = "Submitted";
                foundReq.AgentName = null;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in AftsDb.ResubmitTransferRequest() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public int FetchMaxRequestId(Models.AzureFileTransferContext db)
        {
            try
            {
                db.Database.Connection.ConnectionString = _ConnectionString;

                var maxId = (from rb in db.Requests
                             select rb.RequestID).Max();

                return maxId;
            }
            catch (InvalidOperationException)
            {
               return 0;
            }
            catch (Exception ex)
            {
                return 0;

                //throw new Exception("Exception in AftsDb.FetchMaxRequestID() : " 
                //    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="tagId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public CmpInterfaceModel.Constants.StatusEnum FetchTransferStatus(int tagId, out string message)
        {
            var reqList = FetchRequestByTagId(tagId);
            var transferring = false;
            var submitted = false;
            var complete = false;
            message = "";

            foreach (var req in reqList)
            {
                message = req.ResultDescription;

                if (req.ResultStatusCode.Equals(Constants.StatusEnum.Exception.ToString(),
                    StringComparison.InvariantCultureIgnoreCase))
                    return Constants.StatusEnum.Exception;

                if (req.ResultStatusCode.Contains("FAILED"))
                    return Constants.StatusEnum.Exception;

                if (req.ResultStatusCode.Equals(Constants.StatusEnum.Transferring.ToString(),
                    StringComparison.InvariantCultureIgnoreCase))
                    transferring = true;

                else if (req.ResultStatusCode.Equals(Constants.StatusEnum.Submitted.ToString(),
                    StringComparison.InvariantCultureIgnoreCase))
                    submitted = true;

                else if (req.ResultStatusCode.Equals(Constants.StatusEnum.Complete.ToString(),
                    StringComparison.InvariantCultureIgnoreCase))
                    complete = true;
            }

            if (transferring)
                return Constants.StatusEnum.Transferring;

            if (submitted)
                if (complete)
                    return Constants.StatusEnum.Transferring;
                else
                    return Constants.StatusEnum.Submitted;

            message = "Transfer complete";
            return Constants.StatusEnum.Complete;
        }
    }
}
