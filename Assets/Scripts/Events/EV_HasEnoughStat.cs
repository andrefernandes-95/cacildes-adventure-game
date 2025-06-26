using System.Collections;
using AF.Stats;
using UnityEngine;

namespace AF
{

    public class EV_HasEnoughStat : EV_Condition
    {

        [Header("Stat")]
        public bool isVitality;
        public bool isEndurance;
        public bool isStrength;
        public bool isDexterity;
        public bool isIntelligence;

        public bool equal = false;
        public bool greaterOrEqualThan = false;
        public bool greaterThan = false;
        public bool lessThan = false;
        public bool lessOrEqualThan = false;

        public int value = 0;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        public override IEnumerator Dispatch()
        {
            bool finalValue = false;

            int currentValue = 0;
            if (isVitality)
            {
                currentValue = playerStatsBonusController.character.characterBaseStats.GetVitality();
            }
            else if (isEndurance)
            {
                currentValue = playerStatsBonusController.character.characterBaseStats.GetEndurance();
            }
            else if (isStrength)
            {
                currentValue = playerStatsBonusController.character.characterBaseStats.GetStrength();
            }
            else if (isDexterity)
            {
                currentValue = playerStatsBonusController.character.characterBaseStats.GetDexterity();
            }
            else if (isIntelligence)
            {
                currentValue = playerStatsBonusController.character.characterBaseStats.GetIntelligence();
            }

            if (equal)
            {
                finalValue = currentValue == value;
            }
            else if (greaterOrEqualThan)
            {
                finalValue = currentValue >= value;
            }
            else if (greaterThan)
            {
                finalValue = currentValue > value;
            }
            else if (lessThan)
            {
                finalValue = currentValue < value;
            }
            else if (lessOrEqualThan)
            {
                finalValue = currentValue <= value;
            }

            yield return DispatchConditionResults(finalValue);
        }
    }

}
