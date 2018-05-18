using System.Collections.Generic;

//Achievement stat class. For more info on how this works, see AchievementManager.cs

public class AchievementStat {

    public int ID { get; private set; }
    public int Value { get; private set; }      //Value. All achievements must use an integer value for storage
    public GameAction Stat { get; private set; }
    public List<Achievement> Subscribers;       //List of achievements that depend on this stat

    public AchievementStat(int id, int value, GameAction stat)
    {
        ID = id;
        Stat = stat;
        Subscribers = new List<Achievement>();
        Value = value;
    }

    public void AddDependency(Achievement achievement)
    {
        Subscribers.Add(achievement);
    }

    public void AddToValue(int x)
    {
        Value += x;
    }
}
