using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using TMPro;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public class QuizVideoQuestion : QuizQuestion
    {
        private GameObject videoButton;
        private VideoPlayer videoPlayer;
        private GameObject placeHolderVideo;

        public QuizVideoQuestion(QuizDataQuestion quizDataQuestion, TextMeshProUGUI textTitle, GameObject videoButton, VideoPlayer videoPlayer, GameObject placeHolderVideo):base(quizDataQuestion, textTitle)
        {
            VideoButton = videoButton;
            VideoPlayer = videoPlayer;
            PlaceHolderVideo = placeHolderVideo;
            QuestionType = QuizQuestionType.Video;
        }

        public GameObject VideoButton { get => videoButton; set => videoButton = value; }
        public VideoPlayer VideoPlayer { get => videoPlayer; set => videoPlayer = value; }
        
        public GameObject PlaceHolderVideo { get => placeHolderVideo; set => placeHolderVideo = value; }
        public override void Init()
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = QuizDataQuestion.VideoToShowPath;
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare(); 
        }
        void OnVideoPrepared(VideoPlayer vp)
        {
            vp.gameObject.SetActive(true);
            placeHolderVideo.SetActive(true);
            float videoHeight = vp.texture.height;
            float videoWidth = vp.texture.width;
            float aspectRatio = videoWidth / videoHeight;
            vp.transform.localScale = new Vector3(0.4f, 1 / aspectRatio, 0.4f);
            ResetVideo(vp);
            var btn = videoButton.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                TogglePlayPause(vp);
            });
            vp.loopPointReached += ResetVideo;
        }
        private void ResetVideo(VideoPlayer vp)
        {
            vp.time = 0;
            vp.Play();
            vp.Pause();
        }

        public void TogglePlayPause(VideoPlayer videoPlayer)
        {
            bool isPlaying = !videoPlayer.isPlaying;
            SetPlay(videoPlayer, isPlaying);
            placeHolderVideo.SetActive(!isPlaying);
        }
        public void SetPlay(VideoPlayer videoPlayer, bool value)
        {
            if (value)
                videoPlayer.Play();
            else
                videoPlayer.Pause();
        }


    }
}
