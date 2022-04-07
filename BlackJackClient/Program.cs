namespace BlackJackClient
{
    using BlackJackLibrary;
    using System.ServiceModel;
    using System;
    using System.Threading;

    class Program
    {        
        private class CallBackObject : ICallback
        {
            public void UpdateConsole(int nextPlayer, bool over)
            {
                activeClientId = nextPlayer;
                gameOver = over;
                Console.WriteLine("Update Console...");
            }
        }

        private static IBlackJack _blackJack = null;
        private static int clientId, activeClientId = 0;
        private static CallBackObject CBObj = new CallBackObject();
        private static EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private static bool gameOver = false;

        static void Main(string[] args)
        {
            if (Connect())
            {
                Console.WriteLine("Connected");
                do
                {
                    //waitHandle.WaitOne();

                    if (gameOver)
                    {
                        Console.WriteLine("Game Over");
                        Console.ReadKey();
                    }
                    else
                    {
                        bool valid = false;
                        Console.WriteLine("Hit or stay?\n\tPress h or s then enter.");
                        do
                        {
                            string choice = Console.ReadLine();
                            if (choice == "h")
                                _blackJack.Hit();
                            else if (choice == "s")
                                _blackJack.NextTurn();
                            else
                                Console.WriteLine("Invalid input, try again");
                        } while (!valid);   
                        
                    }
                } while (!gameOver);

                _blackJack.LeaveGame();
            }
            else
                Console.WriteLine("Unable to connect to service");            
        }

        private static bool Connect()
        {
            try
            {
                DuplexChannelFactory<IBlackJack> channel = new DuplexChannelFactory<IBlackJack>(CBObj, "BlackJackEndpoint");
                _blackJack = channel.CreateChannel();

                clientId = _blackJack.JoinGame();
                if (clientId == 1)
                    CBObj.UpdateConsole()
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
