using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

public class FishGuiScript : MonoBehaviour
{
    /* UNITY INSPECTOR VARIABLES */
    [SerializeField]
    private FishMachine fishMachine;
    [SerializeField]
    private VideoClip[] videoClips;
    [SerializeField]
    private GameObject[] animations;
    [SerializeField]
    private OSC leapOSC;    

    /* MEMBER VARIABLES */
    private VideoPlayer videoPlayer;
    private FishMachine.State currState;
    private Queue<FishMachine.State> stateQueue;
    private GameObject playingAnimation;
    private bool justSwitched;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        stateQueue = new Queue<FishMachine.State>();
        currState = fishMachine.CurrentState;
        justSwitched = false;

        foreach (GameObject obj in animations)
            obj.SetActive(false);

        animations[0].SetActive(true);
        animations[0].GetComponent<PlayableDirector>().Play();
        playingAnimation = animations[0];

        if (fishMachine == null)
        {
            throw new UnityException("FishMachine for FishGuiScript not defined!");
        }

        if(videoPlayer == null)
        {
            throw new UnityException("There is no VideoPlayer attached to the FishGuiScript's GameObject!");
        }

}

    // Update is called once per frame
    void Update()
    {
        //If the fish changes state
        if (currState != fishMachine.CurrentState)
        {
            //Disable the current animation object
            playingAnimation.SetActive(false);

            //Pick the new animation, activate it, then play it
            playingAnimation = animations[(int)fishMachine.CurrentState];
            playingAnimation.SetActive(true);
            playingAnimation.GetComponent<PlayableDirector>().Play();

            //Set the current state
            currState = fishMachine.CurrentState;
        }
        
    }

    void OnGUI()
    {
    //    Rect windowRect = new Rect(Screen.width / 2 - width/2, Screen.height/2 - height/2, width, height);
    //    GUI.Box(windowRect, "Fish Machine Display");

        GUILayout.Label($"Current State: {fishMachine.CurrentState}");
        GUILayout.Label($"Happiness    : {fishMachine.happiness * 100.0f}%");
        GUILayout.Label($"Fullness     : {fishMachine.fullness * 100.0f}%");
        GUILayout.Label($"Condition    : {fishMachine.CurrentCondition}");
        foreach(FishMachine.State state in stateQueue.ToArray())
        {
            GUILayout.Label($"State Queue  : {state}");
        }
        
    }
}
