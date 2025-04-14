using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
[System.Serializable]
public class QuizDataQuestion 
{
    private string videoToShowPath;
    private string imageToShowPath;
    private string textToShow;
    private string textTitleToShow;

    public QuizDataQuestion(string textTitleToShow)
    {
        this.textTitleToShow = textTitleToShow;
    }


    public QuizDataQuestion(string text, string textTitleToShow)
    {
        ImageToShowPath = text;
        VideoToShowPath = text;
        TextToShow = text;
        TextTitleToShow = textTitleToShow;
    }

    public QuizDataQuestion(string videoToShowPath, string imageToShowPath, string textToShow, string textTitleToShow)
    {
        VideoToShowPath = videoToShowPath;
        ImageToShowPath = imageToShowPath;
        TextToShow = textToShow;
        TextTitleToShow = textTitleToShow;
    }

    public string VideoToShowPath { get => videoToShowPath; set => videoToShowPath = value; }
    public string ImageToShowPath { get => imageToShowPath; set => imageToShowPath = value; }
    public string TextToShow { get => textToShow; set => textToShow = value; }
    public string TextTitleToShow { get => textTitleToShow; set => textTitleToShow = value; }
}
