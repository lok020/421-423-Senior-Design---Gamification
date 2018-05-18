//Achievement Milestone reward. For more info, see AchievementManager.cs

public class AchievementMilestone  {

    public int ID { get; private set; }
    public int Required { get; private set; }           //Number of achievements in this set that must be completed to get reward
    public GameReward Reward { get; private set; }
    public int State { get; private set; }

    public AchievementMilestone(int id, int state, int required, GameReward reward)
    {
        Required = required;
        Reward = reward;
        State = state;
        ID = id;
    }

    //Returns if achievement has already been completed
    public bool IsCompleted()
    {
        return State > 0;
    }

    public void Complete()
    {
        //Only complete if state is current 0
        if (State == 0)
        {
            State = 1;
        }
    }

    //Returns reward and updates state to 2
    public GameReward GetReward()
    {
        if (State != 1) return null;
        State = 2;
        return Reward;
    }
}
