using Opc.Ua;
using System.Security.Cryptography.X509Certificates;

namespace OPCClientInterface
{
    /// <summary>
    /// 登录功能接口库
    /// </summary>
    public class ClientLogIn
    {
        private OPCUAClient m_OpcUaClient = null;
        public ClientLogIn(OPCUAClient opcUaClient)
        {
            m_OpcUaClient = opcUaClient;
        }
        /// <summary>
        /// 访客登录
        /// </summary>
        /// <returns></returns>
        public bool GuestLogin()
        {
            try
            {
                m_OpcUaClient.UserIdentity = new UserIdentity(new AnonymousIdentityToken());
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 用户密码登录
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        public bool UserIdentityLogin(string UserName, string PassWord)
        {
            try
            {
                m_OpcUaClient.UserIdentity = new UserIdentity(UserName, PassWord);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 证书登录
        /// </summary>
        /// <param name="CertificateFileName"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        public bool CertificateLogin(string CertificateFileName, string PassWord)
        {
            try
            {
                X509Certificate2 certificate = new X509Certificate2(CertificateFileName, PassWord, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                m_OpcUaClient.UserIdentity = new UserIdentity(certificate);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}