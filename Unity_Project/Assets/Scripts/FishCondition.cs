using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Creating a compound condition:
 *      ex. FishCondition compound = FishCondition.HAPPY & FishCondition.FULL;
 *      
 * One benefit of this is that we can check any FishCondition variable to see if any subset of conditions are met
 *      ex. 
 *      private bool isHappyHungry(FishCondition con){
 *           FishCondition compare = FishCondition.Happy & FishCondition.HUNGRY;
 *           return (con & compare) == compare;
 *      }
 */

/// <summary>
/// Enumeration that describes what internal conditions must be necessary for a transition to happen
/// The enumeration is represented as a set of bit flags, such that a compound condition may be represented by a 
/// binary AND of two conditions
/// The underlying data type is uint to leave room for up to 32 conditions
/// </summary>
[Flags]
public enum FishCondition : uint
{
    NONE =      0b0000_0000,
    HAPPY =     0b0000_0001,
    SAD =       0b0000_0010,
    FULL =      0b0000_0100,
    HUNGRY =    0b0000_1000,
    OVERFULL =  0b0001_0000,
    STARVING =  0b0010_0000,
    GLOWING  =  0b0100_0000
}

public static class FishConditionExtensions
{
    
}


