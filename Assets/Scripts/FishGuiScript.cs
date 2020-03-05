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
        //If the fish's state has changed, then add this new state to the queue
        if(currState != fishMachine.CurrentState)
        {
            currState = fishMachine.CurrentState;
            stateQueue.Enqueue(currState);
        }

        //If video is done playing (indicated by videoPlayer not playing AND we didnt JUST try to start it)
        if (playingAnimation.GetComponent<PlayableDirector>().state == PlayState.Paused && !justSwitched)
        {
            //If there are any states in the queue, load them as the next video
            if (stateQueue.Count > 0)
            {
                playingAnimation.SetActive(false);
                playingAnimation = animations[(int)stateQueue.Dequeue()];
            }
            else
                playingAnimation = animations[(int)fishMachine.CurrentState]; //If no new states, just loop the current state

            //playingAnimation.GetComponent<PlayableDirector>()= 1.0f;   //reset the playback speed
            playingAnimation.SetActive(true);
            playingAnimation.GetComponent<PlayableDirector>().Play();
            justSwitched = true; //set our flag
        }
        if (justSwitched && playingAnimation.GetComponent<PlayableDirector>().state == PlayState.Playing)
            justSwitched = false; //reset our flag

        //If the video is playing BUT there is something queued up
        if (videoPlayer.isPlaying && !justSwitched && stateQueue.Count > 0)
        {
            videoPlayer.playbackSpeed = 2.0f; //speed that RILED MFER up
        }
    }

    void OnGUI()
    {
    //    Rect windowRect = new Rect(Screen.width / 2 - width/2, Screen.height/2 - height/2, width, height);
    //    GUI.Box(windowRect, "Fish Machine Display");

        GUILayout.Label($"Current State: {fishMachine.CurrentState}");
        GUILayout.Label($"Happiness    : {fishMachine.happiness}");
        GUILayout.Label($"Fullness     : {fishMachine.fullness}");
        GUILayout.Label($"Condition    : {fishMachine.CurrentCondition}");
        foreach(FishMachine.State state in stateQueue.ToArray())
        {
            GUILayout.Label($"State Queue  : {state}");
        }
        
    }
}
