using UnityEngine;
using System.Collections.Generic;

public class BuffIcon : MonoBehaviour {
    public List<Sprite> Images;

    private bool _isPositioned;
    private SpriteRenderer[] _visibleIcons = new SpriteRenderer[4];

	// Use this for initialization
	void Start () {
        foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            _visibleIcons[(int.Parse(sr.name.Substring(4))) - 1] = sr;
        }
        _isPositioned = false;
	}

    public void Set(PlayerStats stats)
    {
        if (stats == null) return;

        if (!_isPositioned)
        {
            transform.position = stats.transform.position - new Vector3(1.2f, -0.8f, 0);
            _isPositioned = true;
        }

        int currentIndex = 0;

        foreach (AttackBuff type in stats.ActiveBuffs.Keys)
        {
            if (stats.ActiveBuffs[type] == 0) continue;
            if (currentIndex >= 4) break;

            _visibleIcons[currentIndex].gameObject.SetActive(true);
            _visibleIcons[currentIndex].sprite = GetSprite(type);
            currentIndex++;
        }

        foreach (AttackEffect effect in stats.ActiveEffects.Keys)
        {
            if (stats.ActiveEffects[effect] == 0) continue;
            if (currentIndex >= 4) break;

            _visibleIcons[currentIndex].gameObject.SetActive(true);
            _visibleIcons[currentIndex].sprite = GetSprite(effect);
            currentIndex++;
        }
        for (int i = _visibleIcons.Length - 1; i >= currentIndex; i--)
        {
            _visibleIcons[i].gameObject.SetActive(false);
        }
    }

    private Sprite GetSprite(AttackBuff buff)
    {
        switch(buff)
        {
            case AttackBuff.PHYSICAL_ATTACK: return Images[0];
            case AttackBuff.PHYSICAL_DEFENSE: return Images[1];
            case AttackBuff.MAGICAL_ATTACK: return Images[2];
            case AttackBuff.MAGICAL_DEFENSE: return Images[3];
            case AttackBuff.SPEED: return Images[4];
            case AttackBuff.DAMAGE_REDUCTION: return Images[5];
        }
        return null;
    }

    private Sprite GetSprite(AttackEffect effect)
    {
        switch (effect)
        {
            case AttackEffect.PARALYZE: return Images[6];
            case AttackEffect.STUN: return Images[7];
            case AttackEffect.POISON: return Images[8];
            case AttackEffect.TAUNT: return Images[9];
        }
        return null;
    }
}
