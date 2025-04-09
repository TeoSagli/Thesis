using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Video;
using UnityEngine;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public  class QuizImageFeature : QuizQuestion
    {
        [SerializeField] private Sprite imageToShow;
        [SerializeField] private SpriteRenderer imageQuestion;
      
      public SpriteRenderer ImageQuestion { get => imageQuestion; set => imageQuestion = value; }
      public Sprite ImageToShow { get => imageToShow; set => imageToShow = value; }
        
    }
}
