using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    public abstract class QuizAnswer 
    {
        private int nAnswers;
        [SerializeField]
        private int correctAnswer;
        public int NAnswers { get => nAnswers; set => nAnswers = value; }
        public int CorrectAnswer { get => correctAnswer; set => correctAnswer = value; }
    }
}
