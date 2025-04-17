using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using static Enums;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public class QuizThreeAnswers : QuizAnswer
    {
        public QuizThreeAnswers(string answerOne, string answerTwo, string answerThree, int correctAnswer, string answerType): base(answerType)
        {
            NAnswers = 3;
            QuizDataAnswer = new(answerOne, answerTwo, answerThree, correctAnswer, answerType);
        }
        public override void SetUI(TextMeshProUGUI[] t, GameObject[] o)
        {
            for (int i = 0; i < NAnswers; i++)
                t[i] = o[i].GetNamedChild("Text").GetComponent<TextMeshProUGUI>();
            SetOptionText(t[0], QuizDataAnswer.AnswerOne);
            SetOptionText(t[1], QuizDataAnswer.AnswerTwo);
            SetOptionText(t[2], QuizDataAnswer.AnswerThree);
        }
    }
}
