namespace BlackJackClient
{
    using BlackJackLibrary;
    using System.ServiceModel;
    using System;
    using System.Threading;
    using System.Linq;

    class Program
    {        
        private class CallBackObject : ICallback
        {
            public void UpdateConsole(int nextPlayer, bool gameOver)
            {
                activeClientId = nextPlayer;
                if(clientId == activeClientId)
                {
                    Console.WriteLine("It's Your Turn...");
                    waitHandle.Set();
                }
                else
                {
                    Console.WriteLine($"Player {activeClientId}s Turn...");
                }

            }
        }

        private static IBlackJack _blackJack = null;
        private static int clientId, activeClientId = 0;
        private static CallBackObject CBObj = new CallBackObject();
        private static EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private static bool gameOver = false;

        static void Main(string[] args)
        {
            try
            {
                if (Connect())
                {
                    do
                    {
                        waitHandle.WaitOne();

                        if (gameOver)
                        {
                            Console.WriteLine("Game Over");
                            Console.ReadKey();
                        }
                        else
                        {
                            bool choosing = true;
                            int score = 0;
                            Console.Write("Your starting cards are ");
                            int startCard1 = _blackJack.Hit();
                            int startCard2 = _blackJack.Hit();
                            score += startCard1 + startCard2;
                            Console.WriteLine($"{startCard1} and {startCard2}");
                            do
                            {
                                Console.WriteLine($"Your current score is {score}");
                                Console.WriteLine("Would you like to Hit or stay? (press h or s)");
                                char choice = Char.ToLower(Console.ReadKey(true).KeyChar);
                                if (!"hs".Contains(choice))
                                    Console.WriteLine("Invalid input, try again");
                                else if (choice == 'h')
                                {
                                    int value = _blackJack.Hit();
                                    Console.WriteLine(value);
                                    score += value;
                                    if (score > 21)
                                    {
                                        Console.WriteLine($"You lost!, your score was {score}");
                                        choosing = false;
                                        _blackJack.NextTurn();
                                    }
                                }
                                else if (choice == 's')
                                {
                                    _blackJack.NextTurn();
                                    choosing = false;
                                }
                            } while (choosing);
                            waitHandle.Reset();
                        }
                    } while (!gameOver);

                    _blackJack.LeaveGame();
                }
                else
                    Console.WriteLine("Unable to connect to service");

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _blackJack?.LeaveGame();
            }                       
        }

        private static bool Connect()
        {
            try
            {
                DuplexChannelFactory<IBlackJack> channel = new DuplexChannelFactory<IBlackJack>(CBObj, "BlackJackEndpoint");
                _blackJack = channel.CreateChannel();

                clientId = _blackJack.JoinGame();
                Console.WriteLine($"Connected as Player{clientId}");
                if (clientId == 1)
                    CBObj.UpdateConsole(1, false);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }        
    }
}
