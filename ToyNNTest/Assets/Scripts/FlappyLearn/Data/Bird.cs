namespace FlappyLearn
{
    [System.Serializable]
    public class Bird
    {
        public static float BirdRadius = 1;
        public static float MaxVelocity = 15;
        [UnityEngine.SerializeField] float height;
        [UnityEngine.SerializeField] public float velocity;

        bool alive = true;

        public float Height { get => height; private set => height = value; }


        public Bird(float height)
        {
            this.height = height;
            velocity = 0;
        }


        public void ApplyForceOverTime(float force, float deltaTime)
        {
            if (!alive)
                return;

            velocity += force * deltaTime;
            if (velocity > MaxVelocity) velocity = MaxVelocity;
            if (velocity < -MaxVelocity) velocity = -MaxVelocity;

            height += velocity * deltaTime;
        }

        public void Die()
        {
            alive = false;
        }

        public void Reset(float newHeight)
        {
            this.height = newHeight;
            this.velocity = 0;
            alive = true;
        }
    }
}
