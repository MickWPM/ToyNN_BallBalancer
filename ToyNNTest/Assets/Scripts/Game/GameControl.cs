using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public bool useDistanceToTargetInsteadOfRawTargetPos = false;
    public static readonly int NUM_OUTPUTS = 2;

    //For a single dimension
    [System.Serializable]
    public struct DataInputs
    {
        public float ballPos;

        public float targetPos;

        public float angle;

        public float ballVelocity;

        public float distToEdge;

        public float[] Vector()
        {
            return new float[] { ballPos, targetPos, angle, ballVelocity, distToEdge};
        }

        public override string ToString()
        {
            string s = "Ball position: " + ballPos;
            s += "\nBall velocity : " + ballVelocity;
            s += "\nTarget position: " + targetPos;
            s += "\n\nPlane rotation (degrees): " + angle;
            s += "\nNearest edge: " + distToEdge;

            return s;
        }
    }

    [System.Serializable]
    public struct DataOutputs
    {
        public float outputX;
        public float outputY;
    }


    private void UpdateData()
    {
        dataX.ballPos = ballTransform.position.x;
        dataY.ballPos = ballTransform.position.z;

        if (useDistanceToTargetInsteadOfRawTargetPos)
        {
            dataX.targetPos = targetTransform.position.x- ballTransform.position.x;
            dataY.targetPos = targetTransform.position.z - ballTransform.position.z;
        } else
        {
            dataX.targetPos = targetTransform.position.x;
            dataY.targetPos = targetTransform.position.z;
        }

        dataX.ballVelocity = ballRB.velocity.x;
        dataY.ballVelocity = ballRB.velocity.z;

        dataX.angle = transform.rotation.eulerAngles.x > 180 ? transform.rotation.eulerAngles.x - 360 : transform.rotation.eulerAngles.x;
        dataY.angle = transform.rotation.eulerAngles.z > 180 ? transform.rotation.eulerAngles.z - 360 : transform.rotation.eulerAngles.z;

        Vector2 edgeDistances = GetEdgeDistances();
        dataX.distToEdge = edgeDistances.x;
        dataY.distToEdge = edgeDistances.y;

    }

    public float currentScore;
    public float levelTime;
    public float maxLevelCutoffTime = 300f; 

    public DataInputs dataX, dataY;
    [SerializeField]
    private DataOutputs outputs;

    public Transform ballTransform, targetTransform;
    public Rigidbody ballRB;
    private GroundControl groundControl;

    private PlayerController playerController;
    private RandomController randomController;
    private NeuralController neuralController;
    private IControllerBrain[] brains;

    static int NEURAL_HIDDEN = 15;

    private void Awake()
    {
        groundControl = gameObject.GetComponent<GroundControl>();
        ballRB = ballTransform.GetComponent<Rigidbody>();

        dataX = new DataInputs();
        dataY = new DataInputs();
        outputs = new DataOutputs();
        playerController = new PlayerController();
        randomController = new RandomController();
        
        neuralController = new NeuralController(dataX.Vector().Length, NEURAL_HIDDEN, NUM_OUTPUTS);

        brains = new IControllerBrain[] { playerController, randomController, neuralController};
        controllerBrain = brains[0];

        EndGame(false);
    }

    private void Start()
    {

        targetTransform.GetComponent<TargetObject>().BallHitTargetEvent += () => { TargetFound(); };
        gameObject.GetComponentInChildren<LevelBounds>().BallOutOfBoundsEvent += () => { EndGame(); };
        //StartGame();
    }

    private void Update()
    {
        if (playing)
        {
            levelTime += Time.deltaTime/Time.timeScale;
            if (levelTime > maxLevelCutoffTime)
            {
                Debug.LogWarning("MAX LEVEL CUTOFF TIME REACHED. Level time = " + levelTime + ". Cutoff = " + maxLevelCutoffTime);
                EndGame();
            }

        }
        currentScore = GetFitnessScore(); 

        if (Input.GetKeyDown(KeyCode.R))
        {
            EndGame();
            StartGame();
        }

        //HUMAN CONTROL
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            controllerBrain = brains[0];
        }

        //RANDOM CONTROL
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            controllerBrain = brains[1];
        }
        //Neural CONTROL
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            controllerBrain = brains[2];
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            neuralController.RandomiseNetowrk(dataX.Vector().Length, NEURAL_HIDDEN, NUM_OUTPUTS);
        }

        //Get the outputs

    }

    public void SetNeuralController( NeuralController neuralController )
    {
        this.neuralController = neuralController;
    }

    public int DataInputLength()
    {
        return dataX.Vector().Length;
    }

    public float numFoundFitnessScaler = 2;
    public float levelTimeFitnessScaler = 1f;
    public float minTimeToCountFound = 5f;
    public float GetFitnessScore()
    {
        float numFoundVal = 0;
        for (int i = 0; i < numFound; i++)
        {
            numFoundVal += i;
        }
        return numFoundVal * numFoundFitnessScaler + levelTime* levelTimeFitnessScaler;
    }

    int numFound = 0;
    void TargetFound()
    {
        if (!playing)
        {
            Debug.LogError("ERROR - WE ARE NOT IN **PLAYING** mode");
            return;
        }
        if (levelTime > minTimeToCountFound)
        {
            ++numFound;
        }
        targetTransform.localPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
    }

    public bool playing = false;
    public event System.Action<float> GameEndedWithFitness;
    void EndGame(bool throwEvent = true)
    {
        playing = false;
        ballRB.isKinematic = true;
        ballTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ballTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        if (!throwEvent)
            return;
        if (GameEndedWithFitness != null)
        {
            GameEndedWithFitness(GetFitnessScore());
        }
    }

    public void StartGame(IControllerBrain controllerBrain)
    {
        this.controllerBrain = controllerBrain;
        StartGame();
    }

    public int targetRange = 3;
    public void StartGame()
    {
        playing = true;
        numFound = 0;
        currentScore = 0;
        levelTime = 0;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        ballRB.isKinematic = false;
        ballTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ballTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        ballTransform.localPosition = new Vector3(0, 1, 0);
        targetTransform.localPosition = new Vector3(Random.Range(-targetRange, targetRange), 0, Random.Range(-targetRange, targetRange));
    }

    public DataOutputs GetOutput()
    {
        UpdateData();
        UpdateOutputs();

        return outputs;
    }

    public IControllerBrain controllerBrain;
    private void UpdateOutputs()
    {
        outputs = controllerBrain.GetOutputs(dataX, dataY);
    }

    public BoxCollider levelBounds;
    Vector2 GetEdgeDistances()
    {
        float edgeDistanceX = levelBounds.transform.localScale.x / 2 - 1;
        float edgeDistanceY = levelBounds.transform.localScale.z / 2 - 1;

        float nearX, nearY;

        if (ballTransform.localPosition.x >= 0)
            nearX = edgeDistanceX - ballTransform.localPosition.x;
        else
            nearX = edgeDistanceX + ballTransform.localPosition.x;

        if (ballTransform.localPosition.z >= 0)
            nearY = edgeDistanceY - ballTransform.localPosition.z;
        else
            nearY = edgeDistanceY + ballTransform.localPosition.z;

        return new Vector2(nearX, nearY);

    }

    public string GetDataString()
    {
        string s = "SCENE DATA: \n";
        s += "X axis: "+dataX;
        s += "\n\nY axis: " + dataY;
        s += "\n\nCONTROL OUTPUTS: \n";
        s += "X control: " + outputs.outputX;
        s += "\nY control: " + outputs.outputY;

        return s;
    }

}

