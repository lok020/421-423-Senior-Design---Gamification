using System.Collections.Generic;
using UnityEngine;

/* Similar in structure to the quest manager. This is the basic setup:
 * The "Achievement" class stores info about the achievement, including description, reward, etc.
 *     It also subscribes to an AchievementStat object (the stat will store a list of Achievements
 *     to notify upon update).
 * The "AchievementStat" class stores info on a stat to be kept track of by the achievement system.
 *     It is designed this way because each stat (quests completed, gold spent on crafting, etc) has
 *     multiple achievements listening to it, and it makes more sense to track changes for a dozen stats
 *     than for potentially hundreds of achievements.
 *     If a stat is updated, all Achievements that subscribed to it will be updated as well.
 * The "AchievementManager" class listens to world events, check them against the list of AchievementStats,
 *     and if a stat is changed also updates the relevant achievements. If an achievement changes state (say
 *     from "incomplete" -> "collect reward"), it will notify the user of this.
 *     This class will also serve as a one-stop shop for the Achievement GUI to get a list of all achievements
 *     separated by group and the current achievement progress.
 * The "AchievementGroup" class has a label, a list of Achievements, and a list of milestone rewards (as well as
 *     each milestone's state). This class does two things - it is used by the GUI to display stuff, and it
 *     handles milestone rewards.
 * The "AchievementMilestone" class is for milestone rewards. These are granted after a given number of
 *     achievements have been completed. They just consist of a number and an AchievementReward.
 *     
 * Some other general notes:
 *  - Each Achievement has a unique ID, as does each Achievement Group.
 *  - The only data that will sync with the server are Achievement states, AchievementStat values, and
 *    AchievementGroup states.
 *  - Each achievement can only subscribe to one stat and be part of one group (for now). To get the same
 *    achievement (say complete two quests) in two groups, create two different Achievements with the same
 *    completion condition.
 *  - All of the classes in this system will be very interconnected. From a design perspective, consider this
 *    system to be one single object, with AchievementManager being the system's entry point. Most or all
 *    processing will take place in the AchievementManager class.
 */

public class AchievementManager : MonoBehaviour {

    private List<AchievementStat> _achievementStats;        //All processing is done with this list, see UpdateAchievements()
    private List<AchievementGroup> _achievementGroups;      //Used for GUI stuff

    //Achievement stats (elements in _achievementStats)
    private AchievementStat _statCrafted;
    private AchievementStat _statQuestCompleted;
    private AchievementStat _statSecretArea;
    private AchievementStat _statMonsterKilled;
    private AchievementStat _statAttackMelee, _statAttackMagic, _statAttackHeal;
    private AchievementStat _statMouseClicks;

    //Achievement groups (elements in _achievementGroups)
    private AchievementGroup _groupAdventurer;
    private AchievementGroup _groupCraftsman;
    private AchievementGroup _groupExplorer;
    private AchievementGroup _groupWarrior;
    private AchievementGroup _groupFighter, _groupMage, _groupHealer;

    //References
    private NetworkManager _networkManager;
    private MessageOverlayController _messageOverlayController;

	// Use this for initialization
	void Start () {
        _networkManager = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        _messageOverlayController = GameObject.FindGameObjectWithTag("MessageOverlay").GetComponent<MessageOverlayController>();
        Initialize();
	}

