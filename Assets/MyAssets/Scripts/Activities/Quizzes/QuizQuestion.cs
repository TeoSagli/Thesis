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
    public abstract class QuizQuestion
    {
        private QuizDataQuestion quizDataQuestion;
        private TextMeshProUGUI textTitle;
        
        private QuizQuestionType questionType;

        public QuizQuestion(QuizDataQuestion quizDataQuestion, TextMeshProUGUI textTitle)
        {
            this.quizDataQuestion = quizDataQuestion;
            this.textTitle = textTitle;
        }

        public abstract void Init();
        public TextMeshProUGUI TextTitle { get => textTitle; set => textTitle = value; }
        public QuizDataQuestion QuizDataQuestion { get => quizDataQuestion; set => quizDataQuestion = value; }
        public QuizQuestionType QuestionType { get => questionType; set => questionType = value; }
    }
}
