using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This script simply changes a sprite and disables any attached objects at a certain quest stage
//This may work with repeatable quests, will require testing

public class QuestSprite : MonoBehaviour {

    public List<GameObject> ToDisable = new List<GameObject>();

    public double RequiredQuest;
    public double RequiredObjective;

    public Sprite BeforeSprite;
    public Sprite AfterSprite;

    public bool DisableThisObject = false;  //Set to true to disable this object when requirement is met

    private EventManager _em;
    private QuestManager _qm;
    private SpriteRenderer _spriteRenderer;
    private bool _questStageReached;
    private bool _forceSpriteChange;

	// Use this for initialization
	void Start ()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _em = player.GetComponent<EventManager>();
            _qm = player.GetComponent<QuestManager>();
        }
        _spriteRenderer = GetComponent<SpriteRenderer>();

        //Default to showing the before sprite
        _spriteRenderer.sprite = BeforeSprite;
        _questStageReached = false;
        _forceSpriteChange = false;
    }

    void Update()
    {
        if (_questStageReached) return;
        else if (_qm == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _em = player.GetComponent<EventManager>();
                _qm = player.GetComponent<QuestManager>();
            }
        }
        else if ((_qm.ObjectiveCompleted(RequiredQuest, RequiredObjective)) || _forceSpriteChange)
        {
            //Change sprite, disable objects, then set flag
            if (AfterSprite != null)
            {
                _spriteRenderer.sprite = AfterSprite;
            }
            foreach(GameObject go in ToDisable)
            {
                go.SetActive(false);
            }
            _questStageReached = true;
            _em.Event(new List<object>() { GameAction.SPRITE_CHANGED, gameObject.name });
            if(DisableThisObject)
            {
                gameObject.SetActive(false);
            }
        }
    }

    //Manually changes sprite on command, useful for cutscenes
    public void ChangeSprite()
    {
        _forceSpriteChange = true;
    }
}
