using System.Collections.Generic;

//Quest Objective - part of the quest system, read the documentation for more info
public class QuestObjective
{
    //Public enums
    public enum QuestObjectiveCompletion { NO, SUCCESS, FAIL };
    public enum QuestObjectiveEval { AND, OR };

    //Properties - all are readonly and are only set by the constructor
    public double ID { get; private set; }                                      //Quest ID number
    public bool Hidden { get; private set; }                                    //Whether the objective is hidden or not. If so, it doesn't print the condition
    public string Description { get; private set; }                             //Description, not required for hidden variables
    public QuestObjectiveCompletion Completes { get; private set; }             //Whether the quest objective finishes the quest upon completion: no, succeeds, fails
    public List<double> RequiredObjectives { get; private set; }                //List of quest IDs that need to be completed before this objective can be activated
    public QuestObjectiveEval RequiredObjectiveRelationship { get; private set; }   //Whether ALL (AND) or ONE (OR) of the required objectives must be complete to activate quest
    public List<object> Condition { get; private set; }                         //Function for the objective completion parameters. If it returns true, the objective will be completed
    public bool CompleteConditionIfTrue { get; private set; }                   //Whether the quest objective completes if the condition is true or false

    //Initializer: accepts arguments for all the above variables, in order
    public QuestObjective(double id, bool hidden, string description, string completes, List<double> required,
        string relationship, List<object> condition, bool completeObjectiveIfConditionIsTrue)
    {
        ID = id;
        Hidden = hidden;
        Description = description;
        RequiredObjectives = required;
        Condition = condition;
        CompleteConditionIfTrue = completeObjectiveIfConditionIsTrue;
        //Completes?
        switch(completes.ToUpper())
        {
            case "YES":
            case "SUCCESS":
                Completes = QuestObjectiveCompletion.SUCCESS;
                break;
            case "FAIL":
                Completes = QuestObjectiveCompletion.FAIL;
                break;
            default:
                Completes = QuestObjectiveCompletion.NO;
                break;
        }
        //Relationship
        if(relationship.ToUpper() == "AND")
        {
            RequiredObjectiveRelationship = QuestObjectiveEval.AND;
        }
        else
        {
            RequiredObjectiveRelationship = QuestObjectiveEval.OR;
        }
    }
}
