using System;
using System.Collections;
using System.Collections.Generic;
using Assets.MyAssets.Scripts.Features.Activities;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static Enums;

public class QuizFeature : MonoBehaviour
{
    [SerializeField]
    private GameObject options;

    [Header("QUESTIONS")]
    [SerializeField]
    private QuizQuestionType questionType;

    [SerializeField] private QuizTextFeature quizTextQuestion;
    [SerializeField] private QuizVideoFeature quizVideoQuestion;
    [SerializeField] private QuizImageFeature quizImageQuestion;

    [Header("ANSWERS")]
    [SerializeField]
    private QuizAnswersType answersType;

    [SerializeField] private QuizTwoFeature quizTwoAnswers;
    [SerializeField] private QuizThreeFeature quizThreeAnswers;
    [SerializeField] private QuizFourFeature quizFourAnswers;
    [SerializeField] private QuizObjectFeature quizObjectAnswer;



    void Start()
    {
        InitQuestion();
        InitAnswers();
    }

    private void InitQuestion()
    {

        QuizQuestion q;
        switch (questionType)
        {
            case QuizQuestionType.Text:
                q = quizTextQuestion;
                quizTextQuestion.TextQuestion.text = quizTextQuestion.TextToShow;
                break;
            case QuizQuestionType.Image:
                q = quizImageQuestion;
                quizImageQuestion.ImageQuestion.sprite = quizImageQuestion.ImageToShow;
                break;
            case QuizQuestionType.Video:
                q = quizVideoQuestion;
                quizVideoQuestion.HandleVideo();
                break;
            default:
                q=null;
                break;
        }
        SetTitleWithMax(q.TextTitle, q.TextTitleToShow, 77);
    }

    private void InitAnswers()
    {
        int noptions = GetNOptions();
        TextMeshProUGUI[] t = new TextMeshProUGUI[noptions];
        GameObject[] o = new GameObject[noptions];
        Button[] b = new Button[noptions];

        ActivateHandleButtons(noptions,  o, b);
        FillTextByOptions(noptions, t, o);
        FillOthersTypeByOptions();
    }
    private void ActivateHandleButtons(int noptions,  GameObject[] o, Button[] b)
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
            case QuizAnswersType.Text:
                //todo
                break;
            case QuizAnswersType.Object:
                quizObjectAnswer.ObjectAnswer.SetActive(true);
                quizObjectAnswer.ObjectAnswer.GetComponent<XRSocketInteractor>().selectEntered.AddListener((s) =>
                {
                    if (CheckWin(0))
                    {
                        PuzzleManager.Instance.PuzzleAdvancement();
                        DisableAllBtns();
                    }
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
            SetOptionText(t[0], quizTwoAnswers.AnswerOne);
            SetOptionText(t[1], quizTwoAnswers.AnswerTwo);
        }
        if (noptions >= 3)
        {
            t[2] = o[2].GetNamedChild("Text").GetComponent<TextMeshProUGUI>();
            SetOptionText(t[0], quizThreeAnswers.AnswerOne);
            SetOptionText(t[1], quizThreeAnswers.AnswerTwo);
            SetOptionText(t[2], quizThreeAnswers.AnswerThree);
        }
        if (noptions >= 4)
        {
            t[3] = o[3].GetNamedChild("Text").GetComponent<TextMeshProUGUI>();
            SetOptionText(t[0], quizFourAnswers.AnswerOne);
            SetOptionText(t[1], quizFourAnswers.AnswerTwo);
            SetOptionText(t[2], quizFourAnswers.AnswerThree);
            SetOptionText(t[3], quizFourAnswers.AnswerFour);
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
        if (noptions == 0)
        {
            var socket = options.GetNamedChild("ObjectHolder").GetComponent<XRSocketInteractor>();
            var murales = socket.interactablesSelected[0].transform.GetComponent<XRGrabInteractable>();
            var sprite = murales.GetComponent<SpriteRenderer>().sprite;
            if (sprite != null)
                quizImageQuestion.ImageQuestion.sprite = sprite;
            socket.enabled = false;
            options.SetActive(false);
        }

    }
    private int GetNOptions()
    {
        QuizAnswer answ = answersType switch
        {
            QuizAnswersType.TwoAnswers => quizTwoAnswers,
            QuizAnswersType.ThreeAnswers => quizThreeAnswers,
            QuizAnswersType.FourAnswers => quizFourAnswers,
            QuizAnswersType.Object => quizObjectAnswer,
            _ => null,
        };
        if (answ!=null)
            return answ.NAnswers;
        else return 0;
    }
    private bool CheckWin(int correctAnswer)
    {
        return answersType switch
        {
            QuizAnswersType.TwoAnswers => correctAnswer == quizTwoAnswers.CorrectAnswer,
            QuizAnswersType.ThreeAnswers => correctAnswer == quizThreeAnswers.CorrectAnswer,
            QuizAnswersType.FourAnswers => correctAnswer == quizFourAnswers.CorrectAnswer,
            QuizAnswersType.Object => quizObjectAnswer.ObjectAnswer.GetComponent<XRSocketInteractor>().interactablesSelected[0].transform.CompareTag(quizObjectAnswer.RightObjectTag),
            _ => false,
        };
    }
    private void SetTitleWithMax( TextMeshProUGUI textObj, string title, int max)
    {
        if (title.Length > max)
            title = title[..max];
        textObj.text = title;
    }
    private void SetOptionText(TextMeshProUGUI option, string text)
    {
        option.text = text;
    }

}



