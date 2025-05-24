namespace TinklinisPlius.Models
{
    public class WagerDisplayModel
    {
        public int? Chance { get; set; }
        public int? CombinedSum { get; set; }
        public bool? HasFinished { get; set; }
        public string TournamentName { get; set; }
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
    }
}