namespace ThetaFTP.Shared.Classes
{
    public class HttpClientGen
    {
        public static HttpClient Generate(bool allow_untrusted_cert_authority)
        {
            return new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return allow_untrusted_cert_authority;
                }
            });
        }
    }
}
