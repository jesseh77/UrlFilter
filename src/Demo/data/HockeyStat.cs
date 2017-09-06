using Newtonsoft.Json;
using System;

namespace DemoApi.data
{
    public class HockeyStat
    {
        public int assists { get; set; }
        [JsonProperty("faceoffWinPctg")]
        public float faceoffPct { get; set; }
        public int gameWinningGoals { get; set; }
        public int gamesPlayed { get; set; }
        public int goals { get; set; }
        public int otGoals { get; set; }
        public int penaltyMinutes { get; set; }
        [JsonProperty("playerBirthCity")]
        public string birthCity { get; set; }
        [JsonProperty("playerBirthCountry")]
        public string birthCountry { get; set; }
        [JsonProperty("playerBirthDate")]
        public int birthDate { get; set; }
        [JsonProperty("playerBirthStateProvince")]
        public string birthState { get; set; }
        [JsonProperty("playerDraftOverallPickNo")]
        public int? draftPick { get; set; }
        [JsonProperty("playerDraftRoundNo")]
        public int? draftRound { get; set; }
        [JsonProperty("playerDraftYear")]
        public int? draftYear { get; set; }
        [JsonProperty("playerFirstName")]
        public string firstName { get; set; }
        [JsonProperty("playerHeight")]
        public int height { get; set; }
        public int playerId { get; set; }
        public int playerInHockeyHof { get; set; }
        [JsonProperty("playerLastName")]
        public string lastName { get; set; }
        [JsonProperty("playerName")]
        public string name { get; set; }
        [JsonProperty("playerNationality")]
        public string nationality { get; set; }
        [JsonProperty("playerPositionCode")]
        public string position { get; set; }
        [JsonProperty("playerShootsCatches")]
        public string shootsCatches { get; set; }
        public string playerTeamsPlayedFor { get; set; }
        [JsonProperty("playerWeight")]
        public int weight { get; set; }
        public int plusMinus { get; set; }
        public int points { get; set; }
        public float pointsPerGame { get; set; }
        public int ppGoals { get; set; }
        public int ppPoints { get; set; }
        public int seasonId { get; set; }
        public int shGoals { get; set; }
        public int shPoints { get; set; }
        public float shiftsPerGame { get; set; }
        public float shootingPctg { get; set; }
        public int shots { get; set; }
        public float timeOnIcePerGame { get; set; }
    }
}