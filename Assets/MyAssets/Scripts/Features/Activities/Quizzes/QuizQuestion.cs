using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    public abstract class QuizQuestion
    {
        [SerializeField, Header("Title (max 78 chars)")]
        private TextMeshProUGUI textTitle;
        [SerializeField]
        private string textTitleToShow;
        public abstract void Init();
        public TextMeshProUGUI TextTitle { get => textTitle; set => textTitle = value; }
        public string TextTitleToShow { get => textTitleToShow; set => textTitleToShow = value; }
    }
}