    public void UpdateAchievements(List<object> worldEvent)
    {
        //Get the achievement stat that matches this event
        AchievementStat stat = null;
        GameAction? action = worldEvent[0] as GameAction?;
        if (action == null) return;     //Just in case
        foreach(AchievementStat s in _achievementStats)
        {
            if(action == s.Stat)
            {
                stat = s;
                break;
            }
        }
        //If nothing matches the event, return
        if (stat == null) return;

        //Update this stat's value, depending on the message type
        switch (action)
        {
            //Format: <ACTION> <ID> <QUANTITY>. We want quantity
            case GameAction.CRAFTED:
            case GameAction.CONSUMED:
            case GameAction.DISCARDED:
            case GameAction.DROPPED:            //Not implemented
            case GameAction.LOOTED:             //Not implemented
            case GameAction.NPC_GIVE:
            case GameAction.NPC_RECEIVE:
            case GameAction.PICKED_UP:
            case GameAction.QUEST_CONSUMED:     //Not implemented
            case GameAction.TRADE_GIVE:         //Not implemented
            case GameAction.TRADE_RECEIVED:     //Not implemented
                if (worldEvent[2] is int)   //Just in case
                {
                    stat.AddToValue((int)worldEvent[2]);
                }
                break;
            //Format: <ACTION>. Just increment stat by one
            case GameAction.ATTACK_MAGIC:
            case GameAction.ATTACK_MELEE:
            case GameAction.ATTACK_RANGED:
            case GameAction.ATTACK_HEAL:
            case GameAction.MONSTER_KILLED:
            case GameAction.MOUSE_CLICK:
            case GameAction.SECRET_AREA:
            default:
                stat.AddToValue(1);
                break;
            //Many action types are not included - these only send strings and thus can't be used here
        }
        //Save this update on the server
        _networkManager.UpdateAchievementStat(stat.ID, stat.Value);
        
        //Update all achievements that subscribe to this stat
        foreach (Achievement a in stat.Subscribers)
        {
            //Ignore completed achievements
            if (a.IsCompleted()) continue;

            //If this latest stat update completed the achievement, notify the user and save this to the server
            if (a.JustCompleted())
            {
                //Notify user
                if (_messageOverlayController == null)
                {
                    _messageOverlayController = GameObject.FindGameObjectWithTag("MessageOverlay").GetComponent<MessageOverlayController>();
                }
                _messageOverlayController.EnqueueMessage("Achievement \"" + a.Name + "\" completed!");
                //Save this to the server
                _networkManager.UpdateAchievement(a.ID, a.State);

                //Check for milestone completion
                int completedInGroup = a.Group.NumberCompleted();
                foreach(AchievementMilestone m in a.Group.Milestones)
                {
                    //Skip completed milestones
                    if (m.IsCompleted()) continue;
                    //If milestone has been completed, notify user and save to server
                    if(m.Required <= completedInGroup)
                    {
                        m.Complete();
                        //Notify user
                        _messageOverlayController.EnqueueMessage("\"" + a.Group.Name + "\" milestone completed!");
                        //Save to server
                        _networkManager.UpdateMilestone(m.ID, m.State);
                    }
                }
            }

            //Lastly, set that the achievement group has been modified
            a.Group.AnyChanges = true;
        }
    }

    public AchievementGroup GetAcheivementGroup(string name)
    {
        //Loops through all groups and returns the one with the correct name
        foreach(AchievementGroup g in _achievementGroups)
        {
            //Not case sensitive
            if(g.Name.ToLower() == name.ToLower())
            {
                return g;
            }
        }
        return null;
    }

    /*'''''''''''''''''''''''''''''''''''''''''''''''''''*\
    | Create all achievements, groups etc in this section |
    | Make sure to add them to the below function!        |
    \*...................................................*/

    private void Initialize()
    {
        CreateStats();
        CreateGroups();

        CreateAdventurerAchievements();
        CreateCraftsmanAchievements();
        CreateExplorerAchievements();
        CreateWarriorAchievements();
        CreateFighterAchievements();
        CreateMageAchievements();
        CreateHealerAchievements();
    }

    //Create all achievement stats
    private void CreateStats()
    {
        _statCrafted        = new AchievementStat(0, _networkManager.GetAchievementStatStatus(0), GameAction.CRAFTED);
        _statQuestCompleted = new AchievementStat(1, _networkManager.GetAchievementStatStatus(1), GameAction.QUEST_COMPLETED);
        _statSecretArea     = new AchievementStat(2, _networkManager.GetAchievementStatStatus(2), GameAction.SECRET_AREA);
        _statMonsterKilled  = new AchievementStat(3, _networkManager.GetAchievementStatStatus(3), GameAction.MONSTER_KILLED);
        _statAttackMagic    = new AchievementStat(4, _networkManager.GetAchievementStatStatus(4), GameAction.ATTACK_MAGIC);
        _statAttackMelee    = new AchievementStat(5, _networkManager.GetAchievementStatStatus(5), GameAction.ATTACK_MELEE);
        _statAttackHeal     = new AchievementStat(6, _networkManager.GetAchievementStatStatus(6), GameAction.ATTACK_HEAL);

        //Add all of these to a list
        _achievementStats = new List<AchievementStat>() { _statCrafted, _statQuestCompleted, _statSecretArea, _statMonsterKilled,
            _statAttackMagic, _statAttackMelee, _statAttackHeal };
    }

