using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Music")]
    [field: SerializeField] public EventReference battleMusic { get; private set; }

    [field: Header("GUN SFX")]
    [field: SerializeField] public EventReference pistolShot { get; private set; }
    [field: SerializeField] public EventReference shotgunShotNoCock { get; private set; }
    [field: SerializeField] public EventReference shotgunShotCock { get; private set; }
    [field: SerializeField] public EventReference enemyShot { get; private set; }

    [field: Header("Gravity Switch")]
    [field: SerializeField] public EventReference gravitySwitch { get; private set; }

    [field: Header("Enemy Spawn")]
    [field: SerializeField] public EventReference enemySpawn { get; private set; }

    [field: Header("Jump SFX")]
    [field: SerializeField] public EventReference jumpSFX { get; private set; }

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
