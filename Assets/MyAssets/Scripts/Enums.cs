using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum FeatureUsage
    {
        Once,
        Toggle
    }
    public enum FeatureDirection
    {
        Forward,
        Backward
    }
    public enum GameState
    {
        Playing,
        Paused,
        PuzzleSolved,
        ShowProgress
    }
    public enum QuizQuestionType
    {
        Text,
        Video,
        Image,

    }
    public enum QuizAnswersType
    {
        Two,
        Three,
        Four,
        Object,
        Text
    }
    public enum SelectFourAnswers
    {
        A, B, C, D
    }
    public enum SelectThreeAnswers
    {
        A, B, C
    }
    public enum SelectTwoAnswers
    {
        A, B
    }
}
