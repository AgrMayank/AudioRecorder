using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Recorder
{
    public class View : MonoBehaviour
    {

        protected Recorder _recorder;
        
        
        
        
        /// <summary>
        /// Show the Filepath on the screen, etc 
        /// </summary>
        // public TMP_Text ConsoleText;
        [SerializeField] protected TMP_Text _consoleText;
        
        
        
        
        
        
        /// <summary>
        /// Show the Recording Time on the screen
        /// </summary>
        [SerializeField] protected TMP_Text _recordingTimeText;




        /// <summary>
        /// Record or Save Image for the Record Button
        /// </summary>
        // public Image RecordImage, SaveImage;
        // [SerializeField] protected Image RecordImage, SaveImage;
        [SerializeField] protected Image _recordImage;
        [SerializeField] protected Image _saveImage;








        private Coroutine _timeUpdateRoutine;
        

        // public void Init()
        public void Init(Recorder recorder)
        {
            _recorder = recorder;
            // ConsoleText.text = "";
            _consoleText.text = "";
            
            
        }

        public void UpdateView()
        {
            
        }


        protected void Update()
        {
            
        }


        // // private void SetTexts()




        private void SetRecordingTimeTextValue(string minute, string second)
        {
            
        }
        
        
        // public void SetTexts()
        // public void UpdateRecordingTime()
        IEnumerator UpdateRecordingTime()
        {
            // if (isRecording)
            // if (_recorder.isRecording)
            while (_recorder.isRecording)
            {
                _consoleText.text = "";
                _recorder.recordingTime += Time.deltaTime;
        
                _recorder.minute = (int)(_recorder.recordingTime / 60);
                _recorder.second = (int)(_recorder.recordingTime % 60);
        
                if (_recorder.minute < 10)
                {
                    if (_recorder.second < 10)
                    {
                        _recordingTimeText.text = "0" + _recorder.minute + ":0" + _recorder.second;
                    }
                    else
                    {
                        _recordingTimeText.text = "0" + _recorder.minute + ":" + _recorder.second;
                    }
                }
                else if (_recorder.second < 10)
                {
                    _recordingTimeText.text = _recorder.minute + ":0" + _recorder.second;
                }
                else
                {
                    _recordingTimeText.text = _recorder.minute + ":" + _recorder.second;
                }
        
                yield return new WaitForSeconds(1);
                
                
                // _recordImage.gameObject.SetActive(false);
                // _saveImage.gameObject.SetActive(true);
            }



            // yield return new WaitForSeconds(1);

            // UpdateRecordingTime();


            // else
            // {
            //     _recordingTimeText.text = "00:00";
            //
            //     _recordImage.gameObject.SetActive(true);
            //     _saveImage.gameObject.SetActive(false);
            // }
        }





        public virtual void OnStartRecording()
        {
            _recordingTimeText.text = "00:00";
            _recordImage.gameObject.SetActive(true);
            _saveImage.gameObject.SetActive(false);
            _timeUpdateRoutine = StartCoroutine(nameof(UpdateRecordingTime));
        }

        public virtual void OnStopRecording()
        {
            _recordingTimeText.text = "00:00";
            _recordImage.gameObject.SetActive(true);
            _saveImage.gameObject.SetActive(false);
            StopCoroutine(_timeUpdateRoutine);
        }


        public virtual void OnRecordingSaved(string message)
        {
            // ConsoleText.text = message;
            _consoleText.text = message;
        }
        
        
    
    }
}
