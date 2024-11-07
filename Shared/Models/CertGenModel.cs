using Serilog;

namespace ThetaFTP.Shared.Models
{
    public class CertGenModel
    {
        public bool generate_certificate { get; set; }
        public string? client_certificate_file_name { get; set; } = "!!! REPLACE WITH DESIRED CERTIFICATE FILE NAME !!!";
        public string? server_certificate_file_name { get; set; } = "!!! REPLACE WITH DESIRED CERTIFICATE FILE NAME !!!";
        public string? issuer_domain_name { get; set; } = "!!! REPLACE WITH DESIRED CERTIFICATE ISSUER DOMAIN NAME";
        public int number_of_days_after_which_certificate_expires { get; set; } = 365;
        public int key_encryption_strength_in_bits { get; set; } = 2048;
        public string? private_certificate_password { get; set; } = "!!! REPLACE WITH DESIRED PRIVATE CERTIFICATE PASSWORD";
    }
}
