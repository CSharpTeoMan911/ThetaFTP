﻿namespace ThetaFTP.Shared.Classes
{
    public class HttpClientGen
    {
        public static HttpClient Generate()
        {
            int timeout = 600;
            if (Shared.config != null)
                timeout = Shared.config.ConnectionTimeoutSeconds;

            return new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    if (Shared.config != null)
                        return !Shared.config.validate_ssl_certificates;
                    return true;
                }
            })
            {
                Timeout = TimeSpan.FromMinutes(timeout)
            };
        }
    }
}
