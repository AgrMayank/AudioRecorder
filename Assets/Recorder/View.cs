using UnityEngine;

namespace Recorder
{
    public abstract class View : MonoBehaviour
    {
        protected AudioRecordHandler audioRecordHandler;
        
        public virtual void Init(AudioRecordHandler audioRecordHandler) => this.audioRecordHandler = audioRecordHandler;
        public abstract void OnStartRecording();
        public abstract void OnStopRecording();
        public abstract void OnRecordingSaved(string message);
    }
}
