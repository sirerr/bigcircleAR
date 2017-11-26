using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    // This script controls which system to use, ARKit or ARCore


    public static GameManager instance;
    //states of play
    public enum GameState {Dead, Start, Current, End};
    public GameState currentGameState;
    //platform
    public enum BuildDevicePlatform { Editor,IOS,Android};
    public BuildDevicePlatform currentDevicePlatform;
    //canvasese
    public Canvas StartCanvas;
    public Canvas mainCanvas;
    //cameras
    public List <GameObject> cams = new List<GameObject>();
    //references to players 

    //testing varaibles
  //  public Text DeviceTypeText;
    //testing variables

    public  Rect TouchArea = new Rect(0, Screen.height * .1f, Screen.width, Screen.height);

  public void ChoosingCams()
    {
        // compiling based on platform
#if UNITY_IOS
      //  DeviceTypeText.text = "IOS";
        currentDevicePlatform = BuildDevicePlatform.IOS;
#elif UNITY_EDITOR
        DeviceTypeText.text = "Editor";
#elif UNITY_ANDROID
        DeviceTypeText.text = "Android";
        currentDevicePlatform = BuildDevicePlatform.Android;
#endif

        //testing

        switch (currentDevicePlatform)
        {
            case BuildDevicePlatform.Android:
                {
                    cams[0].SetActive(false);
                    cams[2].SetActive(true);
                    // disable start camera
                    // enable arcore camera
                    break;
                }
            case BuildDevicePlatform.IOS:
                {
                    cams[0].SetActive(false);
                    cams[1].SetActive(true);

                    // disable start camera
                    // enable arkit camera
                    break;
                }
            default:
                {
                    break;
                }
        }
        //testing

        StartCanvas.enabled = false;
        mainCanvas.enabled = true;
        currentGameState = GameState.Start;
    }

    void Awake()
    {
        instance = this;
    }

}
