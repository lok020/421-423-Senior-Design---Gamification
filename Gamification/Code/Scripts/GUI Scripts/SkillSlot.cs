using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public Image skillImage;
    public PlayerStats stats;
    public bool isDraggable;
    public bool isUnlocked;

    private GameObject _skill;
    private SkillGUI _gui = null;
    private int _slotNumber;

	// Use this for initialization
	void Start () {
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
	}

    public void Reload(SkillGUI skillGui, int slotNumber, GameObject skill, bool unlocked)
    {
        _gui = skillGui;
        _slotNumber = slotNumber;
        _skill = skill;
        skillImage = GetComponent<Image>();
        skillImage.sprite = _skill.GetComponent<AttackDetailsInstance>().Icon;
        isDraggable = false;
        isUnlocked = unlocked;
        if(!unlocked)
        {
            skillImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        else
        {
            skillImage.color = new Color(1f, 1f, 1f, 1f);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_gui == null) return;
        if (isUnlocked)
        {
            GetComponent<Button>().enabled = false;
            isDraggable = true;
        }
        else
        {
            GetComponent<Button>().enabled = true;
            isDraggable = false;
        }
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_gui == null) return;
        if(!_gui.isBeingDragged)
        {
            _gui.ShowToolTip(GetComponent<RectTransform>().localPosition, _skill);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_gui == null) return;
        _gui.exitToolTip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_gui == null) return;
        if (isDraggable && !_gui.isBeingDragged && isUnlocked)
        {
            _gui.showDraggedItem(_skill, _slotNumber);
        }
    }

    public void UnlockSkill()
    {
        if (_gui == null) return;
        if (_gui.unlockPrompt.activeSelf == false && stats.SkillPoints > 0) //and have skill points ready
        {
            _gui.unlockindex = _slotNumber;
            _gui.unlockPrompt.SetActive(true);
            _gui.unlockPrompt.GetComponent<RectTransform>().SetAsLastSibling();
        }
    }
}
