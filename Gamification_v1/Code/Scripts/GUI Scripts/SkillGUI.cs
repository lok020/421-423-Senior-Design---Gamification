using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class SkillGUI : MonoBehaviour {

    public List<GameObject> skillSlots = new List<GameObject>();
    public Image classImage;
    public GameObject slots;
    public GameObject tooltip;
    public GameObject unlockPrompt;
    public GameObject ItemDraggedIcon;
    public GameObject DraggedItem;
    public Text skillName;
    public Text skillStats;
    public bool isBeingDragged = false;
    public bool inSlot = false;
    public int dragindex;
    public int unlockindex;
    public SkillsManager manager;

    public GameObject[] Skills = new GameObject[16];
    public GameObject[] Passives = new GameObject[2];

    private SkillSlot[] _skillSlots = new SkillSlot[16];
    private SkillSlot[] _passiveSlots = new SkillSlot[2];

    private NetworkManager _network;

    // Use this for initialization
    void Start() {
        manager = GameObject.FindGameObjectWithTag("Skills").GetComponent<SkillsManager>();
        _network = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();

        //Get each skill slot
        foreach (SkillSlot slot in transform.parent.parent.GetComponentsInChildren<SkillSlot>())
        {
            if (slot.name.Contains("Slot"))
            {
                //All are named Slot1 - Slot16
                _skillSlots[(int.Parse(slot.name.Substring(4))) - 1] = slot;
            }
            else
            {
                if (slot.name == "Passive1") _passiveSlots[0] = slot;
                else _passiveSlots[1] = slot;
            }
        }
    }

    public void UpdateSlots() { 
        //Load skills into the corresponding slots
        for (int i = 0; i < Skills.Length; i++)
        {
            if (Skills[i] == null)
            {
                _skillSlots[i].gameObject.SetActive(false);
            }
            else
            {
                _skillSlots[i].gameObject.SetActive(true);
                bool unlocked = _network.SkillUnlocked(Skills[i].GetComponent<AttackDetailsInstance>().ID);
                _skillSlots[i].Reload(this, i, Skills[i], unlocked);
            }
        }
        //Same for passives
        for (int i = 0; i < Passives.Length; i++)
        {
            if (Passives[i] == null)
            {
                _passiveSlots[i].gameObject.SetActive(false);
            }
            else
            {
                _passiveSlots[i].gameObject.SetActive(true);
                bool unlocked = _network.SkillUnlocked(Passives[i].GetComponent<AttackDetailsInstance>().ID);
                _passiveSlots[i].Reload(this, i + 100, Passives[i], unlocked);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if(isBeingDragged)
        {
            //Vector3 position = (Input.mousePosition - GameObject.FindGameObjectWithTag("InventoryCanvas").GetComponent<RectTransform>().localPosition);

            Canvas canvas = GameObject.FindGameObjectWithTag("InventoryCanvas").GetComponent<Canvas>();
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
            ItemDraggedIcon.transform.position = canvas.transform.TransformPoint(pos);

        }
    }

    public void ShowToolTip(Vector3 position, GameObject item)
    {
        AttackDetailsInstance details = item.GetComponent<AttackDetailsInstance>();
        skillName.text = details.AttackName;
        skillStats.text = details.TooltipText();
        var localPosition = GetComponent<RectTransform>().localPosition;

        tooltip.GetComponent<RectTransform>().localPosition = new Vector3(localPosition.x + position.x +
            (_skillSlots[0].GetComponent<RectTransform>().rect.width / 2) + (tooltip.GetComponent<RectTransform>().rect.width / 2),
            localPosition.y + position.y - (tooltip.GetComponent<RectTransform>().rect.height / 2));
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
        manager.IsBeingDragged = true;
        dragindex = number;
        DraggedItem = item;
        ItemDraggedIcon.GetComponent<Image>().sprite = item.GetComponent<AttackDetailsInstance>().Icon;
        ItemDraggedIcon.GetComponent<RectTransform>().SetAsLastSibling();
        ItemDraggedIcon.SetActive(true);
        isBeingDragged = true;
    }

    public void deleteDraggedItem()
    {
        inSlot = false;
        isBeingDragged = false;
        ItemDraggedIcon.SetActive(false);
        DraggedItem = null;
    }

    public void Yes()
    {
        GameObject skill = (unlockindex < 100 ? Skills[unlockindex] : Passives[unlockindex - 100]);
        var adi = skill.GetComponent<AttackDetailsInstance>();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().UnlockSkill(adi.ID, adi.Type != AttackType.PASSIVE);
        unlockPrompt.SetActive(false);
        UpdateSlots();
    }

    public void No()
    {
        unlockPrompt.SetActive(false);
    }
}
