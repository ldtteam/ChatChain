using System.IO;
using System.Security.Cryptography.X509Certificates;
using ChatChainCommon.Config.IdentityServer;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Services
{
    public static class SigninKeyExtension
    {
        public static void AddCertificateFromFile(this IIdentityServerBuilder builder, IdentityServerOptions config)
        {
            if (File.Exists(config.SigningPath))
            {
                builder.AddSigningCredential(new X509Certificate2(config.SigningPath, config.SigningPassword));
            }
        }
    }
}