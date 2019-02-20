using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyLearn
{

    public class BirdMono : MonoBehaviour
    {
        public float maxScore = 0;

        public int numToRun = 5;
        List<FlappyGame> gamesRunning;
        int numStillRunning;

        public bool useHuman = true;
        PillarMono[] pillarGraphics;
        public Transform birdDemo;
        public PillarMono pillarMonoPrefab;

        public FlappyGame FlappyGame { get => flappyGame; protected set => flappyGame = value; }
        public int NumStillRunning { get => numStillRunning; private set => numStillRunning = value; }

        float deltaTime;
        FlappyGame flappyGame;
        int flappyGameIndex;

        void Start()
        {
            if (useHuman)
            {
                FlappyGame = new FlappyGame(new HumanController());
            } else
            {
                FlappyGame = new FlappyGame(new RandomController(), true);
            }

            FlappyGame.OnPillarOffScreenEvent += () =>
            {
                Debug.Log("PILLAR OFF SCREEN");
                ResetPillarArray();
            };

            gamesRunning = new List<FlappyGame>();
            gamesRunning.Add(FlappyGame);

            FlappyGame.OnGameEndedEvent += (float s) => { GameFinished(s); };
            flappyGameIndex = 0;

            for (int i = 1; i < numToRun; i++)
            {
                FlappyGame game = new FlappyGame(new RandomController());
                gamesRunning.Add(game);

                int index = i;
                game.OnGameEndedEvent += (float s) => 
                {
                    //Debug.Log("Game index "+index+"finished with score of " + s);
                    GameFinished(s, index);
                };
            }
        }

        public event System.Action<float> NewMaxScoreEvent;
        public event System.Action<int> NewVisualisedBirdEvent;

        public event System.Action<int> NewGenerationStartedEvent;
        public void GameFinished(float score, int gameIndex = 0)
        {
            --NumStillRunning;

            if (score > maxScore)
            {
                maxScore = score;
                NewMaxScoreEvent?.Invoke(score);
            }


            if (NumStillRunning <= 0)
            {
                AllGamesFinished();
                return;
            }

            if (gameIndex == flappyGameIndex)
            {
                //UPDATE VISUALS
                for (int i = 0; i < gamesRunning.Count; i++)
                {
                    if (gamesRunning[i].GameRunning)
                    {
                        SetVisualsToGame(i);
                        return;
                    }
                }
            }
        }

        public event System.Action AllGenerationsCompleteEvent;
        public int totalGenerationsToRun = 10;
        void AllGamesFinished()
        {
            gameStarted = false;
            Debug.Log("ALL GAMES FINISHED");
            if (currentGen < totalGenerationsToRun)
            {
                NewGeneration();
            }
            else
            {
                Debug.Log("ALL GENERATIONS FINISHED");
                AllGenerationsCompleteEvent?.Invoke();
            }
        }

        void SetVisualsToGame(int index)
        {
            flappyGameIndex = index;
            FlappyGame = gamesRunning[flappyGameIndex];
            NewVisualisedBirdEvent?.Invoke(index);

            ResetPillarArray();
        }

        int currentGen = -1;
        void NewGeneration()
        {
            NewGenerationStartedEvent?.Invoke(++currentGen);
            //TODO: ADD IN WHATEVER NEEDED HERE FOR NN/GA STUFF

            StartNewGame();
        }

        void StartNewGame()
        {
            Debug.Log("GAME STARTING");

            NumStillRunning = gamesRunning.Count;
            for (int i = 0; i < gamesRunning.Count; i++)
            {
                gamesRunning[i].StartNewGame();
            }
            gameStarted = true;

            Pillar[] pillars = FlappyGame.GetPillars();

            if (pillarGraphics == null || pillarGraphics.Length != pillars.Length)
            {
                //TODO: DELETE ALL GRAPHICS
                pillarGraphics = new PillarMono[pillars.Length];
                for (int i = 0; i < pillars.Length; i++)
                {
                    pillarGraphics[i] = (PillarMono)Instantiate(pillarMonoPrefab);
                    pillarGraphics[i].ShowPillar(pillars[i]);
                }
            } else
            {
                for (int i = 0; i < pillars.Length; i++)
                {
                    pillarGraphics[i].ShowPillar(pillars[i]);
                }
            }
        }


        [Range(0f, 10f)]
        public int ticksPerFrame = 1;
        bool gameStarted = false;


        void Update()
        {
            if (!gameStarted)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    NewGeneration();
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Application.LoadLevel(Application.loadedLevel);
                }
                return;
            }

            for (int i = 0; i < ticksPerFrame; i++)
            {
                GameTick(Time.deltaTime);
            }
        }

        void GameTick(float deltaTime)
        {

            for (int i = 0; i < numToRun; i++)
            {
                gamesRunning[i].Tick(deltaTime);
            }

            UpdateBirdMovement();
            UpdatePillarMovement();
        }

        void UpdateBirdMovement()
        {
            birdDemo.position = new Vector3(0, FlappyGame.BirdHeight(), 0);
        }

        void ResetPillarArray()
        {
            Pillar[] pillars = FlappyGame.GetPillars();
            for (int i = 0; i < pillarGraphics.Length; i++)
            {
                pillarGraphics[i].ShowPillar(pillars[i]);
                pillarGraphics[i].UpdatePosition();
            }
        }

        void UpdatePillarMovement()
        {
            for (int i = 0; i < pillarGraphics.Length; i++)
            {
                pillarGraphics[i].UpdatePosition();
            }
        }


    }
}