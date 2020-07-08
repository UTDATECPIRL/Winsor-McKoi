using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishMachine : MonoBehaviour
{
    /* UNITY INSPECTOR VARIABLES */

    /// <summary>
    /// The state the fish starts in
    /// </summary>
    public State initialState;

    /// <summary>
    /// The happiness level at which the fish will be considered happy
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float happyThreshold;

    /// <summary>
    /// The fullness level below which the fish will be considered starving. Should be less than fullThreshold
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float starvingThreshold;

    /// <summary>
    /// The fullness level past which the fish will be considered full. Should be between starvingThreshold and overfullThreshold
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float fullThreshold;

    /// <summary>
    /// The fullness level past which the fish will be considered overfed. Should be greater than fullThreshold
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float overfullThreshold;
    /// <summary>
    /// The amount of time it takes for the fish to fully drain from 100% happiness/fullness
    /// </summary>
    [Tooltip("Time in minutes it takes for happiness/fullness to drain from 100% to 0%")]
    [Range(0.0f, 10.0f * 60.0f)]
    public float decayTime;

    /// <summary>
    /// The amount of time in minutes the tank takes to go from 0% dirty to 100% dirty
    /// </summary>
    [Tooltip("Time in minutes it takes for the tank to reach 100% dirtiness")]
    [Range(0.0f, 10.0f * 60.0f)]
    public float dirtTime;

    /// <summary>
    /// The number of seconds the "feeding window" will be open, such that 3 sprinkles in this window fully feeds the fish
    /// </summary>
    [Tooltip("Length of the window, in seconds, during which 3 sprinkles will completely feed the fish")]
    [Range(0.0f, 2.0f * 60.0f)]
    public float feedingWindow;

    /// <summary>
    /// The length of the window, in seconds, during which 3 'point' interactions will make Winsor 100% happy
    /// </summary>
    [Range(0.0f, 2.0f * 60.0f)]
    [Tooltip("The length of the window, in seconds, during which 3 'point' interactions will make Winsor 100% happy")]
    public float pointingWindow;

    /// <summary>
    /// The happiness level the fish will wake up at
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float initialHappiness;
    /// <summary>
    /// The fullness level the fish will wake up at
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float initialFullness;

    /// <summary>
    /// The initial level of dirtiness in the tank. 100% dirtiness is 75% opacity
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float initialDirtiness;

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
        get
        {
            //The condition variable we will return, i.e. Winsor's current condition
            FishCondition cond = FishCondition.NONE;

            //Are we past the overfeeding point?
            if (fullness >= overfullThreshold)
                cond |= FishCondition.OVERFULL;
            //If not, then are we past the full point?
            else if (fullness >= fullThreshold)
                cond |= FishCondition.FULL;
            //If not, then is the fish above starving?
            else if (fullness >= starvingThreshold)
                cond |= FishCondition.HUNGRY;
            //If not, then the fish is starving :(
            else
                cond |= FishCondition.STARVING;

            //Then just check if the fish is happy or not
            cond |= (happiness >= happyThreshold) ? FishCondition.HAPPY : FishCondition.SAD;

            return cond;
        }
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
    /// A bounded value from 0 to 1 inclusive that represents the tank's dirtiness
    /// </summary>
    [Range(0.0f, 1.0f)]
    internal float dirtiness;

    /// <summary>
    /// When FishMachine.Interact() is called, it sets this variable. In the update loop, the FishMachine will check this variable and 
    /// process the interaction, if applicable.
    /// Note: This does NOT need to be a queue right now because the InputManager can only set this once per update loop
    /// </summary>
    private List<Interaction> inputThisFrame;

    /// <summary>
    /// A timer so the tank can get progressively more dirty
    /// </summary>
    private float dirtTimer;

    /// <summary>
    /// The start time of the decay timer, resets with each decay.
    /// </summary>
    private float decayTimer;

    /// <summary>
    /// The start time of the "feeding window" timer. Resets when a new feeding window opens
    /// </summary>
    private float feedingTimer;

    /// <summary>
    /// The amount by which the fullness level raises when fed. Resets at the beginning of each feeding window
    /// </summary>
    private float feedingIncrement;

    /// <summary>
    /// The timer used for the 3-Pointing system
    /// </summary>
    private float pointingTimer;

    /// <summary>
    /// The value to be added to happiness when pointed at
    /// </summary>
    private float pointingIncrement;

    /* METHODS */
    // Start is called before the first frame update
    void Start()
    {
        CurrentState = State.STANDING; //Start the fish folded up
        
        //Initialize Winsor's state
        happiness = initialHappiness;
        fullness = initialFullness;
        dirtiness = initialDirtiness;
        CurrentState = initialState;

        //Initialize timers and other variables
        inputThisFrame = new List<Interaction>();

        decayTimer = float.MaxValue;
        feedingTimer = float.MaxValue;
        pointingTimer = float.MaxValue;
        dirtTimer = Time.time;

        feedingIncrement = 0.0f;
        pointingIncrement = 0.0f;

        //Create an array with all the transitions, then convert it to an easier-to-use dictionary
        List<Transition> tempTransitionList = new List<Transition>
        {
            //This is all of the state transitions for the Fish Finite-State Machine
            //Each transition has a From and To state, as well as the interaction that causes the transition
            //Conditionals, such as a required FishCondition or an amount of time to pass, can be specified as well
            new Transition(State.STANDING, State.OPENING, Interaction.TRACKING),

            //NOTE: Many, if not all, of the TIME interactions wait for the animation to end. Thus, the time values are hard-coded
            //right now to fit the length of the animation
            new Transition(State.OPENING, State.IDLE, Interaction.TIME, 5.0f),

            new Transition(State.IDLE, State.FEEDING, Interaction.SPRINKLING),
            new Transition(State.IDLE, State.FEEDING, Interaction.TWEETFEED),
            new Transition(State.IDLE, State.SHY, Interaction.POINTING, condition: FishCondition.SAD),
            new Transition(State.IDLE, State.IDLE2CURIOUS, Interaction.POINTING, condition: FishCondition.HAPPY),
            new Transition(State.IDLE, State.DYING, Interaction.TIME, condition: FishCondition.STARVING),
            new Transition(State.IDLE, State.IDLE2HAPPY, Interaction.TWEETPET),

            new Transition(State.IDLE2CURIOUS, State.CURIOUS, Interaction.TIME, 2.0f),
            new Transition(State.IDLE2CURIOUS, State.SHY, Interaction.POINTING, condition: FishCondition.SAD),
            new Transition(State.IDLE2CURIOUS, State.FEEDING, Interaction.SPRINKLING),
            new Transition(State.IDLE2CURIOUS, State.FEEDING, Interaction.TWEETFEED),

            new Transition(State.IDLE2HAPPY, State.HAPPY, Interaction.TIME, 2.0f),
            new Transition(State.IDLE2HAPPY, State.SHY, Interaction.POINTING, condition: FishCondition.SAD),
            new Transition(State.IDLE2HAPPY, State.IDLE2CURIOUS, Interaction.POINTING, condition: FishCondition.HAPPY),
            new Transition(State.IDLE2HAPPY, State.FEEDING, Interaction.SPRINKLING),
            new Transition(State.IDLE2HAPPY, State.FEEDING, Interaction.TWEETFEED),

            new Transition(State.FEEDING, State.HAPPY, Interaction.TIME, 8.0f, FishCondition.STARVING),
            new Transition(State.FEEDING, State.HAPPY, Interaction.TIME, 8.0f, FishCondition.HUNGRY),
            new Transition(State.FEEDING, State.HAPPY, Interaction.TIME, 8.0f, FishCondition.FULL),
            new Transition(State.FEEDING, State.SAD, Interaction.TIME, 8.0f, FishCondition.OVERFULL),
            new Transition(State.FEEDING, State.SHY, Interaction.POINTING, condition: FishCondition.SAD),
            new Transition(State.FEEDING, State.IDLE2CURIOUS, Interaction.POINTING, condition: FishCondition.HAPPY),

            new Transition(State.SHY, State.IDLE, Interaction.TIME, 7.0f),
            new Transition(State.SHY, State.IDLE2CURIOUS, Interaction.POINTING, condition: FishCondition.HAPPY),
            new Transition(State.SHY, State.FEEDING, Interaction.SPRINKLING),
            new Transition(State.SHY, State.FEEDING, Interaction.TWEETFEED),

            new Transition(State.CURIOUS, State.IDLE, Interaction.TIME, 45.0f),
            new Transition(State.CURIOUS, State.SHY, Interaction.POINTING, condition: FishCondition.SAD),
            new Transition(State.CURIOUS, State.IDLE2CURIOUS, Interaction.POINTING, condition: FishCondition.HAPPY),
            new Transition(State.CURIOUS, State.FEEDING, Interaction.SPRINKLING),
            new Transition(State.CURIOUS, State.FEEDING, Interaction.TWEETFEED),

            new Transition(State.HAPPY, State.IDLE, Interaction.TIME, 8.0f),
            new Transition(State.HAPPY, State.SHY, Interaction.POINTING, condition: FishCondition.SAD),
            new Transition(State.HAPPY, State.IDLE2CURIOUS, Interaction.POINTING, condition: FishCondition.HAPPY),
            new Transition(State.HAPPY, State.FEEDING, Interaction.SPRINKLING),
            new Transition(State.HAPPY, State.FEEDING, Interaction.TWEETFEED),

            new Transition(State.SAD, State.IDLE, Interaction.TIME, 7.0f),
            new Transition(State.SAD, State.SHY, Interaction.POINTING, condition: FishCondition.SAD),
            new Transition(State.SAD, State.IDLE2CURIOUS, Interaction.POINTING, condition: FishCondition.HAPPY),
            new Transition(State.SAD, State.FEEDING, Interaction.SPRINKLING),
            new Transition(State.SAD, State.FEEDING, Interaction.TWEETFEED),

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
        //We divide decay time by 10 because we want 10 decays to happen w/ no interaction, and we multiply by 60
        //because decayTime is in minutes, not seconds like Time.time
        if(Time.time - decayTimer >= 60.0 * decayTime / 10.0f)
        {
            happiness -= 0.1f;
            fullness -= 0.1f;
            decayTimer = Time.time;
        }

        //Same sh*t different line
        if (dirtiness < 1.0f && Time.time - dirtTimer >= 60.0f * dirtTime / 10.0f)
        {
            dirtiness += 0.1f;
            dirtTimer = Time.time; 
        }

        //If the feeding timer passes the end of the feeding window, close the feeding window
        if (Time.time - feedingTimer > feedingWindow)
        {
            feedingTimer = float.MaxValue;
            feedingIncrement = 0.0f;
        }

        if (Time.time - pointingTimer > pointingWindow)
        {
            pointingTimer = float.MaxValue;
            pointingIncrement = 0.0f;
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
                    if (interaction == t.By)
                    {
                        //If so, then transition to the next state
                        ChangeState(t.To);

                        //And move to the next frame
                        break;
                    }
                    else if (interaction == Interaction.WAVE)
                    {
                        //This ternary keeps dirtiness from going negative. If dirtiness <= 0.2, just make it 0
                        dirtiness -= dirtiness >= 0.2f ? 0.2f : dirtiness;
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

    /* TODO: maybe put UpdateFishCondition inside ChangeState, since that is the only place it will likely ever be called */
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
                if (feedingTimer == float.MaxValue)
                {
                    //Since fish should be fully fed after 3 sprinkles, take the remaining hunger and divide by 3
                    feedingIncrement = (1.0f - fullness) / 3.0f;

                    //Add the food to the fish's belly
                    fullness += feedingIncrement;

                    //Open the feeding window!
                    feedingTimer = Time.time;
                }
                else if (fullness < 1.0f)
                {
                    //If the timer is running, then just add the food
                    fullness += feedingIncrement;
                }
                break;
            //Being pointed at makes the fish either shy or curious. Handle that by...
            case State.SHY:
            case State.CURIOUS:
                if(pointingTimer == float.MaxValue) //If the timer is STOPPED
                {
                    //...starting the timer...
                    pointingTimer = Time.time;
                    pointingIncrement = (1.0f - happiness) / 3.0f;

                    //...making Winsor happier and...
                    happiness += pointingIncrement;

                    //...expending a little energy
                    fullness -= 0.1f;
                }
                else if(happiness < 1.0f) //if the timer is running...
                {
                    //Just do the stat changes
                    happiness += pointingIncrement;
                    fullness -= 0.05f;
                }
                
                break;
            //When the fish is starting to "hibernate," reset its stats
            case State.DYING:
                happiness = initialHappiness;
                fullness = initialFullness;
                decayTimer = float.MaxValue; //decayTimer is set to its max value so that the timer does not
                                             //tick while the fish is unfolded
                                             
                break;
          //
            case State.OPENING:
                decayTimer = Time.time;
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
        WAVE,
        TIME,
        TWEETFEED,
        TWEETPET,
        NONE
    }
}
