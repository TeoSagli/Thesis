using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.MyAssets.Scripts.Features.Activities;
using DilmerGames.Core.Singletons;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Video;
using static Enums;

public class PuzzleManager : Singleton<PuzzleManager>
{

    [SerializeField]
    [Header("Tot puzzles to solve")]

    private int totPuzzle = 4;
    [Header("Audio")]
    [SerializeField]
    private AudioClip audioClipWin;
    [SerializeField]
    private AudioClip audioClipFail;
    private int totSolvedPuzzle = 0;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private GameObject socket2DObject;
    [SerializeField]
    private GameObject socket3DObject;
    [SerializeField]
    private GameObject quizObject;
    public Action<GameState> onPuzzleSolved;

    private void Start()
    {
        LoadPuzzles();
    }
    private void LoadPuzzles()
    {
        PuzzleData puzzleData = new(0.1f, "Title to show", 2, 2, 2);
        PuzzleData2D puzzleData2D=new("C:/Users/matte/Desktop/Media/scuola.jpg","Scuola");

        Generate2DPuzzle(puzzleData, puzzleData2D,  new Vector3(0.4f, 1.3f, -0.6f), Quaternion.identity);
        //==============================================
        QuizDataAnswer quizDataAnswer = new("11111111111111111","2222222222","33333333333333333","444444444444444",2);
        QuizDataAnswer quizDataAnswer2 = new("oooooooooo","rrrrrrrr",2);
        QuizDataQuestion quizDataQuestion = new("sdiofjmsdiofjmsdkiofsjkmipo");
        QuizDataQuestion quizDataQuestion2 = new("C:/Users/matte/Desktop/Media/scuola.jpg", "aaaaaaaaaaaaaaaaaaaaa");
        QuizDataQuestion quizDataQuestion3 = new("C:/Users/matte/Desktop/Media/NoLo in 100 secondi.mp4", "aaaaaaaaaaaaaaaaaaaaa");

        GameObject quizObjInst = Instantiate(quizObject, new Vector3(1.56f, 1.3f, -0.6f), Quaternion.identity);
        QuizAnswer quizAnswerFeature = GenerateQuizAnswerFour(quizDataAnswer);
        QuizQuestion quizQuestionFeature = GenerateQuizQuestionVideo(quizObjInst, quizDataQuestion3);
        GenerateQuiz(quizObjInst, quizQuestionFeature, quizAnswerFeature);

        Debug.Log("json: " + Serializer.Instance.SerializeToJSON(puzzleData));
        Debug.Log("json 2: " + Serializer.Instance.SerializeToJSON(puzzleData2D));
       // Debug.Log("json 3: " + Serializer.Instance.SerializeToJSON(quizDataAnswer));
        Debug.Log("json 4: " + Serializer.Instance.SerializeToJSON(quizDataQuestion3));
    }
    //===================PUZZLES=========================
    private void Generate2DPuzzle(PuzzleData data,PuzzleData2D data2D, Vector3 socketPos,Quaternion socketRot)
    {
        Puzzle2D scriptSchool2D = new(data.PieceScale,data.TitleStr,data.NCols,data.NRows,data2D.SpritePath,data2D.SpriteName);
        GameObject socketSchool2D = Instantiate(socket2DObject,socketPos,socketRot);
        Puzzle2DSocket scriptSocketSchool2D = socketSchool2D.GetComponent<Puzzle2DSocket>();
        scriptSocketSchool2D.Initialize(scriptSchool2D);
    }
    //===================QUIZ=========================
    private void GenerateQuiz(GameObject quizObjInst, QuizQuestion quizQuestionFeature, QuizAnswer quizAnswerFeature) {
        Quiz qf=quizObjInst.AddComponent<Quiz>();
        qf.Init(quizQuestionFeature, quizAnswerFeature, quizObjInst);
    }
    private QuizQuestion GenerateQuizQuestionVideo(GameObject quizObjInst, QuizDataQuestion dataQuest)
    {
        TextMeshProUGUI questionTitle = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionTitle").gameObject.GetComponent<TextMeshProUGUI>();
        GameObject questionButtonObj = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionVideo/Button").gameObject;
        GameObject videoObj = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionVideo").gameObject;
        videoObj.SetActive(true);
        VideoPlayer questionVideoPlayer = videoObj.GetComponent<VideoPlayer>();
        GameObject questionPlaceholderObj = quizObjInst.transform.Find("QuizCanvas/PlaceHolderPause").gameObject;
        QuizVideoQuestion quizQuestionFeature = new(dataQuest, questionTitle, questionButtonObj, questionVideoPlayer, questionPlaceholderObj );
        return quizQuestionFeature;
    }
    private QuizQuestion GenerateQuizQuestionImage(GameObject quizObjInst, QuizDataQuestion dataQuest)
    {
        TextMeshProUGUI questionTitle = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionTitle").gameObject.GetComponent<TextMeshProUGUI>();
        SpriteRenderer questionImageRenderer = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionImage").gameObject.GetComponent<SpriteRenderer>();
        QuizImageQuestion quizImageFeature = new(dataQuest, questionTitle, questionImageRenderer);
        return quizImageFeature;
    }
    private QuizQuestion GenerateQuizQuestionText(GameObject quizObjInst, QuizDataQuestion dataQuest)
    {
        TextMeshProUGUI questionTitle = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionTitle").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI questionText = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionText").gameObject.GetComponent<TextMeshProUGUI>();
        QuizTextQuestion quizTextFeature = new(dataQuest, questionTitle, questionText);
        return quizTextFeature;
    }
    private QuizAnswer GenerateQuizAnswerTwo(QuizDataAnswer dataAnsw) {
        QuizTwoAnswers quizTwoFeature = new( dataAnsw.AnswerOne, dataAnsw.AnswerTwo, dataAnsw.CorrectAnswer);
        return quizTwoFeature;
    }
    private QuizAnswer GenerateQuizAnswerThree(QuizDataAnswer dataAnsw)
    {
        QuizThreeAnswers quizThreeFeature = new(dataAnsw.AnswerOne, dataAnsw.AnswerTwo, dataAnsw.AnswerThree, dataAnsw.CorrectAnswer);
        return quizThreeFeature;
    }
    private QuizAnswer GenerateQuizAnswerFour(QuizDataAnswer dataAnsw)
    {
        QuizFourAnswers quizFourFeature = new(dataAnsw.AnswerOne, dataAnsw.AnswerTwo, dataAnsw.AnswerThree, dataAnsw.AnswerFour, dataAnsw.CorrectAnswer);
        return quizFourFeature;
    }
    private QuizAnswer GenerateQuizAnswerObject(QuizDataAnswer dataAnsw)
    {
        QuizObjectAnswer quizObjectFeature = new(dataAnsw.ObjectAnswer,dataAnsw.RightObjectTag);
        return quizObjectFeature;
    }
    public void PuzzleAdvancement()
    {
        PlayAdvancementAudio();
        totSolvedPuzzle++;
        ProgressBarFeature.Instance.SetPercentage(totSolvedPuzzle * 100 / totPuzzle);
        if (totSolvedPuzzle == totPuzzle)
            onPuzzleSolved?.Invoke(GameState.PuzzleSolved);
    }
    public int GetTotPuzzle()
    {
        return totPuzzle;
    }
    private void PlayAdvancementAudio()
    {
        audioSource.clip = audioClipWin;
        audioSource.Play();
    }
    public void PlayFailAudio()
    {
        audioSource.clip = audioClipFail;
        audioSource.Play();
    }
}
