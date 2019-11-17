namespace Miki.API.Leaderboards
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class LeaderboardsObject
	{
		[JsonProperty("totalPages")]
		public int TotalPages { get; internal set; }

        [JsonProperty("currentPage")]
		public int CurrentPage { get; internal set; }

        [JsonProperty("items")]
		public List<LeaderboardsItem> Items { get; internal set; } = new List<LeaderboardsItem>();
	}
}