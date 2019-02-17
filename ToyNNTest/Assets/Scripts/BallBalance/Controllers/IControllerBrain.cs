namespace BallBalance
{
    public interface IControllerBrain
    {
        GameControl.DataOutputs GetOutputs(GameControl.DataInputs dataInputX, GameControl.DataInputs dataInputY);
    }
}