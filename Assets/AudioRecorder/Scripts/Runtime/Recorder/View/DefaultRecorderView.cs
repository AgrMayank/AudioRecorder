using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mayank.AudioRecorder.Recorder.View
{
    public class DefaultRecorderView : RecorderView
    {
        /// <summary>
        /// Displays logs on the console text.
        /// </summary>
        [SerializeField] private TMP_Text consoleText;
        
        /// <summary>
        /// Displays the recording time on the screen.
        /// </summary>
        [SerializeField] private TMP_Text recordingTimeText;

        /// <summary>
        /// Record image for the record button.
        /// </summary>
        [SerializeField] private Image recordImage;

        /// <summary>
        /// Save Image for the Record Button.
        /// </summary>
        [SerializeField] private Image saveImage;
        
        /// <summary>
        /// Button component used to trigger recording or saving of the audio file.
        /// </summary>
        [SerializeField] private Button recordButton;
        
        // /// <summary>
        // /// The coroutine that updates time text.
        // /// </summary>
        // private Coroutine _timeUpdateRoutine;

        /// <summary>
        /// The minutes of recording time.
        /// Represents the number of minutes in the recording time.
        /// </summary>
        private int _minute = 0;
        
        /// <summary>
        /// Represents the number of seconds in the recording time.
        /// </summary>
        private int _second = 0;

        /// <summary>
        /// Clears the console text by setting it to an empty string.
        /// </summary>
        private void OnEnable()
        {
            consoleText.text = "";
        }

        /// <summary>
        /// Scales a game object over time using a Lerp function.
        /// </summary>
        /// <param name="button">The game object to scale.</param>
        /// <param name="scaleFactor">The amount to scale the game object by.</param>
        private async UniTask ScaleOverTime(GameObject button, float scaleFactor)
        {
            var originalScale = button.transform.localScale;
            var destinationScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            var currentTime = 0.0f;

            do
            {
                button.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / 0.5f);
                currentTime += Time.deltaTime;
                await UniTask.Delay(1);
            }
            while (currentTime <= 1f);
        }
        
        /// <summary>
        /// Updates the recording time by calculating the number of minutes and seconds elapsed since the start of the recording and displaying the result on the recording time text.
        /// </summary>
        /// <returns>An IEnumerator object.</returns>
        // private IEnumerator UpdateRecordingTime()
        private async UniTask UpdateRecordingTime()
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
        
                // yield return new WaitForSeconds(1);

                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
        }

        /// <summary>
        /// Calculates the number of minutes and seconds elapsed since the start of the recording time.
        /// </summary>
        private void CalculateMinuteAndSecond()
        {
            _minute = (int)(Core.AudioRecorder.RecordingTime / 60);
            _second = (int)(Core.AudioRecorder.RecordingTime % 60);
        }

        /// <summary>
        /// Called when the recording process is started.
        /// </summary>
        public override void OnStartRecording()
        {
            ScaleOverTime(recordButton.gameObject, 1.2f);
            recordingTimeText.text = "00:00";
            recordImage.gameObject.SetActive(true);
            saveImage.gameObject.SetActive(false);
            // _timeUpdateRoutine = StartCoroutine(nameof(UpdateRecordingTime));
            // _timeUpdateRoutine = UpdateRecordingTime();
            UpdateRecordingTime();
        }

        /// <summary>
        /// Called when the recording process is stopped.
        /// </summary>
        public override void OnStopRecording()
        {
            ScaleOverTime(recordButton.gameObject, 1f);
            recordingTimeText.text = "00:00";
            recordImage.gameObject.SetActive(true);
            saveImage.gameObject.SetActive(false);
            // StopCoroutine(_timeUpdateRoutine);
        }
        
        /// <summary>
        /// Called when the recording process is saved.
        /// </summary>
        /// <param name="message">The message to display on the console text.</param>
        public override void OnRecordingSaved(string message) => consoleText.text = message;
    }
}