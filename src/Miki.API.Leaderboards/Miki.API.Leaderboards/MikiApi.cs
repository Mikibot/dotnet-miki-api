﻿namespace Miki.API.Leaderboards
{
    using Miki.Api.Models;
    using Miki.Net.Http;
    using Miki.Rest;
    using Newtonsoft.Json;
    using System;
    using System.Text;
    using System.Threading.Tasks;

    public class MikiLeaderboardsClient : IDisposable
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

		/// <summary>
		/// Builds the url to the leaderboards page on the miki website
		/// </summary>
		/// <param name="options">Leaderboards Options Object</param>
		/// <returns>https://miki.ai/leaderboards/{guild_id?}/{type}</returns>
		public string BuildLeaderboardsUrl(MikiApiClient client, LeaderboardsOptions options)
			=> "https://miki.ai" + BuildLeaderboardsRoute(options);

		/// <summary>
		/// Pulls the leaderboards data from the API
		/// </summary>
		/// <param name="options">Leaderboards Options Object</param>
		public async Task<LeaderboardsObject> GetPagedLeaderboardsAsync(LeaderboardsOptions options)
			=> JsonConvert.DeserializeObject<LeaderboardsObject>(
                (await client.GetAsync(BuildLeaderboardsRoute(options))).Body);

        public async Task<UserInventory> GetUserInventoryAsync(long id)
            => JsonConvert.DeserializeObject<UserInventory>(
                (await client.GetAsync($"users/{id}/inventory")).Body);
		private string BuildLeaderboardsRoute(LeaderboardsOptions options)
		{
			StringBuilder sb = new StringBuilder()
				.Append("/leaderboards");

			if (options.GuildId.HasValue)
			{
				sb.Append($"/{options.GuildId}");
			}

			sb.Append($"/{options.Type.ToString().ToLower()}");

			QueryString qs = new QueryString();

			qs.Add("amount", options.Amount);
			qs.Add("offset", options.Offset);

			return sb + qs.Query;
		}

		public void Dispose()
		{
			client.Dispose();
		}
	}
}