    //Create all achievement groups
    private void CreateGroups()
    {
        //Create the milestones first
        /*
        AchievementMilestone adv1 = new AchievementMilestone(0, _networkManager.GetMilestoneStatus(0), 4, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone adv2 = new AchievementMilestone(1, _networkManager.GetMilestoneStatus(1), 8, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone cft1 = new AchievementMilestone(2, _networkManager.GetMilestoneStatus(2), 4, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone cft2 = new AchievementMilestone(3, _networkManager.GetMilestoneStatus(3), 8, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone exp1 = new AchievementMilestone(4, _networkManager.GetMilestoneStatus(4), 4, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone exp2 = new AchievementMilestone(5, _networkManager.GetMilestoneStatus(5), 8, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone war1 = new AchievementMilestone(6, _networkManager.GetMilestoneStatus(6), 4, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone war2 = new AchievementMilestone(7, _networkManager.GetMilestoneStatus(7), 8, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone ftr1 = new AchievementMilestone(8, _networkManager.GetMilestoneStatus(8), 4, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone ftr2 = new AchievementMilestone(9, _networkManager.GetMilestoneStatus(9), 8, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone mag1 = new AchievementMilestone(10, _networkManager.GetMilestoneStatus(10), 4, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone mag2 = new AchievementMilestone(11, _networkManager.GetMilestoneStatus(11), 8, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone hlr1 = new AchievementMilestone(12, _networkManager.GetMilestoneStatus(12), 4, new GameReward(0.0, 0, 0, 0, 0));
        AchievementMilestone hlr2 = new AchievementMilestone(13, _networkManager.GetMilestoneStatus(13), 8, new GameReward(0.0, 0, 0, 0, 0));
        //*/

        //Create the achievement groups
        _groupAdventurer = new AchievementGroup("Adventurer", "Discover what the world has to offer!", new List<AchievementMilestone>());
            //new List<AchievementMilestone>() { adv1, adv2 });
        _groupCraftsman = new AchievementGroup("Craftsman", "Create and improve items and gear!", new List<AchievementMilestone>());
        //new List<AchievementMilestone>() { cft1, cft2 });
        _groupExplorer = new AchievementGroup("Explorer", "Explore the secrets of the world!", new List<AchievementMilestone>());
        //new List<AchievementMilestone>() { exp1, exp2 });
        _groupWarrior = new AchievementGroup("Warrior", "Fight the evil that plagues the world!", new List<AchievementMilestone>());
        //new List<AchievementMilestone>() { war1, war2 });
        _groupFighter = new AchievementGroup("Fighter", "Fight enemies with melee attacks!", new List<AchievementMilestone>());
        //new List<AchievementMilestone>() { ftr1, ftr2 });
        _groupMage = new AchievementGroup("Mage", "Fight enemies with magic attacks!", new List<AchievementMilestone>());
        //new List<AchievementMilestone>() { mag1, mag2 });
        _groupHealer = new AchievementGroup("Medic", "Heal yourself and your allies!", new List<AchievementMilestone>());
        //new List<AchievementMilestone>() { rng1, rng2 });

        //Add them all to a list
        _achievementGroups = new List<AchievementGroup>() { _groupAdventurer, _groupCraftsman, _groupExplorer, _groupWarrior, _groupFighter,
            _groupMage, _groupHealer };
    }

    /* Each of the following achievement groups uses one of three different requirement scales.
     * 
     * Lvl  Title       Low     Medium  High
     * 1    Newbie      1       1       10
     * 2    Beginner    2       5       20
     * 3    Apprentice  5       10      50
     * 4    Trained     10      20      100
     * 5    Skilled     15      50      200
     * 6    Adept       20      100     500
     * 7    Expert      30      150     1000
     * 8    Master      40      200     2000
     * 
     * The Adventurer Set is different, as it has one achievement per category. Each requirement is between
     * level 2 and 3 in difficulty - this should be the easiest set to complete.
     */

