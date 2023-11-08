using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.EventSystems;

public class gameSettings : MonoBehaviour
{
    public static gameSettings gameSettingsReference
    {
        get
        {
            if (gameSettingsReference == null)
            {
                if (GameObject.FindObjectOfType<gameSettings>() != null)
                    GameObject.FindObjectOfType<gameSettings>();
                else
                {
                    Debug.LogError("There is no Game Settings script in the scene \n <b>Please Add One To The Scene </b>");
                    Debug.Break();
                }
            }
            return gameSettingsReference;

        }
    }

    [Header("Screen Settings")]
    public TMP_Dropdown resolutionSetting;
    private Resolution[] resolutions;
    private List<Resolution> usableResolutions;
    private double currentRefreshRate;
    private screenState fullscreenState;
    public TMP_Dropdown screenStateSetting;
    private List<string> screenStateOptions;

    [Header("Mouse Settings")]
    [Space] public Toggle flipHorizontalMouseSetting;
    public bool flipHorizontalDefault;
    public Toggle flipVerticalMouseSetting;
    public bool flipVerticalDefault;
    public Slider horizontalMouseSensitivitySetting;
    public float maxHorizontalSensDefault, minHorizontalSensDefault, HorizontalSensDefault;
    public Slider verticalMouseSensitivitySetting;
    public float maxVerticalSensDefault, minVerticalSensDefault, VerticalSensDefault;

    [Header("CameraSettings")]
    [Space] public Slider FOVSetting;
    public float maxFOVSettingDefault, minFOVSettingDefault, FOVSettingDefault;
    public Slider screenShakeSetting;
    public float maxScreenShakeSettingDefault, minScreenShakeSettingDefault, screenShakeSettingDefault;

    [Header("Volume Settings")]
    [Space] public Slider soundEffectsVolumeSetting;
    public float maxSoundEffectVolumeSettingDefault, minSoundEffectVolumeSettingDefault, soundEffectVolumeSettingDefault;
    public Slider musicVolumeSetting;
    public float maxMusicVolumeSettingDefault, minMusicVolumeSettingDefault, musicVolumeSettingDefault;
    public Slider voicesVolumeSetting;
    public float maxVoicesVolumeSettingDefault, minVoicesVolumeSettingDefault, voicesVolumeSettingDefault;


    private void Awake()
    {
        resolutionSetup();
    }

    private void Start()
    {
        setupPlayerPrefs();
    }


    void setupPlayerPrefs()
    {
        #region Resolution Player Prefs
        if (!PlayerPrefs.HasKey("resolutionSetting")) PlayerPrefs.SetString("resolutionSetting", resolutionSetting.options[resolutionSetting.value].ToString());
        if (!PlayerPrefs.HasKey("screenStateSetting")) PlayerPrefs.SetString("screenStateSetting", screenStateSetting.options[screenStateSetting.value].ToString());
        #endregion

        #region Mouse Player Prefs
        if (!PlayerPrefs.HasKey("flipHoizontalMouseSetting")) PlayerPrefs.SetString("flipHoizontalMouseSetting", flipHorizontalMouseSetting.spriteState.ToString());
        if (!PlayerPrefs.HasKey("flipVerticalMouseSetting")) PlayerPrefs.SetString("flipVerticalMouseSetting", flipVerticalMouseSetting.spriteState.ToString()); ;
        if (!PlayerPrefs.HasKey("horizontalMouseSensitivitySetting")) PlayerPrefs.SetFloat("horizontalMouseSensitivitySetting", horizontalMouseSensitivitySetting.value);
        if (!PlayerPrefs.HasKey("verticalMouseSensitivitySetting")) PlayerPrefs.SetFloat("verticalMouseSensitivitySetting", verticalMouseSensitivitySetting.value);
        #endregion

        #region Camera Settings Player Prefs
        if (!PlayerPrefs.HasKey("FOVSettings")) PlayerPrefs.SetFloat("FOVSettings", FOVSetting.value);
        if (!PlayerPrefs.HasKey("ScreenShakeSettings")) PlayerPrefs.SetFloat("screenShakeSettings", screenShakeSetting.value);
        #endregion

        #region Volume Setting Player Prefs
        if (!PlayerPrefs.HasKey("soundEffectVolumeSetting")) PlayerPrefs.SetFloat("soundEffectVolumeSetting", soundEffectsVolumeSetting.value);
        if (!PlayerPrefs.HasKey("musicVolumeSetting")) PlayerPrefs.SetFloat("musicVolumeSetting", musicVolumeSetting.value);
        if (!PlayerPrefs.HasKey("voicesVolumeSetting")) PlayerPrefs.SetFloat("voicesVolumeSetting", voicesVolumeSetting.value);
        #endregion
    }

