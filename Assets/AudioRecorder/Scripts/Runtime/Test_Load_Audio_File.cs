using System.Collections;
using System.Collections.Generic;
using Mayank.AudioRecorder.Player;
using Mayank.AudioRecorder.Utility;
using UnityEngine;
using UnityEngine.Networking;

public class Test_Load_Audio_File : MonoBehaviour
{
    private string soundPath, audioName;
    
    
    
    
    [SerializeField] private AudioSource _audioSource;
    
    // Start is called before the first frame update
    async void Start()
    {

        StartCoroutine(GetAudioClip());
        
        
        
        
        //
        //
        // // FindObjectOfType<AudioPlayer>().SetAudioFile();
        //
        // var auCl = await FileReader.LoadAudioClip(
        //     @"C:\Users\start\AppData\LocalLow\AgrMayank\Audio Recorder\Audio 2023_08_09 11_33_33_5590.wav");
        //
        // _audioSource.clip = auCl;
        // _audioSource.Play();
        //
        //
        //
        // WWW request = GetAudioFromFile(soundPath, audioName);
        //
        // var audC = request.GetAudioClip();
        //
        //
        //
        // UnityWebRequestMultimedia

    }

    // private WWW GetAudioFromFile(string path, string audioName)
    // {
    //     
    // }
    //
    
    
    
    IEnumerator GetAudioClip()
    {
        // using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("https://www.my-server.com/audio.ogg", AudioType.OGGVORBIS))
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(
                   @"C:\Users\start\AppData\LocalLow\AgrMayank\Audio Recorder\Audio 2023_08_09 11_33_33_5590.wav"
                   , AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                
                
                Debug.Log("myClip.length    :::    "+myClip.length);
                
                
                
                _audioSource.clip = myClip;
                _audioSource.Play();
            }
        }
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
