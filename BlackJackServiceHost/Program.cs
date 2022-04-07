/*
 * Program:     BlackJackServiceHost.exe
 * Module:      Program.cs
 * Authors:     Michael Nogueira, Ali Oseili, Allyson Griffin
 * Date:        April 8, 2022
 * Description: Implements a service host for Black Jack
 */

namespace BlackJackServiceHost
{
    using System;
    using System.ServiceModel;
    using BlackJackLibrary;
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = null;

            try
            {
                host = new ServiceHost(typeof(BlackJack));

                host.Open();
                Console.WriteLine("BlackJack Service Started... press any key to exit");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey();
                if (host != null)
                    host.Close();
            }
        }
    }
}
