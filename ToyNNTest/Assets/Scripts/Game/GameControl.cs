using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public bool useDistanceToTargetInsteadOfRawTargetPos = false;
    public static readonly int NUM_OUTPUTS = 2;

    [System.Serializable]
    public struct DataInputs
    {
        public float ballPosX;
        public float ballPosY;

        public float targetPosX;
        public float targetPosY;

        public float angleX;
        public float angleY;

        public float ballVelocityX;
        public float ballVelocityY;

        public float distToEdgeX;
        public float distToEdgeY;

        //public float rotationSpeed;

        public float[] Vector()
        {
            return new float[] { ballPosX, ballPosY, targetPosX, targetPosY, angleX, angleY, ballVelocityX, ballVelocityY, distToEdgeX, distToEdgeY/*, rotationSpeed*/ };
        }

        public override string ToString()
        {
            string s = "Ball position: " + ballPosX + ", " + ballPosY;
            s += "\nBall velocity : " + ballVelocityX + ", " + ballVelocityY;
            s += "\nTarget position: " + targetPosX + ", " + targetPosY;
            s += "\n\nPlane rotation (degrees): " + angleX + ", " + angleY;
            //s += "\nPlane rotation speed: " + rotationSpeed;
            s += "\nHorizontal nearest edge: " + distToEdgeX;
            s += "\nVertical nearest edge: " + distToEdgeY;

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
        data.ballPosX = ballTransform.position.x;
        data.ballPosY = ballTransform.position.z;

        if (useDistanceToTargetInsteadOfRawTargetPos)
        {
            data.targetPosX = targetTransform.position.x- ballTransform.position.x;
            data.targetPosY = targetTransform.position.z - ballTransform.position.z;
        } else
        {
            data.targetPosX = targetTransform.position.x;
            data.targetPosY = targetTransform.position.z;
        }

        data.ballVelocityX = ballRB.velocity.x;
        data.ballVelocityY = ballRB.velocity.z;

        data.angleX = transform.rotation.eulerAngles.x > 180 ? transform.rotation.eulerAngles.x - 360 : transform.rotation.eulerAngles.x;
        data.angleY = transform.rotation.eulerAngles.z > 180 ? transform.rotation.eulerAngles.z - 360 : transform.rotation.eulerAngles.z;

        Vector2 edgeDistances = GetEdgeDistances();
        data.distToEdgeX = edgeDistances.x;
        data.distToEdgeY = edgeDistances.y;

        //data.rotationSpeed = groundControl.rotationSpeed;
    }

    public float currentScore;
    public float levelTime;
    public float maxLevelCutoffTime = 300f; 

    public DataInputs data;
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

        data = new DataInputs();
        outputs = new DataOutputs();
        playerController = new PlayerController();
        randomController = new RandomController();
        
        neuralController = new NeuralController(data.Vector().Length, NEURAL_HIDDEN, NUM_OUTPUTS);

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
            neuralController.RandomiseNetowrk(data.Vector().Length, NEURAL_HIDDEN, NUM_OUTPUTS);
        }

        //Get the outputs

    }

    public void SetNeuralController( NeuralController neuralController )
    {
        this.neuralController = neuralController;
    }

    public int DataInputLength()
    {
        return data.Vector().Length;
    }

    public float numFoundFitnessScaler = 2;
    public float levelTimeFitnessScaler = 1f;
    public float minTimeToCountFound = 1.5f;
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
        outputs = controllerBrain.GetOutputs(data);
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
        s += data;
        s += "\n\nCONTROL OUTPUTS: \n";
        s += "X control: " + outputs.outputX;
        s += "\nY control: " + outputs.outputY;

        return s;
    }

}

