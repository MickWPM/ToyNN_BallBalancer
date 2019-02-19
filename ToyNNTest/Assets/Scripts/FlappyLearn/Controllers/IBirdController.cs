
namespace FlappyLearn
{

    public interface IBirdController
    {
        bool DoFlapFromState(FlappyGameState gameState);
    }

    public struct FlappyGameState
    {
        public float distToPipe;
        public float heightAboveMidPoint;
    }


}