using System;
using System.IO;
using Assets.MyAssets.Scripts.Features.Activities;
using DilmerGames.Core.Singletons;
using TMPro;
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
    [SerializeField]
    private Shader noCullShader;
    public Action<GameState> onPuzzleSolved;

    private void Start()
    {
        LoadPuzzles();
    }
    private void LoadPuzzles()
    {
        string jsonData = File.ReadAllText("C:/Users/matte/Desktop/Media/data.txt");
        DataRoot rootData = Serializer.Instance.DeserializeFromJSON(jsonData);
        /* PuzzleData puzzleData = new(0.1f, "Title to show", 2, 2, 2);
         PuzzleData2D puzzleData2D=new("C:/Users/matte/Desktop/Media/scuola.jpg","Scuola");*/
        PuzzleData puzzleData1 = rootData.puzzleData1;
        PuzzleData puzzleData2 = rootData.puzzleData2;
        PuzzleData2D puzzleData2D = rootData.puzzleData2D;
        PuzzleData3D puzzleData3D = rootData.puzzleData3D;

        Generate2DPuzzle(puzzleData1, puzzleData2D,  new Vector3(0.4f, 1.3f, -0.6f), Quaternion.identity);
        Generate3DPuzzle(puzzleData2, puzzleData3D,  new Vector3(-1.1f, 1.3f, -0.6f), Quaternion.identity);
        //==============================================
        /*  QuizDataAnswer quizDataAnswer = new("11111111111111111","2222222222","33333333333333333","444444444444444",2);
          QuizDataAnswer quizDataAnswer2 = new("oooooooooo","rrrrrrrr",2);
          QuizDataQuestion quizDataQuestion = new("sdiofjmsdiofjmsdkiofsjkmipo");
          QuizDataQuestion quizDataQuestion2 = new("C:/Users/matte/Desktop/Media/scuola.jpg", "aaaaaaaaaaaaaaaaaaaaa");
          QuizDataQuestion quizDataQuestion3 = new("C:/Users/matte/Desktop/Media/NoLo in 100 secondi.mp4", "aaaaaaaaaaaaaaaaaaaaa");
        */
        QuizDataQuestion quizDataQuestion = rootData.quizDataQuestion;
        QuizDataAnswer quizDataAnswer = rootData.quizDataAnswer;
        
        GenerateQuiz(quizDataQuestion, quizDataAnswer, new Vector3(1.56f, 1.3f, -0.6f), Quaternion.identity);

   /*     Debug.Log("json: " + Serializer.Instance.SerializeToJSON(puzzleData));
        Debug.Log("json 2: " + Serializer.Instance.SerializeToJSON(puzzleData2D));
        Debug.Log("json 3: " + Serializer.Instance.SerializeToJSON(quizDataAnswer));
        Debug.Log("json 4: " + Serializer.Instance.SerializeToJSON(quizDataQuestion3));*/
    }
    //===================PUZZLES=========================
    private void Generate2DPuzzle(PuzzleData data,PuzzleData2D data2D, Vector3 socketPos,Quaternion socketRot)
    {
        GameObject puzzle2D = new("PuzzlePiecesSpawner2D-" + data2D.SpriteName);
        Puzzle2D script2D = puzzle2D.AddComponent<Puzzle2D>();
        script2D.Init(data.PieceScale,data.TitleStr,data.NCols,data.NRows,data2D.SpritePath,data2D.SpriteName);
        GameObject socket2D = Instantiate(socket2DObject,socketPos,socketRot);
        Puzzle2DSocket scriptSocket2D = socket2D.GetComponent<Puzzle2DSocket>();
        scriptSocket2D.Initialize(script2D);
    }
    private void Generate3DPuzzle(PuzzleData data, PuzzleData3D data3D, Vector3 socketPos, Quaternion socketRot)
    {
        GameObject puzzle3D = new ("PuzzlePiecesSpawner3D-" + data3D.MeshName);
        Puzzle3D script3D = puzzle3D.AddComponent<Puzzle3D>();
        script3D.Init(data.PieceScale, data.TitleStr, data.NCols, data.NRows, data.NDepth, data3D.MeshPath, data3D.MeshName, noCullShader);
        GameObject socket3D = Instantiate(socket3DObject, socketPos, socketRot);
        Puzzle3DSocket scriptSocket3D = socket3D.GetComponent<Puzzle3DSocket>();
        script3D.OnLoaded += (puzzle) => scriptSocket3D.Initialize(script3D);
    }
    //===================QUIZ=========================
    private void GenerateQuiz( QuizDataQuestion quizDataQuestion, QuizDataAnswer quizDataAnswer, Vector3 pos, Quaternion rot) {
        GameObject quizObjInst = Instantiate(quizObject, pos, rot);
        Quiz qf=quizObjInst.AddComponent<Quiz>();
        QuizQuestionType qType = (QuizQuestionType)System.Enum.Parse(typeof(QuizQuestionType), quizDataQuestion.QuestionType);
        QuizQuestion question = qType switch
        {
            QuizQuestionType.Text => GenerateQuizQuestionText(quizObjInst, quizDataQuestion),
            QuizQuestionType.Image => GenerateQuizQuestionImage(quizObjInst, quizDataQuestion),
            QuizQuestionType.Video => GenerateQuizQuestionVideo(quizObjInst, quizDataQuestion),
            _ => null,
        };
        QuizAnswersType aType = (QuizAnswersType)System.Enum.Parse(typeof(QuizAnswersType), quizDataAnswer.AnswerType);
        QuizAnswer answer = aType switch
        {
            QuizAnswersType.Two => GenerateQuizAnswerTwo(quizDataAnswer),
            QuizAnswersType.Three => GenerateQuizAnswerThree(quizDataAnswer),
            QuizAnswersType.Four => GenerateQuizAnswerFour(quizDataAnswer),
            QuizAnswersType.Object => GenerateQuizAnswerObject(quizDataAnswer),

            _ => null,
        };
        qf.Init(question, answer, quizObjInst);
    }
    private QuizQuestion GenerateQuizQuestionVideo(GameObject quizObjInst, QuizDataQuestion dataQuest)
    {
        TextMeshProUGUI questionTitle = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionTitle").gameObject.GetComponent<TextMeshProUGUI>();
        GameObject questionButtonObj = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionVideo/Button").gameObject;
        GameObject videoObj = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionVideo").gameObject;
        videoObj.SetActive(true);
        VideoPlayer questionVideoPlayer = videoObj.GetComponent<VideoPlayer>();
        GameObject questionPlaceholderObj = quizObjInst.transform.Find("QuizCanvas/PlaceHolderPause").gameObject;
        QuizVideoQuestion quizQuestionFeature = new(dataQuest, questionTitle, questionButtonObj, questionVideoPlayer, questionPlaceholderObj, dataQuest.QuestionType);
        return quizQuestionFeature;
    }
    private QuizQuestion GenerateQuizQuestionImage(GameObject quizObjInst, QuizDataQuestion dataQuest)
    {
        TextMeshProUGUI questionTitle = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionTitle").gameObject.GetComponent<TextMeshProUGUI>();
        SpriteRenderer questionImageRenderer = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionImage").gameObject.GetComponent<SpriteRenderer>();
        QuizImageQuestion quizImageFeature = new(dataQuest, questionTitle, questionImageRenderer, dataQuest.QuestionType);
        return quizImageFeature;
    }
    private QuizQuestion GenerateQuizQuestionText(GameObject quizObjInst, QuizDataQuestion dataQuest)
    {
        TextMeshProUGUI questionTitle = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionTitle").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI questionText = quizObjInst.transform.Find("QuizCanvas/Questions/QuestionText").gameObject.GetComponent<TextMeshProUGUI>();
        QuizTextQuestion quizTextFeature = new(dataQuest, questionTitle, questionText, dataQuest.QuestionType);
        return quizTextFeature;
    }
    private QuizAnswer GenerateQuizAnswerTwo(QuizDataAnswer dataAnsw) {
        QuizTwoAnswers quizTwoFeature = new( dataAnsw.AnswerOne, dataAnsw.AnswerTwo, dataAnsw.CorrectAnswer, dataAnsw.AnswerType);
        return quizTwoFeature;
    }
    private QuizAnswer GenerateQuizAnswerThree(QuizDataAnswer dataAnsw)
    {
        QuizThreeAnswers quizThreeFeature = new(dataAnsw.AnswerOne, dataAnsw.AnswerTwo, dataAnsw.AnswerThree, dataAnsw.CorrectAnswer, dataAnsw.AnswerType);
        return quizThreeFeature;
    }
    private QuizAnswer GenerateQuizAnswerFour(QuizDataAnswer dataAnsw)
    {
        QuizFourAnswers quizFourFeature = new(dataAnsw.AnswerOne, dataAnsw.AnswerTwo, dataAnsw.AnswerThree, dataAnsw.AnswerFour, dataAnsw.CorrectAnswer, dataAnsw.AnswerType);
        return quizFourFeature;
    }
    private QuizAnswer GenerateQuizAnswerObject(QuizDataAnswer dataAnsw)
    {
        QuizObjectAnswer quizObjectFeature = new(dataAnsw.ObjectAnswer,dataAnsw.RightObjectTag, dataAnsw.AnswerType);
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
