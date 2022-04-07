namespace BlackJackLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading.Tasks;

    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IBlackJack
    {
        int NumPlayers { [OperationContract] get; }
        [OperationContract]
        int JoinGame();
        [OperationContract(IsOneWay = true)]
        void LeaveGame();
        [OperationContract]
        int Hit();
        [OperationContract(IsOneWay = true)]
        void NextTurn();
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BlackJack : IBlackJack
    {
        public int NumPlayers => _callbacks.Count;

        private Dictionary<int, ICallback> _callbacks = new Dictionary<int, ICallback>();
        private int _nextId = 1;
        private int _clientIndex;
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

        public int Hit() => new Random().Next(1, 10);

        public void NextTurn()
        {
            _clientIndex++;
            UpdateClients();
        }

        private void UpdateClients()
        {
            foreach (ICallback cb in _callbacks.Values)
                cb.UpdateConsole(_clientIndex, gameOver);
        }
    }
}
