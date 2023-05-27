using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
public class BGMPlayer : MonoBehaviour
{

    private List<string> musicFile = new List<string>();
    private string chosenOne;
    private AudioSource _audioSource;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        musicFile.Add("https://drive.google.com/uc?export=download&id=1PLsM1fnu1RWQW8yXdQfuP1I0j9HXDUWg");
        musicFile.Add("https://drive.google.com/uc?export=download&id=1Zu8KbkgcAgoNDIdYhKchkSXfTz4wxM2q");
        musicFile.Add("https://drive.google.com/uc?export=download&id=1W_OKsxDq4UhX71soWlZHDFOeFGKflJTb");
        musicFile.Add("https://drive.google.com/uc?export=download&id=12pMSCAENU2gDiB5MTLCKTQapQZ34d1x4");
        musicFile.Add("https://drive.google.com/uc?export=download&id=149p5LX8ZfOLdj3FBbf5wfCl3LYpVC7fJ");
        StartCoroutine(MusicPlayer(musicFile[Random.Range(0,musicFile.Count)]));
        
    }

    public static string GetFileLocation(string relativePath)
    {
        return "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);
    }

    IEnumerator MusicPlayer(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                _audioSource = gameObject.GetComponent<AudioSource>();
                if (_audioSource == null)
                {
                    _audioSource = gameObject.AddComponent<AudioSource>();
                }
                else if (_audioSource.clip != null)
                {
                    _audioSource.Stop();
                    AudioClip currentClip = _audioSource.clip;
                    _audioSource.clip = null;
                    currentClip.UnloadAudioData();
                    DestroyImmediate(currentClip,false);
                }

                _audioSource.loop = false;
                _audioSource.clip = DownloadHandlerAudioClip.GetContent((uwr));
                _audioSource.PlayOneShot(_audioSource.clip);
                yield return null;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (_audioSource != null && !_audioSource.isPlaying)
        {
            chosenOne = musicFile[Random.Range(0, musicFile.Count)];
            StartCoroutine(MusicPlayer(chosenOne));
        }
    }
}
