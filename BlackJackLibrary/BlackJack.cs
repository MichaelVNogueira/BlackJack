/*
 * Program:     BlackJackLibrary.dll
 * Module:      BlackJack.cs
 * Authors:     Michael Nogueira, Ali Osseili, Allyson Griffin
 * Date:        April 8, 2022
 * Description: Implements a BlackJack object
 */

namespace BlackJackLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BlackJack : IBlackJack
    {
        public int NumPlayers { get; set; }

        private Dictionary<int, ICallback> _callbacks = new Dictionary<int, ICallback>();
        private int _nextId = 1;
        private int winning = 0;
        private int _clientIndex;
        private int roundScore;
        private int maxScore = 0;
        private bool gameOver = false;
        readonly int[] cardValues = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 };
        readonly Random random = new Random();

        /// <summary>
        ///     Lets a client join the game of blackjack
        /// </summary>
        /// <returns>int</returns>
        public int JoinGame()
        {
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();

            if (_callbacks.ContainsValue(cb))
                return _callbacks.Keys.ElementAt(_callbacks.Values.ToList().IndexOf(cb));

            _callbacks.Add(_nextId, cb);
            UpdateClients();
            return _nextId++;
        }

        /// <summary>
        ///     Lets a client leave the game of blackJack
        /// </summary>
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

        /// <summary>
        ///     Returns an int simulating a card drawn from a deck
        /// </summary>
        /// <returns>int</returns>
        public int Hit() => 
            cardValues[random.Next(1, 13)];

        /// <summary>
        ///     Handles a turn change in the game of blackjack
        /// </summary>
        /// <param name="score"></param>
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

        /// <summary>
        ///     Calls each client's callback function
        /// </summary>
        /// 
        private void UpdateClients()
        {
            foreach (ICallback cb in _callbacks.Values)
                cb.UpdateConsole(_clientIndex + 1, gameOver, roundScore, maxScore, winning, NumPlayers == _callbacks.Count);
        }
    }
}
