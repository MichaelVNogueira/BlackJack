namespace BlackJackLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BlackJack : IBlackJack
    {
        public int NumPlayers => _callbacks.Count;

        private Dictionary<int, ICallback> _callbacks = new Dictionary<int, ICallback>();
        private int _nextId = 1;
        private int winning = 0;
        private int _clientIndex;
        private int roundScore;
        private int maxScore = 0;
        private bool gameOver = false;

        public int JoinGame()
        {
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();

            if (_callbacks.ContainsValue(cb))
                return _callbacks.Keys.ElementAt(_callbacks.Values.ToList().IndexOf(cb));

            _callbacks.Add(_nextId, cb);
            return _nextId++;
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

        public int Hit() => new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 }[new Random().Next(1, 13)];

        public void NextTurn(int score = 0)
        {
            if (_clientIndex == _callbacks.Count - 1)
                gameOver = true;
            _clientIndex++;
            roundScore = score;
            if (roundScore > maxScore && roundScore <= 21)
            {
                maxScore = roundScore;
                winning = _clientIndex;
            }
            UpdateClients();
        }

        public void RegisterForCallbacks()
        {
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();
            if (!_callbacks.ContainsValue(cb))
                _callbacks.Add(_nextId++, cb);
        }

        public void UnregisterFromCallbacks()
        {
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();
            if (_callbacks.ContainsValue(cb))
                _callbacks.Remove(_nextId - 1);
        }

        private void UpdateClients()
        {
            foreach (ICallback cb in _callbacks.Values)
                cb.UpdateConsole(_clientIndex + 1, gameOver, roundScore, maxScore, winning);
        }
    }
}
