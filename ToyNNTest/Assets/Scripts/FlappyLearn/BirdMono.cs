using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyLearn
{

    public class BirdMono : MonoBehaviour
    {
        public bool useHuman = true;
        Queue<PillarMono> pillarGraphics;

        float deltaTime;
        FlappyGame flappyGame;

        void Start()
        {
            if (useHuman)
            {
                flappyGame = new FlappyGame(new HumanController());
            } else
            {
                flappyGame = new FlappyGame(new RandomController());
            }

            flappyGame.OnGameEndedEvent += GameFinished;
        }

        public void GameFinished(float score)
        {
            gameStarted = false;
            Debug.Log("SCORE: " + score);
        }

        public Transform birdDemo;
        public PillarMono pillarMonoPrefab;

        bool gameStarted = false;
        void Update()
        {
            if (!gameStarted)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("GAME STARTING");
                    flappyGame.StartNewGame();
                    gameStarted = true;
                }
                return;
            }


            flappyGame.Tick(Time.deltaTime);
            UpdateBirdMovement();
            UpdatePillarMovement();
        }


        void UpdateBirdMovement()
        {
            birdDemo.position = new Vector3(0, flappyGame.BirdHeight(), 0);
        }

        void UpdatePillarMovement()
        {
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