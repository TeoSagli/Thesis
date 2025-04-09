using TMPro;
using Unity.XR.CoreUtils;
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
        public override void SetUI(TextMeshProUGUI[] t, GameObject[] o)
        {
            for (int i = 0; i < NAnswers; i++)
                t[i] = o[i].GetNamedChild("Text").GetComponent<TextMeshProUGUI>();
            SetOptionText(t[0], answerOne);
            SetOptionText(t[1], answerTwo);
            SetOptionText(t[2], answerThree);
        }
    }
}
