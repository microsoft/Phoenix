using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMAApi;
using SMAApi.Interface;
using SMAApi.Entities;
using System.Data.Services.Client;
using SMAApi.SMAWebService;

namespace SMAServiceClient
{
    class Program
    {
       
       
        static void Main(string[] args)
        {
            try
            {

                IRunbookOperations rnbookops = new RunBookOperations();
                Guid? runbookId = new Guid("d00fbdce-a3e4-4125-b135-ac7579d3dbb5");
                string rnbookName = "execute-scriptblock";
                List<NameValuePair> parameters = new List<NameValuePair>();


               //  The service operation is not seen as IQueryable but just as IEnumerable so we need to cast to Ienumerable
                IEnumerable<SMARunbook> runbooks = rnbookops.GetAllRunbooks() as IEnumerable<SMARunbook>;

             //  var runBook=runbooks.Where(r=>r.l)

                var rnbook = runbooks.Where(r => r.Id == runbookId).FirstOrDefault();

                parameters.Add(new NameValuePair { Name = "scriptblock", Value = "write-output get-process" });
                parameters.Add(new NameValuePair { Name = "computername", Value = "localhost" });

                var resu = rnbookops.StartRunBook(rnbookName, parameters);

         

                Console.ReadLine();
            }

         
            catch (Exception ex)
            {
               
            }

         
        }
    }
}
