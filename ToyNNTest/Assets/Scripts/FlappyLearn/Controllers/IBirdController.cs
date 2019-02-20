
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

        public float[] StateToArray {
            get
            {
                return new float[2] { distToPipe, heightAboveMidPoint };
            }
        }
    }


}