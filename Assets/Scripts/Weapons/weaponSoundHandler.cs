using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponSoundHandler : MonoBehaviour
{
    public weaponType weaponType;
    private weaponBase weapon;

    private void Awake()
    {
        weapon = GetComponent<weaponBase>();

        weapon.gunFiredEvent.AddListener(playWeaponSound);
        weapon.powerActivated.AddListener(playPowerSound);
    }

    void playWeaponSound()
    {
        switch (weaponType)
        {
            case weaponType.revolver:
                AudioManager.instance.PlaySFX(FMODEvents.instance.pistolShot, this.transform.position);
                break;
            case weaponType.shotgun:
                AudioManager.instance.PlaySFX(FMODEvents.instance.shotgunShotNoCock, this.transform.position);
                break;
            case weaponType.sniper:
                break;
            case weaponType.rocketLauncher:
                break;
            case weaponType.machineGun:
                break;
        }
    }

    void playPowerSound()
    {
        switch (weaponType)
        {
            case weaponType.revolver:
                break;
            case weaponType.shotgun:
                break;
            case weaponType.sniper:
                break;
            case weaponType.rocketLauncher:
                break;
            case weaponType.machineGun:
                break;
        }
    }
}
