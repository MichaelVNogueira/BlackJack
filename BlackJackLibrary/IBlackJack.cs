namespace BlackJackLibrary
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;

    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IBlackJack
    {
        List<Player> Players { [OperationContract] get; }
        int NumPlayers { [OperationContract] get; }
        [OperationContract]
        int JoinGame();
        [OperationContract(IsOneWay = true)]
        void LeaveGame();
        [OperationContract]
        Card Hit();
        [OperationContract(IsOneWay = true)]
        void NextTurn();
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BlackJack : IBlackJack
    {
        public List<Player> Players => _players;
        public int NumPlayers => _players.Count;

        private Dictionary<int, ICallback> _callbacks = new Dictionary<int, ICallback>();
        private List<Player> _players = new List<Player>();
        private Deck _deck = new Deck();
        private int _nextId = 1;
        private int _clientIndex;
        private bool gameOver = false;

        public int JoinGame()
        {
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();

            if (_callbacks.ContainsValue(cb))
                return _callbacks.Keys.ElementAt(_callbacks.Values.ToList().IndexOf(cb));

            _callbacks.Add(_nextId++, cb);
            return _nextId;
        }

        public void LeaveGame()
        {
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();

            if (_callbacks.ContainsValue(cb))
            {
                int i = _callbacks.Values.ToList().IndexOf(cb);
                int id = _callbacks.ElementAt(i).Key;
                _callbacks.Remove(id);
                if (i == _clientIndex)
                    UpdateClients();                
            }
        }

        public Card Hit()
        {
            Card card = _deck.Draw();
            _players[_clientIndex].Hand.Add(card);
            return card;
        }

        public void NextTurn()
        {
            // Determine index of the next client that gets to "count"
            _clientIndex = ++_clientIndex % _callbacks.Count;

            // Update all clients
            UpdateClients();
        }

        private void UpdateClients()
        {
            foreach (ICallback cb in _callbacks.Values)
                cb.UpdateConsole(_clientIndex, gameOver);
        }
    }
}
