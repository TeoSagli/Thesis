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
    [System.Serializable]
    public class QuizTextQuestion  : QuizQuestion
    {
        private TextMeshProUGUI textQuestion;
         
        public TextMeshProUGUI TextQuestion { get => textQuestion; set => textQuestion = value; }

        public QuizTextQuestion(QuizDataQuestion quizDataQuestion, TextMeshProUGUI textTitle,TextMeshProUGUI textQuestion): base(quizDataQuestion, textTitle)
        {
            this.textQuestion = textQuestion;
            QuestionType = QuizQuestionType.Text;
        }

        public override void Init()
        {
            textQuestion.text = QuizDataQuestion.TextToShow;
        }
    }
}