    //Requirements: varied  Stat: varied
    private void CreateAdventurerAchievements()
    {
        new Achievement(0, "Crafting", "Craft three items", _statCrafted, _networkManager.GetAchievementStatus(0), 3,
            new GameReward(0.0, 15, 0), _groupAdventurer);
        new Achievement(1, "Questing", "Complete three quests", _statQuestCompleted, _networkManager.GetAchievementStatus(1), 3,
            new GameReward(0.0, 15, 0), _groupAdventurer);
        new Achievement(2, "Fighting", "Slay five monsters", _statMonsterKilled, _networkManager.GetAchievementStatus(2), 5,
            new GameReward(0.0, 15, 0), _groupAdventurer);
        new Achievement(3, "Exploring", "Discover two secret areas", _statSecretArea, _networkManager.GetAchievementStatus(3), 2,
            new GameReward(0.0, 15, 0), _groupAdventurer);
        new Achievement(4, "Swordsman", "Use melee attacks twenty times", _statAttackMelee, _networkManager.GetAchievementStatus(4), 20,
            new GameReward(0.0, 15, 0), _groupAdventurer);
        new Achievement(5, "Wizard", "Use magic attacks twenty times", _statAttackMagic, _networkManager.GetAchievementStatus(5), 20,
            new GameReward(0.0, 15, 0), _groupAdventurer);
        new Achievement(6, "Medic", "Use healing spells twenty times", _statAttackHeal, _networkManager.GetAchievementStatus(6), 20,
            new GameReward(0.0, 15, 0), _groupAdventurer);
    }

    //Requirements: medium  Stat: CRAFTED
    private void CreateCraftsmanAchievements()
    {
        new Achievement(10, "Newbie Craftsman", "Craft one item", _statCrafted, _networkManager.GetAchievementStatus(10), 1,
            new GameReward(0.0, 10, 10, 20, 1), _groupCraftsman);
        new Achievement(11, "Beginner Craftsman", "Craft five items", _statCrafted, _networkManager.GetAchievementStatus(11), 5,
            new GameReward(0.0, 20, 20, 20, 1), _groupCraftsman);
        new Achievement(12, "Apprentice Craftsman", "Craft ten items", _statCrafted, _networkManager.GetAchievementStatus(12), 10,
            new GameReward(0.0, 25, 50, 20, 1), _groupCraftsman);
        /*
        new Achievement(13, "Trained Craftsman", "Craft twenty items", _statCrafted, _networkManager.GetAchievementStatus(13), 20,
            new GameReward(0.0, 0, 0, 0, 0), _groupCraftsman);
        new Achievement(14, "Skilled Craftsman", "Craft fifty items", _statCrafted, _networkManager.GetAchievementStatus(14), 50,
            new GameReward(0.0, 0, 0, 0, 0), _groupCraftsman);
        new Achievement(15, "Adept Craftsman", "Craft one hundred items", _statCrafted, _networkManager.GetAchievementStatus(15), 100,
            new GameReward(0.0, 0, 0, 0, 0), _groupCraftsman);
        new Achievement(16, "Expert Craftsman", "Craft one hundred and fifty items", _statCrafted, _networkManager.GetAchievementStatus(16), 150,
            new GameReward(0.0, 0, 0, 0, 0), _groupCraftsman);
        new Achievement(17, "Master Craftsman", "Craft two hundred items", _statCrafted, _networkManager.GetAchievementStatus(17), 200,
            new GameReward(0.0, 0, 0, 0, 0), _groupCraftsman);
        //*/
    }

    //Requirements: low     Stat: SECRET_AREA
    private void CreateExplorerAchievements()
    {
        new Achievement(20, "Newbie Explorer", "Discover one secret area", _statSecretArea, _networkManager.GetAchievementStatus(20), 1,
            new GameReward(0.0, 20, 0, 0, 0), _groupExplorer);
        new Achievement(21, "Beginner Explorer", "Discover two secret areas", _statSecretArea, _networkManager.GetAchievementStatus(21), 2,
            new GameReward(0.0, 40, 0, 0, 0), _groupExplorer);
        new Achievement(22, "Apprentice Explorer", "Discover five secret areas", _statSecretArea, _networkManager.GetAchievementStatus(22), 5,
            new GameReward(0.0, 50, 0, 0, 0), _groupExplorer);
        /*
        new Achievement(23, "Trained Explorer", "Discover ten secret areas", _statSecretArea, _networkManager.GetAchievementStatus(23), 10,
            new GameReward(0.0, 0, 0, 0, 0), _groupExplorer);
        new Achievement(24, "Skilled Explorer", "Discover fifteen secret areas", _statSecretArea, _networkManager.GetAchievementStatus(24), 15,
            new GameReward(0.0, 0, 0, 0, 0), _groupExplorer);
        new Achievement(25, "Adept Explorer", "Discover twenty secret areas", _statSecretArea, _networkManager.GetAchievementStatus(25), 20,
            new GameReward(0.0, 0, 0, 0, 0), _groupExplorer);
        new Achievement(26, "Expert Explorer", "Discover thirty secret areas", _statSecretArea, _networkManager.GetAchievementStatus(26), 30,
            new GameReward(0.0, 0, 0, 0, 0), _groupExplorer);
        new Achievement(27, "Master Explorer", "Discover forty secret areas", _statSecretArea, _networkManager.GetAchievementStatus(27), 40,
            new GameReward(0.0, 0, 0, 0, 0), _groupExplorer);
        //*/
    }

