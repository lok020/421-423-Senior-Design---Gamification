using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[Serializable]
public class CombatUI: MonoBehaviour {

    public GameObject cooldownBar, target;
    public Image greenbar, bluebar;
    public Text hpText, cooldownText, nameText;

    private float _coolClimb = 10f;
    private float _coolCap = -1f;
    private int _healthFade;
    private PlayerStats _stats = null;
	
	// Update is called once per frame
	void Update () {
        if (_stats == null) return;

        //Increase or decrease health bar
        if (_stats.Health < _healthFade)
        {
            _healthFade -= 1;
        }
        else if(_stats.Health > _healthFade)
        {
            _healthFade += 1;
        }
        greenbar.rectTransform.sizeDelta = new Vector2(100 * _healthFade / _stats.BaseHealth, 12);
        hpText.text = (_stats.Health > 0 ? _stats.Health : 0) + " / " + _stats.BaseHealth;

        if (_stats.Cooldown <= 0)
        {
            cooldownText.text = "Ready";
            _coolCap = -1f;
        }
        else
        {
            if (_coolCap < 0)
            {
                _coolCap = _stats.Cooldown;
                _coolClimb = 0;
            }
            cooldownText.text = string.Format("{0:F1}", _stats.Cooldown);
        }

        if (_stats.ResetCooldown)
        {
            _coolClimb = 0;
            _stats.Cooldown = 0;
            _stats.ResetCooldown = false;
        }

        if (_coolClimb < _coolCap)
        {
            _coolClimb += Time.deltaTime;
            bluebar.rectTransform.sizeDelta = new Vector2(100 * _coolClimb / _coolCap, 12);
        }
    }

    public void Set(PlayerStats stats, bool showCooldownBar, string name)
    {
        _stats = stats;
        nameText.text = name;
        _healthFade = stats.BaseHealth;
        transform.position = stats.transform.localPosition - new Vector3(0, 3 * stats.transform.localScale.y / 5, 0);
        target.SetActive(false);
        cooldownBar.SetActive(showCooldownBar);
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }

    public IEnumerator ShowTarget(float time)
    {
        target.SetActive(true);
        yield return new WaitForSeconds(time);
        target.SetActive(false);
    }
}
