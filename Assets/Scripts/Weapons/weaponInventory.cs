using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponInventory : MonoBehaviour
{
    public List<GameObject> weapons = new List<GameObject>();
    [SerializeField] private int currentWeaponIndex;
    [SerializeField] private GameObject currentWeapon;


    InteractionInputActions interactionInput;

    private void Awake()
    {
        interactionInput = new InteractionInputActions();

        interactionInput.Combat.WeaponSlot1.performed += takeOutWeapon1 => takeOutWeapon(0);
        interactionInput.Combat.WeaponSlot1.performed += takeOutWeapon2 => takeOutWeapon(1);
        interactionInput.Combat.WeaponSlot1.performed += takeOutWeapon3 => takeOutWeapon(2);
        interactionInput.Combat.WeaponSlot1.performed += takeOutWeapon4 => takeOutWeapon(3);
        interactionInput.Combat.WeaponSlot1.performed += takeOutWeapon5 => takeOutWeapon(4);
    }

    private void OnEnable()
    {
        interactionInput.Enable();
    }

    private void OnDisable()
    {
        interactionInput.Disable(); 
    }



    private void Update()
    {
        handleWeapons();
    }

    void handleWeapons()
    {
        if (weapons.Count <= 0) return;

        if(interactionInput.Combat.scrollWheel.ReadValue<float>() > 0)
        {
            currentWeaponIndex++;
            if(currentWeaponIndex > weapons.Count-1) currentWeaponIndex = 0;
            takeOutWeapon(currentWeaponIndex);
        }
        else if(interactionInput.Combat.scrollWheel.ReadValue<float>() < 0)
        {
            currentWeaponIndex--;
            if(currentWeaponIndex < 0) currentWeaponIndex = weapons.Count - 1;

            takeOutWeapon(currentWeaponIndex);
        }
    }


    void takeOutWeapon(int weaponIndex)
    {
        if(weapons.Count <= 0 || weapons[weaponIndex] == null) return;

        //Play dequip animation


        foreach (GameObject weapon in weapons)
        {
            weapon.GetComponent<weaponBase>().weaponIsEquipped = false;
        }

        currentWeapon = weapons[weaponIndex];
        currentWeapon.GetComponent<weaponBase>().weaponIsEquipped = true;

        //Play animation for equipping weapon
    }
}
