using MimeKit.Cryptography;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Serilog;
using System.IO;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class CertGenConfig
    {
        private static Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator randomGenerator = new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator();
        private static Org.BouncyCastle.X509.X509V3CertificateGenerator certificateGenerator = new Org.BouncyCastle.X509.X509V3CertificateGenerator();

        private static string cert_json_file = "certificate_generation.json";

        public enum Result
        {
            GeneratedConfigFile,
            GeneratedSslCertificate,
            None
        }

        public static async Task<Result> CertGen()
        {
            Result generation_result = Result.None;

            string? serialised_json = null;

            try
            {
                FileStream fs = File.OpenRead(cert_json_file);
                try
                {
                    byte[] json_binary = new byte[fs.Length];
                    await fs.ReadAsync(json_binary, 0, json_binary.Length);
                    serialised_json = Encoding.UTF8.GetString(json_binary);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error reading certificate configurations");
                }
                finally
                {
                    await fs.DisposeAsync();
                }
            }
            catch { }

            CertGenModel? model = await JsonFormatter.JsonDeserialiser<CertGenModel?>(serialised_json);


            if (model == null)
            {
                await CreateCertGenConfigFile(new CertGenModel());
                Console.WriteLine(new StringBuilder("\n\n\n\tGenerated SSL certificate genertion configuration file at: ").Append(Environment.CurrentDirectory).Append(FileSystemFormatter.PathSeparator()).Append(cert_json_file).Append("\n\n\n"));
                generation_result = Result.GeneratedConfigFile;
            }
            else
            {
                if (model.generate_certificate == true)
                {
                    await Create_X509_Server_Certificate(model);
                    model.generate_certificate = false;
                    await CreateCertGenConfigFile(model);

                    Console.WriteLine(new StringBuilder("\n\n\n\tGenerated client SSL certificate at: ").Append(Environment.CurrentDirectory).Append(FileSystemFormatter.PathSeparator()).Append(model.client_certificate_file_name));
                    Console.WriteLine(new StringBuilder("\n\n\tGenerated server SSL certificate at: ").Append(Environment.CurrentDirectory).Append(FileSystemFormatter.PathSeparator()).Append(model.server_certificate_file_name).Append("\n\n\n"));

                    generation_result = Result.GeneratedSslCertificate;
                }
            }

            return generation_result;
        }

        private async static Task CreateCertGenConfigFile(CertGenModel model)
        {
            FileStream fileStream = File.OpenWrite(cert_json_file);
            try
            {

                string? serialised_model = await JsonFormatter.JsonSerialiser(model);
                if (serialised_model != null)
                {
                    byte[] model_binary = Encoding.UTF8.GetBytes(serialised_model);
                    await fileStream.WriteAsync(model_binary, 0, model_binary.Length);
                    await fileStream.FlushAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error creating server configuration file");
            }
            finally
            {
                await fileStream.DisposeAsync();
            }
        }

        // METHOD THAT IS CREATING A X509 SSL CERTIFICATE THAT HAS SHA256 WITH RSA BASED ENCRYPTION
        private async static Task Create_X509_Server_Certificate(CertGenModel config)
        {

            try
            {

                Org.BouncyCastle.Asn1.X509.X509Name subjectDN = new Org.BouncyCastle.Asn1.X509.X509Name($"CN={config.issuer_domain_name}");
                Org.BouncyCastle.Asn1.X509.X509Name issuerDN = subjectDN;
                Org.BouncyCastle.Asn1.X509.X509Name alternativeDN = subjectDN;

                // CREATE RANDOM SERIAL NUMBER FOR THE CERTIFICATE USING A RANDOM NUMBER GENERATOR
                Org.BouncyCastle.Security.SecureRandom random = new Org.BouncyCastle.Security.SecureRandom(randomGenerator);
                Org.BouncyCastle.Math.BigInteger serialNumber = Org.BouncyCastle.Utilities.BigIntegers.CreateRandomInRange(Org.BouncyCastle.Math.BigInteger.One,
                                                                                                                           Org.BouncyCastle.Math.BigInteger.ValueOf(int.MaxValue), random);

                DerSequence subjectAlternativeNamesExtension = new DerSequence(new Asn1Encodable[]
                {
                     new GeneralName(GeneralName.DnsName, config.issuer_domain_name),
                     new GeneralName(GeneralName.DnsName, "*")
                });

                certificateGenerator.AddExtension(
                    X509Extensions.SubjectAlternativeName.Id, false, subjectAlternativeNamesExtension);

                certificateGenerator.SetSerialNumber(serialNumber);

                // SET THE CERTIFICATE AUTHORITY NAME AND THE SUBJECT NAME OF THE CERTIFICATE
                certificateGenerator.SetIssuerDN(issuerDN);
                certificateGenerator.SetSubjectDN(subjectDN);

                // SET THE CERTIFICATE'S EXPIRATION DATE
                DateTime notBefore = DateTime.UtcNow.Date;
                DateTime notAfter = notBefore.AddDays(config.number_of_days_after_which_certificate_expires);
                certificateGenerator.SetNotBefore(notBefore);
                certificateGenerator.SetNotAfter(notAfter);

                // GENERATE A RANDOM PRIVATE KEY WITH A 2048 BIT STRENGTH
                Org.BouncyCastle.Crypto.KeyGenerationParameters keyGenerationParameters = new Org.BouncyCastle.Crypto.KeyGenerationParameters(random, config.key_encryption_strength_in_bits);
                Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator keyPairGenerator = new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();
                keyPairGenerator.Init(keyGenerationParameters);
                Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair subjectKeyPair = keyPairGenerator.GenerateKeyPair();

                // SET THE PUBLIC KEY OF THE CERTIFICATE AND GENERATE THE CERTIFICATE USING SHA256 HASHING ALGORITHM WITH THE RSA ALGORITHM
                certificateGenerator.SetPublicKey(subjectKeyPair.Public);
                Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory = new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory("SHA256WithRSA", subjectKeyPair.Private, random);
                Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(signatureFactory);

                PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectKeyPair.Private);


                if (config != null)
                {
                    if (config.server_certificate_file_name != null)
                    {
                        if (config.client_certificate_file_name != null)
                        {
                            var builder = new StringBuilder();

                            StringWriter writer = new StringWriter(builder);
                            try
                            {
                                PemWriter pemWriter = new PemWriter(writer);
                                try
                                {
                                    pemWriter.WriteObject(certificate);
                                    string cert = builder.ToString();
                                    builder.Clear();

                                    pemWriter.WriteObject(privateKeyInfo);
                                    string privateKey = builder.ToString();

                                    X509Certificate2 certificate_bundle = X509Certificate2.CreateFromPem(cert, privateKey);
                                    X509Certificate2 public_certificate = X509Certificate2.CreateFromPem(cert);


                                    byte[] server_cert = certificate_bundle.Export(X509ContentType.Pkcs12, config.private_certificate_password);
                                    byte[] public_cert = public_certificate.Export(X509ContentType.Cert);


                                    FileStream server_cert_stream = File.Create($"{config.server_certificate_file_name}.pfx");
                                    try
                                    {
                                        server_cert_stream.Write(server_cert);
                                        await server_cert_stream.FlushAsync();

                                        FileStream public_cert_stream = File.Create($"{config.client_certificate_file_name}.crt");
                                        try
                                        {
                                            public_cert_stream.Write(public_cert);
                                            await public_cert_stream.FlushAsync();

                                            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                                            store.Open(OpenFlags.ReadWrite);
                                            store.Add(public_certificate);
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error(e, "X509 certificate generation error");
                                        }
                                        finally
                                        {
                                            public_cert_stream.Dispose();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error(e, "X509 certificate generation error");
                                    }
                                    finally
                                    {
                                        server_cert_stream.Dispose();
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e, "X509 certificate generation error");
                                }
                                finally
                                {
                                    pemWriter.Dispose();
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "X509 certificate generation error");
                            }
                            finally
                            {
                                await writer.DisposeAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "X509 certificate generation error");
            }
        }
    }
}
