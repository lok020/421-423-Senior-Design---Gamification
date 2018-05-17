using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public int SlotNumber;

    private Image _itemImage;
    private PlayerController _player;
    private PlayerGUI _gui;
    private InventoryGUI _inventory;
    private Sprite _baseSprite;

	// Use this for initialization
	void Start () {
        _gui = GetComponentInParent<PlayerGUI>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _inventory = GameObject.FindGameObjectWithTag("InventoryCanvas").GetComponent<GUIManager>().inventoryGUI;
        _itemImage = GetComponentInChildren<Image>();
        _baseSprite = GetDefaultSprite();
	}
	
	// Update is called once per frame
	void Update () {
        if(_player.equippedGear[SlotNumber] != null)
        {
            //_itemImage.enabled = true;
            _itemImage.sprite = _player.equippedGear[SlotNumber].Sprite;
        }
        else
        {
            //_itemImage.enabled = false;
            _itemImage.sprite = _baseSprite;
        }
	}

    public void OnPointerEnter(PointerEventData data)
    {
        _gui.ExitItemOptions();
        if (_player.equippedGear[SlotNumber] != null)
        {
            _gui.ShowToolTip(GetComponent<RectTransform>(), _player.equippedGear[SlotNumber]);
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (_player.equippedGear[SlotNumber] != null)
        {
            _gui.ExitToolTip();
        }
    }

    public void OnPointerClick(PointerEventData data)
    {
        //Dragging item from inventory
        if (data.pointerId == -1 && _inventory.isBeingDragged && SlotNumber == (int)_inventory.DraggedItem.GearType)
        {
            _player.Equip(_inventory.DraggedItem);
            _player.Stats.ReloadEnchantments();
            _inventory.DeleteDraggedItem();
        }
        //Right click on equipped item
        if (data.pointerId == -2 && (_player.equippedGear[SlotNumber] != null)) 
        {
            _gui.ExitToolTip();
            _gui.ShowItemOptions(GetComponent<RectTransform>(), _player.equippedGear[SlotNumber]);
        }
    }

    private Sprite GetDefaultSprite()
    {
        Sprite sprite = null;
        switch(SlotNumber)
        {
            //Sword
            case 0: sprite = Resources.Load<Sprite>("Equipment_Sword"); break;
            //Shield
            case 1: sprite = Resources.Load<Sprite>("Equipment_Shield"); break;
            //Helmet
            case 2: sprite = Resources.Load<Sprite>("Equipment_Helmet"); break;
            //Chest
            case 3: sprite = Resources.Load<Sprite>("Equipment_Platebody"); break;
            //Gloves
            case 4: sprite = Resources.Load<Sprite>("Equipment_Gauntlets"); break;
            //Pants
            case 5: sprite = Resources.Load<Sprite>("Equipment_Platelegs"); break;
            //Boots
            case 6: sprite = Resources.Load<Sprite>("Equipment_Boots"); break;
            //Necklace
            case 7: sprite = Resources.Load<Sprite>("Equipment_Necklace"); break;
            //Left Ring
            case 8: sprite = Resources.Load<Sprite>("Equipment_LeftRing"); break;
            //Right Ring
            case 9: sprite = Resources.Load<Sprite>("Equipment_RightRing"); break;
        }
        return sprite;
    }
}
