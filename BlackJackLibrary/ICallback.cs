/*
 * Program:     BlackJackLibrary.dll
 * Module:      ICallBack.cs
 * Authors:     Michael Nogueira, Ali Osseili, Allyson Griffin
 * Date:        April 8, 2022
 * Description: Defines a CallBack interface for the BlackJackService
 */
namespace BlackJackLibrary
{
    using System.ServiceModel;

    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateConsole(int nextPlayer, bool over, int lastScore, int maxScore, int winning, bool isLobbyFull);
    }
}