namespace Miki.API.Leaderboards
{
    using Miki.Api.Models;
    using Miki.Net.Http;
    using Miki.Rest;
    using Newtonsoft.Json;
    using System;
    using System.Text;
    using System.Threading.Tasks;

    public class MikiApiClient : IDisposable
	{
		protected readonly HttpClient client;

		private const string baseUrl = "https://api.miki.ai/";

		public MikiApiClient(string token)
		{
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

			client = new HttpClient(baseUrl);
            client.AddHeader("Authorization", "Bearer " + token);
		}
        public void Dispose()
		{
			client.Dispose();
		}
	}
}