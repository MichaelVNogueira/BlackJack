namespace BlackJackLibrary
{
    using System.Collections.Generic;
    public class Player
    {
        public int Wins { get; set; }
        public int Id { get; set; }
        public List<Card> Hand { get; set; }
        public int CardScore
        {
            get
            {
                int score = 0;
                Hand.ForEach(c => c.GetValue());
                return score;
            }
        }
    }
}
