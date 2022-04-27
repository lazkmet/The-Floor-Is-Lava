using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingMusic : MonoBehaviour
{
    public Sound introTrack;
    public Sound loopingAudio;
    private AudioSource current;
    private bool paused;
    private void Awake()
    {
        introTrack.source = gameObject.AddComponent<AudioSource>();
        introTrack.source.playOnAwake = true;
        introTrack.source.clip = introTrack.clip;
        introTrack.source.loop = false;
        introTrack.source.volume = introTrack.volume;
        introTrack.source.pitch = introTrack.pitch;

        loopingAudio.source = gameObject.AddComponent<AudioSource>();
        loopingAudio.source.playOnAwake = false;
        loopingAudio.source.clip = loopingAudio.clip;
        loopingAudio.source.loop = true;
        loopingAudio.source.volume = loopingAudio.volume;
        loopingAudio.source.pitch = loopingAudio.pitch;

        Reset();
    }
    private void Update()
    {
        if (!(current.isPlaying || paused)) {
            loopingAudio.source.Play();
            current = loopingAudio.source;          
        }
    }
    public void Reset()
    {
        if (current != null) { current.Stop(); }
        paused = false;
        current = introTrack.source;
        current.Play();
    }
    public void Pause()
    {
        paused = true;
        current.Pause();
    }
    public void UnPause() {
        paused = false;
        current.UnPause();
    } 
}
