using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Recorder
{
    public abstract class View : MonoBehaviour
    {
        protected Recorder _recorder;



        public abstract void Init(Recorder recorder);
        public abstract void OnStartRecording();
        public abstract void OnStopRecording();
        public abstract void OnRecordingSaved(string message);

        // /// <summary>
        // /// Show the Filepath on the screen, etc 
        // /// </summary>
        // // public TMP_Text ConsoleText;
        // [SerializeField] protected TMP_Text _consoleText;
        //
        // /// <summary>
        // /// Show the Recording Time on the screen
        // /// </summary>
        // [SerializeField] protected TMP_Text _recordingTimeText;
        //
        // /// <summary>
        // /// Record or Save Image for the Record Button
        // /// </summary>
        // [SerializeField] protected Image _recordImage;
        //
        // [SerializeField] protected Image _saveImage;
        //
        // private Coroutine _timeUpdateRoutine;
        //
        //
        // /// <summary>
        // /// Recording Time Minute and Seconds
        // /// </summary>
        // // private int minute = 0, second = 0;
        // // public int minute = 0, second = 0;
        // private int _minute = 0, _second = 0;
        //
        //
        // public void Init(Recorder recorder)
        // {
        //     _recorder = recorder;
        //     _consoleText.text = "";
        // }
        //
        //
        // private void SetRecordingTimeTextValue(string minute, string second)
        // {
        //     
        // }
        //
        //
        // IEnumerator UpdateRecordingTime()
        // {
        //     while (_recorder.isRecording)
        //     {
        //         _consoleText.text = "";
        //         // _recorder.minute = (int)(_recorder.recordingTime / 60);
        //         // _recorder.second = (int)(_recorder.recordingTime % 60);
        //
        //         CalculateMinuteAndSecond();
        //         
        //         
        //         // if (_recorder.minute < 10)
        //         if (_minute < 10)
        //         {
        //             // if (_recorder.second < 10)
        //             if (_second < 10)
        //             {
        //                 // _recordingTimeText.text = "0" + _recorder.minute + ":0" + _recorder.second;
        //                 _recordingTimeText.text = "0" + _minute + ":0" + _second;
        //             }
        //             else
        //             {
        //                 // _recordingTimeText.text = "0" + _recorder.minute + ":" + _recorder.second;
        //                 _recordingTimeText.text = "0" + _minute + ":" + _second;
        //             }
        //         }
        //         // else if (_recorder.second < 10)
        //         else if (_second < 10)
        //         {
        //             // _recordingTimeText.text = _recorder.minute + ":0" + _recorder.second;
        //             _recordingTimeText.text = _minute + ":0" + _second;
        //         }
        //         else
        //         {
        //             // _recordingTimeText.text = _recorder.minute + ":" + _recorder.second;
        //             _recordingTimeText.text = _minute + ":" + _second;
        //         }
        //
        //         yield return new WaitForSeconds(1);
        //     }
        // }
        //
        // private void CalculateMinuteAndSecond()
        // {
        //     // _recorder.minute = (int)(_recorder.recordingTime / 60);
        //     // _recorder.second = (int)(_recorder.recordingTime % 60);
        //     _minute = (int)(_recorder.recordingTime / 60);
        //     _second = (int)(_recorder.recordingTime % 60);
        // }
        //
        //
        // public virtual void OnStartRecording()
        // {
        //     _recordingTimeText.text = "00:00";
        //     _recordImage.gameObject.SetActive(true);
        //     _saveImage.gameObject.SetActive(false);
        //     _timeUpdateRoutine = StartCoroutine(nameof(UpdateRecordingTime));
        // }
        //
        // public virtual void OnStopRecording()
        // {
        //     _recordingTimeText.text = "00:00";
        //     _recordImage.gameObject.SetActive(true);
        //     _saveImage.gameObject.SetActive(false);
        //     StopCoroutine(_timeUpdateRoutine);
        // }
        //
        //
        // public virtual void OnRecordingSaved(string message)
        // {
        //     _consoleText.text = message;
        // }
    }
}
