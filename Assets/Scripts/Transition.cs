using UnityEngine;

public struct Transition
{

    public readonly FishMachine.State From;
    public readonly FishMachine.State To;
    public readonly FishMachine.Interaction By;
    public readonly float Time; //Time should only have a non-negative value if this is a transition by time
    public readonly FishCondition Condition;

  /// <summary>
  /// This struct is a record containing all information related to a state transition in FishMachine.
  /// Given a current state, and inputs (including time spent in current state), a fishmachine should only need a Transition to determine
  /// if it should make a given transition
  /// </summary>
  /// <param name="from">The state the machine is transitioning from</param>
  /// <param name="to">The state the machine is transitioning to</param>
  /// <param name="by">The type of input that will cause the machine to execute this transition</param>
  /// <param name="time">If this transition occurs by Time, then this optional parameter determines 
  ///                    how long the machine will wait before making this transition</param>
  /// <param name="condition">This optional parameter should be set if the machine must be in a specific state to make this transition</param>
    public Transition(FishMachine.State from, FishMachine.State to, FishMachine.Interaction by, 
        float time = 0.0f, FishCondition condition = FishCondition.NONE)
    {
        From = from;
        To = to;
        By = by;
        Time = time;
        Condition = condition;
    }
}
