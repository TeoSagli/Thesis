using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static Enums;

public class QuizFeature : MonoBehaviour
{
    [Header("Multimedia Players")]
    [SerializeField]
    private GameObject videoButton;
    [SerializeField]
    private VideoPlayer videoQuestion;
    [SerializeField]
    private TextMeshProUGUI textQuestion;
    [SerializeField]
    private SpriteRenderer imageQuestion;
    [SerializeField]
    private GameObject options;
    [SerializeField]
    private GameObject objectAnswer;
    //==================================================
    [SerializeField, Header("Title (max 78 chars)")]
    private TextMeshProUGUI textTitle;
    [SerializeField]
    private string textTitleToShow;
    //==================================================
    [Header("Question")]
    [SerializeField, Header("Select question type")]
    private QuizQuestion questionType;


    [SerializeField, ShowIf("questionType", QuizQuestion.Video)]
    private VideoClip videoToShow;
    //==================================================

    [SerializeField, ShowIf("questionType", QuizQuestion.Text)]
    private string textToShow;
    //==================================================
    [SerializeField, ShowIf("questionType", QuizQuestion.Image)]
    private Sprite imageToShow;

    [Header("Answers")]
    //==================================================
    //DEFINE ANSWERS TYPE
    [SerializeField]
    private QuizAnswers answersType;

    //==================================================
    [SerializeField, ShowIf("answersType", QuizAnswers.TwoAnswers)]
    private string answerOne;
    [SerializeField, ShowIf("answersType", QuizAnswers.TwoAnswers)]
    private string answerTwo;
    //==================================================
    [SerializeField, ShowIf("answersType", QuizAnswers.ThreeAnswers)]
    private string answerA;
    [SerializeField, ShowIf("answersType", QuizAnswers.ThreeAnswers)]
    private string answerB;
    [SerializeField, ShowIf("answersType", QuizAnswers.ThreeAnswers)]
    private string answerC;
    //==================================================
    [SerializeField, ShowIf("answersType", QuizAnswers.FourAnswers)]
    private string answer1;
    [SerializeField, ShowIf("answersType", QuizAnswers.FourAnswers)]
    private string answer2;
    [SerializeField, ShowIf("answersType", QuizAnswers.FourAnswers)]
    private string answer3;
    [SerializeField, ShowIf("answersType", QuizAnswers.FourAnswers)]
    private string answer4;
    //==================================================
    [Header("Right answer")]
    //CORRECT ANSWER
    [SerializeField, ShowIf("answersType", QuizAnswers.FourAnswers)]
    private SelectFourAnswers rightAnswersOf4;
    [SerializeField, ShowIf("answersType", QuizAnswers.ThreeAnswers)]
    private SelectThreeAnswers rightAnswersOf3;
    [SerializeField, ShowIf("answersType", QuizAnswers.TwoAnswers)]
    private SelectTwoAnswers rightAnswersOf2;
    [SerializeField, ShowIf("answersType", QuizAnswers.Object)]
    private string rightObjectTag;


    void Start()
    {
        InitQuestion();
        InitAnswers();
    }

