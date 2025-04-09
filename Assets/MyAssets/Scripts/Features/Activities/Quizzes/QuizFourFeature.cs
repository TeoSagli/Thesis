using UnityEngine;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public  class QuizFourFeature : QuizAnswer
    {
        [SerializeField] private string answerOne;
        [SerializeField] private string answerTwo;
        [SerializeField] private string answerThree;
        [SerializeField] private string answerFour;

        protected QuizFourFeature()
        {
            NAnswers = 4;
        }

        public string AnswerOne { get => answerOne; set => answerOne = value; }
        public string AnswerTwo { get => answerTwo; set => answerTwo = value; }
        public string AnswerThree { get => answerThree; set => answerThree = value; }
        public string AnswerFour { get => answerFour; set => answerFour = value; }

    }
}
