using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class HotbarGUI : MonoBehaviour {

    public List<GameObject> HotbarSlots = new List<GameObject>();
    public GameObject tooltip;
    public GameObject ItemDraggedIcon;
    public GameObject DraggedItem;
    public bool isBeingDragged = false;
    public Text skillName;
    public Text skillStats;
    public int slotnumber;
    public PlayerController player;
    public int numSlots;
    public string hotbarType;

    public bool ActiveSkillbar;     //True for active bar, false for passives

    private bool equippedSkillsFromDB = false;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < numSlots; i++)
        {
            HotbarSlots.Add(gameObject.transform.GetChild(i).gameObject);
            HotbarSlots[i].GetComponent<HotBarSlot>().slotNumber = i;
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //Equip skills
        var db = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        db.EquipPlayerSkills(this, ActiveSkillbar);
    }
	
	// Update is called once per frame
	void Update () {
        if (isBeingDragged)
        {
            Canvas canvas = GameObject.FindGameObjectWithTag("InventoryCanvas").GetComponent<Canvas>();
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
            ItemDraggedIcon.transform.position = canvas.transform.TransformPoint(pos);
        }
        if (hotbarType == "Active")
        {
            player.LoadAttacks(HotbarSlots);
        }
        else
        {
            player.LoadPassives(HotbarSlots);
        }

        if(equippedSkillsFromDB == false)
        {
            equippedSkillsFromDB = true;
            //Equip skills
            var db = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
            db.EquipPlayerSkills(this, ActiveSkillbar);
        }
	}

    public void showToolTip(Vector3 position, GameObject item)
    {
        AttackDetailsInstance details = item.GetComponent<AttackDetailsInstance>();
        skillName.text = details.AttackName;
        skillStats.text = details.TooltipText();
        tooltip.GetComponent<RectTransform>().localPosition = new Vector3(this.GetComponent<RectTransform>().localPosition.x + position.x +
            (HotbarSlots[0].GetComponent<RectTransform>().rect.width / 2) + (tooltip.GetComponent<RectTransform>().rect.width / 2),
            this.GetComponent<RectTransform>().localPosition.y + position.y + (tooltip.GetComponent<RectTransform>().rect.height / 2));
        tooltip.GetComponent<RectTransform>().SetAsLastSibling();
        tooltip.SetActive(true);
    }

    public void exitToolTip()
    {
        tooltip.SetActive(false);
        skillName.text = "";
        skillStats.text = "";
    }

    public void showDraggedItem(GameObject item, int number)
    {
        exitToolTip();
        slotnumber = number;
        DraggedItem = item;
        ItemDraggedIcon.GetComponent<Image>().sprite = item.GetComponent<AttackDetailsInstance>().Icon;
        ItemDraggedIcon.GetComponent<RectTransform>().sizeDelta = new Vector3(HotbarSlots[slotnumber].GetComponent<RectTransform>().rect.width, HotbarSlots[slotnumber].GetComponent<RectTransform>().rect.height);
        ItemDraggedIcon.GetComponent<RectTransform>().SetAsLastSibling();
        ItemDraggedIcon.SetActive(true);
        isBeingDragged = true;
    }

    public void deleteDraggedItem()
    {
        isBeingDragged = false;
        ItemDraggedIcon.SetActive(false);
        DraggedItem = null;
    }

    public void EquipSkill(int skillID, int slotNumber)
    {
        HotbarSlots[slotNumber].GetComponent<HotBarSlot>().skill = Resources.Load(AttackDetailsInstance.SkillPathnames[skillID]) as GameObject;
        HotbarSlots[slotNumber].GetComponent<HotBarSlot>().isSlotted = true;
        HotbarSlots[slotNumber].GetComponent<HotBarSlot>().GetComponent<Image>().sprite = HotbarSlots[slotNumber].GetComponent<HotBarSlot>().skill.GetComponent<AttackDetailsInstance>().Icon;
    }

    public int GetSlot(int skillID, int ignoreSlot)
    {
        for(int i = 0; i < HotbarSlots.Count; i++)
        {
            if (i == ignoreSlot) continue;
            var hbs = HotbarSlots[i].GetComponent<HotBarSlot>();
            if (hbs.skill == null) continue;
            if(hbs.skill.GetComponent<AttackDetailsInstance>().ID == skillID)
            {
                return i;
            }
        }
        return -1;
    }
}
