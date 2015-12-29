using System;
using System.Collections.Generic;
using CmpInterfaceModel;
using Microsoft.WindowsAzure.Storage.DataMovement;
using Microsoft.WindowsAzure.Storage.DataMovement.BlobTransferCallbacks;


namespace AzureAdminClientLib
{
    public class StorageOps3
    {
        static long PAGEBLOBCHUNCKSIZE = 512;
        DateTime StartTime = DateTime.UtcNow;
        int ElapsedTimeMinutes = 0;
        double _SourceFileLength = 0;
        double _PercentTransferred = 0;
        bool _OverwriteDestination = false;

        //int _TransferThreadCount = Environment.ProcessorCount * 8;

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        ///  <returns></returns>
        ///  
        //*********************************************************************

        private bool OverwritePromptCallback(string sourcePath, string destinationPath)
        {
            return _OverwriteDestination;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// 
        //*********************************************************************

        void StartCallback(object obj)
        {
            return;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ex"></param>
        /// 
        //*********************************************************************

        void ExceptionCallback(object obj, Exception ex)
        {
            var resp = obj as AzureAdminClientLib.Response;

            if (null != ex)
            {
                resp.ExceptionMessage = Utilities.UnwindExceptionMessages(ex);
                resp.ResultStatus = Response.ResultStatusEnum.EXCEPTION;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="entryData"></param>
        /// 
        //*********************************************************************

        void StartFileCallback(object obj, EntryData entryData)
        {
            StartTime = DateTime.UtcNow;
            return;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="entryData"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// 
        //*********************************************************************

        void ProgressFileCallback(object obj, EntryData entryData, double speed, double percent)
        {
            if ((percent > _PercentTransferred + 5) | (_PercentTransferred == (double)0))
            {
                _PercentTransferred = percent;

                if(null != _PFCBD)
                    _PFCBD(obj, speed, _SourceFileLength, percent);
            }
        }

        public delegate void ProgressFileCallbackDelegate(object obj, double speed, double size, double percent);
        ProgressFileCallbackDelegate _PFCBD = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="entryData"></param>
        /// <param name="ex"></param>
        /// 
        //*********************************************************************

        void FinishFileCallback(object obj, EntryData entryData, Exception ex)
        {
            var resp = obj as AzureAdminClientLib.Response;

            if (null != ex)
            {
                resp.ExceptionMessage = ex.Message;
                resp.ResultStatus = Response.ResultStatusEnum.EXCEPTION;
            }
            else
            {
                if (resp.ResultStatus == Response.ResultStatusEnum.Unassigned)
                {
                    var ElapsedTime = DateTime.UtcNow.Subtract(StartTime);
                    ElapsedTimeMinutes = (int)ElapsedTime.TotalMinutes;
                    resp.ResultStatus = Response.ResultStatusEnum.OK;
                }
            }

            return;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// 
        //*********************************************************************

        private void SpeedChangeCallback(object sender, BlobTransferManagerEventArgs eventArgs)
        {
            if (null != _PFCBD)
                _SCD(sender, eventArgs.GlobalSpeed);
        }

        public delegate void SpeedChangeDelegate(object obj, double speed);
        SpeedChangeDelegate _SCD = null;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadCount"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        int GetTransferThreadCount(int threadCount)
        {
            if(1 > threadCount)
                threadCount = Environment.ProcessorCount * 8;

            return threadCount;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string GetAzCopyClientRequestID()
        {
            var name = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            return (name.Name + "/" + name.Version.ToString());
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        private void ConfigureServicePointManager(int threadCount)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = this.GetTransferThreadCount(threadCount);
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.UseNagleAlgorithm = true;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="progressCallback"></param>
        /// <param name="speedChangeCallback"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        //*** NOTE * Network

        public AzureAdminClientLib.Response UploadFileToCloud(BlobTransferInfo config,
            ProgressFileCallbackDelegate progressCallback, SpeedChangeDelegate speedChangeCallback)
        {
            var resp = new AzureAdminClientLib.Response();
            _PFCBD = progressCallback;
            _SCD = speedChangeCallback;
            _OverwriteDestination = config.OverwriteDestination;
            _SourceFileLength = config.LocalFile.Length;

            var pivot = config.LocalFile.FullName.LastIndexOf('\\');

            var sourceLocation = config.LocalFile.FullName.Substring(0, pivot);
            var theFilePatterns = new List<string>(1) 
                {config.LocalFile.FullName.Substring(pivot + 1)};

            var destinationLocation = config.Url.AbsoluteUri;
            var destinationStorageKey = config.Key;

            var rem = config.LocalFile.Length % PAGEBLOBCHUNCKSIZE;

            Microsoft.WindowsAzure.Storage.Blob.BlobType theUploadBlobType;

            if (0 == rem)
                theUploadBlobType = Microsoft.WindowsAzure.Storage.Blob.BlobType.PageBlob;
            else
                theUploadBlobType = Microsoft.WindowsAzure.Storage.Blob.BlobType.BlockBlob;

            var ShouldTransferSnapshots = false;
            var ShouldExcludeNewer = false;
            var ShouldExcludeOlder = false;
            var FileIncludedAttributes = new System.IO.FileAttributes();
            var FileExcludedAttributes = new System.IO.FileAttributes();
            var ShouldOnlyFilesWithArchiveBits = false;
            var ShouldRecursive = false;
            var ShouldFakeTransfer = false;
            object TagData = resp;

            if (0 == config.TransferThreadCount)
                config.TransferThreadCount = Environment.ProcessorCount*8;

            ConfigureServicePointManager(config.TransferThreadCount);

            var transferOptions = new BlobTransferOptions
            {
                Concurrency = GetTransferThreadCount(config.TransferThreadCount),
                OverwritePromptCallback = 
                    new BlobTransferOverwritePromptCallback(this.OverwritePromptCallback)
            };

            transferOptions.AppendToClientRequestId(GetAzCopyClientRequestID());
            BlobTransferRecursiveTransferOptions recursiveTransferOptions = null;

            using (var transferManager = new BlobTransferManager(transferOptions))
            {
                try
                {
                    recursiveTransferOptions = new BlobTransferRecursiveTransferOptions
                    {
                        DestinationKey = destinationStorageKey,
                        TransferSnapshots = ShouldTransferSnapshots,
                        ExcludeNewer = ShouldExcludeNewer,
                        ExcludeOlder = ShouldExcludeOlder,
                        IncludedAttributes = FileIncludedAttributes,
                        ExcludedAttributes = FileExcludedAttributes,
                        OnlyFilesWithArchiveBit = ShouldOnlyFilesWithArchiveBits,
                        UploadBlobType = theUploadBlobType,
                        FilePatterns = theFilePatterns,
                        Recursive = ShouldRecursive,
                        FakeTransfer = ShouldFakeTransfer,
                        MoveFile = config.DeleteSource,
                        FileTransferStatus = null
                    };

                    transferManager.GlobalCopySpeedUpdated +=
                        new EventHandler<BlobTransferManagerEventArgs>(this.SpeedChangeCallback);
                    transferManager.GlobalDownloadSpeedUpdated +=
                        new EventHandler<BlobTransferManagerEventArgs>(this.SpeedChangeCallback);
                    transferManager.GlobalUploadSpeedUpdated +=
                        new EventHandler<BlobTransferManagerEventArgs>(this.SpeedChangeCallback);
                }
                catch (Exception ex)
                {
                    resp.ResultStatus = Response.ResultStatusEnum.EXCEPTION;
                    resp.ExceptionMessage = "Exception in UploadFileToCloud() during setup : " + 
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex);
                }

                try
                {
                    transferManager.QueueRecursiveTransfer(sourceLocation, destinationLocation, 
                        recursiveTransferOptions, StartCallback, ExceptionCallback, StartFileCallback, 
                        ProgressFileCallback, FinishFileCallback, TagData);
                }
                catch (Exception ex)
                {
                    resp.ResultStatus = Response.ResultStatusEnum.EXCEPTION;
                    resp.ExceptionMessage = "Exception in UploadFileToCloud() during queue up : " +
                        CmpInterfaceModel.Utilities.UnwindExceptionMessages(ex);
                }

                transferManager.WaitForCompletion();
                return resp;
            }

        }
    }
}
