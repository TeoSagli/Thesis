using TMPro;
using Unity.XR.CoreUtils;
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
        public override void SetUI(TextMeshProUGUI[] t, GameObject[] o)
        {
            for (int i = 0; i < NAnswers; i++)
                t[i] = o[i].GetNamedChild("Text").GetComponent<TextMeshProUGUI>();
            SetOptionText(t[0], answerOne);
            SetOptionText(t[1], answerTwo);
        }
    }
}
