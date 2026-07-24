using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceObject : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip clip;  //재생할 클립
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Init();
    }
    public void Init()
    {
        clip = null;
    }
    public async UniTaskVoid PlayClip(AudioClip clip, Action onComplete = null)
    {
        await PlaySoundAsync(clip);
        onComplete?.Invoke();
    }
    private async UniTask PlaySoundAsync(AudioClip audioClip)
    {
        clip = audioClip;
        audioSource.clip = clip;
        audioSource.PlayOneShot(clip);
        await UniTask.Delay(TimeSpan.FromSeconds(clip.length));
    }
}
