using System;
using System.Collections;
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

        PuzzleData puzzleData1 = rootData.puzzleData1;
        PuzzleData puzzleData2 = rootData.puzzleData2;
        PuzzleData puzzleData3 = rootData.puzzleData3;

        PuzzleData2D puzzleData2D1 = rootData.puzzleData2D1;
        PuzzleData2D puzzleData2D2 = rootData.puzzleData2D2;
        PuzzleData3D puzzleData3D = rootData.puzzleData3D;

        Debug.Log("Download p2d1 started");
        if (!File.Exists(DownloadManager.GetLoacalPath(puzzleData2D1.SpriteId.ToString(), "jpg")))
        {
            StartCoroutine(DownloadManager.DownloadAndSaveMediaElement(rootData.puzzleData2D1.SpriteId, (s) =>
            {
                if(s == null)
                {
                    Debug.LogError("Puzzle not instantiated");
                    return;
                }

                Debug.Log("Download p2d1 ended");
                puzzleData2D1.SpritePath = s;
                Generate2DPuzzle(puzzleData1, puzzleData2D1, new Vector3(0.3f, 1.3f, 1.2f), Quaternion.identity);
            }, "jpg"));
        }
        else
        {
            puzzleData2D1.SpritePath = DownloadManager.GetLoacalPath(puzzleData2D1.SpriteId.ToString(), "jpg");
            Generate2DPuzzle(puzzleData1, puzzleData2D1, new Vector3(0.3f, 1.3f, 1.2f), Quaternion.identity);
        }
        
        yield return null;

        Debug.Log("Download p2d2 started");
        StartCoroutine(DownloadManager.DownloadAndSaveMediaElement(rootData.puzzleData2D2.SpriteId, (s) =>
        {
            Debug.Log("Download p2d2 ended");
            puzzleData2D2.SpritePath = s;
            Generate2DPuzzle(puzzleData3, puzzleData2D2, new Vector3(1.56f, 1.3f, 1.2f), Quaternion.identity);
        }, "jpg"));

        /* PuzzleData puzzleData = new(0.1f, "Title to show", 2, 2, 2);
         PuzzleData2D puzzleData2D=new("C:/Users/matte/Desktop/Media/scuola.jpg","Scuola");*/

        StartCoroutine(DownloadManager.DownloadAndSaveMediaElement(rootData.puzzleData3D.MeshId, (s) =>
        {
            Debug.Log("Download p2d2 ended");
            puzzleData3D.MeshPath = s;
            Generate3DPuzzle(puzzleData2, puzzleData3D, new Vector3(-0.9f, 1.3f, 1.2f), Quaternion.identity);
        }, "glb"));
        
        //==============================================
        /*  QuizDataAnswer quizDataAnswer = new("11111111111111111","2222222222","33333333333333333","444444444444444",2);
          QuizDataAnswer quizDataAnswer2 = new("oooooooooo","rrrrrrrr",2);
          QuizDataQuestion quizDataQuestion = new("sdiofjmsdiofjmsdkiofsjkmipo");
          QuizDataQuestion quizDataQuestion2 = new("C:/Users/matte/Desktop/Media/scuola.jpg", "aaaaaaaaaaaaaaaaaaaaa");
          QuizDataQuestion quizDataQuestion3 = new("C:/Users/matte/Desktop/Media/NoLo in 100 secondi.mp4", "aaaaaaaaaaaaaaaaaaaaa");
        */
        QuizDataQuestion quizDataQuestion1 = rootData.quizDataQuestion1;
        QuizDataQuestion quizDataQuestion2 = rootData.quizDataQuestion2;
        QuizDataQuestion quizDataQuestion3 = rootData.quizDataQuestion3;
        QuizDataQuestion quizDataQuestion4 = rootData.quizDataQuestion4;
        QuizDataAnswer quizDataAnswer1 = rootData.quizDataAnswer1;
        QuizDataAnswer quizDataAnswer2 = rootData.quizDataAnswer2;
        QuizDataAnswer quizDataAnswer3 = rootData.quizDataAnswer3;
        QuizDataAnswer quizDataAnswer4 = rootData.quizDataAnswer4;

        GenerateQuiz(quizDataQuestion1, quizDataAnswer1, new Vector3(1.75f, 1.3f, -0.2f), Quaternion.Euler(0, 180, 0));
        GenerateQuiz(quizDataQuestion2, quizDataAnswer2, new Vector3(0.85f, 1.3f, -0.2f), Quaternion.Euler(0, 180, 0));
        GenerateQuiz(quizDataQuestion3, quizDataAnswer3, new Vector3(-0.05f, 1.3f, -0.2f), Quaternion.Euler(0, 180, 0));
        GenerateQuiz(quizDataQuestion4, quizDataAnswer4, new Vector3(-1f, 1.3f, -0.2f), Quaternion.Euler(0, 180, 0));

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
        script2D.Init(data.PieceScale,data.TitleStr,data.NCols,data.NRows,data2D.SpritePath,data2D.SpriteName,data2D.SpriteId);
        GameObject socket2D = Instantiate(socket2DObject,socketPos,socketRot);
        Puzzle2DSocket scriptSocket2D = socket2D.GetComponent<Puzzle2DSocket>();
        scriptSocket2D.Initialize(script2D);
    }
    private void Generate3DPuzzle(PuzzleData data, PuzzleData3D data3D, Vector3 socketPos, Quaternion socketRot)
    {
        GameObject puzzle3D = new ("PuzzlePiecesSpawner3D-" + data3D.MeshName);
        Puzzle3D script3D = puzzle3D.AddComponent<Puzzle3D>();
        script3D.Init(data.PieceScale, data.TitleStr, data.NCols, data.NRows, data.NDepth, data3D.MeshPath, data3D.MeshName, data3D.MeshId, noCullShader);
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
