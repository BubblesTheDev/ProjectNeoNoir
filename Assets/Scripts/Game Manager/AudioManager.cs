using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than  one audio manager in scene");
        }
        instance = this;
    }


    public void PlayPistolShot(EventReference pistolShoot, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(pistolShoot, worldPos);
    }
}
