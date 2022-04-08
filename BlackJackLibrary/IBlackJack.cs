namespace BlackJackLibrary
{
    using System.ServiceModel;

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
        void NextTurn(int score = 0);
        [OperationContract(IsOneWay = true)]
        void RegisterForCallbacks();
        [OperationContract(IsOneWay = true)]
        void UnregisterFromCallbacks();
    }
}
