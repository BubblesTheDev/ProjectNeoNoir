using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UIElements;
using System;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;

    private EventInstance musicEventInstance;

    FMOD.Studio.EventInstance slidingSFX;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
    }

    private void Start()
    {
        InitializeMusic(FMODEvents.instance.battleMusic);
    }

    public void PlaySFX(EventReference SFX, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(SFX, worldPos);
    } 

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = EventInstance(musicEventReference);
        musicEventInstance.start();
    }

    public EventInstance EventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public void PlaySlideSFX(Boolean sliding)
    {
        if (sliding == true)
        {
            slidingSFX = RuntimeManager.CreateInstance(FMODEvents.instance.slideSFX);
            slidingSFX.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            slidingSFX.start();
        }

        if (sliding == false)
        {
           slidingSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Debug.Log("stop sliding");
        }
     
    }

    private void CleanUp()
    {
        //stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
