using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mayank.AudioRecorder.Recorder.View
{
    public class DefaultView : View
    {
        /// <summary>
        /// Show the Filepath on the screen, etc 
        /// </summary>
        [SerializeField] protected TMP_Text _consoleText;
        
        /// <summary>
        /// Show the Recording Time on the screen
        /// </summary>
        [SerializeField] protected TMP_Text _recordingTimeText;

        /// <summary>
        /// Record Image for the Record Button
        /// </summary>
        [SerializeField] protected Image _recordImage;

        /// <summary>
        /// Save Image for the Record Button
        /// </summary>
        [SerializeField] protected Image _saveImage;
        
        /// <summary>
        /// Set a Button to trigger recording or saving the Audio WAV file 
        /// </summary>
        [SerializeField] private Button _recordButton;
        
        /// <summary>
        /// The coroutine that updates time text.
        /// </summary>
        private Coroutine _timeUpdateRoutine;

        /// <summary>
        /// Recording Time-Minute
        /// </summary>
        private int _minute = 0;
        
        /// <summary>
        /// Recording Time-Seconds
        /// </summary>
        private int _second = 0;

        private void OnEnable()
        {
            _consoleText.text = "";
        }

        private IEnumerator ScaleOverTime(GameObject button, float scaleFactor)
        {
            var originalScale = button.transform.localScale;
            var destinationScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            var currentTime = 0.0f;

            do
            {
                button.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / 0.5f);
                currentTime += Time.deltaTime;
                yield return null;
            }
            while (currentTime <= 1f);
        }
        
        private IEnumerator UpdateRecordingTime()
        {
            while (Core.AudioRecorder.IsRecording)
            {
                _consoleText.text = "";
                CalculateMinuteAndSecond();
                
                if (_minute < 10)
                {
                    if (_second < 10) _recordingTimeText.text = "0" + _minute + ":0" + _second;
                    else _recordingTimeText.text = "0" + _minute + ":" + _second;
                }
                else if (_second < 10) _recordingTimeText.text = _minute + ":0" + _second;
                else _recordingTimeText.text = _minute + ":" + _second;
        
                yield return new WaitForSeconds(1);
            }
        }

        private void CalculateMinuteAndSecond()
        {
            _minute = (int)(Core.AudioRecorder.RecordingTime / 60);
            _second = (int)(Core.AudioRecorder.RecordingTime % 60);
        }

        public override void OnStartRecording()
        {
            StartCoroutine(ScaleOverTime(_recordButton.gameObject, 1.2f));
            _recordingTimeText.text = "00:00";
            _recordImage.gameObject.SetActive(true);
            _saveImage.gameObject.SetActive(false);
            _timeUpdateRoutine = StartCoroutine(nameof(UpdateRecordingTime));
        }

        public override void OnStopRecording()
        {
            StartCoroutine(ScaleOverTime(_recordButton.gameObject, 1f));
            _recordingTimeText.text = "00:00";
            _recordImage.gameObject.SetActive(true);
            _saveImage.gameObject.SetActive(false);
            StopCoroutine(_timeUpdateRoutine);
        }
        
        public override void OnRecordingSaved(string message)
        {
            _consoleText.text = message;
        }
    }
}