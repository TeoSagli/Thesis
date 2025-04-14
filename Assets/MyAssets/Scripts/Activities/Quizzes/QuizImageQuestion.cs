using System.IO;
using TMPro;
using UnityEngine;
using static Enums;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public class QuizImageQuestion : QuizQuestion
    {
        
      private SpriteRenderer imageQuestion;

        public QuizImageQuestion(QuizDataQuestion quizDataQuestion, TextMeshProUGUI textTitle, SpriteRenderer imageQuestion):base(quizDataQuestion, textTitle)
        {
            this.imageQuestion = imageQuestion;
            QuestionType = QuizQuestionType.Image;
        }

        public SpriteRenderer ImageQuestion { get => imageQuestion; set => imageQuestion = value; }
      
        public override void Init()
        {
            imageQuestion.sprite = LoadFromPath(QuizDataQuestion.ImageToShowPath,"quizImage");
        }
        protected Sprite LoadFromPath(string path, string name)
        {
            if (File.Exists(path))
            {
                byte[] fileData = File.ReadAllBytes(path);
                Texture2D tex = new(2, 2);
                tex.LoadImage(fileData);
                Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                s.name = name;
                return s;
            }
            else
            {
                Debug.Log("File not found!");
                return null;
            }
        }
    }
}
