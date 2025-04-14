using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using static Enums;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public class QuizTwoAnswers : QuizAnswer
    {
 
        public QuizTwoAnswers(string answerOne, string answerTwo, int correctAnswer)
        {
            NAnswers = 2;
            QuizDataAnswer = new (answerOne, answerTwo, correctAnswer);
            QuizAnswerType = QuizAnswersType.TwoAnswers;
        }

        public override void SetUI(TextMeshProUGUI[] t, GameObject[] o)
        {
            for (int i = 0; i < NAnswers; i++)
                t[i] = o[i].GetNamedChild("Text").GetComponent<TextMeshProUGUI>();
            SetOptionText(t[0], QuizDataAnswer.AnswerOne);
            SetOptionText(t[1], QuizDataAnswer.AnswerTwo);
        }
    }
}
