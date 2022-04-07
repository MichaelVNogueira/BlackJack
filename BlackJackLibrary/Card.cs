/*namespace BlackJackLibrary
{
    using System.Runtime.Serialization;

    public enum Suit
    {
        Club, Diamond, Heart, Spade
    }
    public enum Rank
    {
        Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
    }

    public struct Card
    {
        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }
        public Suit Suit { get; }
        public Rank Rank { get; }
        public int GetValue()
        {
            switch(Rank)
            {
                case Rank.King:
                case Rank.Queen:
                case Rank.Jack:
                case Rank.Ten: return 10;
                case Rank.Nine: return 9;
                case Rank.Eight: return 8;
                case Rank.Seven: return 7;
                case Rank.Six: return 6;
                case Rank.Five: return 5;
                case Rank.Four: return 4;
                case Rank.Three: return 3;
                case Rank.Two: return 2;
                case Rank.Ace: return 1;
                default: return 0;
            }
        }
        public override string ToString() => $"{Rank} of {Suit}s";
    }
}
*/