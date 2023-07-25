using UnityEngine;

namespace Recorder
{
    public abstract class View : MonoBehaviour
    {
        public abstract void OnStartRecording();
        public abstract void OnStopRecording();
        public abstract void OnRecordingSaved(string message);
    }
}
