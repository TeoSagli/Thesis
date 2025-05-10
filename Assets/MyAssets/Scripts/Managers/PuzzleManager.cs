using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.MyAssets.Scripts.Features.Activities;
using DilmerGames.Core.Singletons;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OVRSimpleJSON;
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
        StartCoroutine(ApiManager.GetJson(callback: ((b, s) => StartCoroutine(LoadPuzzles(b, s))),"1978"));
    }
    private IEnumerator LoadPuzzles(bool success, string jsonData)
    {
        int contPuzzles = 0;


        if (!success) { Debug.LogError("Error api"); yield break; }
        DataRoot rootData = null;
        try
        {
            JObject obj = JObject.Parse(jsonData);
            if (obj["customParameters"] == null)
            {
                Debug.Log("Error not parsed");
                yield break;
            }

            rootData = obj["customParameters"].ToObject<DataRoot>(new JsonSerializer()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        }
        catch (Exception e)
        {
            Debug.LogError("Parse error"+e);
        }
        if(rootData == null)
        {
            Debug.LogError("Root data null, json not parsed");
            yield break;
        }

        List<PuzzleData> puzzleData = new(rootData.puzzleData.Count);

        foreach (var puzzle in rootData.puzzleData)
        {
            puzzleData.Add(puzzle);
        }

        float startPos = 0.3f;
        float shift = 1.26f;
        //===== ACTIVITY: PUZZLE 2D======
        foreach (var puzzle in rootData.puzzleData2D)
        {
            if (!File.Exists(DownloadManager.GetLoacalPath(puzzle.SpriteId.ToString(), puzzle.Ext)))
            {
                StartCoroutine(DownloadManager.DownloadAndSaveMediaElement(puzzle.SpriteId, (s) =>
                {
                    if (s == null)
                    {
                        Debug.LogError("Puzzle 2D not instantiated");
                        return;
                    }
                    puzzle.SpritePath = s;
                }, puzzle.Ext));
            }
            else
            {
                puzzle.SpritePath = DownloadManager.GetLoacalPath(puzzle.SpriteId.ToString(), puzzle.Ext);
            }
            Generate2DPuzzle(puzzleData[contPuzzles], puzzle, new Vector3(startPos, 1.3f, 1.2f), Quaternion.identity);
            startPos += shift;
            contPuzzles++;
            yield return null;
        }
         startPos = -0.9f;
         shift = 1.26f;
        //===== ACTIVITY: PUZZLE 3D======
        foreach (var puzzle in rootData.puzzleData3D)
        {
            if (!File.Exists(DownloadManager.GetLoacalPath(puzzle.MeshId.ToString(), puzzle.Ext)))
            {
                StartCoroutine(DownloadManager.DownloadAndSaveMediaElement(puzzle.MeshId, (s) =>
                {
                    if (s == null)
                    {
                        Debug.LogError("Puzzle 3D not instantiated");
                        return;
                    }
                    puzzle.MeshPath = s;
                    
                }, puzzle.Ext));
            }
            else
            {
                puzzle.MeshPath = DownloadManager.GetLoacalPath(puzzle.MeshId.ToString(), puzzle.Ext);
            }
            Generate3DPuzzle(puzzleData[contPuzzles], puzzle, new Vector3(startPos, 1.3f, 1.2f), Quaternion.identity);
            startPos += shift;
            contPuzzles++;
            yield return null;
        }
        //===== ACTIVITY: QUIZ======
        startPos = 1.75f;
        shift = -0.9f;
        foreach (var quiz in rootData.quizData)
        {
            QuizDataQuestion quizDataQuestion = quiz.QuizDataQuestion;
            QuizDataAnswer quizDataAnswer = quiz.QuizDataAnswer;
            if (quizDataQuestion.QuestionType == "Video" || quizDataQuestion.QuestionType == "Image")
            {
                if (!File.Exists(DownloadManager.GetLoacalPath(quizDataQuestion.Id.ToString(), quizDataQuestion.Ext)))
                {
                    StartCoroutine(DownloadManager.DownloadAndSaveMediaElement(quizDataQuestion.Id, (s) =>
                    {
                        if (s == null)
                        {
                            Debug.LogError("Quiz not instantiated");
                            return;
                        }
                        quizDataQuestion.Path = s;
                    }, quizDataQuestion.Ext));
                }
                else
                {
                    quizDataQuestion.Path = DownloadManager.GetLoacalPath(quizDataQuestion.Id.ToString(), quizDataQuestion.Ext);
                }
            }
            GenerateQuiz(quiz, new Vector3(startPos, 1.3f, -0.2f), Quaternion.Euler(0, 180, 0));
            startPos += shift;
            yield return null;
        }
    }
    //===================PUZZLES=========================
    private void Generate2DPuzzle(PuzzleData data,PuzzleData2D data2D, Vector3 socketPos,Quaternion socketRot)
    {
        GameObject puzzle2D = new("PuzzlePiecesSpawner2D-" + data2D.SpriteName);
        Puzzle2D script2D = puzzle2D.AddComponent<Puzzle2D>();
        script2D.Init(data.PieceScale, data.TitleStr, data.NCols, data.NRows, data2D.SpritePath, data2D.SpriteName, data2D.SpriteId, data2D.Ext);
        GameObject socket2D = Instantiate(socket2DObject,socketPos,socketRot);
        Puzzle2DSocket scriptSocket2D = socket2D.GetComponent<Puzzle2DSocket>();
        scriptSocket2D.Initialize(script2D);
    }
    private void Generate3DPuzzle(PuzzleData data, PuzzleData3D data3D, Vector3 socketPos, Quaternion socketRot)
    {
        GameObject puzzle3D = new ("PuzzlePiecesSpawner3D-" + data3D.MeshName);
        Puzzle3D script3D = puzzle3D.AddComponent<Puzzle3D>();
        script3D.Init(data.PieceScale, data.TitleStr, data.NCols, data.NRows, data.NDepth, data3D.MeshPath, data3D.MeshName, data3D.MeshId, data3D.Ext, noCullShader);
        GameObject socket3D = Instantiate(socket3DObject, socketPos, socketRot);
        Puzzle3DSocket scriptSocket3D = socket3D.GetComponent<Puzzle3DSocket>();
        script3D.OnLoaded += (puzzle) => scriptSocket3D.Initialize(script3D);
    }
    //===================QUIZ=========================
    private void GenerateQuiz(QuizData quizData, Vector3 pos, Quaternion rot) {
        QuizDataAnswer quizDataAnswer = quizData.QuizDataAnswer;
        QuizDataQuestion quizDataQuestion = quizData.QuizDataQuestion;
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
