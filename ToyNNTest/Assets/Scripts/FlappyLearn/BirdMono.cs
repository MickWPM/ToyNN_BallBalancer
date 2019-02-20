using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyLearn
{

    public class BirdMono : MonoBehaviour
    {
        public bool useHuman = true;
        PillarMono[] pillarGraphics;
        public Transform birdDemo;
        public PillarMono pillarMonoPrefab;


        float deltaTime;
        FlappyGame flappyGame;

        void Start()
        {
            if (useHuman)
            {
                flappyGame = new FlappyGame(new HumanController());
            } else
            {
                flappyGame = new FlappyGame(new RandomController(), true);
            }

            flappyGame.OnGameEndedEvent += GameFinished;
            flappyGame.OnPillarOffScreenEvent += () =>
            {
                Debug.Log("PILLAR OFF SCREEN");
                ResetPillarArray();
            };
        }

        public void GameFinished(float score)
        {
            gameStarted = false;
            Debug.Log("SCORE: " + score);
        }

        void StartNewGame()
        {
            Debug.Log("GAME STARTING");
            flappyGame.StartNewGame();
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
            flappyGame.Tick(Time.deltaTime);
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