using System.Collections;
using System.Collections.Generic;
using AF.Health;
using UnityEngine;

namespace AF
{
    public abstract class CharacterBaseBuffManager : MonoBehaviour
    {
        public List<BuffAttribute> currentBuffs = new();

        [Header("Buffs")]
        public Dictionary<BuffAttribute, int> physicalAttackModifiers = new();

        public abstract CharacterBaseManager GetCharacter();

        public void StartBuffAttribute(BuffAttribute buffAttribute)
        {
            StartCoroutine(HandleBuffAttribute(buffAttribute));
        }

        IEnumerator HandleBuffAttribute(BuffAttribute buffAttribute)
        {
            if (currentBuffs.Contains(buffAttribute))
            {
                yield break;
            }

            currentBuffs.Add(buffAttribute);

            CharacterStatusEffectUI buffAttributeBar = Instantiate(
                GetCharacter().characterHUD.characterStatusEffectUIPrefab,
                GetCharacter().characterHUD.hudContainerToSpawnObjects);

            float currentDuration = buffAttribute.durationInSeconds;
            int maxDuration = (int)currentDuration;

            buffAttribute.OnAppliedStart(GetCharacter());

            while (currentDuration > 0)
            {
                currentDuration -= Time.deltaTime;

                buffAttributeBar.UpdateUI(
                    buffAttribute.name.GetLocalizedString(),
                    buffAttribute.name.GetLocalizedString(),
                    buffAttribute.icon,
                    buffAttribute.barColor,
                    (int)currentDuration,
                    maxDuration,
                    true
                );

                buffAttribute.OnAppliedUpdate(GetCharacter());

                yield return null;
            }

            buffAttribute.OnAppliedEnd(GetCharacter());
            Destroy(buffAttributeBar.gameObject);
            currentBuffs.Remove(buffAttribute);
        }

        public Damage EnhanceAttackDamage(Damage baseDamage)
        {
            baseDamage.physical += GetTotalPhysicalAttack();

            return baseDamage;
        }

        public int GetTotalPhysicalAttack()
        {
            int total = 0;
            foreach (var mod in physicalAttackModifiers.Values)
            {
                total += mod;
            }
            return total;
        }
    }
}
