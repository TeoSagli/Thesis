using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class QuizDataAnswer 
{
    private string answerOne;
    private string answerTwo;
    private string answerThree;
    private string answerFour;
    private int correctAnswer;
    private GameObject objectAnswer;
    private string rightObjectTag;
    private string answerType;
    [JsonConstructor]
    public QuizDataAnswer(string answerOne, string answerTwo, string answerThree, string answerFour, GameObject objectAnswer, string rightObjectTag, int correctAnswer, string answerType)
    {
        this.answerOne = answerOne;
        this.answerTwo = answerTwo;
        this.answerThree = answerThree;
        this.answerFour = answerFour;
        this.objectAnswer = objectAnswer;
        this.rightObjectTag = rightObjectTag;
        this.correctAnswer = correctAnswer;
        this.answerType = answerType;
    }
    public QuizDataAnswer(string answerOne, string answerTwo, string answerThree, string answerFour, int correctAnswer, string answerType)
    {
        this.answerOne = answerOne;
        this.answerTwo = answerTwo;
        this.answerThree = answerThree;
        this.answerFour = answerFour;
        this.correctAnswer = correctAnswer;
        this.answerType = answerType;
    }
    public QuizDataAnswer(string answerOne, string answerTwo, string answerThree, int correctAnswer, string answerType)
    {
        this.answerOne = answerOne;
        this.answerTwo = answerTwo;
        this.answerThree = answerThree;
        this.correctAnswer = correctAnswer;
        this.answerType = answerType;
    }
    public QuizDataAnswer(string answerOne, string answerTwo, int correctAnswer, string answerType)
    {
        this.answerOne = answerOne;
        this.answerTwo = answerTwo;
        this.correctAnswer = correctAnswer;
        this.answerType = answerType;
    }
    public QuizDataAnswer(GameObject objectAnswer, string rightObjectTag, string answerType)
    {
        this.objectAnswer = objectAnswer;
        this.rightObjectTag = rightObjectTag;
        this.answerType = answerType;
    }

    public string AnswerOne { get => answerOne; set => answerOne = value; }
    public string AnswerTwo { get => answerTwo; set => answerTwo = value; }
    public string AnswerThree { get => answerThree; set => answerThree = value; }
    public string AnswerFour { get => answerFour; set => answerFour = value; }
    public GameObject ObjectAnswer { get => objectAnswer; set => objectAnswer = value; }
    public string RightObjectTag { get => rightObjectTag; set => rightObjectTag = value; }
    public int CorrectAnswer { get => correctAnswer; set => correctAnswer = value; }
    public string AnswerType { get => answerType; set => answerType = value; }
}
