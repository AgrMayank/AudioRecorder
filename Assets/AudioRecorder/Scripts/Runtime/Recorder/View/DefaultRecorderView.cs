using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mayank.AudioRecorder.Recorder.View
{
    public class DefaultRecorderView : RecorderView
    {
        /// <summary>
        /// Show the Filepath on the screen, etc 
        /// </summary>
        [SerializeField] private TMP_Text consoleText;
        
        /// <summary>
        /// Show the Recording Time on the screen
        /// </summary>
        [SerializeField] private TMP_Text recordingTimeText;

        /// <summary>
        /// Record Image for the Record Button
        /// </summary>
        [SerializeField] private Image recordImage;

        /// <summary>
        /// Save Image for the Record Button
        /// </summary>
        [SerializeField] private Image saveImage;
        
        /// <summary>
        /// Set a Button to trigger recording or saving the Audio WAV file 
        /// </summary>
        [SerializeField] private Button recordButton;
        
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

        /// <summary>
        /// Clears the consoleText.
        /// </summary>
        private void OnEnable()
        {
            consoleText.text = "";
        }

        /// <summary>
        /// Scales the record button.
        /// </summary>
        /// <param name="button">Record button.</param>
        /// <param name="scaleFactor">The amount of changing scale of the record button.</param>
        /// <returns>IEnumerator object.</returns>
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
        
        /// <summary>
        /// Updates the recording time.
        /// </summary>
        /// <returns>IEnumerator object.</returns>
        private IEnumerator UpdateRecordingTime()
        {
            while (Core.AudioRecorder.IsRecording)
            {
                consoleText.text = "";
                CalculateMinuteAndSecond();
                
                if (_minute < 10)
                {
                    if (_second < 10) recordingTimeText.text = "0" + _minute + ":0" + _second;
                    else recordingTimeText.text = "0" + _minute + ":" + _second;
                }
                else if (_second < 10) recordingTimeText.text = _minute + ":0" + _second;
                else recordingTimeText.text = _minute + ":" + _second;
        
                yield return new WaitForSeconds(1);
            }
        }

        /// <summary>
        /// Calculates minutes and seconds of recording time.
        /// </summary>
        private void CalculateMinuteAndSecond()
        {
            _minute = (int)(Core.AudioRecorder.RecordingTime / 60);
            _second = (int)(Core.AudioRecorder.RecordingTime % 60);
        }

        /// <summary>
        /// It will be called after recording has been started.
        /// </summary>
        public override void OnStartRecording()
        {
            StartCoroutine(ScaleOverTime(recordButton.gameObject, 1.2f));
            recordingTimeText.text = "00:00";
            recordImage.gameObject.SetActive(true);
            saveImage.gameObject.SetActive(false);
            _timeUpdateRoutine = StartCoroutine(nameof(UpdateRecordingTime));
        }

        /// <summary>
        /// It will be called after recording has been stopped.
        /// </summary>
        public override void OnStopRecording()
        {
            StartCoroutine(ScaleOverTime(recordButton.gameObject, 1f));
            recordingTimeText.text = "00:00";
            recordImage.gameObject.SetActive(true);
            saveImage.gameObject.SetActive(false);
            StopCoroutine(_timeUpdateRoutine);
        }
        
        /// <summary>
        /// It will be called after recording has been saved.
        /// </summary>
        public override void OnRecordingSaved(string message) => consoleText.text = message;
    }
}