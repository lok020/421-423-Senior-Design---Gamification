using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Text;

public class AchievementGUI : MonoBehaviour {

    //Each of the eight achievement entries. This is the simplest way to track them
    public GameObject Entry1, Entry2, Entry3, Entry4, Entry5, Entry6, Entry7, Entry8;
    //Milestone bar
    public GameObject MilestoneBar;

    //Achievement group currently selected
    private AchievementGroup _group;

    //Milestone currently active
    private AchievementMilestone _milestone;

    //Colors used by buttons
    private Color _greyButton = new Color(0.8f, 0.8f, 0.8f);
    private Color _greyButtonPressed = new Color(0.6f, 0.6f, 0.6f);
    private Color _orangeButton = new Color(1.0f, 0.8f, 0.0f);
    private Color _orangeButtonPressed = new Color(0.9f, 0.7f, 0.1f);
    private Color _greenButton = new Color(0.0f, 0.9f, 0.0f);
    private Color _greenButtonPressed = new Color(0.0f, 0.8f, 0.0f);

    //Simple flag to indicate if a new tab was just selected
    private bool _needsUpdate;

    //References
    private AchievementManager _manager;    //Very important!

	void Start () {
        _manager = GameObject.FindGameObjectWithTag("Player").GetComponent<AchievementManager>();
        _group = _manager.GetAcheivementGroup("Adventurer");    //Default
        _needsUpdate = true;
	}
	
    void Update()
    {
        //If _group is null, the group was not found
        if (_group == null) return;

        //If a new tab was selected or anything changed in the group, update the page
        if(_needsUpdate || _group.AnyChanges)
        {
            _group.AnyChanges = false;
            _needsUpdate = false;
            UpdatePage();
        }
    }

    //When a tab is clicked, update the GUI to show that campaign's achievements
    public void TabClicked(string campaignName)
    {
        _group = _manager.GetAcheivementGroup(campaignName);
        _needsUpdate = true;
    }

    //When a button is clicked, if it is ready to be completed do so
    public void ButtonClicked(Button button)
    {
        //Get the achievement the player is trying to complete
        //There are better ways of doing this, but I'm lazy
        //From the button just clicked, get the name of the achievement it's attached to
        string achievementName = button.transform.parent.Find("Title").GetComponent<Text>().text;
        //Get the corresponding achievement from the current group
        Achievement a = _group.Achievements.Find(x => x.Name == achievementName);
        //If not found, return
        if (a == null) return;
        //If state != 1, return
        if (a.State != 1) return;

        //Get reference to network manager
        var network = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        //If maximum number of achievements have been collected, notify user and return
        if(network.AchievementRewardCount >= NetworkManager.AchievementRewardLimit)
        {
            var messageOverlay = GameObject.FindGameObjectWithTag("MessageOverlay").GetComponent<MessageOverlayController>();
            messageOverlay.EnqueueMessage("You have collected the max number of");
            messageOverlay.EnqueueMessage("rewards for today. Come back tomorrow!");
            return;
        }
        
        //Collect the reward
        DistributeRewards(a.GetReward());
        //Notify the database that this achievement has been completed
        network.DBAchievementCompleted(a.ID);
        //Update the achievement's status server-side
        network.UpdateAchievement(a.ID, a.State);
        //Mark group as changed
        a.Group.AnyChanges = true;
        //Increment reward counter
        network.AchievementRewardCount++;
    }

    //Similar logic to above, except this is only for milestones
    public void MilestoneButtonClicked()
    {
        //Return if reward is not ready to be collected
        if (_milestone.State != 1) return;

        //Get reference to network manager
        var network = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        //If maximum number of achievements have been collected, notify user and return
        if (network.AchievementRewardCount >= NetworkManager.AchievementRewardLimit)
        {
            var messageOverlay = GameObject.FindGameObjectWithTag("MessageOverlay").GetComponent<MessageOverlayController>();
            messageOverlay.EnqueueMessage("You have collected the max number of");
            messageOverlay.EnqueueMessage("rewards for today. Come back tomorrow!"); 
            return;
        }

        //Collect the reward
        DistributeRewards(_milestone.GetReward());
        //Notify the database that this milestone has been completed
        network.DBMilestoneCompleted(_milestone.ID);
        //Update the achievement's status server-side
        network.UpdateMilestone(_milestone.ID, _milestone.State);
        //Mark that UI needs to be changed
        _needsUpdate = true;
        //Increment reward counter
        network.AchievementRewardCount++;
    }

