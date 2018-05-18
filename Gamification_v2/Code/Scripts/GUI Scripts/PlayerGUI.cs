using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerGUI : MonoBehaviour {

    public List<GameObject> equipmentSlots = new List<GameObject>();
    public List<GameObject> statAllocationPanels = new List<GameObject>();
    public Button acceptStats;
    public GameObject statButtonPanel;
    public GameObject slots;
    public GameObject tooltip;
    public GameObject itemOptions;
    public GameObject statPointsText;
    public Text itemName;
    public Text itemStats;
    public Text unequip;
    public Text discard;
    public PlayerController player;
    public Text playerName;
    public Text playerStats;
    private float x = 0;
    private float y = 0;
    private int scaledHeight;
    private int scaledWidth;
    private PlayerStats stats;
    public bool statsBeingAllocated;
    private bool baseStatsGrabbed;
    public int patk, pdef, matk, mdef;
    public float speed;

    private int _basePhysicalAttack, _basePhysicalDefense, _baseMagicalAttack, _baseMagicalDefense;
    private float _baseSpeed;

    public Item TempItem;

	void Start () {
        int slotNumber = 0;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        scaledHeight = (int)(GetComponent<RectTransform>().rect.height - 30) / 5;
        scaledWidth = scaledHeight;
        acceptStats.gameObject.SetActive(true);
        baseStatsGrabbed = false;

        x = 5;
        y = GetComponent<RectTransform>().rect.yMax - 2;
        //Armor slots; helmet, platebody, gauntlets, platelegs, boots
        for(int i = 0; i < 5; i++)
        {
            GameObject slot = Instantiate(slots);
            slot.GetComponent<EquipmentSlot>().SlotNumber = i + 2;
            equipmentSlots.Add(slot);
            slot.transform.SetParent(transform, false);
            slot.name = "Slot " + (i + 3);
            slot.GetComponent<RectTransform>().sizeDelta = new Vector3(scaledWidth, scaledHeight, 0);
            slot.GetComponent<RectTransform>().localPosition = new Vector3(x + (GetComponent<RectTransform>().rect.xMax * 2 / 8), y + (5 + scaledHeight) * (-0.5f - i), 0);
            slotNumber++;
        }
        //Weapon and shield slots
        for (int i = 1; i < 3; i++)
        {
            GameObject slot = Instantiate(slots);
            slot.GetComponent<EquipmentSlot>().SlotNumber = i - 1;
            equipmentSlots.Add(slot);
            slot.transform.SetParent(transform, false);
            slot.name = "Slot " + i;
            slot.GetComponent<RectTransform>().sizeDelta = new Vector3(scaledWidth, scaledHeight, 0);
            slot.GetComponent<RectTransform>().localPosition = new Vector3(x + (GetComponent<RectTransform>().rect.xMax * 4 / 8), y + (5 + scaledHeight) * (-0.5f - i), 0);
            slotNumber++;
        }
        //Accessory slots: necklace, left ring, right ring
        for (int i = 0; i < 3; i++)
        {
            GameObject slot = Instantiate(slots);
            slot.GetComponent<EquipmentSlot>().SlotNumber = i + 7;
            equipmentSlots.Add(slot);
            slot.transform.SetParent(transform, false);
            slot.name = "Slot " + (i + 7);
            slot.GetComponent<RectTransform>().sizeDelta = new Vector3(scaledWidth, scaledHeight, 0);
            slot.GetComponent<RectTransform>().localPosition = new Vector3(x + (GetComponent<RectTransform>().rect.xMax * 6 / 8), y + (5 + scaledHeight) * (-0.5f - i), 0);
            slotNumber++;
        }
        //Point allocation thingies
        for (int i = 0; i < 5; i++)
        {
            GameObject panel = (GameObject)Instantiate(statButtonPanel);
            statAllocationPanels.Add(panel);
            panel.transform.SetParent(this.gameObject.transform, false);
            int tempValue = i;
            panel.GetComponent<RectTransform>().GetChild(0).GetComponent<Button>().onClick.AddListener(() => DecreaseStat(tempValue));
            panel.GetComponent<RectTransform>().GetChild(1).GetComponent<Button>().onClick.AddListener(() => IncreaseStat(tempValue));
            panel.GetComponent<RectTransform>().localPosition = new Vector3(-10, -5 - (i * 18.5f), 0);
            panel.SetActive(false);
        }
    }
	
	void Update () {
        DisplayStats();
        if(stats.StatPoints > 0) //there are stats ready to be allocated
        {
            statsBeingAllocated = true;
        }
        if(statsBeingAllocated)
        {
            if(!baseStatsGrabbed) //only want to grab them once
            {
                baseStatsGrabbed = true;
                _basePhysicalAttack = stats.PhysicalAttack;
                _basePhysicalDefense = stats.PhysicalDefense;
                _baseMagicalAttack = stats.MagicalAttack;
                _baseMagicalDefense = stats.MagicalDefense;
                _baseSpeed = stats.Speed;
                acceptStats.gameObject.SetActive(true);
                for (int i = 0; i < 5; i++)
                {
                    statAllocationPanels[i].SetActive(true);
                }
            }
            //Show number of stat points available

        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                statAllocationPanels[i].SetActive(false);
            }
            acceptStats.gameObject.SetActive(false);
        }

	}

    public void DisplayStats()
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        playerName.text = player.Name;
        playerStats.text = String.Format("{0,-15} {1,4:0.##}\n\n", "Level", stats.PlayerLevel) +
                           String.Format("{0,-12} {1,4:0.##}\n\n", "Health", player.Health + "/" + player.BaseHealth) +
                           String.Format("{0,-12} {1,4:0.##}\n\n", "Exp", stats.XP + "/" + LimitController.LevelReqs[stats.PlayerLevel +1]) +
                           String.Format("{0,-16} {1,3:0.##}\n\n", "Gold", player.GetComponent<Inventory>().Gold) +
                           String.Format("{0,-16} {1,3:0.##}\n\n", "Physical Attack", stats.EffectivePhysicalAttack()) +
                           String.Format("{0,-16} {1,3:0.##}\n\n", "Magical Attack", stats.EffectiveMagicalAttack()) +
                           String.Format("{0,-16} {1,3:0.##}\n\n", "Physical Defense", stats.EffectivePhysicalDefense()) +
                           String.Format("{0,-16} {1,3:0.##}\n\n", "Magical Defense", stats.EffectiveMagicalDefense()) +
                           String.Format("{0,-16} {1,3:0.##}\n\n", "Speed", stats.EffectiveSpeed());
        if (stats.StatPoints > 0)
        {
            statPointsText.GetComponent<Text>().text = "Stat Points  " + stats.StatPoints;
        }
        else
        {
            statPointsText.GetComponent<Text>().text = "";
        }
    }

    public void ShowToolTip(RectTransform slotTransform, Item item)
    {
        itemName.text = item.Name;

        itemStats.text = item.GetTooltip();

        var local = GetComponent<RectTransform>().localPosition;
        var tRect = tooltip.GetComponent<RectTransform>();
        tRect.localPosition = new Vector3(local.x + slotTransform.localPosition.x + (slotTransform.rect.width / 2),
            local.y + slotTransform.localPosition.y + (slotTransform.rect.height / 2));
        tRect.SetAsLastSibling();
        tooltip.SetActive(true);
    }

    public void ExitToolTip()
    {
        tooltip.SetActive(false);
        itemName.text = "";
        itemStats.text = "";
    }

    public void ShowItemOptions(RectTransform slotTransform, Item item)
    {
        TempItem = item;
        unequip.text = "Unequip";
        
        var local = GetComponent<RectTransform>().localPosition;
        var tRect = itemOptions.GetComponent<RectTransform>();
        tRect.localPosition = new Vector3(local.x + slotTransform.localPosition.x + (slotTransform.rect.width / 2),
            local.y + slotTransform.localPosition.y + (slotTransform.rect.height / 2));
        tRect.SetAsLastSibling();
        itemOptions.SetActive(true);
    }

    public void ExitItemOptions()
    {
        itemOptions.SetActive(false);
        unequip.text = "";
    }

    public void Unequip()
    {
        player.Unequip(TempItem);
        player.Stats.ReloadEnchantments();
        ExitItemOptions();
    }

    public void Discard()
    {
        player.equippedGear[(int)TempItem.GearType] = null;
        player.Stats.ReloadEnchantments();
        ExitItemOptions();
    }

    public void IncreaseStat(int statIndex)
    {
        switch(statIndex)
        {
            case 0:
                stats.LevelUpStat(PlayerStats.Stat.PHYSICAL_ATTACK);
                break;
            case 1:
                stats.LevelUpStat(PlayerStats.Stat.MAGICAL_ATTACK);
                break;
            case 2:
                stats.LevelUpStat(PlayerStats.Stat.PHYSICAL_DEFENSE);
                break;
            case 3:
                stats.LevelUpStat(PlayerStats.Stat.MAGICAL_DEFENSE);
                break;
            case 4:
                stats.LevelUpStat(PlayerStats.Stat.SPEED);
                break;
        }
    }

    public void DecreaseStat(int statIndex)
    {
        Debug.Log("Decreasing!");
        switch (statIndex)
        {
            case 0:
                stats.DecreaseStat(PlayerStats.Stat.PHYSICAL_ATTACK, _basePhysicalAttack);
                break;
            case 1:
                stats.DecreaseStat(PlayerStats.Stat.MAGICAL_ATTACK, _baseMagicalAttack);
                break;
            case 2:
                stats.DecreaseStat(PlayerStats.Stat.PHYSICAL_DEFENSE, _basePhysicalDefense);
                break;
            case 3:
                stats.DecreaseStat(PlayerStats.Stat.MAGICAL_DEFENSE, _baseMagicalDefense);
                break;
            case 4:
                stats.DecreaseStat(PlayerStats.Stat.SPEED, _baseSpeed);
                break;
        }
    }

    public void Accept()
    {
        if (stats.StatPoints == 0)
        {
            statsBeingAllocated = false;
            baseStatsGrabbed = false;
            acceptStats.gameObject.SetActive(false);
        }
        _basePhysicalAttack = stats.PhysicalAttack;
        _basePhysicalDefense = stats.PhysicalDefense;
        _baseMagicalAttack = stats.MagicalAttack;
        _baseMagicalDefense = stats.MagicalDefense;
        _baseSpeed = stats.Speed;
    }
}
