using System.Collections.Generic;

//Achievement groups. For more information, see AchievementManager.cs

public class AchievementGroup {
    
    public string Name { get; private set; }
    public string Description { get; private set; }

    public bool AnyChanges;

    public List<Achievement> Achievements { get; private set; }
    public List<AchievementMilestone> Milestones { get; private set; }

    public AchievementGroup(string name, string description, List<AchievementMilestone> milestones)
    {
        Name = name;
        Description = description;
        Milestones = milestones;
        Achievements = new List<Achievement>();
        AnyChanges = true;
    }

    public void Add(Achievement achievement)
    {
        Achievements.Add(achievement);
    }

    public int NumberCompleted()
    {
        int i = 0;
        foreach(Achievement a in Achievements)
        {
            if (a.IsCompleted()) i++; 
        }
        return i;
    }
}
