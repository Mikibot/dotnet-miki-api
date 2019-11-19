using System;
using System.Reflection;

namespace Miki.API.Images
{
    public class Config : IEquatable<Config>
    {
        public string Tenancy { get; set; } = "production";
        public string Endpoint { get; set; } = "https://api.miki.ai/images";
        public string UserAgent { get; set; } = 
            $"Miki.API.Images/"+
            Assembly.GetExecutingAssembly()
                .GetName()
                .Version
                .ToString()
                .Substring(0, 3) +
            " (https://github.com/Mikibot/dotnet-miki-api)";
        
        /// <summary>
        /// Allows the client to use unstable, experimental features
        /// </summary>
        public bool Experimental { get; set; } = false;

        public static Config Default()
        {
            return new Config();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Config);
        }

        public bool Equals(Config other)
        {
            return other != null &&
                   Tenancy == other.Tenancy &&
                   Endpoint == other.Endpoint &&
                   UserAgent == other.UserAgent &&
                   Experimental == other.Experimental;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Tenancy, Endpoint, UserAgent, Experimental);
        }
    }
}
