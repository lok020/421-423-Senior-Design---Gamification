using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class SkillsManager : MonoBehaviour {
    
    public bool IsBeingDragged = false;
    public Text SkillPointsAmount;

    private PlayerStats _stats;
    private SkillGUI _activePanel;
    private Dictionary<string, SkillGUI> _panels = new Dictionary<string, SkillGUI>();
    private bool _initialLoad = false;

    // Use this for initialization
    void Start () {
        _stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        //Get panels
        foreach(SkillGUI s in GetComponentsInChildren<SkillGUI>())
        {
            _panels.Add(s.transform.parent.name, s);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(_initialLoad == false)
        {
            //Load "Fighter" to start
            _initialLoad = true;
            TabClicked("Fighter");
        }
        if (_stats.SkillPoints > 0)
        {
            SkillPointsAmount.gameObject.SetActive(true);
            SkillPointsAmount.text = String.Format("\t{0,-12} {1,2:0.##}", "Skill Points", _stats.SkillPoints);
        }
        else
        {
            SkillPointsAmount.gameObject.SetActive(false);
        }
	}

    public void DeleteDraggedItem()
    {
        IsBeingDragged = false;
        _activePanel.GetComponent<SkillGUI>().deleteDraggedItem();
    }

    public GameObject getDraggedItem()
    {
        return _activePanel.GetComponent<SkillGUI>().DraggedItem;
    }

    public void TabClicked(string groupName)
    {
        _activePanel = _panels[groupName];
        _activePanel.UpdateSlots();
    }
}
