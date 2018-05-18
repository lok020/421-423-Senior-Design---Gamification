using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

    public GameObject Achievements;
    public GameObject Inventory;
    public GameObject Equipment;
    public GameObject Crafting;
    public GameObject UpgradeUI;
    public GameObject Skills;
    public GameObject HotBar;
    public GameObject PassivesHotbar;
    public GameObject GUIPanel;
    public GameObject DeletePrompt;
    public GameObject LogoutPrompt;
    public PlayerController player;
    public Inventory inventory;
    public CraftingGUI craftingGUI;
    public InventoryGUI inventoryGUI;
    public PlayerGUI playerGUI;
    public GameObject sword;
    public GameObject potion;
    public GameObject tooltip;
    public GameObject RunAwayPrompt;
    public GameObject ChatInputGUI;

    public void ClickAchievements()
    {
        Achievements.SetActive(!Achievements.activeSelf);
    }

    public void ClickInventory()
    {
        if (Inventory.activeSelf == false)
        {
            Inventory.SetActive(true);
        }
        else
        {
            ExitInventory();
        }
        HotBar.SetActive(true);
        PassivesHotbar.SetActive(true);
    }

    public void ClickSkills()
    {
        if (Skills.activeSelf == false)
        {
            Skills.SetActive(true);
        }
        else
        {
            ExitSkills();
        }
        HotBar.SetActive(true);
        PassivesHotbar.SetActive(true);
    }

    public void ClickCrafting()
    {
        if (Crafting.activeSelf == false)
        {
            Crafting.SetActive(true);
        }
        else
        {
            ExitCrafting();
        }
    }

    public void ClickEquipment()
    {
        if (Equipment.activeSelf == false)
        {
            Equipment.SetActive(true);
        }
        else
        {
            ExitEquipment();
        }
    }

    public void ClickLogout()
    {
        LogoutPrompt.SetActive(true);
    }

    public void ClickUpgrade()
    {
        if (UpgradeUI.activeSelf == false)
        {
            UpgradeUI.SetActive(true);
        }
        else
        {
            ExitUpgrade();
        }
    }

    public void ExitAchievements()
    {
        Achievements.SetActive(false);
    }

    public void ExitInventory()
    {
        if (inventoryGUI.isBeingDragged)
        {
            inventoryGUI.ReturnItemToInventory();
        }
        Inventory.GetComponentInChildren<InventoryGUI>().ExitToolTip();
        Inventory.GetComponentInChildren<InventoryGUI>().ExitItemOptions();
        Inventory.SetActive(false);
    }

    public void ExitEquipment()
    {
        if (inventoryGUI.isBeingDragged)
        {
            inventoryGUI.ReturnItemToInventory();
        }
        Equipment.GetComponentInChildren<PlayerGUI>().ExitToolTip();
        Equipment.GetComponentInChildren<PlayerGUI>().ExitItemOptions();
        Equipment.SetActive(false);
    }

    public void ExitCrafting()
    {
        if (inventoryGUI.isBeingDragged)
        {
            inventoryGUI.ReturnItemToInventory();
        }
        Crafting.GetComponentInChildren<CraftingGUI>().ExitToolTip();
        Crafting.SetActive(false);
    }

    public void ExitSkills()
    {
        if (inventoryGUI.isBeingDragged)
        {
            inventoryGUI.ReturnItemToInventory();
        }
        if (!Skills.GetComponent<SkillsManager>().IsBeingDragged)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(Skills.GetComponent<RectTransform>(), Input.mousePosition))
            {
                if (tooltip != null) tooltip.SetActive(false);
            }
            Skills.SetActive(false);
        }
    }

    public void ExitUpgrade()
    {
        if (inventoryGUI.isBeingDragged)
        {
            inventoryGUI.ReturnItemToInventory();
        }
        var gui = UpgradeUI.GetComponentInChildren<UpgradeGUI>();
        gui.ExitTooltip();
        gui.ExitContextMenu();
        gui.ReturnItemsToInventory();
        UpgradeUI.SetActive(false);
    }

    public void Yes()
    {
        Application.Quit();
    }

    public void No()
    {
        LogoutPrompt.SetActive(false);
    }

	// Use this for initialization
	void Start () 
    {
        GetComponent<Canvas>().enabled = true;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        Inventory.SetActive(false);
        Equipment.SetActive(false);
        Crafting.SetActive(false);
        Skills.SetActive(false);
        HotBar.SetActive(false);
        PassivesHotbar.SetActive(false);
        GUIPanel.SetActive(true);
        DeletePrompt.SetActive(false);
        LogoutPrompt.SetActive(false);
        RunAwayPrompt.SetActive(false);
        ChatInputGUI.SetActive(true);
        DontDestroyOnLoad(this);
    }
	
	// Update is called once per frame
    void Update()
    {
        if (player.InCombat || player.InCutscene)
        {
            HotBar.SetActive(true);
            if (player.InCutscene)
            {
                HotBar.SetActive(false);
            }
            PassivesHotbar.SetActive(false);
            Inventory.SetActive(false);
            Equipment.SetActive(false);
            Crafting.SetActive(false);
            Skills.SetActive(false);
            GUIPanel.SetActive(false);
            if (Input.GetKeyDown("escape"))
            {
                RunAwayPrompt.SetActive(true);
            }
            ChatInputGUI.SetActive(false);
        }
        else
        {
            GUIPanel.SetActive(true);
            ChatInputGUI.SetActive(true);
            /*if(Input.GetKeyDown("t"))
            {
                inventory.AddItemToInventory(sword);
            }
            if (Input.GetKeyDown("y"))
            {
                inventory.AddItemToInventory(potion);
            }*/
            if (Input.GetKeyDown("i") && !player.IsTypingChatMessage) //inventory
            {
                ClickInventory();
            }
            if (Input.GetKeyDown("c") && !player.IsTypingChatMessage) //equipment
            {
                ClickEquipment();
            }
            if (Input.GetKeyDown("n") && !player.IsTypingChatMessage) //crafting
            {
                ClickCrafting();
            }
            if (Input.GetKeyDown("u") && !player.IsTypingChatMessage) //skills
            {
                ClickSkills();
            }
            if (Input.GetKeyDown("escape") && player.InCombat == false && !player.IsTypingChatMessage)
            {
                HotBar.SetActive(false);
                PassivesHotbar.SetActive(false);
                Inventory.SetActive(false);
                Equipment.SetActive(false);
                Crafting.SetActive(false);
                Skills.SetActive(false);
                GUIPanel.SetActive(false);
            }
            if(Inventory.activeSelf == false && Skills.activeSelf == false)
            {
                if (tooltip != null) tooltip.SetActive(false);
                HotBar.SetActive(false);
                PassivesHotbar.SetActive(false);
            }
            if (Input.GetMouseButtonDown(0) && (inventoryGUI.isBeingDragged || Skills.GetComponent<SkillsManager>().IsBeingDragged || HotBar.GetComponent<HotbarGUI>().isBeingDragged || PassivesHotbar.GetComponent<HotbarGUI>().isBeingDragged) && ((Inventory.activeSelf == true && !RectTransformUtility.RectangleContainsScreenPoint(Inventory.GetComponent<RectTransform>(), Input.mousePosition)) || Inventory.activeSelf == false) && ((Equipment.activeSelf == true && !RectTransformUtility.RectangleContainsScreenPoint(Equipment.GetComponent<RectTransform>(), Input.mousePosition)) || Equipment.activeSelf == false) && ((Crafting.activeSelf == true && !RectTransformUtility.RectangleContainsScreenPoint(Crafting.GetComponent<RectTransform>(), Input.mousePosition)) || Crafting.activeSelf == false) && ((Skills.activeSelf == true && !RectTransformUtility.RectangleContainsScreenPoint(Skills.GetComponent<RectTransform>(), Input.mousePosition)) || Skills.activeSelf == false) && ((HotBar.activeSelf == true && !RectTransformUtility.RectangleContainsScreenPoint(HotBar.GetComponent<RectTransform>(), Input.mousePosition)) || HotBar.activeSelf == false) && ((PassivesHotbar.activeSelf == true && !RectTransformUtility.RectangleContainsScreenPoint(PassivesHotbar.GetComponent<RectTransform>(), Input.mousePosition)) || PassivesHotbar.activeSelf == false))
            {
                if (inventoryGUI.isBeingDragged)
                {
                    if (inventoryGUI.DraggedItem.NotDisposable)
                    {
                        inventoryGUI.ReturnItemToInventory();
                    }
                    else
                    {
                        inventoryGUI.ItemDraggedIcon.SetActive(false);
                        DeletePrompt.SetActive(true);
                    }
                }
                else if (Skills.GetComponent<SkillsManager>().IsBeingDragged)
                {
                    Skills.GetComponent<SkillsManager>().DeleteDraggedItem();
                }
                else if (HotBar.GetComponent<HotbarGUI>().isBeingDragged)
                {
                    HotBar.GetComponent<HotbarGUI>().deleteDraggedItem();
                }
                else if (PassivesHotbar.GetComponent<HotbarGUI>().isBeingDragged)
                {
                    PassivesHotbar.GetComponent<HotbarGUI>().deleteDraggedItem();
                }
            }
        }
    }

    public void DeleteItem()
    {
        DeletePrompt.SetActive(false);
        inventoryGUI.DeleteDraggedItem();
    }

    public void DontDeleteItem()
    {
        DeletePrompt.SetActive(false);
        inventoryGUI.ItemDraggedIcon.SetActive(true);
    }

    public void RunAwayYes()
    {
        RunAwayPrompt.SetActive(false);
        player.RunAway();
    }

    public void RunAwayNo()
    {
        RunAwayPrompt.SetActive(false);
    }
}
