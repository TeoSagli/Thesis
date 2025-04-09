using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    public abstract class QuizAnswer 
    {
        private int nAnswers;
        [SerializeField]
        private int correctAnswer;

        public abstract void SetUI(TextMeshProUGUI[] t, GameObject[] o);
        protected void SetOptionText(TextMeshProUGUI option, string text)
        {
            option.text = text;

        }
        public int NAnswers { get => nAnswers; set => nAnswers = value; }
        public int CorrectAnswer { get => correctAnswer; set => correctAnswer = value; }
    }
}
