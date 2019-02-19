using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyLearn
{

    public class BirdMono : MonoBehaviour
    {
        public float startHeight = 5f;
        public float flapForce = 5f;
        public BirdManager birdController;

        Pillar pillar;
        Queue<Pillar> pillars;
        Queue<PillarMono> pillarGraphics;

        public float pillarStartLoc = 40;
        public float pillarEndLoc = -10;
        public int numPillars = 5;

        public float minGapMidpoint = 3;
        public float maxGapMidpoint = 17;

        float deltaTime;
        void Start()
        {
            birdController = new BirdManager(new Bird(startHeight), flapForce, new RandomController());

            pillars = new Queue<Pillar>();
            pillarGraphics = new Queue<PillarMono>();

            float pillarSpacing = (pillarStartLoc - pillarEndLoc) / numPillars;
            for (int i = 0; i < numPillars; i++)
            {
                float xPos = pillarSpacing * i + pillarStartLoc;
                Pillar p = new Pillar(xPos, Random.Range(minGapMidpoint, maxGapMidpoint));
                pillars.Enqueue(p);

                PillarMono pillarShow = (PillarMono)Instantiate(pillarMonoPrefab);
                pillarGraphics.Enqueue(pillarShow);
                pillarShow.ShowPillar(p);
            }

            
        }

        float gameTime;
        void StartGame()
        {
            gameStarted = true;
            gameTime = 0;
        }

        public Transform birdDemo;
        public PillarMono pillarMonoPrefab;

        bool gameStarted = false;
        void Update()
        {
            if (!gameStarted)
                return;
            deltaTime = Time.deltaTime;
            UpdateBirdMovement();
            UpdatePillarMovement();
            CheckDeath();

            if (Input.GetKeyDown(KeyCode.R))
            {
                birdController.Reset();
            }
        }


        void UpdateBirdMovement()
        {
            birdController.Tick(deltaTime, pillars.ToArray());
            birdDemo.position = new Vector3(0, birdController.GetBird(0).Height, 0);
        }

        float movementPerSecond = 5;
        void UpdatePillarMovement()
        {
            foreach (Pillar p in pillars)
            {
                p.MovePillar(movementPerSecond * deltaTime);
            }

            Pillar firstPillar = pillars.Peek();

            if (firstPillar.CentrePointX < pillarEndLoc)
            {
                pillars.Dequeue();
                PillarMono pillarMono = pillarGraphics.Dequeue();

                Pillar p = new Pillar(pillarStartLoc, Random.Range(minGapMidpoint, maxGapMidpoint));
                pillarMono.ShowPillar(p);

                pillars.Enqueue(p);
                pillarGraphics.Enqueue(pillarMono);
            }


        }


        void CheckDeath()
        {
            foreach (Bird bird in birdController.birds)
            {
                foreach (var pillar in pillars)
                {
                    if (pillar.HitBird
                        (bird))
                    {
                        bird.Die();
                        break;
                    }
                }
            }
        }


    }
}