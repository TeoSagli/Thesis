using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizData : MonoBehaviour
{
    QuizDataAnswer quizDataAnswer;
    QuizDataQuestion quizDataQuestion;

    public QuizData(QuizDataAnswer quizDataAnswer, QuizDataQuestion quizDataQuestion)
    {
        this.quizDataAnswer = quizDataAnswer;
        this.quizDataQuestion = quizDataQuestion;
    }

    public QuizDataAnswer QuizDataAnswer { get => quizDataAnswer; set => quizDataAnswer = value; }
    public QuizDataQuestion QuizDataQuestion { get => quizDataQuestion; set => quizDataQuestion = value; }
}
