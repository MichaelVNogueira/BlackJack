namespace BlackJackClient
{
    using BlackJackLibrary;
    using System.ServiceModel;
    using System;
    using System.Threading;
    using System.Linq;

    public class Program
    {
        private class CallBackObject : ICallback
        {
            public void UpdateConsole(int nextPlayer = 1, bool over = false, int lastScore = 0, int maxScore = 0, int winning = 0)
            {
                if (nextPlayer != 1)
                    Console.WriteLine($"Player {nextPlayer - 1} ended their turn with a score of {lastScore}");
                if (winning != 0)
                    Console.WriteLine($"Player {winning} is winning with a score of {maxScore}");

                activeClientId = nextPlayer;
                gameOver = over;
                if (!gameOver)
                {
                    if (clientId == activeClientId)
                        Console.WriteLine("It's Your Turn...");
                    else
                        Console.WriteLine($"Player {activeClientId}s Turn...");
                }
                waitHandle.Set();

            }
        }

        private static IBlackJack blackJack = null;
        private static int clientId, activeClientId = 0;
        private static readonly CallBackObject CBObj = new CallBackObject();
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
                        if (activeClientId == clientId)
                        {
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
                                int startCard1 = blackJack.Hit();
                                int startCard2 = blackJack.Hit();
                                score += startCard1 + startCard2;
                                Console.WriteLine($"{startCard1} and {startCard2}");
                                do
                                {
                                    Console.WriteLine($"Your current score is {score}");
                                    if (activeClientId == clientId)
                                        Console.WriteLine("Would you like to Hit or stay? (press h or s)");
                                    char choice = Char.ToLower(Console.ReadKey(true).KeyChar);
                                    if (!"hs".Contains(choice))
                                        Console.WriteLine("Invalid input, try again");
                                    else if (choice == 'h')
                                    {
                                        int value = blackJack.Hit();
                                        Console.WriteLine(value);
                                        score += value;
                                        if (score > 21)
                                        {
                                            Console.WriteLine($"You lost!, your score was {score}");
                                            choosing = false;
                                            blackJack.NextTurn(score);
                                        }
                                    }
                                    else
                                    {
                                        blackJack.NextTurn(score);
                                        choosing = false;
                                    }
                                } while (choosing);
                                waitHandle.Reset();
                            }
                        }
                    } while (!gameOver);
                    waitHandle.Reset();
                    Console.WriteLine("Game Is Over");
                    Console.ReadKey();
                }
                else
                    Console.WriteLine("Unable to connect to service");

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                blackJack?.LeaveGame();
            }                       
        }

        private static bool Connect()
        {
            try
            {
                DuplexChannelFactory<IBlackJack> channel = new DuplexChannelFactory<IBlackJack>(CBObj, "BlackJackEndpoint");
                blackJack = channel.CreateChannel();

                clientId = blackJack.JoinGame();
                Console.WriteLine($"Connected as Player{clientId}");
                if (clientId == 1)
                    CBObj.UpdateConsole();
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
