public class GameReward {

	//Variables
    public double RequiredObjective { get; private set; }   //Objective that must be marked as complete to receive these rewards
    public int XPReward { get; private set; }               //XP to be rewarded
    public int GoldReward { get; private set; }             //Gold to be rewarded
    public int ItemReward { get; private set; }             //Item to be rewarded
    public int ItemRewardCount { get; private set; }        //Number of item to be rewarded
    public int StatPoints { get; private set; }             //Stat points to be rewarded
    public int SkillPoints { get; private set; }            //Skill points to be rewarded

    //Initializer, accepts the above values as its arguments, in order
    public GameReward(double requiredObjective, int xpReward, int goldReward, int itemReward = 0, int itemRewardCount = 0, int statPoints = 0, int skillPoints = 0)
    {
        RequiredObjective = requiredObjective;
        XPReward = xpReward;
        GoldReward = goldReward;
        ItemReward = itemReward;
        ItemRewardCount = itemRewardCount;
        StatPoints = statPoints;
        SkillPoints = skillPoints;
    }
}
