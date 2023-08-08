using UnityEngine;

namespace Mayank.AudioRecorder.Recorder.View
{
    /// <summary>
    /// Abstract class that defines the interface for recorder views.
    /// </summary>
    public abstract class RecorderView : MonoBehaviour
    {
        /// <summary>
        /// Called when the recording process is started.
        /// </summary>
        public abstract void OnStartRecording();
        
        /// <summary>
        /// Called when the recording process is stopped.
        /// </summary>
        public abstract void OnStopRecording();
        
        /// <summary>
        /// Called when the recording process is saved.
        /// </summary>
        /// <param name="message">The message to display on the console text.</param>
        public abstract void OnRecordingSaved(string message);
    }
}