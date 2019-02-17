using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyLearn
{

    public class Pillar
    {
        public static readonly float halfWidth = 1.5f;
        public static readonly float gapHalfHeight = 3f;

        float centrePointX;
        float centrePointGap;

        public float CentrePointX { get => centrePointX; private set => centrePointX = value; }
        public float CentrePointGap { get => centrePointGap; private set => centrePointGap = value; }

        public Pillar(float centrePointX, float centrePointGap)
        {
            this.centrePointX = centrePointX;
            this.centrePointGap = centrePointGap;
        }

        public void MovePillar(float moveDistance)
        {
            centrePointX -= moveDistance;
        }

        public bool HitBird(Bird bird)
        {
            if (Bird.BirdRadius < centrePointX - halfWidth)
                return false;
            if (Bird.BirdRadius > centrePointX + halfWidth)
                return false;

            //At this point, the bird is 'inside' the pillar X

            if (bird.Height - Bird.BirdRadius < centrePointGap - gapHalfHeight)
                return true;

            if (Bird.BirdRadius + bird.Height > centrePointGap + gapHalfHeight)
                return true;

            return false;
        }
    }
}