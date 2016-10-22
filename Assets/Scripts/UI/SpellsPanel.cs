using System;
using System.Collections.Generic;
using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SpellsPanel : Singleton<SpellsPanel>
    {
        [Serializable]
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
            if (spell == null)
            {
                Debug.LogWarning("Trying to register a null spell in SpellPanel", this);
                return;
            }

            if (SpellUIPrefab != null)
            {
                var ui = GameObject.Instantiate(SpellUIPrefab, transform) as GameObject;
                var info = ui.GetComponent<SpellShotInfo>();
                var slot = new SpellSlot()
                {
                    CooldownText = info.CoolDownText,
                    CooldownImage = info.CoolDownImage,
                    Icon = info.Image,
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
            SpellSlot slot;
            if (_slots.TryGetValue(spell, out slot))
            {
                slot = _slots[spell];
                UpdateCooldownText(slot.CooldownText, cooldown);
                UpdateCooldownImage(slot.CooldownImage, cooldown, slot.Spell.CoolDown);
            }
            else
            {
                Debug.LogWarning("Trying to update cooldown of not registred spell");
            }
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
}
