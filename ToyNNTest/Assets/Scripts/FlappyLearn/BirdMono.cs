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
        public int numStillRunning;

        public bool useHuman = true;
        PillarMono[] pillarGraphics;
        public Transform birdDemo;
        public PillarMono pillarMonoPrefab;


        float deltaTime;
        FlappyGame flappyGame;
        int flappyGameIndex;

        void Start()
        {
            if (useHuman)
            {
                flappyGame = new FlappyGame(new HumanController());
            } else
            {
                flappyGame = new FlappyGame(new RandomController(), true);
            }

            flappyGame.OnPillarOffScreenEvent += () =>
            {
                Debug.Log("PILLAR OFF SCREEN");
                ResetPillarArray();
            };

            gamesRunning = new List<FlappyGame>();
            gamesRunning.Add(flappyGame);

            flappyGame.OnGameEndedEvent += (float s) => { GameFinished(s); };
            flappyGameIndex = 0;

            for (int i = 1; i < numToRun; i++)
            {
                FlappyGame game = new FlappyGame(new RandomController());
                gamesRunning.Add(game);

                int index = i;
                game.OnGameEndedEvent += (float s) => 
                {
                    Debug.Log("Game index "+index+"finished with score of " + s);
                    GameFinished(s, index);
                };
            }
        }

        public void GameFinished(float score, int gameIndex = 0)
        {
            --numStillRunning;
            Debug.Log("SCORE FOR GAME INDEX " + gameIndex +" : " + score);
            if (score > maxScore)
                maxScore = score;


            if (numStillRunning <= 0)
            {
                gameStarted = false;
                Debug.Log("ALL GAMES FINISHED");
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

        void SetVisualsToGame(int index)
        {
            flappyGameIndex = index;
            flappyGame = gamesRunning[flappyGameIndex];
            //TODO: UPATE VISUALSResetPillarArray()
            ResetPillarArray();
        }

        void StartNewGame()
        {
            Debug.Log("GAME STARTING");

            numStillRunning = gamesRunning.Count;
            for (int i = 0; i < gamesRunning.Count; i++)
            {
                gamesRunning[i].StartNewGame();
            }

            //flappyGame.StartNewGame();
            
            gameStarted = true;

            Pillar[] pillars = flappyGame.GetPillars();

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
                    StartNewGame();
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
            birdDemo.position = new Vector3(0, flappyGame.BirdHeight(), 0);
        }

        void ResetPillarArray()
        {
            Pillar[] pillars = flappyGame.GetPillars();
            for (int i = 0; i < pillarGraphics.Length; i++)
            {
                pillarGraphics[i].ShowPillar(pillars[i]);
                pillarGraphics[i].UpdatePosition();
            }
        }

        void UpdatePillarMovement()
        {
            //Pillar[] pillars = flappyGame.GetPillars();

            for (int i = 0; i < pillarGraphics.Length; i++)
            {
                pillarGraphics[i].UpdatePosition();
            }


            ////foreach (Pillar p in pillars)
            ////{
            ////    p.MovePillar(movementPerSecond * deltaTime);
            ////}

            ////Pillar firstPillar = pillars.Peek();

            ////if (firstPillar.CentrePointX < pillarEndLoc)
            ////{
            ////    pillars.Dequeue();
            ////    PillarMono pillarMono = pillarGraphics.Dequeue();

            ////    Pillar p = new Pillar(pillarStartLoc, Random.Range(minGapMidpoint, maxGapMidpoint));
            ////    pillarMono.ShowPillar(p);

            ////    pillars.Enqueue(p);
            ////    pillarGraphics.Enqueue(pillarMono);
            ////}
        }


    }
}