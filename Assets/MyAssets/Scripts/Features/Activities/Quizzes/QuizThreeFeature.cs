using UnityEngine;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public class QuizThreeFeature : QuizAnswer
    {
        [SerializeField] private string answerOne;
        [SerializeField] private string answerTwo;
        [SerializeField] private string answerThree;

        public QuizThreeFeature()
        {
            NAnswers = 3;
        }

        public string AnswerOne { get => answerOne; set => answerOne = value; }
        public string AnswerTwo { get => answerTwo; set => answerTwo = value; }
        public string AnswerThree { get => answerThree; set => answerThree = value; }
    }
}
