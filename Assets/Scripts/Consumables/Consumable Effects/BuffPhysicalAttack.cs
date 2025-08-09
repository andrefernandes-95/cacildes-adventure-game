using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [CreateAssetMenu(menuName = "Consumable Effect / Buff Physical Attack")]
    public class BuffPhysicalAttack : BuffAttribute
    {
        [Header("Options")]
        [SerializeField] int physicalAttackBonusPoints = 30;

        public override void OnAppliedStart(CharacterBaseManager characterBaseManager)
        {
            if (!characterBaseManager.characterBaseBuffManager.physicalAttackModifiers.ContainsKey(this))
            {
                characterBaseManager.characterBaseBuffManager.physicalAttackModifiers[this] = physicalAttackBonusPoints;
            }
        }

        public override void OnAppliedUpdate(CharacterBaseManager characterBaseManager)
        {

        }

        public override void OnAppliedEnd(CharacterBaseManager characterBaseManager)
        {
            if (characterBaseManager.characterBaseBuffManager.physicalAttackModifiers.ContainsKey(this))
            {
                characterBaseManager.characterBaseBuffManager.physicalAttackModifiers.Remove(this);
            }
        }
    }
}
