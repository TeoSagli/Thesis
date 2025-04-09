using UnityEngine;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public class QuizTwoFeature : QuizAnswer
    {
       [SerializeField] private string answerOne;
       [SerializeField] private string answerTwo;

        public QuizTwoFeature()
        {
            NAnswers = 2;
        }

        public string AnswerOne { get => answerOne; set => answerOne = value; }
        public string AnswerTwo { get => answerTwo; set => answerTwo = value; }
    }
}
