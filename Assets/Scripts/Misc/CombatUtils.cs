namespace AF
{
    public static class CombatUtils
    {
        static readonly string hashLightAttack1 = "Weapon Light Attack 1";
        static readonly string hashLightAttack2 = "Weapon Light Attack 2";
        static readonly string hashLightAttack3 = "Weapon Light Attack 3";
        static readonly string hashLightAttack4 = "Weapon Light Attack 4";
        static readonly string hashLeftLightAttack1 = "Weapon Left Light Attack 1";
        static readonly string hashLeftLightAttack2 = "Weapon Left Light Attack 2";
        static readonly string hashPowerStanceAttack1 = "Weapon Power Stance Attack 1";
        static readonly string hashPowerStanceAttack2 = "Weapon Power Stance Attack 2";
        static readonly string hashHeavyAttack1 = "Weapon Heavy Attack 1";
        static readonly string hashHeavyAttack2 = "Weapon Heavy Attack 2";
        static readonly string hashHeavyPowerStanceAttack1 = "Weapon Heavy Power Stance Attack 1";
        static readonly string hashHeavyPowerStanceAttack2 = "Weapon Heavy Power Stance Attack 2";

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