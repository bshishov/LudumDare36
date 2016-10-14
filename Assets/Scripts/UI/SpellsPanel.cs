using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpellsPanel : Singleton<SpellsPanel>
{
    class SpellSlot
    {
        public Text CooldownText;
        public Image CooldownImage;
        public Image Icon;
        public SpellData Spell;
    }

    private readonly Dictionary<SpellData, SpellSlot> _slots = new Dictionary<SpellData, SpellSlot>();

    public GameObject SpellUIPrefab;

    public void RegisterSpell(SpellData spell)
    {
        if (SpellUIPrefab != null)
        {
            var ui = GameObject.Instantiate(SpellUIPrefab, transform) as GameObject;
            var slot = new SpellSlot()
            {
                CooldownText = ui.transform.FindChild("CooldownText").GetComponent<Text>(),
                CooldownImage = ui.transform.FindChild("CooldownImage").GetComponent<Image>(),
                Icon = ui.transform.FindChild("Image").GetComponent<Image>(),
                Spell = spell,
            };

            slot.Icon.sprite = spell.Icon;
            _slots.Add(spell, slot);
        }
        else
        {
            Debug.LogWarning("Trying to instantiate spell UI prefab but it's not set");
        }
    }

    public void UpdateCooldown(SpellData spell, float cooldown)
    {
        var slot = _slots[spell];
        UpdateCooldownText(slot.CooldownText, cooldown);
        UpdateCooldownImage(slot.CooldownImage, cooldown, slot.Spell.CoolDown);
    }

    private void UpdateCooldownText(Text textComponent, float cooldown)
    {
        if (!textComponent.enabled && cooldown > 0f)
        {
            textComponent.enabled = true;
        }


        if (textComponent.enabled)
        {
            textComponent.text = string.Format("{0:F1}", cooldown);

            if(cooldown <= 0.1f)
                textComponent.enabled = false;
        }
    }

    private void UpdateCooldownImage(Image imgComponent, float cooldown, float total)
    {
        if (!imgComponent.enabled && cooldown > 0f)
        {
            imgComponent.enabled = true;
        }

        if (imgComponent.enabled)
        {
            imgComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cooldown / total * 60f);

            if (cooldown <= 0.1f)
                imgComponent.enabled = false;
        }
    }
}
