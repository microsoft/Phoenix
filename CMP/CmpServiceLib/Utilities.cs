//*****************************************************************************
//*
//* File:
//* Author: Mark West (mark.west@microsoft.com)
//* Copyright: Microsoft 2011
//*
//*****************************************************************************

using System;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace CmpServiceLib
{
    /// <summary>
    /// 
    /// </summary>
    public class Utilities
    {
        /// <summary> </summary>
        public const int EXCEPTION_Utilities_WriteStringToFile = 1;
        /// <summary> </summary>
        public const int EXCEPTION_Utilities_FileToString = 2;

        //*********************************************************************
        ///
        /// <summary>
        /// Converts a string to a stream
        /// </summary>
        /// <param name="sString">The string to convert to a stream</param>
        /// <returns>The stream created with the provided string</returns>
        /// <exception cref="UtilityException">Thrown when an error occurs</exception>
        /// 
        //*********************************************************************

        public static Stream String2Stream(string sString)
        {
            var bArray = new byte[sString.Length];

            for (var iIndex = 0; iIndex < sString.Length; iIndex++)
                bArray[iIndex] = (byte)sString[iIndex];

            Stream sStream = new MemoryStream(bArray);

            return sStream;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Converts a stream to a string
        /// </summary>
        /// <param name="sStream">The input stream</param>
        /// <returns>The output string</returns>
        /// <exception cref="UtilityException">Thrown when an error occurs</exception>
        /// 
        //*********************************************************************

        public static string Stream2String(Stream sStream)
        {
            var sbSource = new StringBuilder((int)(sStream.Length), (int)(sStream.Length));

            sStream.Position = 0;

            for (var iIndex = 0; iIndex < sStream.Length; iIndex++)
                sbSource.Append((char)sStream.ReadByte());

            return sbSource.ToString();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Serializes all public members of a class to an XML string
        /// </summary>
        /// <param name="subjectType">The type of object to be serialized</param>
        /// <param name="subject">The object to be serialized</param>
        /// <returns>An XML string containing the serialized object</returns>
        /// <exception cref="UtilityException">Thrown when an error occurs</exception>
        /// 
        //*********************************************************************

        public static string Serialize(System.Type subjectType, Object subject)
        {
            if (null == subject)
                return null;

            var serializer =
                new XmlSerializer(subjectType);

            Stream strXmlJob = new MemoryStream(6000);

            TextWriter writer = new StreamWriter(strXmlJob);

            serializer.Serialize(writer, subject);

            var sRequest = Stream2String(strXmlJob);

            writer.Close();

            return sRequest;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// De-serializes am XML string into a class object
        /// </summary>
        /// <param name="subjectType">The type of object to be de-serialized</param>
        /// <param name="sSerializedObject">An XML string containing the serialized object</param>
        /// <returns>A class object de-serialized from the XML string</returns>
        /// <exception cref="UtilityException">Thrown when an error occurs</exception>
        /// 
        //*********************************************************************

        public static object DeSerialize(System.Type subjectType, string sSerializedObject)
        {
            try
            {
                if (null == sSerializedObject)
                    return null;

                var serializer = new XmlSerializer(subjectType);

                /* If the XML document has been altered with unknown 
                nodes or attributes, handle them with the 
                UnknownNode and UnknownAttribute events.*/

                /*serializer.UnknownNode+= new 
                    XmlNodeEventHandler(serializer_UnknownNode);
                serializer.UnknownAttribute+= new 
                    XmlAttributeEventHandler(serializer_UnknownAttribute);*/

                var os = Utilities.String2Stream(sSerializedObject);
                var subject = serializer.Deserialize(os);

                return subject;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Writes a string to a file
        /// </summary>
        /// <param name="sContents">The string to write to a file</param>
        /// <param name="sFileName">The name of the file</param>
        /// <returns>void</returns>
        /// <exception cref="UtilityException">Thrown when an error occurs</exception>
        /// 
        //*********************************************************************

        public static void WriteStringToFile(string sContents, string sFileName)
        {
            var iIndex = sFileName.LastIndexOf("\\");
            FileStream fsOut = null;
            StreamWriter sw = null;

            if (-1 == iIndex)
                iIndex = sFileName.LastIndexOf("/");

            if (-1 == iIndex || 0 == iIndex)
                throw new UtilityException("File name contains no path information", EXCEPTION_Utilities_WriteStringToFile);

            var sPath = sFileName.Substring(0, iIndex);

            if (null == sPath)
                throw new UtilityException("File name contains no path information", EXCEPTION_Utilities_WriteStringToFile);

            if (2 < sPath.Length)
            {
                try
                {
                    Directory.CreateDirectory(sPath);
                }
                catch (Exception e)
                {
                    throw new UtilityException("Could not create directory. " + e.Message, EXCEPTION_Utilities_WriteStringToFile, e);
                }
            }

            try
            {
                fsOut = new FileStream(sFileName, FileMode.Create,
                    FileAccess.Write, FileShare.None);
            }
            catch (Exception e)
            {
                throw new UtilityException("Error creating file stream. " + e.Message, EXCEPTION_Utilities_WriteStringToFile, e);
            }

            if (null == fsOut)
            {
                throw new UtilityException("Error creating file stream", EXCEPTION_Utilities_WriteStringToFile);
            }

            try
            {
                sw = new StreamWriter(fsOut);
                sw.Write(sContents);
            }
            catch (Exception e)
            {
                throw new UtilityException("Could not write to file. " + e.Message, EXCEPTION_Utilities_WriteStringToFile, e);
            }

            finally
            {
                if (null != sw)
                    sw.Close();

                if (null != fsOut)
                    fsOut.Close();
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Return the contents of a file as a string
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <returns>The contents of the file</returns>
        /// <exception cref="UtilityException">Thrown when an error occurs</exception>
        /// 
        //*********************************************************************

        public static string FileToString(string filename)
        {
            try
            {
                var fi = new FileInfo(filename);
                if (!fi.Exists)
                    return null;

                var s = fi.OpenText();
                var output = s.ReadToEnd();
                s.Close();
                return output;
            }
            catch (Exception e)
            {
                throw new UtilityException(e.Message, EXCEPTION_Utilities_FileToString, e);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Unwinds exception messages
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns>The unwound messages</returns>
        /// 
        //*********************************************************************

        public static string UnwindExceptionMessages(Exception ex)
        {
            var message = ex.Message;

            if (null != ex.InnerException)
            {
                ex = ex.InnerException;
                message += " - " + ex.Message;

                if (null != ex.InnerException)
                {
                    ex = ex.InnerException;
                    message += " - " + ex.Message;
                }
            }

            return message;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEncode"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        static public string StringToB64(string toEncode)
        {
            var toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

            return System.Convert.ToBase64String(toEncodeAsBytes);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encodedData"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        static public string B64ToString(string encodedData)
        {
            var encodedDataAsBytes = System.Convert.FromBase64String(encodedData);

            return System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        static public string GetXmlInnerText(string body, string tag)
        {
            string Out;

            try
            {
                var Index = body.IndexOf("<" + tag + ">");

                if (-1 == Index)
                    return null;

                Out = body.Substring(Index + tag.Length + 2);
                Index = Out.IndexOf("</" + tag + ">");
                Out = Out.Substring(0, Index);
            }
            catch (Exception)
            {
                return null;
            }

            return Out;
        }

        public static String purgeString(string text)
        {
            char[] block = null;
            var buffer = new StringBuilder();
            int i;
            var last = 0;
            var size = text.Length;
            for (i = 0; i < size; i++)
            {
                var c = text[i];
                if ((c < 32) && (c != '\t') && (c != '\n') && (c != '\r'))
                {
                    // remove character
                    if (block == null)
                    {
                        block = text.ToCharArray();
                    }
                    buffer.Append(block, last, i - last);
                    last = i + 1;
                }
            }
            if (last == 0)
            {
                return text;
            }
            if (last < size)
            {
                if (block == null)
                {
                    block = text.ToCharArray();
                }
                buffer.Append(block, last, i - last);
            }
            return buffer.ToString();
        }
        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static DateTime GetDbDateTime(DateTime? data)
        {
            try
            {
                return (DateTime)data;
            }
            catch (Exception)
            {
                return new DateTime(1900, 1, 1);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static int GetDbInt(int? data)
        {
            try
            {
                return (int)data;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static bool GetDbBool(bool? data)
        {
            try
            {
                return (bool)data;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vmReq"></param>
        /// <param name="exceptionType"></param>
        /// 
        //*********************************************************************

        public static void SetVmReqExceptionType(CmpServiceLib.Models.VmDeploymentRequest vmReq,
            CmpInterfaceModel.Constants.RequestExceptionTypeCodeEnum exceptionType)
        {
            try
            {
                if (null != vmReq)
                    vmReq.ExceptionTypeCode = exceptionType.ToString();
            }
            catch (Exception)
            { }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*********************************************************************

        public static bool IsArm
        {
            get
            {
                var azureApi =
                    Microsoft.Azure.CloudConfigurationManager.GetSetting("AzureApi") as string;

                return null != azureApi && azureApi.Equals("ARM");
            }
        }
        public static bool HasRdfe
        {
            get
            {
                var azureApi =
                    Microsoft.Azure.CloudConfigurationManager.GetSetting("AzureApi") as string;

                return null != azureApi && azureApi.ToLower().Contains("rdfe");
            }
        }

        public static bool PostProcessingEnabled
        {
            get
            {
                var postProcessingEnabled_t =
                    Microsoft.Azure.CloudConfigurationManager.GetSetting("PostProcessingEnabled");

                if (null == postProcessingEnabled_t)
                    return false;

                return postProcessingEnabled_t.Equals("true", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }

    //*************************************************************************
    ///
    /// <summary>
    /// Thrown by members of the Utilities class
    /// </summary>
    /// 
    //*************************************************************************
    [Serializable()]
    public class UtilityException : Exception, ISerializable
    {
        /// <summary>
        /// Thrown by members of the Utilities class
        /// </summary>
        public UtilityException()
        {
        }

        /// <summary>
        /// Thrown by members of the Utilities class
        /// </summary>
        /// <param name="message">Human readable text specific to thsi exception</param>
        /// <param name="hResult"></param>
        public UtilityException(string message, int hResult)
            : base(message)
        {
            base.Data.Add("HRESULT", hResult); base.Data.Add("TYPE", this.GetType().Name);
        }

        /// <summary>
        /// Thrown by members of the Utilities class
        /// </summary>
        /// <param name="message">Human readable text specific to thsi exception</param>
        /// <param name="hResult"></param>
        /// <param name="inner">The inner exception if available</param>
        public UtilityException(string message, int hResult, Exception inner)
            : base(message, inner)
        {
            base.Data.Add("HRESULT", hResult); base.Data.Add("TYPE", this.GetType().Name);
        }
    }
}


