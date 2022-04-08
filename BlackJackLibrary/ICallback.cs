namespace BlackJackLibrary
{
    using System.ServiceModel;

    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateConsole(int nextPlayer, bool over, int lastScore, int maxScore, int winning);
    }
}