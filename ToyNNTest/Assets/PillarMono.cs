using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyLearn
{
    public class PillarMono : MonoBehaviour
    {
        public Transform pillarParent, pillarUp, pillarDown;
        Pillar pillar;

        public void ShowPillar(Pillar pillar)
        {
            this.pillar = pillar;
            pillarUp.localScale = new Vector3(Pillar.halfWidth, 1, 1);
            pillarDown.localScale = new Vector3(Pillar.halfWidth, 1, 1);

            
            pillarUp.localPosition = new Vector3(0, pillar.CentrePointGap + Pillar.gapHalfHeight, 0);
            pillarDown.localPosition = new Vector3(0, pillar.CentrePointGap - Pillar.gapHalfHeight, 0);
        }

        private void Update()
        {
            UpdatePosition();
        }

        void UpdatePosition()
        {
            pillarParent.position = new Vector3(pillar.CentrePointX,0,0);
        }

    }
}