    void resolutionSetup()
    {
        if (resolutionSetting != null)
        {
            resolutions = Screen.resolutions;
            usableResolutions = new List<Resolution>();

            resolutionSetting.ClearOptions();
            currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;

            List<string> resolutionOptions = new List<string>();

            for (int i = 0; i < resolutions.Length - 1; i++)
            {
                if (resolutions[i].refreshRateRatio.value == currentRefreshRate)
                {
                    usableResolutions.Add(resolutions[i]);
                    resolutionOptions.Add(resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRateRatio.value.ToString());

                }
            }

            resolutionSetting.value = usableResolutions.IndexOf(usableResolutions.Find(x => x.height == Screen.height && x.width == Screen.width));
            resolutionSetting.RefreshShownValue();

            PlayerPrefs.SetString("resolutionSetting", resolutionSetting.options[resolutionSetting.value].ToString());
        }
        else
        {
            Debug.LogWarning("There is no dropdown for the resolution options. \n Please create a dropdown for this and assign it in this scene");
            Debug.Break();
        }

        if (screenStateSetting != null)
        {
            for (int i = 0; i < Enum.GetNames(typeof(screenState)).Length - 1; i++)
            {
                screenStateOptions.Add(Enum.GetName(typeof(screenState), i));
            }
            screenStateSetting.AddOptions(screenStateOptions);
            screenStateSetting.value = screenStateSetting.options.FindIndex(x => x.text == Screen.fullScreenMode.ToString());

            PlayerPrefs.SetString("screenStateSetting", screenStateSetting.options[screenStateSetting.value].ToString());
        }
        else
        {
            Debug.LogWarning("There is no dropdown for the screen state options. \n Please create a dropdown for this and assign it in this scene");
        }

    }

    public void setResolution(int index)
    {
        switch (fullscreenState)
        {
            case screenState.fullScreen:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Screen.SetResolution(usableResolutions[index].width, usableResolutions[index].height, true);
                break;
            case screenState.windowed:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(usableResolutions[index].width, usableResolutions[index].height, false);
                break;
            case screenState.borderlessWindowed:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.SetResolution(usableResolutions[index].width, usableResolutions[index].height, true);
                break;
        }

        PlayerPrefs.SetString("resolutionSetting", resolutionSetting.options[resolutionSetting.value].ToString());
    }

    public void setScreenState(int index)
    {
        switch (index)
        {
            case 0:
                fullscreenState = screenState.fullScreen;
                break;
            case 1:
                fullscreenState = screenState.windowed;
                break;
            case 2:
                fullscreenState = screenState.borderlessWindowed;
                break;
        }

        PlayerPrefs.SetString("screenStateSetting", fullscreenState.ToString());
    }
}

enum screenState
{
    fullScreen,
    windowed,
    borderlessWindowed
}


