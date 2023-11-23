using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    public Texture[] imageArray;
    private int CurrentImage;

    float deltaTime = 0.0f;
    public float timer = 5.0f;
    public float timerRemaining = 5.0f;
    public bool timerIsRunning = false;
    public string timerText;


    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        Rect imageRect = new Rect(0, 0, Screen.width, Screen.height);
        GUI.DrawTexture(imageRect, imageArray[CurrentImage]);
    }
    // Start is called before the first frame update
    void Start()
    {
        CurrentImage = 0;
        bool timerIsRunning = true;
        timerRemaining = timer;
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * .1f;
        if (Input.GetKeyDown(KeyCode.Space)) {
            CurrentImage++;
            if (CurrentImage >= imageArray.Length) {
                SceneManager.LoadScene(1);
            }
        }
    }
}
