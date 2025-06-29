namespace AF
{
    public static class StatsUtils
    {
        public static string GetVitalityDescription()
        {
            if (!Utils.IsPortuguese())
            {
                return "Governs maximum health and posture.";
            }

            return "Governa os pontos de vida máximos e a tua postura.";
        }

        public static string GetEnduranceDescription()
        {
            if (!Utils.IsPortuguese())
            {
                return "Governs maximum stamina, physical defense, and poise.";
            }

            return "Governa a stamina máxima, a defesa física e o teu equilíbrio.";
        }

        public static string GetStrengthDescription()
        {
            if (!Utils.IsPortuguese())
            {
                return "Governs physical attack power and maximum equip load. Heavy weapons deal more damage.";
            }

            return "Governa o poder de ataque físico e a carga máxima de equipamentos. Armas pesadas causam mais dano.";
        }

        public static string GetDexterityDescription()
        {
            if (!Utils.IsPortuguese())
            {
                return "Governs physical attack power. Bows and other light weapons deal more damage.";
            }

            return "Governa o poder de ataque físico. Arcos e outras armas leves causam mais dano.";
        }

        public static string GetIntelligenceDescription()
        {
            if (!Utils.IsPortuguese())
            {
                return "Governs maximum mana and elemental attack power. Spells deal more damage.";
            }

            return "Governa a mana máxima e o poder dos ataques elementais. Feitiços causam mais dano.";
        }
    }
}
