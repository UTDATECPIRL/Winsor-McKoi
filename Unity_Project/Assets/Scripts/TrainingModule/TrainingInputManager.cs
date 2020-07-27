using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingInputManager : MonoBehaviour
{
    /* UNITY INSPECTOR VARIABLES */
    [SerializeField]
    private OSC leapOsc;
    [SerializeField]
    TrainingGuiScript guiController;

    /* MEMBER VARIABLES */
    private readonly Dictionary<KeyCode, OscCallback> controls;

    public TrainingInputManager()
    {
        //Define the mapping between the controls available to the user and the valid interactions
        controls = new Dictionary<KeyCode, OscCallback>()
        {
            { KeyCode.S, OnSprinkle },
            { KeyCode.P, OnPointing },
            { KeyCode.T, OnGetAttention },
            { KeyCode.W, OnWave}
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        leapOsc.SetAddressHandler("/Sprinkle_food", OnSprinkle);
        leapOsc.SetAddressHandler("/Finger_point", OnPointing);
        leapOsc.SetAddressHandler("/Cleaning_wave", OnWave);
    }

    // Update is called once per frame
    void Update()
    {
        //Check to see if any of our inputs are being detected this frame
        foreach (KeyValuePair<KeyCode, OscCallback> pair in controls)
        {
            //If one of the keys we are watching for is pressed
            if (Input.GetKeyDown(pair.Key)) //We use GetKeyDown() to ensure each key press simulates a discrete interacton
            {
                //Then tell the fishmachine
                pair.Value(new OscMessage());
            }
        }

        //If someone hits escape, then close the app
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private delegate void OscCallback(OscMessage msg);

    void OnFeedFish(OscMessage message)
    {
        Debug.Log($"FeedFish: {message.ToString()}");
        guiController.Interact(FishMachine.Interaction.TWEETFEED);
    }

    void OnPetFish(OscMessage message)
    {
        Debug.Log($"PetFish: {message.ToString()}");
        guiController.Interact(FishMachine.Interaction.TWEETPET);
    }

    void OnSprinkle(OscMessage message)
    {
        Debug.Log($"Sprinkle: {message.ToString()}");
        guiController.Interact(FishMachine.Interaction.SPRINKLING);
    }

    void OnGetAttention(OscMessage message)
    {
        Debug.Log($"Get Attention: {message.ToString()}");
        guiController.Interact(FishMachine.Interaction.TRACKING);
    }

    void OnPointing(OscMessage message)
    {
        Debug.Log($"Finger Point: {message.ToString()}");
        guiController.Interact(FishMachine.Interaction.POINTING);
    }

    void OnWave(OscMessage message)
    {
        Debug.Log($"Wave: {message.ToString()}");
        guiController.Interact(FishMachine.Interaction.WAVE);
    }
}
