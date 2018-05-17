using UnityEngine;
using System.Collections;

public class QuestMarker : MonoBehaviour {

    public string Name;
    public double RequiredQuestID, RequiredObjectiveID;
    public double HideAfterQuestID, HideAfterObjectiveID;

    private int State = 0;  //Inactive, fade in, active, fade out, done
    private float FadeInTime = 0.5f;    //Self explanatory
    private float FadeOutTime = 0.5f;
    private float TimeRemaining = 0;
    private QuestManager questManager = null;   //Component links
    private SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
        var player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) questManager = player.GetComponent<QuestManager>();
        sprite = GetComponent<SpriteRenderer>();
	}

    void Update()
    {
        //Don't update if this is disabled
        if (State == 4) return;
        
        //If quest manager hasn't been linked, get it
        if(questManager == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                questManager = player.GetComponent<QuestManager>();
            }

        }
        //If marker is not active, keep checking if it can be activated
        if(questManager != null && State == 0)
        {
            //Check if it can be activated
            bool canActivate = questManager.ObjectiveCompleted(RequiredQuestID, RequiredObjectiveID);
            bool shouldHide = questManager.ObjectiveCompleted(HideAfterQuestID, HideAfterObjectiveID);
            //Disable this if it should be hidden
            if(shouldHide)
            {
                State = 4;
                return;
            }
            else if(canActivate && !shouldHide)
            {
                State = 1;
                TimeRemaining = FadeInTime;
            }
        }
        //Fade in
        else if(State == 1)
        {
            TimeRemaining -= Time.deltaTime;
            if(TimeRemaining <= 0)
            {
                State = 2;
                TimeRemaining = 0;
            }
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, (FadeInTime - TimeRemaining) / FadeInTime);
        }
        //Update() doesn't need to do anything if the marker is active
        //Fade out
        else if(State == 3)
        {
            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0)
            {
                State = 4;
                TimeRemaining = 0;
            }
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, (TimeRemaining) / FadeOutTime);
        }
    }

    //Hide this marker if the player collides with it and update their quest log
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && State == 2)
        {
            //Notify the event manager
            other.GetComponent<EventManager>().Event(new System.Collections.Generic.List<object>() { GameAction.QUEST_MARKER, Name });
            //Hide this object
            State = 3;
            TimeRemaining = FadeOutTime;
        }
    }
}
