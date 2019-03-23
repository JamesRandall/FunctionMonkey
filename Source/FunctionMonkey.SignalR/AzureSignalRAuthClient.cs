using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.IdentityModel.Tokens;

namespace FunctionMonkey.SignalR
{
    public class AzureSignalRAuthClient
    {
        internal string BaseEndpoint { get; }

        internal string AccessKey { get; }

        internal string Version { get; set; }

        public AzureSignalRAuthClient(string connectionString)
        {
            ValueTuple<string, string, string> connectionString1 = ParseConnectionString(connectionString);
            this.BaseEndpoint = connectionString1.Item1;
            this.AccessKey = connectionString1.Item2;
            string str;
            this.Version = str = connectionString1.Item3;
        }

        public SignalRConnectionInfo GetClientConnectionInfo(string hubName, IEnumerable<Claim> claims = null)
        {
            string audience = string.Format("{0}/client/?hub={1}", (object)this.BaseEndpoint, (object)hubName);
            ClaimsIdentity subject = new ClaimsIdentity(claims);
            string jwtBearer = this.GenerateJwtBearer((string)null, audience, subject, new DateTime?(DateTime.UtcNow.AddMinutes(30.0)), this.AccessKey);
            return new SignalRConnectionInfo()
            {
                Url = audience,
                AccessToken = jwtBearer
            };
        }

        private string GenerateJwtBearer(string issuer, string audience, ClaimsIdentity subject, DateTime? expires, string signingKey)
        {
            SigningCredentials signingCredentials = (SigningCredentials)null;
            if (!string.IsNullOrEmpty(signingKey))
                signingCredentials = new SigningCredentials((SecurityKey)new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)), "HS256");
            JwtSecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler();
            return securityTokenHandler.WriteToken((Microsoft.IdentityModel.Tokens.SecurityToken)securityTokenHandler.CreateJwtSecurityToken(issuer, audience, subject, new DateTime?(), expires, new DateTime?(), signingCredentials));
        }

        /*public SignalRConnectionInfo GetClientConnectionInfo(SignalRConnectionInfoAttribute attribute)
        {
            IEnumerable<Claim> claims1 = attribute.GetClaims();
            string hubName = attribute.HubName;
            IEnumerable<Claim> claims2 = claims1;
            return GetClientConnectionInfo(hubName, claims2);
        }*/

        private static ValueTuple<string, string, string> ParseConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("SignalR Service connection string is empty");
            Match match1 = Regex.Match(connectionString, "endpoint=([^;]+)", RegexOptions.IgnoreCase);
            if (!match1.Success)
                throw new ArgumentException("No endpoint present in SignalR Service connection string");
            Match match2 = Regex.Match(connectionString, "accesskey=([^;]+)", RegexOptions.IgnoreCase);
            if (!match2.Success)
                throw new ArgumentException("No access key present in SignalR Service connection string");
            Match match3 = Regex.Match(connectionString, "Version=([^;]+)", RegexOptions.IgnoreCase);
            System.Version result;
            if (match3.Success && !System.Version.TryParse(match3.Groups[1].Value, out result))
                throw new ArgumentException("Invalid version format in SignalR Service connection string");
            return new ValueTuple<string, string, string>(match1.Groups[1].Value, match2.Groups[1].Value, match3.Groups[1].Value);
        }
    }
}