[CustomEditor(typeof(gameSettings))]
public class gameSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        gameSettings currentGameSettings = (gameSettings)target;


        EditorGUILayout.LabelField("Resolution Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.resolutionSetting = (TMP_Dropdown)EditorGUILayout.ObjectField("Resolution Dropdown Obj", currentGameSettings.resolutionSetting, typeof(TMP_Dropdown), true);
        currentGameSettings.screenStateSetting = (TMP_Dropdown)EditorGUILayout.ObjectField("Screenstate Dropdown Obj", currentGameSettings.screenStateSetting, typeof(TMP_Dropdown), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Mouse Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();

        #region Mouse Toggle Settings
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.flipHorizontalMouseSetting = (Toggle)EditorGUILayout.ObjectField("Flip H mouse Toggle Obj", currentGameSettings.flipHorizontalMouseSetting, typeof(Toggle), true);
        currentGameSettings.flipHorizontalDefault = (bool)EditorGUILayout.Toggle("Flip H mouse Bool", currentGameSettings.flipHorizontalDefault);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        currentGameSettings.flipVerticalMouseSetting = (Toggle)EditorGUILayout.ObjectField("Flip V mouse Toggle Obj", currentGameSettings.flipVerticalMouseSetting, typeof(Toggle), true);
        currentGameSettings.flipVerticalDefault = (bool)EditorGUILayout.Toggle("Flip V mouse Bool", currentGameSettings.flipVerticalDefault);
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Horizontal Mouse Sensitivity Block
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.horizontalMouseSensitivitySetting = (Slider)EditorGUILayout.ObjectField("H Mouse Sens SLider", currentGameSettings.horizontalMouseSensitivitySetting, typeof(Slider), true);
        currentGameSettings.HorizontalSensDefault = (float)EditorGUILayout.FloatField("H Mouse Sens Value", currentGameSettings.HorizontalSensDefault);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.maxHorizontalSensDefault = (float)EditorGUILayout.FloatField("Max", currentGameSettings.maxHorizontalSensDefault);
        currentGameSettings.minHorizontalSensDefault = (float)EditorGUILayout.FloatField("Min", currentGameSettings.minHorizontalSensDefault);
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Vertical Mouse Sensitivity Block
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.verticalMouseSensitivitySetting = (Slider)EditorGUILayout.ObjectField("V Mouse Sens SLider", currentGameSettings.verticalMouseSensitivitySetting, typeof(Slider), true);
        currentGameSettings.VerticalSensDefault = (float)EditorGUILayout.FloatField("V Mouse Sens Value", currentGameSettings.HorizontalSensDefault);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.maxVerticalSensDefault = (float)EditorGUILayout.FloatField("Max", currentGameSettings.maxVerticalSensDefault);
        currentGameSettings.minVerticalSensDefault = (float)EditorGUILayout.FloatField("Min", currentGameSettings.minVerticalSensDefault);
        EditorGUILayout.EndHorizontal();
        #endregion

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();


        EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();
        #region FOV Settings Block
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.FOVSetting = (Slider)EditorGUILayout.ObjectField("Fov Slider", currentGameSettings.FOVSetting, typeof(Slider), true);
        currentGameSettings.FOVSettingDefault = (float)EditorGUILayout.FloatField("Fov Value", currentGameSettings.FOVSettingDefault);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.maxFOVSettingDefault = (float)EditorGUILayout.FloatField("Max", currentGameSettings.maxFOVSettingDefault);
        currentGameSettings.minFOVSettingDefault = (float)EditorGUILayout.FloatField("Min", currentGameSettings.minFOVSettingDefault);
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Screen shake Settings Block
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.screenShakeSetting = (Slider)EditorGUILayout.ObjectField("Screenshake Slider", currentGameSettings.screenShakeSetting, typeof(Slider), true);
        currentGameSettings.screenShakeSettingDefault = (float)EditorGUILayout.FloatField("Screenshake Value", currentGameSettings.screenShakeSettingDefault);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.maxScreenShakeSettingDefault = (float)EditorGUILayout.FloatField("Max", currentGameSettings.maxScreenShakeSettingDefault);
        currentGameSettings.minScreenShakeSettingDefault = (float)EditorGUILayout.FloatField("Min", currentGameSettings.minScreenShakeSettingDefault);
        EditorGUILayout.EndHorizontal();
        #endregion
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Volume Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();
        #region Sound Effect Volume Settings Block
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.soundEffectsVolumeSetting = (Slider)EditorGUILayout.ObjectField("SFX Slider", currentGameSettings.soundEffectsVolumeSetting, typeof(Slider), true);
        currentGameSettings.soundEffectVolumeSettingDefault = (float)EditorGUILayout.FloatField("SFX Volume", currentGameSettings.soundEffectVolumeSettingDefault);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.maxSoundEffectVolumeSettingDefault = (float)EditorGUILayout.FloatField("Max", currentGameSettings.maxSoundEffectVolumeSettingDefault);
        currentGameSettings.minSoundEffectVolumeSettingDefault = (float)EditorGUILayout.FloatField("Min", currentGameSettings.minSoundEffectVolumeSettingDefault);
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Music Volume Settings Block
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.musicVolumeSetting = (Slider)EditorGUILayout.ObjectField("Music Slider", currentGameSettings.musicVolumeSetting, typeof(Slider), true);
        currentGameSettings.musicVolumeSettingDefault = (float)EditorGUILayout.FloatField("Music Volume", currentGameSettings.musicVolumeSettingDefault);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.maxMusicVolumeSettingDefault = (float)EditorGUILayout.FloatField("Max", currentGameSettings.maxMusicVolumeSettingDefault);
        currentGameSettings.minMusicVolumeSettingDefault = (float)EditorGUILayout.FloatField("Min", currentGameSettings.minMusicVolumeSettingDefault);
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Voice Volume Settings Block
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.voicesVolumeSetting = (Slider)EditorGUILayout.ObjectField("Voice Slider", currentGameSettings.voicesVolumeSetting, typeof(Slider), true);
        currentGameSettings.voicesVolumeSettingDefault = (float)EditorGUILayout.FloatField("Voice Volume", currentGameSettings.voicesVolumeSettingDefault);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        currentGameSettings.maxVoicesVolumeSettingDefault = (float)EditorGUILayout.FloatField("Max", currentGameSettings.maxVoicesVolumeSettingDefault);
        currentGameSettings.minVoicesVolumeSettingDefault = (float)EditorGUILayout.FloatField("Min", currentGameSettings.minVoicesVolumeSettingDefault);
        EditorGUILayout.EndHorizontal();
        #endregion
        EditorGUILayout.EndVertical();
    }

}


