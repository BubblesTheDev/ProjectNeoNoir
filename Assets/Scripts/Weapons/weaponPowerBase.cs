using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPowerBase : MonoBehaviour
{
    public bool canUsePower;
    public float powerCooldown;


    public IEnumerator usePower()
    {
        yield return null;
    }
}
