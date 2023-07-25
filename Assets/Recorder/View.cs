using UnityEngine;

namespace Recorder
{
    public abstract class View : MonoBehaviour
    {
        protected Recorder _recorder;
        
        public virtual void Init(Recorder recorder) => _recorder = recorder;
        public abstract void OnStartRecording();
        public abstract void OnStopRecording();
        public abstract void OnRecordingSaved(string message);
    }
}
