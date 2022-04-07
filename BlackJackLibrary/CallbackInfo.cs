namespace BlackJackLibrary
{
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    
    [DataContract]
    public class CallbackInfo
    {
        public int NextPlayer;
        public int PlayersPoints;
        public bool GameOver;
    }
}