    //Requirements: medium  Stat: MONSTER_KILLE
    private void CreateWarriorAchievements()
    {
        new Achievement(30, "Newbie Warrior", "Slay one monster", _statMonsterKilled, _networkManager.GetAchievementStatus(30), 1,
            new GameReward(0.0, 10, 0, 0, 0), _groupWarrior);
        new Achievement(31, "Beginner Warrior", "Slay five monsters", _statMonsterKilled, _networkManager.GetAchievementStatus(31), 5,
            new GameReward(0.0, 20, 0, 0, 0, 1), _groupWarrior);
        new Achievement(32, "Apprentice Warrior", "Slay ten monsters", _statMonsterKilled, _networkManager.GetAchievementStatus(32), 10,
            new GameReward(0.0, 25, 0, 0, 0, 1), _groupWarrior);
        /*
        new Achievement(33, "Trained Warrior", "Slay twenty monsters", _statMonsterKilled, _networkManager.GetAchievementStatus(33), 20,
            new GameReward(0.0, 0, 0, 0, 0), _groupWarrior);
        new Achievement(34, "Skilled Warrior", "Slay fifty monsters", _statMonsterKilled, _networkManager.GetAchievementStatus(34), 50,
            new GameReward(0.0, 0, 0, 0, 0), _groupWarrior);
        new Achievement(35, "Adept Warrior", "Slay one hundred monsters", _statMonsterKilled, _networkManager.GetAchievementStatus(35), 100,
            new GameReward(0.0, 0, 0, 0, 0), _groupWarrior);
        new Achievement(36, "Expert Warrior", "Slay one hundred and fifty monsters", _statMonsterKilled, _networkManager.GetAchievementStatus(36), 150,
            new GameReward(0.0, 0, 0, 0, 0), _groupWarrior);
        new Achievement(37, "Master Warrior", "Slay two hundred monsters", _statMonsterKilled, _networkManager.GetAchievementStatus(37), 200,
            new GameReward(0.0, 0, 0, 0, 0), _groupWarrior);
        //*/
    }

    //Requirements: high    Stat: ATTACK_MELEE
    private void CreateFighterAchievements()
    {
        new Achievement(40, "Newbie Fighter", "Use melee attacks ten times", _statAttackMelee, _networkManager.GetAchievementStatus(40), 10,
            new GameReward(0.0, 10, 0, 0, 0), _groupFighter);
        new Achievement(41, "Beginner Fighter", "Use melee attacks twenty times", _statAttackMelee, _networkManager.GetAchievementStatus(41), 20,
            new GameReward(0.0, 20, 0, 0, 0, 1), _groupFighter);
        new Achievement(42, "Apprentice Fighter", "Use melee attacks fifty times", _statAttackMelee, _networkManager.GetAchievementStatus(42), 50,
            new GameReward(0.0, 25, 0, 0, 0, 0, 1), _groupFighter);
        /*
        new Achievement(43, "Trained Fighter", "Use melee attacks one hundred times", _statAttackMelee, _networkManager.GetAchievementStatus(43), 100,
            new GameReward(0.0, 0, 0, 0, 0), _groupFighter);
        new Achievement(44, "Skilled Fighter", "Use melee attacks two hundred times", _statAttackMelee, _networkManager.GetAchievementStatus(44), 200,
            new GameReward(0.0, 0, 0, 0, 0), _groupFighter);
        new Achievement(45, "Adept Fighter", "Use melee attacks five hundred times", _statAttackMelee, _networkManager.GetAchievementStatus(45), 500,
            new GameReward(0.0, 0, 0, 0, 0), _groupFighter);
        new Achievement(46, "Expert Fighter", "Use melee attacks one thousand times", _statAttackMelee, _networkManager.GetAchievementStatus(46), 1000,
            new GameReward(0.0, 0, 0, 0, 0), _groupFighter);
        new Achievement(47, "Master Fighter", "Use melee attacks two thousand times", _statAttackMelee, _networkManager.GetAchievementStatus(47), 2000,
            new GameReward(0.0, 0, 0, 0, 0), _groupFighter);
        //*/
    }

