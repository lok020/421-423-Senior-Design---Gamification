//Achievement class. For an explanation on how this works, see AchievementManager.cs

public class Achievement {

    public int ID { get; private set; }     // MUST BE UNIQUE
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int State { get; private set; }  // 0 = not complete, 1 = collect reward, 2 = complete
    public int Requirement { get; private set; }
    public AchievementStat Stat { get; private set; }
    public AchievementGroup Group { get; private set; }

    //Reward info
    private GameReward _reward;

    //Constructor: accepts arguments for the above variables
    public Achievement(int id, string name, string description, AchievementStat stat, int state, int requirement, GameReward reward, AchievementGroup group)
    {
        //Assign values
        ID = id;
        Name = name;
        Description = description;
        Stat = stat;
        State = state;
        Requirement = requirement;
        _reward = reward;
        Group = group;

        //Initialize
        Stat.AddDependency(this);
        Group.Add(this);
    }

    //Returns if achievement has already been completed
    public bool IsCompleted()
    {
        return State > 0;
    }

    //If this was just completed, update state and return true
    public bool JustCompleted()
    {
        if (State > 0) return false;
        if(Stat.Value >= Requirement)
        {
            State = 1;
            return true;
        }
        return false;
    }

    //Returns reward and updates state to 2
    public GameReward GetReward()
    {
        if (State != 1) return null;
        State = 2;
        return _reward;
    }
}
