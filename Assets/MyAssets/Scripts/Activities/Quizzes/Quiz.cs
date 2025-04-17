using Assets.MyAssets.Scripts.Features.Activities;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static Enums;

public class Quiz : MonoBehaviour
{
    private GameObject options;
    private QuizQuestionType questionType;
    private QuizAnswersType answersType;
    private QuizAnswer quizAnswerFeature;
    private QuizQuestion quizQuestionFeature;

    public void Init(QuizQuestion quizQuestionFeature, QuizAnswer quizAnswerFeature, GameObject quizObj)
    {
        Options = quizObj.transform.Find("QuizCanvas/Answers").gameObject;
        QuizAnswerFeature = quizAnswerFeature;
        QuizQuestionFeature = quizQuestionFeature;
        InitQuestion();
        InitAnswers();
    }

    public QuizAnswersType AnswersType { get => answersType; set => answersType = value; }
    public QuizAnswer QuizAnswerFeature { get => quizAnswerFeature; set => quizAnswerFeature = value; }
    public QuizQuestionType QuestionType { get => questionType; set => questionType = value; }
    public QuizQuestion QuizQuestionFeature { get => quizQuestionFeature; set => quizQuestionFeature = value; }
    public GameObject Options { get => options; set => options = value; }

    private void InitQuestion()
    {
        QuestionType = quizQuestionFeature.QuestionType;
        quizQuestionFeature.Init();
        SetTitleWithMax(quizQuestionFeature.TextTitle, quizQuestionFeature.QuizDataQuestion.TextTitleToShow, 77);
    }

    private void InitAnswers()
    {
        AnswersType = QuizAnswerFeature.QuizAnswerType;
        int noptions = QuizAnswerFeature.NAnswers;
        TextMeshProUGUI[] t = new TextMeshProUGUI[noptions];
        GameObject[] o = new GameObject[noptions];
        Button[] b = new Button[noptions];

        ActivateHandleButtons(noptions, o, b);
        FillTextByOptions(t, o);
    }
    private void ActivateHandleButtons(int noptions,  GameObject[] o, Button[] b)
    {
        for (int i = 0; i < noptions; i++)
        {
            Options.transform.GetChild(i).gameObject.SetActive(true);
            o[i] = Options.transform.GetChild(i).gameObject;
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
    private void FillTextByOptions(TextMeshProUGUI[] t, GameObject[] o)
    {
        quizAnswerFeature.SetUI(t, o);
        if (quizAnswerFeature.QuizAnswerType == QuizAnswersType.Object) HandleQuizObject();

    }
    private void HandleQuizObject()
    {
        quizAnswerFeature.QuizDataAnswer.ObjectAnswer.SetActive(true);
        quizAnswerFeature.QuizDataAnswer.ObjectAnswer.GetComponent<XRSocketInteractor>().selectEntered.AddListener((s) =>
        {
        if (CheckWin(0))
        {
            PuzzleManager.Instance.PuzzleAdvancement();
            DisableAllBtns();
        }
        else
            PuzzleManager.Instance.PlayFailAudio();
        });
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
        int noptions = quizAnswerFeature.NAnswers;
        for (int i = 0; i < noptions; i++)
            Options.GetNamedChild("Option" + (i + 1)).GetComponent<Button>().interactable = false;
        if (noptions == 0)
        {
            var socket = Options.GetNamedChild("ObjectHolder").GetComponent<XRSocketInteractor>();
            var murales = socket.interactablesSelected[0].transform.GetComponent<XRGrabInteractable>();
            var sprite = murales.GetComponent<SpriteRenderer>().sprite;
            if (sprite != null)
                ((QuizImageQuestion) QuizQuestionFeature).ImageQuestion.sprite = sprite;
            socket.enabled = false;
            Options.SetActive(false);
        }

    }
    private bool CheckWin(int correctAnswer)
    {
        return AnswersType switch
        {
            QuizAnswersType.Two => correctAnswer == quizAnswerFeature.QuizDataAnswer.CorrectAnswer,
            QuizAnswersType.Three => correctAnswer == quizAnswerFeature.QuizDataAnswer.CorrectAnswer,
            QuizAnswersType.Four => correctAnswer == quizAnswerFeature.QuizDataAnswer.CorrectAnswer,
            QuizAnswersType.Object => quizAnswerFeature.QuizDataAnswer.ObjectAnswer.GetComponent<XRSocketInteractor>().interactablesSelected[0].transform.CompareTag(quizAnswerFeature.QuizDataAnswer.RightObjectTag),
            _ => false,
        };
    }
    private void SetTitleWithMax(TextMeshProUGUI textObj, string title, int max)
    {
        if (title.Length > max)
            title = title[..max];
        textObj.text = title;
    }


}



