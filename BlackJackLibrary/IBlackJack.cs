/*
 * Program:     BlackJackLibrary.dll
 * Module:      IBlackJack.cs
 * Authors:     Michael Nogueira, Ali Osseili, Allyson Griffin
 * Date:        April 8, 2022
 * Description: Defines a service contract for a BlackJack object
 */
namespace BlackJackLibrary
{
    using System.ServiceModel;

    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IBlackJack
    {
        int NumPlayers { [OperationContract] get; [OperationContract] set; }
        [OperationContract]
        int JoinGame();
        [OperationContract(IsOneWay = true)]
        void LeaveGame();
        [OperationContract]
        int Hit();
        [OperationContract(IsOneWay = true)]
        void NextTurn(int score = 0);
    }
}
