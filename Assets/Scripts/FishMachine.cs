using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishMachine : MonoBehaviour
{
    /* UNITY INSPECTOR VARIABLES */
    public float happyThreshold;
    public float fullThreshold;
    public float decayTime;
    
    /* PROPERTIES */
    ///The current state of the machine
    public State CurrentState { get; private set; }

    /// <summary>
    /// Indicates whether or not the fish is glowing from a twitter interaction
    /// </summary>
    public bool IsGlowing { get; private set; }

    /* TODO: Deprecate IsHappy and IsFull in lieu of CurrentCondition */
    /// <summary>
    /// Flag for happiness level, returns true if happiness is above the given threshold.
    /// </summary>
    public bool IsHappy { get => happiness >= happyThreshold; }

    /// <summary>
    /// Flag for hunger level. Will return true if the fish's fullness is above the given limit
    /// </summary>
    public bool IsFull { get => fullness >= fullThreshold; }
    
    /// <summary>
    /// The fish's current FishCondition. Since this is essentially a compilation of the fish's statuses, return a bitwise OR 
    /// of the appropriate FishConditions
    /// </summary>
    public FishCondition CurrentCondition
    {
        get => (IsHappy ? FishCondition.HAPPY : FishCondition.SAD) | 
            (IsFull ? FishCondition.FULL : FishCondition.HUNGRY);
    }

    /* MEMBER VARIABLES */
    /// <summary>
    /// Map from given state to all possible transitions FROM that state
    /// </summary>
    private Dictionary<State, List<Transition>> transitions;

    /// <summary>
    /// To implement a timer, we must keep track of the time we started our timer
    /// </summary>
    private float timerStartTime;

    /// <summary>
    /// A bounded value from 0.0f to 1.0f inclusive, indicates the fish's happines out of 100%
    /// </summary>
    [Range(0.0f, 1.0f)]
    internal float happiness;

    /// <summary>
    /// A bounded value from 0.0f to 2.0f inclusive, indicates fish's fullness out of 100%, with extra room to be overfed
    /// </summary>
    [Range(0.0f, 2.0f)]
    internal float fullness;

    /// <summary>
    /// When FishMachine.Interact() is called, it sets this variable. In the update loop, the FishMachine will check this variable and 
    /// process the interaction, if applicable.
    /// Note: This does NOT need to be a queue right now because the InputManager can only set this once per update loop
    /// </summary>
    private List<Interaction> inputThisFrame;

    /// <summary>
    /// The start time of the decay timer, resets with each decay.
    /// </summary>
    private float decayTimer;

    /* METHODS */
    // Start is called before the first frame update
    void Start()
    {
        CurrentState = State.STANDING; //Start the fish folded up
        happiness = happyThreshold;
        fullness = fullThreshold;
        inputThisFrame = new List<Interaction>();
        decayTimer = Time.time;

        //Create an array with all the transitions, then convert it to an easier-to-use dictionary
        List<Transition> tempTransitionList = new List<Transition>
        {
            new Transition(State.STANDING, State.OPENING, Interaction.TRACKING),

            new Transition(State.OPENING, State.IDLE, Interaction.TIME, 11.0f),

            new Transition(State.IDLE, State.FEEDING, Interaction.SPRINKLING),
            new Transition(State.IDLE, State.FEEDING, Interaction.TWEETFEED),
            new Transition(State.IDLE, State.SHY, Interaction.POINTING, condition: FishCondition.SAD),
            new Transition(State.IDLE, State.IDLE2CURIOUS, Interaction.POINTING, condition: FishCondition.HAPPY),
            new Transition(State.IDLE, State.DYING, Interaction.TIME, 30.0f, FishCondition.HUNGRY),
            new Transition(State.IDLE, State.IDLE2HAPPY, Interaction.TWEETPET),

            new Transition(State.IDLE2CURIOUS, State.CURIOUS, Interaction.TIME, 2.0f),

            new Transition(State.IDLE2HAPPY, State.HAPPY, Interaction.TIME, 2.0f),

            new Transition(State.FEEDING, State.HAPPY, Interaction.TIME, 8.0f, FishCondition.HUNGRY),
            new Transition(State.FEEDING, State.SAD, Interaction.TIME, 8.0f, FishCondition.FULL),

            new Transition(State.SHY, State.IDLE, Interaction.TIME, 7.0f),

            new Transition(State.CURIOUS, State.IDLE, Interaction.TIME, 45.0f),

            new Transition(State.HAPPY, State.IDLE, Interaction.TIME, 8.0f),

            new Transition(State.SAD, State.IDLE, Interaction.TIME, 7.0f),

            new Transition(State.DYING, State.STANDING, Interaction.TIME, 3.0f)
        };

        transitions = new Dictionary<State, List<Transition>>();

        //For each of our transitions we will
        foreach (Transition t in tempTransitionList)
        {
            //Add a new, empty list if this state is not in the dictionary
            if (!transitions.ContainsKey(t.From))
            {
                transitions.Add(t.From, new List<Transition>());
            }

            //and add this transition to this state's list of transitions
            transitions[t.From].Add(t);

        }

        //Set the start time to its "default" value
        timerStartTime = -1.0f;

        Debug.Log(CurrentState);
    }

    /*
     TODO: FSM must understand video/animation state
            Immediate Response
     */
    // Update is called once per frame
    void Update()
    {
        if(Time.time - decayTimer >= decayTime)
        {
            happiness -= 0.1f;
            fullness -= 0.1f;
            decayTimer = Time.time;
        }
        //Loop through all transitions FROM this state and check whether we should transition
        foreach(Transition t in transitions[CurrentState])
        {
            //TODO: Make Time related to animation times when applicable
            if(t.By == Interaction.TIME)
            {
                //If the timer has not been set
                if (timerStartTime == -1.0f)
                {
                    //then set it
                    timerStartTime = Time.time;
                    Debug.Log($"Starting timer, waiting for {t.Time} seconds");
                }
                //If the time elapsed is less than the required wait time
                else if(Time.time - timerStartTime < t.Time) 
                {
                    //then do nothing while we wait for time to pass
                    continue;
                }
                //If the required time has elapsed, and the required condition is met
                else if((t.Condition & CurrentCondition) == t.Condition) //TODO: Maybe have the timer only start if the condition is met
                {
                    //Time's up! so we change the state
                    ChangeState(t.To);
                    //and move on to the next frame
                    break;
                }
                //If the condition is not met but time is up, this code just waits for the condition to be met
                //i.e. the timer starts when the state is entered, not when the transition condition is met
                //Related to TODO above
            }
            //If this is NOT a time-triggered transition, then check to see if the required condition is met
            else if((t.Condition & CurrentCondition) == t.Condition)
            { 
                //For all of the inputs given this frame
                foreach(Interaction interaction in inputThisFrame)
                {
                    //Check to see if each input causes a state transition
                    if(interaction == t.By)
                    {
                        //If so, then transition to the next state
                        ChangeState(t.To);

                        //And move to the next frame
                        break;
                    }
                }
            }
        }
        //Clear the inputThisFrame list, since the frame is ending for the FishMachine
        inputThisFrame.Clear();
    }
    
    /// <summary>
    /// Gives input to the fishmachine
    /// </summary>
    /// <param name="interaction">The input that is given to the machine to process</param>
    public void Interact(Interaction interaction)
    {
        inputThisFrame.Add(interaction);
    }

    /// <summary>
    /// Does all the miscellaneous work associated with changing states. Calling this function will properly change states,
    /// but does not check to make sure the transition requirements are fufilled
    /// </summary>
    /// <param name="to">The state that the machine will change to</param>
    private void ChangeState(State to)
    {
        Debug.Log($"Changing from {CurrentState} to {to}");

        //Change the current state
        CurrentState = to;

        //Reset the timer
        timerStartTime = -1.0f;

        //If the state we are transitioning to has any associated mood/condition changes, enact them
        UpdateFishCondition(to);
    }

    /* TODO: maybe put UpdateFishCondition inside ChangeState, since that is the only place it will likely ever be called*/
    /// <summary>
    /// UpdateFishCondition takes one parameter - the state we are changing to. This function will update the 
    /// fish's stats accordingly.
    /// </summary>
    /// <param name="to">The state the the fish will transition to</param>
    private void UpdateFishCondition(State to)
    {
        switch (to)
        {
            //Eating makes the fish fuller, and if the fish is not overfull, then it will make the fish happier
            case State.FEEDING:
                fullness += 0.2f;
                if (fullness > 1.0f) //TODO: Create a variable for the overfull threshold?
                    happiness -= 0.1f;
                break;
            //Being shy makes the fish somewhat happier because of the interaction (someone has to talk to you for you to get shy)
            case State.SHY:
                happiness += 0.2f;
                break;
            //Being curious makes the fish more happy than being shy; the fish is engaged by the interaction
            case State.CURIOUS:
                happiness += 0.3f;
                break;
        }
    }

    /// <summary>
    /// FishMachine.State - represents the possible behavior states [in future, maybe get these from config file?]
    /// </summary>
    public enum State
    {
        STANDING,
        OPENING,
        DYING,
        IDLE,
        IDLE2HAPPY,
        IDLE2CURIOUS,
        FEEDING,
        CURIOUS,
        SHY,
        HAPPY,
        SAD,
        SIZE
    }

    /// <summary>
    /// FishMachine.Interaction - this enumeration lists the possible inputs the state machine accepts
    /// </summary>
    public enum Interaction
    {
        TRACKING,
        POINTING,
        SPRINKLING,
        TIME,
        TWEETFEED,
        TWEETPET,
        NONE
    }
}
