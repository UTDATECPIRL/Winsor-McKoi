using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;
using UnityEngine.UI;

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
    private Material fishMaterial;
    [SerializeField]
    private Material glowingMaterial;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("The playback speed for the fish's folding-up animation")]
    private float foldupSpeed;

    [SerializeField]
    private LEAP_coordinates leapMotionController;

    /* MEMBER VARIABLES */
    private VideoPlayer videoPlayer;
    private FishMachine.State currState;
    private Queue<FishMachine.State> stateQueue;
    private GameObject playingAnimation;
    private bool justSwitched;
    private RawImage[] dirtImages;

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

        dirtImages = GetComponentsInChildren<RawImage>();

        if (dirtImages == null)
        {
            throw new UnityException("No dirt images specified in FishGui!");
        }

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

            //Since the opening animation is too fast, slow it down
            if(fishMachine.CurrentState == FishMachine.State.OPENING)
            {
                videoPlayer.playbackSpeed = foldupSpeed;
            }
            else
            {
                videoPlayer.playbackSpeed = 1.0f;
            }

            //Pick the new animation, activate it, then play it
            playingAnimation = animations[(int)fishMachine.CurrentState];
            playingAnimation.SetActive(true);
            playingAnimation.GetComponent<PlayableDirector>().Play();

            //Set the current state
            currState = fishMachine.CurrentState;
        }

        //Tie the dirt image's opacity to the tank's dirtiness
        foreach(RawImage img in dirtImages)
        {
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, fishMachine.dirtiness * 0.4f);
        }

        if (leapMotionController.HandDetected)
        {
            playingAnimation.GetComponent<Renderer>().material = glowingMaterial;
        }
        else
        {
            playingAnimation.GetComponent<Renderer>().material = fishMaterial;
        }
        
    }

    void OnGUI()
    {

        GUILayout.Label($"Current State: {fishMachine.CurrentState}");
        GUILayout.Label($"Happiness    : {fishMachine.happiness * 100.0f}%");
        GUILayout.Label($"Fullness     : {fishMachine.fullness * 100.0f}%");
        GUILayout.Label($"Dirtiness    : {fishMachine.dirtiness * 100.0f}%");
        GUILayout.Label($"Condition    : {fishMachine.CurrentCondition}");
        GUILayout.Label($"Hand Detected: {leapMotionController.HandDetected}");

        foreach(FishMachine.State state in stateQueue.ToArray())
        {
            GUILayout.Label($"State Queue  : {state}");
        }
        
    }
}
