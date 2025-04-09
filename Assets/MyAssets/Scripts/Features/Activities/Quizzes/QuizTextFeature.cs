using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public class QuizTextFeature : QuizQuestion
    {
        [SerializeField] private TextMeshProUGUI textQuestion;
        [SerializeField] private string textToShow;

        public TextMeshProUGUI TextQuestion { get => textQuestion; set => textQuestion = value; }
        public string TextToShow { get => textToShow; set => textToShow = value; }

        public override void Init()
        {
            textQuestion.text = textToShow;
        }
    }
}
