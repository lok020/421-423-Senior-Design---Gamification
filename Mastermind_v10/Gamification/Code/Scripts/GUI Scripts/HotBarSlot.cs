using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HotBarSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public bool isSlotted; //to check if there is a skill in the slot
    public GameObject skill;
    public int slotNumber;
    public HotbarGUI hotbarGUI;
    public SkillsManager skillManager;
    public Sprite defaultIcon;
    public PlayerController player;
    public string slotType;
    public CombatManager manager;
    private IEnumerator routine;
    private NetworkManager db;
    private bool ActiveSkillbar;

	// Use this for initialization
	void Start () {
        hotbarGUI = GetComponentInParent<HotbarGUI>();
        ActiveSkillbar = hotbarGUI.ActiveSkillbar;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        db = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        isSlotted = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if(player.InCombat)
        {
            manager = GameObject.FindGameObjectWithTag("CombatManager").GetComponent<CombatManager>();
        }
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSlotted && !hotbarGUI.isBeingDragged && !player.InCombat)
        {
            hotbarGUI.showToolTip(GetComponent<RectTransform>().localPosition, skill);
        }
        else if (isSlotted && !hotbarGUI.isBeingDragged && player.InCombat)
        {
            routine = delayToolTip();
            StartCoroutine(routine);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!player.InCombat)
        {
            hotbarGUI.exitToolTip();
        }
        else if(player.InCombat && routine != null)
        {
            StopCoroutine(routine);
            hotbarGUI.exitToolTip();
        }
    }

    IEnumerator delayToolTip()
    {
        yield return new WaitForSeconds(0.5f);
        hotbarGUI.showToolTip(GetComponent<RectTransform>().localPosition, skill);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("slot clicked");
        hotbarGUI.exitToolTip();
        if (player.InCombat && isSlotted) //in combat, do combat things
        {
            AttackDetails details = new AttackDetails(skill.GetComponent<AttackDetailsInstance>());
            manager.AttackHelper(details);
        }
        else //not in combat, customization of hotbar
        {
            if (slotNumber < 10) //can only use slots 0-9
            {
                //Equipping from skill manager, not in bar already
                if (eventData.pointerId == -1 && skillManager.IsBeingDragged)
                {
                    if (ActiveOrPassive(skillManager.getDraggedItem().GetComponent<AttackDetailsInstance>().Type))
                    {
                        skill = skillManager.getDraggedItem();
                        int id = skill.GetComponent<AttackDetailsInstance>().ID;
                        //If this skill is already equipped, unequip it
                        int currentSlot = hotbarGUI.GetSlot(id, slotNumber);
                        if(currentSlot >= 0)
                        {
                            var hbs = hotbarGUI.HotbarSlots[currentSlot].GetComponent<HotBarSlot>();
                            hbs.skill = null;
                            hbs.GetComponent<Image>().sprite = defaultIcon;
                            hbs.isSlotted = false;
                        }
                        //Equip this skill in the specified slot
                        GetComponent<Image>().sprite = skill.GetComponent<AttackDetailsInstance>().Icon;
                        isSlotted = true;
                        db.UpdateSkill(id, slotNumber, ActiveSkillbar);
                        skillManager.DeleteDraggedItem();
                    }
                }
                //Equipping from hot bar to...
                else
                {
                    //Empty space (unequip)
                    if (eventData.pointerId == -1 && isSlotted && !hotbarGUI.isBeingDragged)
                    {
                        hotbarGUI.showDraggedItem(skill, slotNumber);
                        int skillId = skill.GetComponent<AttackDetailsInstance>().ID;
                        skill = null;
                        GetComponent<Image>().sprite = defaultIcon;
                        isSlotted = false;
                        db.UpdateSkill(skillId, -1, ActiveSkillbar);
                    }
                    //To empty hotbar slot
                    else if (eventData.pointerId == -1 && !isSlotted && hotbarGUI.isBeingDragged)
                    {
                        if (ActiveOrPassive(hotbarGUI.DraggedItem.GetComponent<AttackDetailsInstance>().Type))
                        {
                            skill = hotbarGUI.DraggedItem;
                            GetComponent<Image>().sprite = skill.GetComponent<AttackDetailsInstance>().Icon;
                            hotbarGUI.deleteDraggedItem();
                            isSlotted = true;
                            db.UpdateSkill(skill.GetComponent<AttackDetailsInstance>().ID, slotNumber, ActiveSkillbar);
                        }
                    }
                    //To occupied hotbar slot (swap the skills)
                    else if (eventData.pointerId == -1 && isSlotted && hotbarGUI.isBeingDragged)
                    {
                        if (ActiveOrPassive(hotbarGUI.DraggedItem.GetComponent<AttackDetailsInstance>().Type))
                        {
                            hotbarGUI.HotbarSlots[hotbarGUI.slotnumber].GetComponent<HotBarSlot>().skill = skill;
                            hotbarGUI.HotbarSlots[hotbarGUI.slotnumber].GetComponent<Image>().sprite = skill.GetComponent<AttackDetailsInstance>().Icon;
                            hotbarGUI.HotbarSlots[hotbarGUI.slotnumber].GetComponent<HotBarSlot>().isSlotted = true;
                            skill = hotbarGUI.DraggedItem;
                            isSlotted = true;
                            GetComponent<Image>().sprite = skill.GetComponent<AttackDetailsInstance>().Icon;
                            hotbarGUI.deleteDraggedItem();
                            db.UpdateSkill(hotbarGUI.HotbarSlots[hotbarGUI.slotnumber].GetComponent<HotBarSlot>().skill.GetComponent<AttackDetailsInstance>().ID, hotbarGUI.slotnumber, ActiveSkillbar);
                            db.UpdateSkill(skill.GetComponent<AttackDetailsInstance>().ID, slotNumber, ActiveSkillbar);
                        }
                    }
                }
            }
        }
    }

    //Returns true if this is an active slot and skill is active and vice versa
    private bool ActiveOrPassive(AttackType Type)
    {
        return ((Type != AttackType.PASSIVE && slotType == "Active") ||
                (Type == AttackType.PASSIVE && slotType == "Passive"));
    }
}
