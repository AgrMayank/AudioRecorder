using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Recorder
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

        private Coroutine _timeUpdateRoutine;

        /// <summary>
        /// Recording Time-Minute
        /// </summary>
        private int _minute = 0;
        
        /// <summary>
        /// Recording Time-Seconds
        /// </summary>
        private int _second = 0;

        public override void Init(Recorder recorder)
        {
            base.Init(recorder);
            _consoleText.text = "";
        }

        private void SetRecordingTimeTextValue(string minute, string second)
        {
            
        }
        
        IEnumerator UpdateRecordingTime()
        {
            while (_recorder.isRecording)
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
            _minute = (int)(_recorder.recordingTime / 60);
            _second = (int)(_recorder.recordingTime % 60);
        }

        public override void OnStartRecording()
        {
            _recordingTimeText.text = "00:00";
            _recordImage.gameObject.SetActive(true);
            _saveImage.gameObject.SetActive(false);
            _timeUpdateRoutine = StartCoroutine(nameof(UpdateRecordingTime));
        }

        public override void OnStopRecording()
        {
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