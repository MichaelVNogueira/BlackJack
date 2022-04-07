namespace BlackJackLibrary
{
    using System.ServiceModel;

    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateConsole(string nextPlayer, bool over, );
    }
}
