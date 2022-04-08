/*
 * Program:     BlackJackClient.exe
 * Module:      Program.cs
 * Authors:     Michael Nogueira, Ali Osseili, Allyson Griffin
 * Date:        April 8, 2022
 * Description: Implements the game logic of BlackJack using the BlackJackService and the BlackJackLibrary
 */

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
            /// <summary>
            ///     This function allows the service to update the clients via callbacks
            /// </summary>
            /// <param name="nextPlayer"></param>
            /// <param name="over"></param>
            /// <param name="lastScore"></param>
            /// <param name="maxScore"></param>
            /// <param name="winning"></param>
            /// <param name="isLobbyFull"></param>
            public void UpdateConsole(int nextPlayer = 1, bool over = false, int lastScore = 0, int maxScore = 0, int winning = 0, bool isLobbyFull = false)
            {
                if (!isLobbyFull) return;

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
                try
                {
                    waitHandle.Set();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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
                                // start turn
                                bool choosing = true;
                                int score = 0;
                                Console.Write("Your starting cards are ");
                                int startCard1 = blackJack.Hit();
                                int startCard2 = blackJack.Hit();
                                score += startCard1 + startCard2;
                                Console.WriteLine($"{startCard1} and {startCard2}");

                                // prompt for hit or stay
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
                blackJack.LeaveGame();
            }                       
        }

        /// <summary>
        ///     Connects a client to the BlackJackService
        /// </summary>
        /// <returns>bool</returns>
        private static bool Connect()
        {
            try
            {
                DuplexChannelFactory<IBlackJack> channel = new DuplexChannelFactory<IBlackJack>(CBObj, "BlackJackEndpoint");
                blackJack = channel.CreateChannel();

                clientId = blackJack.JoinGame();
                Console.WriteLine($"Connected as Player{clientId}");
                if (clientId == 1)
                {
                    bool valid = false;
                    do
                    {
                        Console.Write("Enter number of players?:");
                        if (valid = Int32.TryParse(Console.ReadLine(), out int val))
                            blackJack.NumPlayers = val;
                    } while (!valid);
                    Console.WriteLine($"Waiting for {blackJack.NumPlayers} players...");
                    CBObj.UpdateConsole();
                }
                else
                {
                    if (clientId < blackJack.NumPlayers)
                        Console.WriteLine($"Waiting for {blackJack.NumPlayers} players...");
                    else
                        CBObj.UpdateConsole(isLobbyFull: true);
                }

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
