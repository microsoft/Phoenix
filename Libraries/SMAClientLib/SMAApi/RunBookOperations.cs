using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Services.Client;
using System.Net;
using System.Web.Script.Serialization;
using SMAApi;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Specialized;
using SMAApi.SMAWebService;
using SMAApi.Entities;
using System.Runtime.Serialization.Json;
using System.Configuration;
using SMAApi.Interface;
using Newtonsoft.Json;


namespace SMAApi
{
    public class RunBookOperations : IRunbookOperations
    {
        #region Variables
        string serviceRoot = string.Empty;
        OrchestratorApi api;
        #endregion

        #region Constants for runbook URI


        const string HttpPost = "POST";
        const string JobParameterName = "parameters";
        const string StartRunbookActionName = "Start";

        #endregion Constants for runbook URI

        #region Constructors
        /// <summary>
        ///  Constructor
        /// </summary>

        public RunBookOperations()
        {
            if (String.IsNullOrEmpty(this.serviceRoot))
            {
                this.serviceRoot = ConfigurationManager.AppSettings.Get("SMAService");
                if (string.IsNullOrEmpty(this.serviceRoot))
                    throw new Exception("Invalid Service Uri");
            }

            this.api = new OrchestratorApi(new Uri(serviceRoot));
            ((DataServiceContext)this.api).Credentials = CredentialCache.DefaultCredentials;

        }

        public RunBookOperations(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new Exception("Invalid Service Uri");

            this.serviceRoot = uri;

            this.api = new OrchestratorApi(new Uri(this.serviceRoot));
            ((DataServiceContext)this.api).Credentials = CredentialCache.DefaultCredentials;
        }

        #endregion

        #region GetAllRunboos
        /// <summary>
        /// Get All the Runbooks from SMA Service
        /// </summary>
        /// <returns></returns>
        public IQueryable<SMARunbook> GetAllRunbooks()
        {
            //try
            //{

            IgnoreCertificate();

            return api.Runbooks.Where(r => r.Tags != "SystemRunbook").Select(r => new
            SMARunbook
            {
                Name = r.RunbookName,
                Id = r.RunbookID,
                Description = r.Description,
                Tags = r.Tags,
                Parameters = GetRunbookParameters(r.PublishedRunbookVersionID)

            }).AsQueryable();
            //  }
            //catch (DataServiceQueryException ex)
            //{
            //    throw new ApplicationException("Error getting runbooks.", ex);
            //}

        }
        #endregion

        #region GetJobStatus
        /// <summary>
        /// GetjobStatus based on the jobId
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public JobStatus GetJobStatus(Guid jobId)
        {
            var job = api.Jobs.Where(j => j.JobID == jobId).First();
            var jobStream = api.GetReadStream(job);

            using (var reader = new StreamReader(jobStream.Stream))
            {
                var jobStreamText = reader.ReadToEnd();

                return new JobStatus
                {
                    Status = job.JobStatus,
                    Exception = job.JobException,
                    Output = jobStreamText
                };
            }
        }

        #endregion

        #region StartRunbook with Name and Parameters
        /// <summary>
        /// Start Runbook for a given RunbookName  and Parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>

