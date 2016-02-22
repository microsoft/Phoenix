using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace KryptoLib
{
    public class X509Krypto : IDisposable
    {
        X509Certificate2 _Cert = null;

        public X509Certificate2 Certificate
        {
            set { _Cert = value; }
            get { return _Cert;  }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// Instantiate an instance bound to the certificate at the 'KryptoCert' key value
        /// specified in the <appSettings> section of the application.config file.
        /// Value format: '[store location],[store name],[cert thumbprint]'
        /// Example: '<add key="KryptoCert" value="LocalMachine, My, 339C80283637D69D9D9F0F5FE00970D93CB0A148" />'
        /// </summary>
        /// 
        //*************************************************************************

        public X509Krypto()
        {
            LoadCertFromConfig("KryptoCert");
        }

        //*************************************************************************
        ///
        /// <summary>
        /// Instantiate an instance bound to the certificate at the named key value
        /// specified in the <appSettings> section of the application.config file.
        /// Value format: '[store location],[store name],[cert thumbprint]'
        /// Example: '<add key="KryptoCert" value="LocalMachine, My, 339C80283637D69D9D9F0F5FE00970D93CB0A148" />'
        /// </summary>
        /// <param name="appSettingName"></param>
        /// 
        //*************************************************************************

        public X509Krypto(string appSettingName)
        {
            LoadCertFromConfig(appSettingName);
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSettingName"></param>
        /// 
        //*************************************************************************

        private void LoadCertFromConfig(string appSettingName)
        {
            try
            {
                if (null == appSettingName)
                    appSettingName = "KryptoCert";

                // key="KryptoCert" value="LocalMachine, My, 339C80283637D69D9D9F0F5FE00970D93CB0A148"
                var KryptoCert = Microsoft.Azure.CloudConfigurationManager.GetSetting(appSettingName) as string;

                if (null == KryptoCert)
                    throw new Exception("KryptoCert key '" + appSettingName +
                        "' not found in <appSettings> section of the application.config file.");

                var StoreInfo = KryptoCert.Split(new char[] { ',' });

                if (3 > StoreInfo.Count())
                    throw new Exception("KryptoCert key '" + appSettingName +
                        "' value does not contain three CSVs. Value must be '[store location],[store name],[cert thumbprint]'");

                var SN = (StoreName)Enum.Parse(typeof(StoreName), StoreInfo[1], true);
                var SL = (StoreLocation)Enum.Parse(typeof(StoreLocation), StoreInfo[0], true);

                _Cert = FetchCertFromStore(SN, SL, StoreInfo[2]);
            }
            catch (Exception ex)
            {
                throw new Exception("exception in X509Krypto.X509Krypto() " + UnwindExceptionMessages(ex));
            }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="thumbprint"></param>
        /// 
        //*************************************************************************

        public X509Krypto(StoreName storeName, StoreLocation
            storeLocation, string thumbprint)
        {
            _Cert = FetchCertFromStore(storeName, storeLocation, thumbprint);
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="thumbprint"></param>
        /// 
        //*************************************************************************

        public X509Krypto(string storeName, string
            storeLocation, string thumbprint)
        {
            var SN = (StoreName)Enum.Parse(typeof(StoreName), storeName, true);
            var SL = (StoreLocation)Enum.Parse(typeof(StoreLocation), storeLocation, true);

            _Cert = FetchCertFromStore(SN, SL, thumbprint);

            if(null == _Cert)
                throw new Exception("Certificate not found in store");
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// 
        //*************************************************************************

        public void Dispose()
        {
            if (null != _Cert)
            {
                _Cert.Reset();
                _Cert = null;
            }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainData"></param>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public byte[] Encrypt(byte[] plainData)
        {
            return Encrypt(plainData, true, _Cert);
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public byte[] Decrypt(byte[] encryptedData)
        {
            return Decrypt(encryptedData, true, _Cert);
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public string Encrypt(string plainText)
        {
            var PlainData = System.Text.UTF8Encoding.UTF8.GetBytes(plainText);
            var CipherData = Encrypt(PlainData, true, _Cert);
            return Convert.ToBase64String(CipherData);
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherData"></param>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public string Decrypt(string cipherText)
        {
            var CipherData = Convert.FromBase64String(cipherText);
            var PlainData = Decrypt(CipherData, true, _Cert);
            return System.Text.UTF8Encoding.UTF8.GetString(PlainData);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Decrypt a string which contains 'KText'. KText is text which appears 
        /// between two identifiers: '[KText]' and '[/KText]'. For example,
        /// 'The secret is -> [KText]ZCkX/llcHWfTwDl/aksGhelekw==[/KText]'
        /// would resolve to a string like this 'The secret is -> Azure'. Of
        /// course the ciphertext must have been encrypted with the Ku of the
        /// certificate used to perform the decrypt. The Krypto class contains
        /// a number of methods which perform this encryption.
        /// </summary>
        /// <param name="kText"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string DecrpytKText(string kText)
        {
            if (null == kText)
                return null;

            try
            {
                while (true)
                {
                    var CipherText = GetInnerKText(kText, "KText");

                    if (null == CipherText)
                        return kText;

                    var ClearText = Decrypt(CipherText);

                    kText = ReplaceKText(kText, "KText", ClearText);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Exception in DecrpytKText() : " + UnwindExceptionMessages(ex));
            }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="thumbprint"></param>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public X509Certificate2 FetchCertFromStore(StoreName storeName, StoreLocation
            storeLocation, string thumbprint)
        {
            try
            {
                return X509Krypto.FetchCertFromStoreImpl(storeName, storeLocation, thumbprint);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchCertFromStore() : " + 
                    UnwindExceptionMessages(ex));
            }
        }

        public static X509Certificate2 FetchCertFromStoreImpl(StoreName storeName, StoreLocation storeLocation, string thumbprint)
        {
            // The following code gets the cert from the keystore
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            var certCollection =
                     store.Certificates.Find(X509FindType.FindByThumbprint,
                     thumbprint, false);

            var enumerator = certCollection.GetEnumerator();
            X509Certificate2 cert = null;

            while (enumerator.MoveNext())
            {
                cert = enumerator.Current;
            }

            return cert;
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainData"></param>
        /// <param name="fOAEP"></param>
        /// <param name="certificate"></param>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public byte[] Encrypt(byte[] plainData, bool fOAEP,
                 X509Certificate2 certificate)
        {
            try
            {
                return X509Krypto.EncryptImpl(plainData, fOAEP, certificate);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Encrypt() : " +
                    UnwindExceptionMessages(ex));
            }
        }

        public static byte[] EncryptImpl(byte[] plainData, bool fOAEP, X509Certificate2 certificate)
        {
            if (plainData == null)
            {
                throw new ArgumentNullException("plainData");
            }

            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }

            using (var provider = new RSACryptoServiceProvider())
            {
                provider.FromXmlString(X509Krypto.GetPublicKey(certificate));
                return provider.Encrypt(plainData, fOAEP);
            }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <param name="fOAEP"></param>
        /// <param name="certificate"></param>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public byte[] Decrypt(byte[] encryptedData, bool fOAEP,
                 X509Certificate2 certificate)
        {
            try
            {
                if (encryptedData == null)
                {
                    throw new ArgumentNullException("encryptedData");
                }

                if (certificate == null)
                {
                    throw new ArgumentNullException("certificate");
                }

                using (var provider = (RSACryptoServiceProvider)
                    certificate.PrivateKey)
                {
                    // We use the private key to decrypt.
                    return provider.Decrypt(encryptedData, fOAEP);
                }
            }
            catch (Exception ex)
            {
                //*** "keyset does not exist" remediation -> http://stackoverflow.com/questions/602345/cryptographicexception-keyset-does-not-exist-but-only-through-wcf

                throw new Exception("Exception in Decrypt() : " +
                    UnwindExceptionMessages(ex));
            }
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public static string GetPublicKey(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }

            return certificate.PublicKey.Key.ToXmlString(false);
        }

        //*************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        /// 
        //*************************************************************************

        public string GetXmlKeyPair(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }

            if (!certificate.HasPrivateKey)
            {
                throw new ArgumentException("certificate does not have a PK");
            }
            else
            {
                return certificate.PrivateKey.ToXmlString(true);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="tag"></param>
        /// <param name="replacementValue"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private string ReplaceKText(string body, string tag, string replacementValue)
        {
            string Out;

            try
            {
                var Index = body.IndexOf("[" + tag + "]");

                if (-1 == Index)
                    return body;

                Out = body.Substring(0, Index);
                Out += replacementValue;
                Index = body.IndexOf("[/" + tag + "]") + tag.Length + 3;
                Out += body.Substring(Index);
            }
            catch (Exception)
            {
                return null;
            }

            return Out;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <param name="passwordConfigName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string GetKTextConnectionString(string connectionStringName, string passwordConfigName)
        {
            try
            {
                var dbConnectionString =
                    GetConnectionString(connectionStringName);
                var contextPassword =
                    Microsoft.Azure.CloudConfigurationManager.GetSetting(passwordConfigName) as string;

                if (0 > dbConnectionString.IndexOf("Password=", StringComparison.InvariantCultureIgnoreCase))
                    return dbConnectionString;

                if (string.IsNullOrEmpty(dbConnectionString))
                    throw new Exception("Value for '" + connectionStringName + "' not found in app settings");

                if (string.IsNullOrEmpty(contextPassword))
                    return dbConnectionString;

                contextPassword = DecrpytKText(contextPassword);
                var index1 = dbConnectionString.IndexOf("Password=", StringComparison.Ordinal) + 9;
                var postfix = dbConnectionString.Substring(index1);
                var index2 = postfix.IndexOf(";", StringComparison.Ordinal);
                postfix = postfix.Substring(index2);
                dbConnectionString = dbConnectionString.Substring(0, index1) +
                    contextPassword + postfix;

                return dbConnectionString;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetKTextConnectionString() : " +
                    UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_connectionStringsName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static string GetConnectionString(string _connectionStringName)
        {
            var config =
                System.Configuration.ConfigurationManager.ConnectionStrings;

            for (var i = 0; i < config.Count; i++)
            {
                if (config[i].Name.Equals(_connectionStringName, StringComparison.OrdinalIgnoreCase))
                    return config[i].ToString();
            }

            var connectionString =
                Microsoft.Azure.CloudConfigurationManager.GetSetting(_connectionStringName) as string;

            return connectionString;
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

        private string GetInnerKText(string body, string tag)
        {
            string Out;

            try
            {
                var Index = body.IndexOf("[" + tag + "]");

                if (-1 == Index)
                    return null;

                Out = body.Substring(Index + tag.Length + 2);
                Index = Out.IndexOf("[/" + tag + "]");
                Out = Out.Substring(0, Index);
            }
            catch (Exception)
            {
                return null;
            }

            return Out;
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

        private string UnwindExceptionMessages(Exception ex)
        {
            var Message = ex.Message;

            if (null != ex.InnerException)
            {
                ex = ex.InnerException;
                Message += " - " + ex.Message;

                if (null != ex.InnerException)
                {
                    ex = ex.InnerException;
                    Message += " - " + ex.Message;
                }
            }

            return Message;
        }
    }
}

