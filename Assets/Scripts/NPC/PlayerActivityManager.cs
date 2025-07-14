namespace AF
{
    using UnityEngine;

    public class PlayerActivityManager : CharacterBaseActivityManager
    {
        [SerializeField] PlayerManager playerManager;

        public override CharacterBaseManager GetCharacter()
        {
            return playerManager;
        }
    }
}
