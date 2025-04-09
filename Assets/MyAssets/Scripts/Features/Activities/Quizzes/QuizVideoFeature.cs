using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MyAssets.Scripts.Features.Activities
{
    [System.Serializable]
    public class QuizVideoFeature : QuizQuestion
    {
        [SerializeField] private VideoClip videoToShow;
        [SerializeField] private GameObject videoButton;
        [SerializeField] private VideoPlayer videoQuestion;
        [SerializeField] private GameObject placeHolderVideo;

        public GameObject VideoButton { get => videoButton; set => videoButton = value; }
        public VideoPlayer VideoQuestion { get => videoQuestion; set => videoQuestion = value; }
        public VideoClip VideoToShow { get => videoToShow; set => videoToShow = value; }
        public GameObject PlaceHolderVideo { get => placeHolderVideo; set => placeHolderVideo = value; }
        public void HandleVideo()
        {

            videoQuestion.gameObject.SetActive(true);
            placeHolderVideo.SetActive(true);
            float videoHeight = videoToShow.height;
            float videoWidth = videoToShow.width;
            float aspectRatio = videoWidth / videoHeight;
            videoQuestion.transform.localScale = new Vector3(0.4f, 1 / aspectRatio, 0.4f);
            videoQuestion.clip = videoToShow;
            ResetVideo(videoQuestion);
            var btn = videoButton.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                TogglePlayPause(videoQuestion);
            });
            videoQuestion.loopPointReached += ResetVideo;
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
