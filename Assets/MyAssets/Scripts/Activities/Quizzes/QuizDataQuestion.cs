using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
[System.Serializable]
public class QuizDataQuestion 
{
    private string path;
    private int id;
    private string ext;
    private string textToShow;
    private string textTitleToShow;
    private string questionType;

    public QuizDataQuestion(string textTitleToShow, string questionType)
    {
        TextTitleToShow = textTitleToShow;
        QuestionType = questionType;
    }


    public QuizDataQuestion(string text, string textTitleToShow, string questionType)
    {
        Path = text;
        TextToShow = text;
        TextTitleToShow = textTitleToShow;
        QuestionType = questionType;
    }
    [JsonConstructor]
    public QuizDataQuestion(string path, string textToShow, string textTitleToShow, string questionType, int id, string ext)
    {
        Path = path;
        TextToShow = textToShow;
        TextTitleToShow = textTitleToShow;
        QuestionType = questionType;
        Id = id;
        Ext = ext;
    }

    public string Path { get => path; set => path = value; }
    public string TextToShow { get => textToShow; set => textToShow = value; }
    public string TextTitleToShow { get => textTitleToShow; set => textTitleToShow = value; }
    public string QuestionType { get => questionType; set => questionType = value; }
    public int Id { get => id; set => id = value; }
    public string Ext { get => ext; set => ext = value; }
}
