using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    /* UNITY INSPECTOR VARIABLES */
    [SerializeField]
    private FishMachine fishMachine;
    [SerializeField]
    private OSC twitterOsc;
    [SerializeField]
    private OSC leapOsc;

    /* MEMBER VARIABLES */
    private readonly Dictionary<KeyCode, FishMachine.Interaction> controls;

    public InputManager()
    {
        //Define the mapping between the controls available to the user and the valid interactions
        controls = new Dictionary<KeyCode, FishMachine.Interaction>()
        {
            { KeyCode.S, FishMachine.Interaction.SPRINKLING },
            { KeyCode.P, FishMachine.Interaction.POINTING },
            { KeyCode.T, FishMachine.Interaction.TRACKING },
            { KeyCode.W, FishMachine.Interaction.WAVE}
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        twitterOsc.SetAddressHandler("/feedfish", OnFeedFish);
        twitterOsc.SetAddressHandler("/petfish", OnPetFish);

        leapOsc.SetAddressHandler("/Sprinkle_food", OnSprinkle);
        //Deprecated leapOsc.SetAddressHandler("/Get_attention", OnGetAttention);
        leapOsc.SetAddressHandler("/Finger_point", OnPointing);
        leapOsc.SetAddressHandler("/Cleaning_wave", OnWave);


        //If our fish machine was not specified in the Unity Editor, throw an informative exception now :)
        if(fishMachine == null)
        {
            throw new UnityException("FishMachine for InputManager not defined!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check to see if any of our inputs are being detected this frame
        foreach(KeyValuePair<KeyCode, FishMachine.Interaction> pair in controls)
        {
            //If one of the keys we are watching for is pressed
            if (Input.GetKeyDown(pair.Key)) //We use GetKeyDown() to ensure each key press simulates a discrete interacton
            {
                //Then tell the fishmachine
                fishMachine.Interact(pair.Value);
            }
        }
    }

    void OnFeedFish(OscMessage message)
    {
        Debug.Log($"FeedFish: {message.ToString()}");
        fishMachine.Interact(FishMachine.Interaction.TWEETFEED);
    }

    void OnPetFish(OscMessage message)
    {
        Debug.Log($"PetFish: {message.ToString()}");
        fishMachine.Interact(FishMachine.Interaction.TWEETPET);
    }

    void OnSprinkle(OscMessage message)
    {
        Debug.Log($"Sprinkle: {message.ToString()}");
        fishMachine.Interact(FishMachine.Interaction.SPRINKLING);
    }

    void OnGetAttention(OscMessage message)
    {
        Debug.Log($"Get Attention: {message.ToString()}");
        fishMachine.Interact(FishMachine.Interaction.TRACKING);
    }

    void OnPointing(OscMessage message)
    {
        Debug.Log($"Finger Point: {message.ToString()}");
        fishMachine.Interact(FishMachine.Interaction.POINTING);
    }

    void OnWave(OscMessage message)
    {
        Debug.Log($"Wave: {message.ToString()}");
        fishMachine.Interact(FishMachine.Interaction.WAVE);
    }
}
