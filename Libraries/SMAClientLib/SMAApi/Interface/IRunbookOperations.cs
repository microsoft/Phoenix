using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMAApi.Entities;
using SMAApi.SMAWebService;

namespace SMAApi.Interface
{
    public interface IRunbookOperations
    {

        #region StartRunbook with Name and Parameters
        /// <summary>
        /// Start Runbook for a given RunbookName  and Parameters
        /// </summary>
        /// <param name="name">Runbook Name</param>
        /// <param name="parameters">list of Runbook parameters</param>
        /// <returns>returns the Job details which includes Id and Message</returns> 
        
         RunbookJob StartRunBook(string name, List<NameValuePair> parameters);
        #endregion

        #region StartRunbook with Id and and Parameters
        /// <summary>
        /// Start Runbook for a given RunbookId  and Parameters
        /// </summary>
        /// <param name="runBookId">Id of the Runbook</param>
        /// <param name="parameters">list of parameters of the Runbook</param>
         /// <returns>returns the Job details which includes Id and Message</returns>
        RunbookJob StartRunBook(Guid runBookId, List<NameValuePair> parameters);
        #endregion

        #region GetAllRunboos
         /// <summary>
         /// Get All the Runbooks from SMA Service
         /// </summary>
         /// <returns>Returns all the runbooks excluding system runbooks</returns>
         IQueryable<SMARunbook> GetAllRunbooks();
         #endregion

        #region GetJobStatus
         /// <summary>
         /// GetjobStatus based on the jobId
         /// </summary>
         /// <param name="jobId">jobId for which the status to be known</param>
         /// <returns>returns JobStatus details</returns>

         JobStatus GetJobStatus(Guid jobId);
         #endregion

        #region Get Job Output from JSON
         dynamic GetJSONJobOutput(Guid jobGuid);
         T GetJSONJobOutput<T>(Guid jobGuid);
        #endregion Get
    }
}
