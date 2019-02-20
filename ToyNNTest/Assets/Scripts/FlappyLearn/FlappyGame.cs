using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyLearn
{

    public class FlappyGame
    {

        public static float gravity = -50f;
        public static float baseDeltaTime = 0.01f;
        public static float birdStartHeight = 10;
        public static float flapForce = 3000;

        public static float pillarStartLoc = 40;
        public static float pillarEndLoc = -10;
        public static float numPillars = 5;
        public static float minPillarGapMidpoint = 3;
        public static float maxPillarGapMidpoint = 17;


        Pillar[] pillarsArray;
        Queue<Pillar> pillars;
        float gameTime;
        Bird bird;
        IBirdController controller;
        bool gameRunning = false;

        public event System.Action<float> OnGameEndedEvent;

        public FlappyGame(IBirdController controller, bool autostart = false)
        {
            bird = new Bird(birdStartHeight);
            this.controller = controller;

            if (autostart)
            {
                StartNewGame();
            }
        }

        public void StartNewGame()
        {
            gameTime = 0;
            bird = new Bird( birdStartHeight );
            pillars = new Queue<Pillar>();
            float pillarSpacing = (pillarStartLoc - pillarEndLoc) / numPillars;
            for (int i = 0; i < numPillars; i++)
            {
                float xPos = pillarSpacing * i + pillarStartLoc;
                Pillar p = new Pillar(xPos, Random.Range(minPillarGapMidpoint, maxPillarGapMidpoint));
                pillars.Enqueue(p);
            }
            gameRunning = true;
        }

        public float BirdHeight()
        {
            return bird.Height;
        }

        public Pillar[] GetPillars()
        {
            return pillarsArray;
        }

        //Returns true if game is still running
        float deltaTime;
        float movementPerSecond = 5;
        public bool Tick(float deltaTime)
        {
            if (gameRunning == false)
                return false;

            this.deltaTime = deltaTime;
            gameTime += movementPerSecond * deltaTime;

            UpdatePillarMovement();
            pillarsArray = pillars.ToArray();

            UpdateBirdMovement();

            return !CheckDeath();
        }

        public bool Tick()
        {
            return Tick(baseDeltaTime);
        }

        FlappyGameState GetGameStateForBird()
        {
            FlappyGameState gameState = new FlappyGameState();
            gameState.distToPipe = distToNearestPillar;
            gameState.heightAboveMidPoint = bird.Height - nearestPillar.CentrePointGap;

            return gameState;
        }


        float distToNearestPillar;
        Pillar nearestPillar;
        public void UpdateBirdMovement()
        {
            nearestPillar = (pillarsArray[0].CentrePointX + Pillar.halfWidth) > 0 ? pillarsArray[0] : pillarsArray[1];
            distToNearestPillar = Mathf.Max(nearestPillar.CentrePointX - Pillar.halfWidth, float.Epsilon);


            bird.ApplyForceOverTime(gravity, deltaTime);

            if (controller.DoFlapFromState(GetGameStateForBird()))
            {
                bird.ApplyForceOverTime(flapForce, deltaTime);
            }

        }


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
                Pillar p = new Pillar(pillarStartLoc, Random.Range(minPillarGapMidpoint, maxPillarGapMidpoint));
                pillars.Enqueue(p);
            }
        }


        bool CheckDeath()
        {

            if (bird.Height < 0)
            {
                BirdDie();
                return true;
            }

            foreach (var pillar in pillars)
            {
                if (pillar.HitBird(bird))
                {
                    BirdDie();
                    return true;
                }
            }
            return false;
        }

        void BirdDie()
        {
            bird.Die();
            OnGameEndedEvent?.Invoke(gameTime);
        }

    }

}