    private void InitQuestion()
    {
        SetTitleWithMax(ref textTitle, textTitleToShow, 77);
        switch (questionType)
        {
            case QuizQuestion.Text:
                textQuestion.text = textToShow;
                break;
            case QuizQuestion.Image:
                imageQuestion.sprite = imageToShow;
                break;
            case QuizQuestion.Video:
                videoQuestion.gameObject.SetActive(true);
                PlayVideo.Instance.AddVideoClip(videoToShow);
                float videoHeight = videoToShow.height;
                float videoWidth = videoToShow.width;
                float aspectRatio = videoWidth / videoHeight;
                videoQuestion.transform.localScale = new Vector3(0.4f, 1 / aspectRatio, 0.4f);
                videoQuestion.clip = videoToShow;
                videoButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            TogglePlayPause(videoQuestion);
        });
                break;
        }
    }
    private void InitAnswers()
    {
        int noptions = GetNOptions();
        TextMeshProUGUI[] t = new TextMeshProUGUI[noptions];
        GameObject[] o = new GameObject[noptions];
        Button[] b = new Button[noptions];

        ActivateHandleButtons(noptions, t, o, b);
        FillTextByOptions(noptions, t, o);
        FillOthersTypeByOptions();
    }
    private void ActivateHandleButtons(int noptions, TextMeshProUGUI[] t, GameObject[] o, Button[] b)
    {
        for (int i = 0; i < noptions; i++)
        {
            options.transform.GetChild(i).gameObject.SetActive(true);
            o[i] = options.transform.GetChild(i).gameObject;
            b[i] = o[i].GetComponent<Button>();
            Button btn = b[i];
            Image img = o[i].GetComponent<Image>();
            int num = i;
            b[i].onClick.AddListener(() =>
            {
                OnButtonClicked(ref btn, num, img);
            });
        }
    }
    private void FillOthersTypeByOptions()
    {
        switch (answersType)
        {
            case QuizAnswers.Text:
                //todo
                break;
            case QuizAnswers.Object:
                objectAnswer.SetActive(true);
                objectAnswer.GetComponent<XRSocketInteractor>().selectEntered.AddListener((s) =>
                {
                    if (CheckWin(0))
                        PuzzleManager.Instance.PuzzleAdvancement();
                    else
                        PuzzleManager.Instance.PlayFailAudio();
                });
                break;
        }
    }
    private void FillTextByOptions(int noptions, TextMeshProUGUI[] t, GameObject[] o)
    {
        if (noptions >= 2)
        {
            t[0] = o[0].GetNamedChild("Text").GetComponent<TextMeshProUGUI>();
            t[1] = o[1].GetNamedChild("Text").GetComponent<TextMeshProUGUI>();
            SetOptionText(t[0], answerOne);
            SetOptionText(t[1], answerTwo);
        }
        if (noptions >= 3)
        {
            t[2] = o[2].GetNamedChild("Text").GetComponent<TextMeshProUGUI>();
            SetOptionText(t[0], answerA);
            SetOptionText(t[1], answerB);
            SetOptionText(t[2], answerC);
        }
        if (noptions >= 4)
        {
            t[3] = o[3].GetNamedChild("Text").GetComponent<TextMeshProUGUI>();
            SetOptionText(t[0], answer1);
            SetOptionText(t[1], answer2);
            SetOptionText(t[2], answer3);
            SetOptionText(t[3], answer4);
        }
    }
    private void OnButtonClicked(ref Button button, int text, Image img)
    {
        if (CheckWin(text))
        {
            img.color = Color.green;
            PuzzleManager.Instance.PuzzleAdvancement();
            DisableAllBtns();
        }
        else
        {
            img.color = Color.red;
            PuzzleManager.Instance.PlayFailAudio();
        }
        button.interactable = false;
    }
    private void DisableAllBtns()
    {
        int noptions = GetNOptions();
        for (int i = 0; i < noptions; i++)
            options.GetNamedChild("Option" + (i + 1)).GetComponent<Button>().interactable = false;
    }
    private int GetNOptions()
    {
        int noptions = 0;
        switch (answersType)
        {
            case QuizAnswers.TwoAnswers:
                noptions = 2;
                break;
            case QuizAnswers.ThreeAnswers:
                noptions = 3;
                break;
            case QuizAnswers.FourAnswers:
                noptions = 4;
                break;
            default:
                break;
        }
        return noptions;
    }
    private bool CheckWin(int correctAnswer)
    {
        return answersType switch
        {
            QuizAnswers.TwoAnswers => correctAnswer == (int)rightAnswersOf2,
            QuizAnswers.ThreeAnswers => correctAnswer == (int)rightAnswersOf3,
            QuizAnswers.FourAnswers => correctAnswer == (int)rightAnswersOf4,
            QuizAnswers.Object => objectAnswer.GetComponent<XRSocketInteractor>().interactablesSelected[0].transform.CompareTag(rightObjectTag),
            _ => false,
        };
    }
    private void SetTitleWithMax(ref TextMeshProUGUI textObj, string title, int max)
    {
        if (title.Length > max)
            title = title[..max];
        textObj.text = title;
    }
    private void SetOptionText(TextMeshProUGUI option, string text)
    {
        option.text = text;
    }
    public void TogglePlayPause(VideoPlayer videoPlayer)
    {
        bool isPlaying = !videoPlayer.isPlaying;
        SetPlay(videoPlayer, isPlaying);
    }
    public void SetPlay(VideoPlayer videoPlayer, bool value)
    {
        if (value)
            videoPlayer.Play();
        else
            videoPlayer.Pause();
    }
}