        public RunbookJob StartRunBook(string runbookName, List<NameValuePair> parameters)
        {

            try
            {
                if (string.IsNullOrEmpty(runbookName))
                {
                    throw new ArgumentNullException("'runbookName' parameter cannot be null or empty.");
                }

                var runbooks = api.Runbooks.Where(r => r.RunbookName == runbookName);

                if (0 == runbooks.Count())
                    throw new Exception(String.Format("could not find the Runbook: '{0}'", runbookName));

                var runbook = runbooks.First();

                return StartRunBook(runbook, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in StartRunBook() : " + UnwindExceptionMessages(ex));
            }
        }

        #endregion

        #region StartRunbook with Id and and Parameters
        /// <summary>
        /// Start Runbook for a given RunbookId  and Parameters
        /// </summary>
        /// <param name="runBookId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public RunbookJob StartRunBook(Guid runBookId, List<NameValuePair> parameters)
        {

            #region  Checking if runbook exists
            var runbook = api.Runbooks.Where(r => r.RunbookID == runBookId).First();

            if (runbook == null)
            {
                throw new Exception(String.Format("could not find the RunbookId: {0}", runBookId));
            }

            #endregion

            return StartRunBook(runbook, parameters);

        }

        #endregion

        #region Get Job Output from JSON
        public dynamic GetJSONJobOutput(Guid jobGuid)
        {
            return GetJSONJobOutput<dynamic>(jobGuid, JsonConvert.DeserializeObject);
        }

        public T GetJSONJobOutput<T>(Guid jobGuid)
        {
            return GetJSONJobOutput<T>(jobGuid, JsonConvert.DeserializeObject<T>);
        }

        private T GetJSONJobOutput<T>(Guid jobGuid, Func<string, T> deserializer)
        {
            var OutputObject = default(T);

            var job = api.Jobs.Where(j => j.JobID == jobGuid).First();  // Get job details based on Job Id

            if (job == null)
            {
                return default(T);
            }

            try
            {
                var jobStream = api.GetReadStream(job);
                var reader = new StreamReader(jobStream.Stream); // Read result from stream reader
                string line;
                var list = new List<string>();

                string temp = null;
                while ((line = reader.ReadLine()) != null)
                {  // Create an array of strings without any null or blank spaces in the array.
                    // Done this way because the content-type returned is Octet Stream and I could not find a way to return the content type as JSON from the Orchestrator API
                    if ((line != "") && (line != " "))
                    {
                        temp += line;
                    }
                    else if (temp != null)
                    {
                        list.Add(temp);
                        temp = null;
                    }
                    else
                    {
                        continue;
                    }
                }

                if ((list[list.Count - 2] != null) && IsJson(list[list.Count - 2])) // return the second to last information which is JSON string based on Runbook standards followed in SDO
                {
                    OutputObject = deserializer(list[list.Count - 2]);
                }
                else
                {
                    // Error
                    //   Console.WriteLine("Cannot continue as JSON cannot be parsed");
                    return default(T);
                }
            }
            catch (Exception ex)
            {

                // TODO Error
                return default(T);
            }

            return OutputObject;
        }
        #endregion

        #region Hepler methods

        #region StartRunbook
        /// <summary>
        /// Start Runbook with the the given Runbook and list of NameValuepair parameters
        /// </summary>
        /// <param name="runbook"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private RunbookJob StartRunBook(Runbook runbook, List<NameValuePair> parameters)
        {
            //try
            //{
            IgnoreCertificate();

            #region Setting up parameters for runbook



            OperationParameter operationParameters = new BodyOperationParameter(JobParameterName, parameters);


            #endregion Setting up parameters for runbook

            ValidateParameters(runbook, parameters);

            #region Create runbook job
            // Format the uri
            var uri = string.Concat(api.Runbooks, string.Format("(guid'{0}')/{1}", runbook.RunbookID, StartRunbookActionName));
            var uriSMA = new Uri(uri, UriKind.Absolute);



            var jobIdValue = api.Execute<Guid>(uriSMA, HttpPost, true, operationParameters) as QueryOperationResponse<Guid>;


            var jobId = jobIdValue.Single();

            var job = api.Jobs.Where(j => j.JobID == jobId).First();
            if (job == null)
            {

                return new RunbookJob { OutputMessage = "Job not started!" };
            }
            else
            {

                return new RunbookJob { Id = jobId, OutputMessage = String.Format("Job Started. JobID: {0}, JobStatus: {1}", jobId, job.JobStatus) };
            }
            #endregion Create runbook job


            //  }

            //catch (DataServiceQueryException ex)
            //{
            //    throw new ApplicationException("Error starting runbook.", ex);

            //}
        }

        #endregion StartRunbook

        #region GetRunbookParameters

        /// <summary>
        /// GetRunbookParameters based on the runbookId
        /// </summary>
        /// <param name="runbookId"></param>
        /// <returns></returns>
        private IQueryable<SMARunbookParameter> GetRunbookParameters(Guid? runbookversionId)
        {
            //    var runbook = api.Runbooks.Where(r => r.RunbookID == runbookId).First();
            return api.RunbookParameters.Where(runbookParam => runbookParam.RunbookVersionID == runbookversionId).Select(
                     r => new SMARunbookParameter
                     {
                         Name = r.Name,
                         RunbookVersionID = r.RunbookVersionID,
                         Type = r.Type
                     }).AsQueryable();

        }

        #endregion

        #region ignore cert

        private void IgnoreCertificate()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(
                delegate
                {
                    return true;
                });

        }

        #endregion ignore cert

        #region ValidateParameters
        /// <summary>
        /// validating parameters of Runbook
        /// </summary>
        /// <param name="runbook"></param>
        /// <param name="parameters"></param>
        private void ValidateParameters(Runbook runbook, List<NameValuePair> parameters)
        {
            var pamts = GetRunbookParameters(runbook.PublishedRunbookVersionID) as IEnumerable<SMARunbookParameter>;


            var lstErrors = new List<ParameterError>();


            foreach (var val in parameters)
            {
                var param = pamts.Where(p => p.Name == val.Name).FirstOrDefault();
                if (param == null)
                {

                    lstErrors.Add(new ParameterError
                    {
                        Parameter = new SMARunbookParameter { Name = val.Name, Value = val.Value },
                        ErrorMessage = "Parameter does not exist."
                    });

                }
                else
                {

                    var type = Type.GetType(param.Type);
                    var result = ConvertType(val.Value, type);
                    if (param.Type == "System.Management.Automation.SwitchParameter")
                    {
                        result = val.Value;
                    }

                    if (result == null)
                    {

                        lstErrors.Add(new ParameterError
                        {
                            Parameter = new SMARunbookParameter { Name = val.Name, Value = val.Value },
                            ErrorMessage = String.Format("This parameter expects value of type {0}.The passed value cannot be converted to {0}."
                            , param.Type)
                        });

                    }
                }
            }

            if (lstErrors.Count > 0)
            {

                throw new ParameterException(lstErrors);
            }




        }

        #endregion

        #region ConvertType
        /// <summary>
        /// Converting the given object type to given type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private object ConvertType(object value, Type type)
        {
            object result;
            try
            {
                result = Convert.ChangeType(value, type);
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
        #endregion ConvertType

        #region check to see if returned output is JSON
        public static bool IsJson(string input)  // An additional validation to check if the stream returned is actually JSON 
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }

        #endregion

        public static string UnwindExceptionMessages( Exception ex )
        {
            var e = ex;
            var s = new StringBuilder();

            while (e != null)
            {
                //s.AppendLine("Exception type: " + e.GetType().FullName);
                s.AppendLine(e.Message + " : ");
                /*s.AppendLine("Stacktrace:");
                s.AppendLine(e.StackTrace);
                s.AppendLine();*/
                e = e.InnerException;
            }
            return s.ToString();
        }


        #endregion Helper methods

    }
}
