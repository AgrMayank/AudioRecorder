using UnityEngine;

namespace Mayank.AudioRecorder.Recorder.View
{
    
    // ToDo: Add comments
    public abstract class RecorderView : MonoBehaviour
    {
        public abstract void OnStartRecording();
        public abstract void OnStopRecording();
        public abstract void OnRecordingSaved(string message);
    }
}
