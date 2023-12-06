using FMOD;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    //  public SeekerLad mySeeker;
    public GameObject Seeker;
    public GameObject prefab;
    public int poolSize = 10;
    public List<SeekerAfterImage> afterImages;
    public int interval = 10;
    public int time = 0;
    // Start is called before the first frame update
    void Start()
    {
        Seeker = transform.root.GetComponent<GameObject>();
        afterImages = new List<SeekerAfterImage>(poolSize);
        for (int i = 0; i < poolSize; i++) {
            GameObject nextAfterImage = Instantiate(prefab);
            afterImages.Add(nextAfterImage.GetComponent<SeekerAfterImage>());
          nextAfterImage.GetComponent<SeekerAfterImage>().targetObject = Seeker.gameObject; // gets target gameobject
        //  nextAfterImage.GetComponent<SeekerAfterImage>().targetAnimator = Seeker.animator; // gets target animator
        }


    }

    // Update is called once per frame
    void Update()
    {
        time++;
        if (time > interval) { time = 0; AddafterImages(); }


    }

    void AddafterImages() {
        for(int i = 0; i < poolSize; i++)
        if (!afterImages[i].active){ afterImages[i].Activate(); break; }
    }
        
       
}
