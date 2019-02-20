using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyLearn
{

    public class FlappyUI : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI currentBirdText, remainingThisGenText, currentGenerationText, currentScoreText, highScoreText, highScoreGenomeText;
        public GameObject generationsCompleteGO;

        public BirdMono gameController;
        string numberFormat = "F2";

        int generation, highScoreGen;
        private void Start()
        {
            generationsCompleteGO.SetActive(false);
            UpdateHighScore(0);
            UpdateScore(0);
            UpdateCurrentBird(0);
            UpdateAlive(0);
            NewGeneration(0);

            gameController.NewGenerationStartedEvent += NewGeneration;
            gameController.NewVisualisedBirdEvent += UpdateCurrentBird;
            gameController.NewMaxScoreEvent += UpdateHighScore;
            gameController.AllGenerationsCompleteEvent += () => { generationsCompleteGO.SetActive(true); };
            gameController.NewMaxScoreGenome += UpdatehighScoreGenome;
        }

        private void Update()
        {
            if (gameController.FlappyGame.GameRunning)
            {
                UpdateScore(gameController.FlappyGame.GameTime);
                UpdateAlive(gameController.NumStillRunning);
            }
        }

        void UpdateHighScore(float newScore)
        {
            highScoreGen = generation;
            highScoreText.text = "Current high score : " + newScore.ToString(numberFormat) + "\n(Generation " + highScoreGen + ")";
        }

        void UpdatehighScoreGenome(float[] highScoreGenome)
        {
            string s = "Current high score genome: \n";
            s += "[" + highScoreGenome[0];
            for (int i = 1; i < highScoreGenome.Length; i++)
            {
                s += ",\n" + highScoreGenome[i];
            }
            s += "]";

            highScoreGenomeText.text = s;

        }

        void UpdateScore(float score)
        {
            currentScoreText.text = "SCORE : " + score.ToString(numberFormat);
        }

        void UpdateAlive(int alive)
        {
            remainingThisGenText.text = "Remaining this generation : " + alive;
        }

        void UpdateCurrentBird(int birdID)
        {
            currentBirdText.text = "Current Bird ID: " + birdID.ToString();
        }

        void NewGeneration(int generation)
        {
            UpdateCurrentBird(0);
            this.generation = generation;
            currentGenerationText.text = "Current generation : " + generation.ToString();
        }

    }
}