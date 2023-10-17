using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Pistol Shot SFX")]
    [field: SerializeField] public EventReference pistolShot { get; private set; }
    [field: Header("Shotgun Shot SFX")]
    [field: SerializeField] public EventReference shotgunShotNoCock { get; private set; }
    [field: SerializeField] public EventReference shotgunShotCock { get; private set; }

    public static FMODEvents instance {  get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }
}
