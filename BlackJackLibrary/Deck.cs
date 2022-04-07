/*namespace BlackJackLibrary
{
    using System.Collections.Generic;
    using System;
    using System.ServiceModel;

    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IDeck
    {
        int Count { get; }
        bool HasCards { get; }
        Card Draw();
        void Shuffle();
        void Print();
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Deck : IDeck
    {

        public Deck() => PopulateDeck();
        public int Count => _cards.Count;
        public bool HasCards => _cards.Count > 0;

        private readonly Stack<Card> _cards = new Stack<Card>();
        private readonly HashSet<ICallback> _callbacks = new HashSet<ICallback>();

        public Card Draw() {
            if (_cards.Count <= 0)
                PopulateDeck();
            _cards.Pop();
        }

        public void Shuffle() { }

        public void Print()
        {
            foreach (var card in _cards) Console.WriteLine(card);
        }

        public void RegisterForCallbacks()
        {
            ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
            if (!_callbacks.Contains(callback)) _callbacks.Add(callback);
        }

        public void UnregisterFromCallbacks()
        {
            ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
            if (_callbacks.Contains(callback)) _callbacks.Remove(callback);
        }

        private void PopulateDeck()
        {
            _cards.Clear();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                    _cards.Push(new Card(rank, suit));
        }
    }
}
*/