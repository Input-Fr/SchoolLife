using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using PlayerScripts;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class BGMPlayer : MonoBehaviour
{

    private List<string> musicFile = new List<string>();
    private AudioSource _audioSource;
    private bool firstTime = true;
    private bool isSelected;
    // Start is called before the first frame update
    void Start()
    {
        musicFile.Add("https://cdn.discordapp.com/attachments/500246166428975105/1112143936761385061/Synthwave_Retrowave_-_Last_Stop_Royalty_Free_Copyright_Safe_Music1.mp3");
        musicFile.Add("https://cdn.discordapp.com/attachments/500246166428975105/1112143834286141490/80s_Retrowave_Synthwave_Music_-_Hackers_by_Karl_Casey_Royalty_Free_Copyright_Safe_Music1.mp3");
        musicFile.Add("https://cdn.discordapp.com/attachments/500246166428975105/1112143878066286683/80s_Synthwave_Chill_Synth_-_Worship_the_Night_Royalty_Free_No_Copyright_Background_Music1.mp3");
        musicFile.Add("https://cdn.discordapp.com/attachments/500246166428975105/1112143929400369303/Krosia_-_Azur1.mp3");
        musicFile.Add("https://cdn.discordapp.com/attachments/500246166428975105/1112143936761385061/Synthwave_Retrowave_-_Last_Stop_Royalty_Free_Copyright_Safe_Music1.mp3");
        //musicFile.Add("https://cdn.discordapp.com/attachments/500246166428975105/1112270446621110272/KLAXON_-_SOUND_EFFECT_HD2.mp3");
        StartCoroutine(MusicPlayer(musicFile[Random.Range(0,musicFile.Count)]));
        firstTime = false;
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
                Debug.Log("entered the statement");
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

    private void OnEnable()
    {
        if (!firstTime) StartCoroutine(MusicPlayer(musicFile[Random.Range(0,musicFile.Count)]));
    }

    private IEnumerator putSelection()
    {
        yield return new WaitForSeconds(5);
        isSelected = false;
        
    }
    
    private void Update()
    {
        if (!PlayerManager.LocalInstance.inventoryManager.hasHeadphone)
        {
            gameObject.SetActive(false);
        }

        if (_audioSource != null && !_audioSource.isPlaying && !isSelected)
        {
            StartCoroutine(MusicPlayer(musicFile[Random.Range(0,musicFile.Count)]));
            isSelected = true;
            StartCoroutine(putSelection());
        }
    }
    
}
