using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public class QuizObjectFeature : QuizAnswer
    {
        [SerializeField] private GameObject objectAnswer;
        [SerializeField] private string rightObjectTag;
        public QuizObjectFeature()
        {
            NAnswers = 0;
        }

        public GameObject ObjectAnswer { get => objectAnswer; set => objectAnswer = value; }
        public string RightObjectTag { get => rightObjectTag; set => rightObjectTag = value; }
    }
}