    //Updates current page. If a scroll bar is added, this function will need to be modified slightly
    private void UpdatePage()
    {
        //Update each entry with the first eight achievements (if there are eight)
        for (int i = 0; i < _group.Achievements.Count && i < 8; i++)
        {
            UpdateEntry(i + 1, _group.Achievements[i]);
        }
        //If there are less than eight achievements, hide the rest
        for (int i = _group.Achievements.Count; i < 8; i++)
        {
            HideEntry(i + 1);
        }
        //Update the milestone bar
        //UpdateMilestoneBar();
    }

    //Hides an entry
    private void HideEntry(int index)
    {
        GameObject entry = GetEntryByIndex(index);
        if (entry == null) return;
        entry.SetActive(false);
    }

    //Updates an entry's text and button
    private void UpdateEntry(int index, Achievement achievement)
    {
        //Get the entry based on the index
        GameObject entry = GetEntryByIndex(index);
        //If entry was not found do nothing and return now
        if (entry == null) return;
        entry.SetActive(true);

        //Update entry->Title to the achievement's title text
        entry.transform.Find("Title").GetComponent<Text>().text = achievement.Name;

        //If state == 0, achievement needs to be completed
        if(achievement.State == 0)
        {
            //Button should be grey
            var button = entry.transform.Find("Button");
            var colors = button.GetComponent<Button>().colors;
            colors.normalColor = _greyButton;
            colors.highlightedColor = _greyButton;
            colors.pressedColor = _greyButtonPressed;
            button.GetComponent<Button>().colors = colors;

            //Set "ButtonText" to "<stat>/<requirement>"
            button.transform.Find("ButtonText").GetComponent<Text>().text = achievement.Stat.Value.ToString() + "/" + achievement.Requirement.ToString();
        }
        //If state == 1, achievement reward is ready to be completed
        else if(achievement.State == 1)
        {
            //Button should be orange
            var button = entry.transform.Find("Button");
            var colors = button.GetComponent<Button>().colors;
            colors.normalColor = _orangeButton;
            colors.highlightedColor = _orangeButton;
            colors.pressedColor = _orangeButtonPressed;
            button.GetComponent<Button>().colors = colors;

            //Set "ButtonText" to "Get Reward"
            button.transform.Find("ButtonText").GetComponent<Text>().text = "Get Reward";
        }
        //If state == 2, achievement is complete
        else if(achievement.State == 2)
        {
            //Button should be green
            var button = entry.transform.Find("Button");
            var colors = button.GetComponent<Button>().colors;
            colors.normalColor = _greenButton;
            colors.highlightedColor = _greenButton;
            colors.pressedColor = _greenButtonPressed;
            button.GetComponent<Button>().colors = colors;

            //Set "ButtonText" to "Complete!"
            button.transform.Find("ButtonText").GetComponent<Text>().text = "Complete!";
        }
    }

