using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static Enums;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    public abstract class QuizAnswer 
    {
        private int nAnswers;
        private QuizAnswersType quizAnswerType;
        private QuizDataAnswer quizDataAnswer;

        protected QuizAnswer(string quizAnswerType)
        {
            QuizAnswerType = GetQuizAnswerType(quizAnswerType);
        }

        public abstract void SetUI(TextMeshProUGUI[] t, GameObject[] o);
        protected void SetOptionText(TextMeshProUGUI option, string text)
        {
            option.text = text;
        }
        private QuizAnswersType GetQuizAnswerType(string type)
        {
            return  (QuizAnswersType)System.Enum.Parse(typeof(QuizAnswersType), type);
        }
        public int NAnswers { get => nAnswers; set => nAnswers = value; }
        public QuizDataAnswer QuizDataAnswer { get => quizDataAnswer; set => quizDataAnswer = value; }
        public QuizAnswersType QuizAnswerType { get => quizAnswerType; set => quizAnswerType = value; }
    }
}
