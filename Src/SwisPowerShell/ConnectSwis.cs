using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using SolarWinds.InformationService.Contract2;

namespace SwisPowerShell
{
    public class ConnectSwis
    {
        public string UserName { get; set; }

        public string Password { get; set; }
        public string TrustX509Thumbprint { get; set; }

        private bool IsUserNamePresent
        {
            get { return !string.IsNullOrEmpty(UserName); }
        }

        private class EndpointAddresses
        {
            public string activeDirectory;
            public string certificate;
            public string usernamePassword;
        }

        private class V2EndpointAddresses : EndpointAddresses
        {
            public V2EndpointAddresses()
            {
                activeDirectory = "net.tcp://{0}:17777/SolarWinds/InformationService/Orion/ad";
                certificate = "net.tcp://{0}:17777/SolarWinds/InformationService/Orion/certificate";
                usernamePassword = "net.tcp://{0}:17777/SolarWinds/InformationService/Orion/ssl";
            }
        }

        private class V3EndpointAddresses : EndpointAddresses
        {
            public readonly string streamedCertificate =
                "net.tcp://{0}:17777/SolarWinds/InformationService/v3/Orion/Streamed/Certificate";

            public V3EndpointAddresses()
            {
                activeDirectory = "net.tcp://{0}:17777/SolarWinds/InformationService/v3/Orion/ad";
                certificate = "net.tcp://{0}:17777/SolarWinds/InformationService/v3/Orion/certificate";
                usernamePassword = "net.tcp://{0}:17777/SolarWinds/InformationService/v3/Orion/ssl";
            }
        }

        protected void ProcessRecord()
        {
            InfoServiceProxy infoServiceProxy;

            if (true)
            {
                infoServiceProxy = ConnectSoap12(null, UserName, Password);

                if (true)
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                        AllTrustingServerCertificateValidationCallback;
                }
                else if (!string.IsNullOrEmpty(TrustX509Thumbprint))
                {
                    string[] arr = TrustX509Thumbprint.Split('-', ':', ' ');
                    var trustedThumbprint = new byte[arr.Length];
                    for (int i = 0; i < arr.Length; i++)
                        trustedThumbprint[i] = Convert.ToByte(arr[i], 16);

                    ServicePointManager.ServerCertificateValidationCallback += delegate(object sender,
                        X509Certificate certificate, X509Chain chain,
                        SslPolicyErrors errors)
                    {
                        if (errors == SslPolicyErrors.None)
                            return true;

                        if (certificate.GetCertHash().SequenceEqual(trustedThumbprint))
                            return true;

                        return false;
                    };
                }
            }
            else
                infoServiceProxy = ConnectNetTcp();
        }

        private bool AllTrustingServerCertificateValidationCallback(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Debug.WriteLine("Accepting certificate with thumbprint " +
                            BitConverter.ToString(certificate.GetCertHash()));
            // Looks good to me!
            return true;
        }

        private static InfoServiceProxy ConnectSoap12(Uri address, string username, string password)
        {
            var binding = new WSHttpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            binding.ReaderQuotas.MaxDepth = 32;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.AllowCookies = true;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;
            binding.ReaderQuotas.MaxNameTableCharCount = 16384;
            binding.UseDefaultWebProxy = true;

            var credentials = new UsernameCredentials(username, password);

            return new InfoServiceProxy(address, binding, credentials);
        }

        private InfoServiceProxy ConnectNetTcp()
        {
            InfoServiceProxy infoServiceProxy;

            var binding = new NetTcpBinding {MaxReceivedMessageSize = int.MaxValue, MaxBufferSize = int.MaxValue};
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;


            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;

            binding.TransferMode = TransferMode.Streamed;
            binding.PortSharingEnabled = true;
            binding.ReceiveTimeout = new TimeSpan(15, 0, 0);
            binding.SendTimeout = new TimeSpan(15, 0, 0);


            return null;
        }

        private static string SecureStringToString(SecureString input)
        {
            IntPtr ptr = SecureStringToBSTR(input);
            return PtrToStringBSTR(ptr);
        }

        private static SecureString StringToSecureString(string input)
        {
            char[] passwordChars = (input ?? string.Empty).ToCharArray();

            SecureString securePassword = new SecureString();

            foreach (char c in passwordChars)
            {
                securePassword.AppendChar(c);
            }

            return securePassword;
        }

        private static IntPtr SecureStringToBSTR(SecureString ss)
        {
            return Marshal.SecureStringToBSTR(ss);
        }

        private static string PtrToStringBSTR(IntPtr ptr)
        {
            string s = Marshal.PtrToStringBSTR(ptr);
            Marshal.ZeroFreeBSTR(ptr);
            return s;
        }

        private class MyCertificateCredential : CertificateCredential
        {
            public MyCertificateCredential(string subjectName, StoreLocation storeLocation, StoreName storeName)
                : base(subjectName, storeLocation, storeName)
            {
            }

            public override void ApplyTo(ChannelFactory channelFactory)
            {
                base.ApplyTo(channelFactory);
                if (channelFactory.Credentials != null)
                {
                    channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode =
                        X509CertificateValidationMode.Custom;
                    channelFactory.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator =
                        new CustomCertificateValidator();
                }
            }
        }
    }
}
