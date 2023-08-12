namespace Mayank.AudioRecorder.Utility.Result
{
    public class ResultModel<T>
    {
        /// <summary>
        /// The status of the file writing operation.
        /// </summary>
        public bool status;
        
        /// <summary>
        /// The result of the file writing operation.
        /// </summary>
        // public string result = null;
        public T result;
        
        /// <summary>
        /// The error message, if any, encountered during the file writing operation.
        /// </summary>
        public string error = null;
    }
}