    //Updates milestone bar, has some similar logic to UpdateEntry
    private void UpdateMilestoneBar()
    {
        //First, find either any milestone whose reward has not been found, or
        //the first uncompleted one with the lowest requirement
        _milestone = null;
        foreach (AchievementMilestone m in _group.Milestones)
        {
            //Select any milestone whose reward has not been found
            if (m.State == 1)
            {
                _milestone = m;
                break;
            }
            //Else, pick an incomplete milestone with the lowest requirement
            else if (m.State == 0)
            {
                //Default to the first one found
                if (_milestone == null)
                {
                    _milestone = m;
                }
                //Save this one if it has a lower requirement
                else if (m.Required < _milestone.Required)
                {
                    _milestone = m;
                }
            }
        }
        //There are three possibilities now
        // 1 - milestone is null, aka all milestones have been completed
        if (_milestone == null)
        {
            //Set milestone text to "Campaign Complete!"
            MilestoneBar.transform.Find("Title").GetComponent<Text>().text = "Campaign Complete!";

            //Set button to green
            var button = MilestoneBar.transform.Find("Button");
            var colors = button.GetComponent<Button>().colors;
            colors.normalColor = _greenButton;
            colors.highlightedColor = _greenButton;
            colors.pressedColor = _greenButtonPressed;
            button.GetComponent<Button>().colors = colors;

            //Set buttom text to "Complete!"
            button.transform.Find("ButtonText").GetComponent<Text>().text = "Complete!";
        }
        // 2 - milestone state is 0, no rewards to be collected
        else if (_milestone.State == 0)
        {
            //Set milestone text to "Campaign Reward"
            MilestoneBar.transform.Find("Title").GetComponent<Text>().text = "Campaign Reward";

            //Button should be grey
            var button = MilestoneBar.transform.Find("Button");
            var colors = button.GetComponent<Button>().colors;
            colors.normalColor = _greyButton;
            colors.highlightedColor = _greyButton;
            colors.pressedColor = _greyButtonPressed;
            button.GetComponent<Button>().colors = colors;

            //Get number of achievements completed
            int completed = 0;
            foreach (Achievement a in _group.Achievements)
            {
                if (a.State == 2)
                {
                    completed++;
                }
            }

            //Set "ButtonText" to "<stat>/<requirement>"
            button.transform.Find("ButtonText").GetComponent<Text>().text = completed.ToString() + "/" + _milestone.Required.ToString();
        }
        // 3 - milestone state is 1, rewards needs to be collected
        else
        {
            //Set milestone text to "Campaign Reward"
            MilestoneBar.transform.Find("Title").GetComponent<Text>().text = "Campaign Reward";

            //Button should be orange
            var button = MilestoneBar.transform.Find("Button");
            var colors = button.GetComponent<Button>().colors;
            colors.normalColor = _orangeButton;
            colors.highlightedColor = _orangeButton;
            colors.pressedColor = _orangeButtonPressed;
            button.GetComponent<Button>().colors = colors;

            //Set "ButtonText" to "Get Reward"
            button.transform.Find("ButtonText").GetComponent<Text>().text = "Get Reward";
        }
    }

    private void DistributeRewards(GameReward reward)
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        var player = p.GetComponent<PlayerController>();
        var inventory = p.GetComponent<Inventory>();
        var messageOverlay = GameObject.FindGameObjectWithTag("MessageOverlay").GetComponent<MessageOverlayController>();

        StringBuilder sb = new StringBuilder();
        bool anyAdded = false;
        sb.Append("Received ");

        //XP
        if (reward.XPReward > 0)
        {
            anyAdded = true;
            player.AddXP(reward.XPReward);
            sb.Append(reward.XPReward + " XP");
        }
        //Gold
        if (reward.GoldReward > 0)
        {
            anyAdded = true;
            inventory.AddGold(reward.GoldReward);
            if (anyAdded) sb.Append(" and ");
            sb.Append(reward.GoldReward + " gold");
        }
        //Item
        if (reward.ItemRewardCount > 0)
        {
            anyAdded = true;
            inventory.AddItemToInventory(reward.ItemReward, reward.ItemRewardCount);
            if (anyAdded) sb.Append(" and ");
            string itemName = (Resources.Load(Item.Path[reward.ItemReward]) as GameObject).GetComponent<Item>().Name;
            sb.Append(reward.ItemRewardCount + " " + itemName);
        }
        //Stat points
        if(reward.StatPoints > 0)
        {
            anyAdded = true;
            player.Stats.AddStatPoints(reward.StatPoints);
            if (anyAdded) sb.Append(" and ");
            sb.Append(reward.StatPoints + " stat points");
        }
        //Skill points
        if(reward.SkillPoints > 0)
        {
            anyAdded = true;
            player.Stats.AddSkillPoints(reward.SkillPoints);
            if (anyAdded) sb.Append(" and ");
            sb.Append(reward.SkillPoints + " skill points");
        }

        //Format end of string
        if (anyAdded)
        {
            sb.Append("!");
        }
        else
        {
            sb.Append("nothing!");
        }

        //Print reward description to user
        messageOverlay.EnqueueMessage(sb.ToString());
    }


    //Gets entry by index, pretty straightforward
    private GameObject GetEntryByIndex(int index)
    {
        switch (index)
        {
            case 1: return Entry1;
            case 2: return Entry2;
            case 3: return Entry3;
            case 4: return Entry4;
            case 5: return Entry5;
            case 6: return Entry6;
            case 7: return Entry7;
            case 8: return Entry8;
        }
        return null;
    }
}
