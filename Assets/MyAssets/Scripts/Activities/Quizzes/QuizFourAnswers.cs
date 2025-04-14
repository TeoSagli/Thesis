using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using static Enums;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public  class QuizFourAnswers : QuizAnswer
    {
        public QuizFourAnswers(string answerOne, string answerTwo, string answerThree, string answerFour, int correctAnswer)
        {
            NAnswers = 4;
            QuizAnswerType=QuizAnswersType.FourAnswers;
            QuizDataAnswer = new(answerOne, answerTwo, answerThree, answerFour, correctAnswer);
        }

        public override void SetUI(TextMeshProUGUI[] t, GameObject[] o)
        {
            for(int i=0;i<NAnswers;i++)
                 t[i] = o[i].GetNamedChild("Text").GetComponent<TextMeshProUGUI>();
            SetOptionText(t[0], QuizDataAnswer.AnswerOne);
            SetOptionText(t[1], QuizDataAnswer.AnswerTwo);
            SetOptionText(t[2], QuizDataAnswer.AnswerThree);
            SetOptionText(t[3], QuizDataAnswer.AnswerFour);
        }
    }
}
