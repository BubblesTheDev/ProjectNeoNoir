using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField]
    private float dashCD;

    // needs to be attached to the functional dash timers and whatnot
    [SerializeField]
    private float dashTimer;

    private Vector3 currentBarScale;

    private Image im;

    // private Color red = Color.red;
    private Color yellow;

    // Start is called before the first frame update
    void Start()
    {
        dashCD = 0.4f;
        dashTimer = 0.4f;
        im = gameObject.GetComponent<Image>();
        yellow = im.color;
    }

    // Update is called once per frame
    void Update()
    {
        // sets stamina bar colour to red and fades back to original colour. can adjust later but we'll need to calculate values lol
        im.color = new Color(1, (dashTimer / dashCD) * yellow.g, (dashTimer / dashCD) * yellow.b);

        if (dashTimer < 0.4)
        {
            dashTimer += Time.deltaTime;
        }
        if(dashTimer >= 0.4)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                dashTimer = 0;
            }

        }
        currentBarScale = new Vector3((float)dashTimer / (float)dashCD, transform.localScale.y, transform.localScale.z);
        transform.localScale = currentBarScale;

    }
}