    private void CreateMageAchievements()
    {
        new Achievement(50, "Newbie Mage", "Use magic attacks ten times", _statAttackMagic, _networkManager.GetAchievementStatus(50), 10,
            new GameReward(0.0, 10, 0, 0, 0), _groupMage);
        new Achievement(51, "Beginner Mage", "Use magic attacks twenty times", _statAttackMagic, _networkManager.GetAchievementStatus(51), 20,
            new GameReward(0.0, 20, 0, 0, 0, 1), _groupMage);
        new Achievement(52, "Apprentice Mage", "Use magic attacks fifty times", _statAttackMagic, _networkManager.GetAchievementStatus(52), 50,
            new GameReward(0.0, 25, 0, 0, 0, 0, 1), _groupMage);
        /*
        new Achievement(53, "Trained Mage", "Use magic attacks one hundred times", _statAttackMagic, _networkManager.GetAchievementStatus(53), 100,
            new GameReward(0.0, 0, 0, 0, 0), _groupMage);
        new Achievement(54, "Skilled Mage", "Use magic attacks two hundred times", _statAttackMagic, _networkManager.GetAchievementStatus(54), 200,
            new GameReward(0.0, 0, 0, 0, 0), _groupMage);
        new Achievement(55, "Adept Mage", "Use magic attacks five hundred times", _statAttackMagic, _networkManager.GetAchievementStatus(55), 500,
            new GameReward(0.0, 0, 0, 0, 0), _groupMage);
        new Achievement(56, "Expert Mage", "Use magic attacks one thousand times", _statAttackMagic, _networkManager.GetAchievementStatus(56), 1000,
            new GameReward(0.0, 0, 0, 0, 0), _groupMage);
        new Achievement(57, "Master Mage", "Use magic attacks two thousand times", _statAttackMagic, _networkManager.GetAchievementStatus(57), 2000,
            new GameReward(0.0, 0, 0, 0, 0), _groupMage);
        //*/
    }

    private void CreateHealerAchievements()
    {
        new Achievement(60, "Newbie Medic", "Use healing spells ten times", _statAttackHeal, _networkManager.GetAchievementStatus(60), 10,
            new GameReward(0.0, 10, 0, 0, 0), _groupHealer);
        new Achievement(61, "Beginner Medic", "Use healing spells twenty times", _statAttackHeal, _networkManager.GetAchievementStatus(61), 20,
            new GameReward(0.0, 20, 0, 0, 0, 1), _groupHealer);
        new Achievement(62, "Apprentice Medic", "Use healing spells fifty times", _statAttackHeal, _networkManager.GetAchievementStatus(62), 50,
            new GameReward(0.0, 25, 0, 0, 0, 0, 1), _groupHealer);
        /*
        new Achievement(63, "Trained Medic", "Use healing spells one hundred times", _statAttackHeal, _networkManager.GetAchievementStatus(63), 100,
            new GameReward(0.0, 0, 0, 0, 0), _groupHealer);
        new Achievement(64, "Skilled Medic", "Use healing spells two hundred times", _statAttackHeal, _networkManager.GetAchievementStatus(64), 200,
            new GameReward(0.0, 0, 0, 0, 0), _groupHealer);
        new Achievement(65, "Adept Medic", "Use healing spells five hundred times", _statAttackHeal, _networkManager.GetAchievementStatus(65), 500,
            new GameReward(0.0, 0, 0, 0, 0), _groupHealer);
        new Achievement(66, "Expert Medic", "Use healing spells one thousand times", _statAttackHeal, _networkManager.GetAchievementStatus(66), 1000,
            new GameReward(0.0, 0, 0, 0, 0), _groupHealer);
        new Achievement(67, "Master Medic", "Use healing spells two thousand times", _statAttackHeal, _networkManager.GetAchievementStatus(67), 2000,
            new GameReward(0.0, 0, 0, 0, 0), _groupHealer);
        //*/
    }
}
