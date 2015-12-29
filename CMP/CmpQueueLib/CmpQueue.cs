using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Azure;
using System.Threading;
using CmpCommon;
//using Microsoft.Azure;

namespace CmpQueueLib 
{
    public class CmpQueueInfo
    {
        public int Id;
        public string Name;
        public int Count;
        public bool Exists;
        public QueueDescription QDescription;
        public CmpQueue.QueueNameEnum QNameEnum;
    }

    public class CmpQueue : IDisposable
    {
        const int QueueTransientRetryTime = 2000;
        const int QueueTransientRetryLimit = 5;
        const int _LockDurationMinutes = 5;
        const int _MaxDeliveryCount = 600;
        static readonly TimeSpan _BatchReadTimeSpan = new TimeSpan(0, 0, 3);
        private string _storeName = null;
        private string _storeLocation = null;
        private string _thumbprint = null;
        private string _connectionString = null;

        public enum QueueNameEnum
        {
            Exception, QcVmRequest, QcVmRequestPassed,
            CreateService, UploadServiceCert, CreateVM, CheckVmCreation,
            ContactingVM, MovePagefile, WaitForReboot1, CreateDrives, InstallIpack,
            PostProcessStage1, PostProcessStage2, PostProcessStage2Complete, 
            StartingSequences, RunningSequences, Complete, Rejected
        };

        static List<QueueClient> _QueueList =
            new List<QueueClient>(Enum.GetValues(typeof(QueueNameEnum)).Length);
        static Object GetQueueLock = new Object();
        static string _queueConnectionString = null;

        public CmpQueue()
        { }

        public CmpQueue(string connectionString)
            : base()
        {
            _queueConnectionString = connectionString;
        }

        public CmpQueue(string storeName, string
            storeLocation, string thumbprint, string connectionString)
            : base()
        {
            _storeName = storeName;
            _storeLocation = storeLocation;
            _thumbprint = thumbprint;
            _connectionString = connectionString;
        }

        bool _disposed = false;

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        //*********************************************************************

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                //*** TODO * close these on the fly, they are a leak ***

                //foreach (var Q in _QueueList)
                //    Q.Close();

                _QueueList.Clear();
            }

            // Free any unmanaged objects here. 
            //
            _disposed = true;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string GetQueueConnectionString()
        {
            if (null == _queueConnectionString)
            {
                _queueConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
                var xk = new KryptoLib.X509Krypto(null);
                _queueConnectionString = xk.DecrpytKText(_queueConnectionString);
            }

            return _queueConnectionString;
        }

