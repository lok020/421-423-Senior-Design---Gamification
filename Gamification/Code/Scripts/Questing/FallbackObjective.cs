using System.Collections.Generic;

public class FallbackObjective
{

    //Variables
    public double FallbackID { get; private set; }          //ID of the quest objective to reset to
    public List<double> AffectedIDs { get; private set; }   //List of IDs for all quests that this affects
    public List<object> Condition { get; private set; }     //Condition that when completed will reset quest back to fallbackID
    public bool CompleteIfTrue { get; private set; }            //Whether the above condition needs to be "true" or "false" to fall back

    //Initializer, accepts the above values as its arguments, in order
    public FallbackObjective(double fallbackId, List<double> affectedIds, List<object> condition, bool completeIfTrue)
    {
        FallbackID = fallbackId;
        AffectedIDs = affectedIds;
        Condition = condition;
        CompleteIfTrue = completeIfTrue;
    }
}
