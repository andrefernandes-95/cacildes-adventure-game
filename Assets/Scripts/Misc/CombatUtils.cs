namespace AF
{
    public static class CombatUtils
    {
        static readonly string hashLightAttack1 = "Light Attack 1";
        static readonly string hashLightAttack2 = "Light Attack 2";
        static readonly string hashLightAttack3 = "Light Attack 3";
        static readonly string hashLightAttack4 = "Light Attack 4";
        static readonly string hashLeftLightAttack1 = "Left Light Attack 1";
        static readonly string hashLeftLightAttack2 = "Left Light Attack 2";
        static readonly string hashPowerStanceAttack1 = "Power Stance Attack 1";
        static readonly string hashPowerStanceAttack2 = "Power Stance Attack 2";
        static readonly string hashHeavyAttack1 = "Heavy Attack 1";
        static readonly string hashHeavyAttack2 = "Heavy Attack 2";
        static readonly string hashHeavyPowerStanceAttack1 = "Heavy Power Stance Attack 1";
        static readonly string hashHeavyPowerStanceAttack2 = "Heavy Power Stance Attack 2";

        public static string GetLightAttackAnimationName(int lightAttackComboIndex, bool isAttackingWithLeftHand, bool canPowerStance)
        {
            string hashAttack = "";

            if (lightAttackComboIndex == 0)
            {
                if (isAttackingWithLeftHand)
                {
                    hashAttack = canPowerStance ? hashPowerStanceAttack1 : hashLeftLightAttack1;
                }
                else
                {
                    hashAttack = hashLightAttack1;
                }
            }
            else if (lightAttackComboIndex == 1)
            {
                if (isAttackingWithLeftHand)
                {
                    hashAttack = canPowerStance ? hashPowerStanceAttack2 : hashLeftLightAttack2;
                }
                else
                {
                    hashAttack = hashLightAttack2;
                }
            }
            else if (lightAttackComboIndex == 2)
            {
                if (isAttackingWithLeftHand)
                {
                    hashAttack = canPowerStance ? hashPowerStanceAttack1 : hashLeftLightAttack1;
                }
                else
                {
                    hashAttack = hashLightAttack3;
                }
            }
            else if (lightAttackComboIndex == 3)
            {
                if (isAttackingWithLeftHand)
                {
                    hashAttack = canPowerStance ? hashPowerStanceAttack2 : hashLeftLightAttack2;
                }
                else
                {
                    hashAttack = hashLightAttack4;
                }
            }

            return hashAttack;
        }

        public static string GetHeavyAttackAnimationName(int heavyAttackComboIndex, bool canPowerStance)
        {
            string hashAttack = "";

            if (heavyAttackComboIndex == 0)
            {
                hashAttack = canPowerStance ? hashHeavyPowerStanceAttack1 : hashHeavyAttack1;
            }
            else if (heavyAttackComboIndex == 1)
            {
                hashAttack = canPowerStance ? hashHeavyPowerStanceAttack2 : hashHeavyAttack2;
            }

            return hashAttack;
        }


    }
}