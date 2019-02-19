using UnityEngine;
using System.Collections.Generic;

namespace FlappyLearn
{
    [System.Serializable]
    public class BirdManager
    {
        public static float gravity = -50f;
        public float baseDeltaTime = 0.01f;

        public List<Bird> birds;
        public Dictionary<Bird, IBirdController> birdControllers;

        float upwardsFlapForce;

        public BirdManager(Bird bird, float upwardsFlapForce)
        {
            this.birds = new List<Bird>();
            birds.Add(bird);
            this.upwardsFlapForce = upwardsFlapForce;
            birdControllers = new Dictionary<Bird, IBirdController>();
            birdControllers.Add(bird, new HumanController());
        }

        public BirdManager(Bird bird, float upwardsFlapForce, IBirdController controller)
        {
            this.birds = new List<Bird>();
            birds.Add(bird);
            this.upwardsFlapForce = upwardsFlapForce;
            birdControllers = new Dictionary<Bird, IBirdController>();
            birdControllers.Add(bird, controller);
        }

        public BirdManager(List<Bird> birds, float upwardsFlapForce)
        {
            this.birds = birds;
            this.upwardsFlapForce = upwardsFlapForce;
            birdControllers = new Dictionary<Bird, IBirdController>();

            for (int i = 0; i < birds.Count; i++)
            {
                IBirdController controller = i == 0 ? (IBirdController)new HumanController() : (IBirdController)new RandomController();
                birdControllers.Add(birds[i], controller);
            }
        }

        public BirdManager(int birdCount, float startHeightMin, float startHeightMax, float upwardsForce)
        {
            this.birds = new List<Bird>();
            for (int i = 0; i < birdCount; i++)
            {
                float startHeight = Random.Range(startHeightMin, startHeightMax);
                birds.Add(new Bird(startHeight));
            }

            this.upwardsFlapForce = upwardsForce;

            birdControllers = new Dictionary<Bird, IBirdController>();

            for (int i = 0; i < birds.Count; i++)
            {
                IBirdController controller = i == 0 ? (IBirdController)new HumanController() : (IBirdController)new RandomController();
                birdControllers.Add(birds[i], controller);
            }
        }

        public Bird GetBird(int index)
        {
            return birds[index];
        }

        public void Flap(int index, float deltaTime)
        {
            birds[index].ApplyForceOverTime(upwardsFlapForce, deltaTime);
        }

        public void Flap(int index)
        {
            Flap(index, baseDeltaTime);
        }

        public void Tick(Pillar[] pillars)
        {
            Tick(baseDeltaTime, pillars);
        }

        IBirdController ControllerFromBird(Bird bird)
        {
            return birdControllers[bird];
        }


        FlappyGameState GetGameStateForBird(int index)
        {
            FlappyGameState gameState = new FlappyGameState();
            gameState.distToPipe = distToNearestPillar;
            gameState.heightAboveMidPoint = birds[index].Height - nearestPillar.CentrePointGap;

            return gameState;
        }

        float distToNearestPillar;
        Pillar nearestPillar;
        public void Tick(float deltaTime, Pillar[] pillars)
        {
            nearestPillar = (pillars[0].CentrePointX + Pillar.halfWidth) > 0 ? pillars[0] : pillars[1];
            distToNearestPillar = Mathf.Max(nearestPillar.CentrePointX - Pillar.halfWidth, float.Epsilon);

            for (int i = 0; i < birds.Count; i++)
            {
                FlappyGameState state = GetGameStateForBird(i);
                birds[i].ApplyForceOverTime(gravity, deltaTime);
                IBirdController controller = ControllerFromBird(birds[i]);

                if (controller.DoFlapFromState(state))
                    Flap(i);

                if (birds[i].Height < 0)
                {
                    birds[i].Die();
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < birds.Count; i++)
            {
                birds[i].Reset(5);
            }
        }

        public bool BirdCollidesWithPoint(Bird bird, float height, float xPos)
        {
            //Is the point in range based 
            if (height > bird.Height + Bird.BirdRadius || height < bird.Height - Bird.BirdRadius)
            {
                //We do not collide with this point
                return false;
            }

            if (xPos > Bird.BirdRadius || xPos < -Bird.BirdRadius)
            {
                //We do not collide with this point
                return false;
            }

            return true;
        }

}

}
