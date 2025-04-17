using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static Enums;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public class QuizObjectAnswer : QuizAnswer
    {
        public QuizObjectAnswer(GameObject objectAnswer, string rightObjectTag, string answerType) :base(answerType)
        {
            NAnswers = 0;
            QuizDataAnswer = new(objectAnswer, rightObjectTag, answerType);
        }

        public override void SetUI(TextMeshProUGUI[] t, GameObject[] o)
        {
            
        }
    }
}
