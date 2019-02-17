using UnityEngine;
using System.Collections.Generic;

namespace FlappyLearn
{
    [System.Serializable]
    public class BirdController
    {
        public static float gravity = -50f;
        public float baseDeltaTime = 0.01f;

        public List <Bird> birds;
        float upwardsFlapForce;

        public BirdController(Bird bird, float upwardsFlapForce)
        {
            this.birds = new List<Bird>();
            birds.Add(bird);
            this.upwardsFlapForce = upwardsFlapForce;
        }

        public BirdController(List<Bird> birds, float upwardsFlapForce)
        {
            this.birds = birds;
            this.upwardsFlapForce = upwardsFlapForce;
        }

        public BirdController(int birdCount, float startHeightMin, float startHeightMax, float upwardsForce)
        {
            this.birds = new List<Bird>();
            for (int i = 0; i < birdCount; i++)
            {
                float startHeight = Random.Range(startHeightMin, startHeightMax);
                birds.Add(new Bird(startHeight));
            }

            this.upwardsFlapForce = upwardsForce;
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

        public void Tick()
        {
            Tick(baseDeltaTime);
        }

        public void Tick(float deltaTime)
        {
            for (int i = 0; i < birds.Count; i++)
            {
                birds[i].ApplyForceOverTime(gravity, deltaTime);

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