        private string GetQueueConnectionString(string storeName, string storeLocation, string thumbprint, string connectionString)
        {
            if (null == _queueConnectionString)
            {
                _queueConnectionString = connectionString;
                var xk = new KryptoLib.X509Krypto(storeName, storeLocation, thumbprint);
                _queueConnectionString = xk.DecrpytKText(_queueConnectionString);
            }

            return _queueConnectionString;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="queueNameEnum"></param>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        private QueueClient GetQueue(QueueNameEnum queueNameEnum, bool createIfNotExists)
        {
            return GetQueue(queueNameEnum.ToString(), createIfNotExists);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private QueueClient GetQueue(string queueName, bool createIfNotExists)
        {
            var QueueName = queueName.ToString();
            try
            {
                lock (GetQueueLock)
                {
                    foreach (var qc in _QueueList)
                        if (qc.Path.Equals(queueName))
                            return qc;

                    if (null == _thumbprint)
                        GetQueueConnectionString();
                    else
                        GetQueueConnectionString(_storeName, _storeLocation, _thumbprint, _connectionString);

                    var namespaceManager = NamespaceManager.CreateFromConnectionString(_queueConnectionString);

                    if(!queueName.Contains("/$DeadLetterQueue"))
                        if (!namespaceManager.QueueExists(QueueName))
                        {
                            if (!createIfNotExists)
                                return null;

                            var qd = new QueueDescription(QueueName)
                            {
                                LockDuration = new TimeSpan(0, _LockDurationMinutes, 0),
                                MaxDeliveryCount = _MaxDeliveryCount,
                                DefaultMessageTimeToLive = new TimeSpan(7,0,0,0),
                                EnableDeadLetteringOnMessageExpiration = true
                            };

                            namespaceManager.CreateQueue(qd);
                        }

                    var qCout = QueueClient.CreateFromConnectionString(_queueConnectionString, QueueName, ReceiveMode.PeekLock);
                    _QueueList.Add(qCout);
                    return (qCout);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpQueue.GetQueue() : " + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="messageId"></param>
        /// <param name="payload"></param>
        /// 
        //*********************************************************************

        public void EnQueue(QueueNameEnum queueName, string messageId, object payload)
        {
            try
            {
                var qc = GetQueue(queueName,true);
                EnQueue(qc, messageId, payload, queueName);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpQueue.EnQueue() : " + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="queueName"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public BrokeredMessage ReadQueue(QueueNameEnum queueName, int timeoutSeconds)
        {
            try
            {
                var qc = GetQueue(queueName,true);
                return ReadQueue(qc, timeoutSeconds);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpQueue.ReadQueue() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="queueName"></param>
        ///  <param name="count"></param>
        /// <param name="abandon"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        /*public IEnumerable<BrokeredMessage> ReadQueueBatch(QueueNameEnum queueName, int count, bool abandon)
        {
            try
            {
                QueueClient qc = GetQueue(queueName, true);
                return ReadQueueBatch(qc, count, abandon);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpQueue.ReadQueueBatch() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }*/

        public IEnumerable<BrokeredMessage> ReadQueueBatch(QueueNameEnum queueName, 
            int count, bool abandon)
        {
            return ReadQueueBatch(queueName.ToString(), count, abandon);
        }

        public IEnumerable<BrokeredMessage> ReadQueueBatch(string queueName,
            int count, bool abandon)
        {
            try
            {
                var qc = GetQueue(queueName, true);
                return ReadQueueBatch(qc, count, abandon);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpQueue.ReadQueueBatch() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        public IEnumerable<BrokeredMessage> ReadAllBatch(QueueNameEnum queueNameEnum, 
            int count, bool getDeadLetter, bool abandon)
        {
            try
            {
                QueueClient qc = null;
                var queueName = queueNameEnum.ToString();

                if(getDeadLetter)
                    queueName = QueueClient.FormatDeadLetterPath(queueName);

                qc = GetQueue(queueName, true);

                return ReadAllBatch(qc, count, abandon);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpQueue.ReadQueueBatch() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  Be sure to call CompleteMessage on the returned BrokeredMessage 
        ///  when you are done processing it.
        ///  </summary>
        ///  <typeparam name="T"></typeparam>
        ///  <param name="queueName"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="messageId"></param>
        ///  <param name="payload"></param>
        ///  <returns></returns>
        ///  
        //*********************************************************************

        public BrokeredMessage ReadQueue<T>(QueueNameEnum queueName,
            int timeoutSeconds, ref string messageId, ref T payload)
        {
            try
            {
                var qc = GetQueue(queueName, true);
                var bm = ReadQueue(qc, timeoutSeconds);

                if (null != bm)
                {
                    messageId = bm.MessageId;
                    payload = bm.GetBody<T>();
                }

                return bm;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpQueue.ReadQueue() : " + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// 
        //*********************************************************************

        public void CompleteMessage(BrokeredMessage message)
        {
            try
            {
                var queName = message.Properties["QueueName"] as string;
                object deadLetterReason;

                if (message.Properties.TryGetValue("DeadLetterReason", out deadLetterReason))
                    queName = QueueClient.FormatDeadLetterPath(queName);
                else if (0 < message.LockedUntilUtc.CompareTo(DateTime.UtcNow))
                {
                    try
                    {
                        message.Complete();
                        return;
                    }
                    catch(Exception ex)
                    {
                        if(!ex.Message.Contains("already been removed"))
                            throw;
                    }
                }

                //var bmList = ReadQueueBatch((QueueNameEnum)Enum.Parse(typeof(QueueNameEnum), queName), 100, false);
                var bmList = ReadQueueBatch(queName, 100, false);

                foreach (var bm in bmList)
                {
                    if (bm.SequenceNumber == message.SequenceNumber)
                        bm.Complete();
                    else
                        bm.Abandon();

                    bm.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in CmpQueue.CompleteMessage() : " + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        /*public void CompleteDeadMessage(BrokeredMessage message)
        {
            object deadLetterReason;
            var queName = message.Properties["QueueName"] as string;
            
            if(message.Properties.TryGetValue("DeadLetterReason", out deadLetterReason))
                queName = QueueClient.FormatDeadLetterPath(queName);

            var bmList = ReadQueueBatch(queName, 100, false);

            foreach (BrokeredMessage bm in bmList)
            {
                if (bm.SequenceNumber == message.SequenceNumber)
                    bm.Complete();
                else
                    bm.Abandon();

                bm.Dispose();
            }
        }*/

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// 
        //*********************************************************************

        public void AbamdonMessage(BrokeredMessage message)
        {
            message.Abandon();
        }

        public void DeferMessage(BrokeredMessage message)
        {
            message.Defer();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// 
        //*********************************************************************

        public void Close(QueueNameEnum queueName)
        {
            var qc = GetQueue(queueName, true);
            qc.Close();
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="queue"></param>
        ///  <param name="messageId"></param>
        ///  <param name="payload"></param>
        /// <param name="queueName"></param>
        ///  
        //*********************************************************************
        private void EnQueue(QueueClient queue, string messageId,
            object payload, QueueNameEnum queueName)
        {
            var message = new BrokeredMessage(payload) { MessageId = messageId };

            while (true)
            {
                try
                {
                    message.Properties.Add("QueueName", queueName.ToString());
                    queue.Send(message);
                    break;
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(QueueTransientRetryTime);
                    }
                }
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="queue"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        private BrokeredMessage ReadQueue(QueueClient queue, int timeoutSeconds)
        {
            while (true)
            {
                try
                {
                    //receive messages from Queue
                    using (var message = queue.Receive(TimeSpan.FromSeconds(timeoutSeconds)))
                    {
                        if (message != null)
                            return message;

                        //no more messages in the queue
                        return null;
                    }
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(QueueTransientRetryTime);
                    }
                }
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="queue"></param>
        ///  <param name="count"></param>
        /// <param name="abandon"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        private IEnumerable<BrokeredMessage> ReadQueueBatch(QueueClient queue, 
            int count, bool abandon)
        {
            IEnumerable<BrokeredMessage> messageList = null;

            while (true)
            {
                try
                {
                    //receive messages from Queue
                    //MessageList = queue.PeekBatch(count);
                    //MessageList = queue.ReceiveBatch(count);

                    //**********************************

                    messageList = queue.ReceiveBatch(count, _BatchReadTimeSpan);

                    if (!messageList.Any())
                        return messageList;

                    while (count > messageList.Count())
                    {
                        var temp = queue.ReceiveBatch(count, _BatchReadTimeSpan);

                        if (null == temp)
                            break;

                        if (!temp.Any())
                            break;

                        messageList = messageList.Concat(temp);
                    }

                    //**********************************

                    //var _BatchReadTimeSpan2 = new TimeSpan(0, 1, 0);
                    //var messageList = queue.ReceiveBatch(count, _BatchReadTimeSpan2);

                    //**********************************

                    return messageList;
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(QueueTransientRetryTime);
                    }
                }
                finally
                {
                    if (abandon)
                        if (null != messageList)
                            foreach (var message in messageList)
                                message.Abandon();
                }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="count"></param>
        /// <param name="abandon"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private IEnumerable<BrokeredMessage> ReadAllBatch(QueueClient queue, 
            int count, bool abandon)
        {
            IEnumerable<BrokeredMessage> messageList = null;

            while (true)
            {
                try
                {
                    //receive messages from Queue
                    //MessageList = queue.PeekBatch(count);
                    //MessageList = queue.ReceiveBatch(count);

                    //**********************************

                    messageList = queue.ReceiveBatch(count, _BatchReadTimeSpan);

                    //var sessionList = queue.GetMessageSessions();

                    if (!messageList.Any())
                        return messageList;

                    while (count > messageList.Count())
                    {
                        var temp = queue.ReceiveBatch(count, _BatchReadTimeSpan);

                        if (null == temp)
                            break;

                        if (!temp.Any())
                            break;

                        messageList = messageList.Concat(temp);
                    }

                    

                    //**********************************

                    //var _BatchReadTimeSpan2 = new TimeSpan(0, 1, 0);
                    //var messageList = queue.ReceiveBatch(count, _BatchReadTimeSpan2);

                    //**********************************

                    return messageList;
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(QueueTransientRetryTime);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    if (abandon)
                        if (null != messageList)
                            foreach (var message in messageList)
                                message.Abandon();
                }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public IEnumerable<BrokeredMessage> ReadMessagesbyQueueName(QueueNameEnum queueName)
        {
            var qc = GetQueue(queueName, true);
            var messages = new List<BrokeredMessage>();
            try
            {
                IEnumerable<BrokeredMessage> recdMsg;
                while ((recdMsg = qc.ReceiveBatch(200, _BatchReadTimeSpan)) != null)
                {
                    messages.AddRange(recdMsg);
                    foreach (var msg in recdMsg)
                    {
                        msg.Abandon();
                    }

                    return messages;
                }
            }
            catch (MessagingException e)
            {
                if (!e.IsTransient)
                {
                    throw;
                }
                Thread.Sleep(QueueTransientRetryTime);
            }

            return messages;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<CmpQueueInfo> GetQueueList()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_queueConnectionString);
            var queues = namespaceManager.GetQueues();

            var queueInfoList = new List<CmpQueueInfo>();

            foreach (var q in queues)
            {
                QueueNameEnum qne;
                Enum.TryParse(q.Path,true, out qne);

                queueInfoList.Add(new CmpQueueInfo()
                {
                    QDescription = q, 
                    QNameEnum = qne, 
                    Count = (int)q.MessageCount,Exists = true,
                    Id = 1,Name=q.Path
                });
            }

            return queueInfoList;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageList"></param>
        /// 
        //*********************************************************************

        public static void DisposeMessages(IEnumerable<BrokeredMessage> messageList)
        {
            if (null == messageList)
                return;

            try
            {
                foreach (var message in messageList)
                    message.Dispose();
            }
            catch (Exception)
            { }

            try
            {
                //messageList.Clear();
            }
            catch (Exception)
            { }
        }
    }
}
