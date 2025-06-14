namespace AF.Events
{
    public static class EventMessages
    {
        public static readonly string ON_EQUIPMENT_CHANGED = "ON_EQUIPMENT_CHANGED";

        // Useful to calculate which active shield bonuses there are
        public static readonly string ON_SHIELD_EQUIPMENT_CHANGED = "ON_SHIELD_EQUIPMENT_CHANGED";

        public static readonly string ON_TWO_HANDING_CHANGED = "ON_TWO_HANDING_CHANGED";
        // Events
        public static readonly string ON_MOMENT_START = "ON_MOMENT_START";
        public static readonly string ON_MOMENT_END = "ON_MOMENT_END";
        public static readonly string ON_BOSS_BATTLE_BEGINS = "ON_BOSS_BATTLE_BEGINS";
        public static readonly string ON_BOSS_BATTLE_ENDS = "ON_BOSS_BATTLE_ENDS";

        // Quests
        public static readonly string ON_QUEST_ADDED = "ON_QUEST_ADDED";
        public static readonly string ON_QUEST_TRACKED = "ON_QUEST_TRACKED";
        public static readonly string ON_QUESTS_PROGRESS_CHANGED = "ON_QUESTS_PROGRESS_CHANGED";

        //Day / Night
        public static readonly string ON_HOUR_CHANGED = "ON_HOUR_CHANGED";

        // Companions
        public static readonly string ON_PARTY_CHANGED = "ON_PARTY_CHANGED";

        // Flags
        public static readonly string ON_FLAGS_CHANGED = "ON_FLAGS_CHANGED";

        // Combat
        public static readonly string ON_CHARACTER_KILLED = "ON_CHARACTER_KILLED";

        // Misc
        public static readonly string ON_LEAVING_BONFIRE = "ON_LEAVING_BONFIRE";

        // Game Settings
        public static readonly string ON_GRAPHICS_QUALITY_CHANGED = "ON_GRAPHICS_QUALITY_CHANGED";
        public static readonly string ON_MUSIC_VOLUME_CHANGED = "ON_MUSIC_VOLUME_CHANGED";
        public static readonly string ON_INPUT_BINDINGS_CHANGED = "ON_INPUT_BINDINGS_CHANGED";
        public static readonly string ON_CAMERA_SENSITIVITY_CHANGED = "ON_CAMERA_SENSITIVITY_CHANGED";
        public static readonly string ON_CAMERA_DISTANCE_CHANGED = "ON_CAMERA_DISTANCE_CHANGED";
        public static readonly string ON_INVERT_Y_AXIS = "ON_INVERT_Y_AXIS";

        public static readonly string ON_CAN_USE_IK_IS_TRUE = "ON_CAN_USE_IK_IS_TRUE";

        public static readonly string ON_ARENA_LOST = "ON_ARENA_LOST";

        public static readonly string ON_REPUTATION_CHANGED = "ON_REPUTATION_CHANGED";

        public static readonly string ON_BODY_TYPE_CHANGED = "ON_BODY_TYPE_CHANGED";
        public static readonly string ON_PLAYER_DODGING_FINISHED = "ON_PLAYER_DODGING_FINISHED";

        public static readonly string ON_PLAYER_HUD_VISIBILITY_CHANGED = "ON_PLAYER_HUD_VISIBILITY_CHANGED";


    }
}