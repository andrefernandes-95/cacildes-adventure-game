namespace AF
{
    using UnityEngine;

    public class PlayerBuffManager : CharacterBaseBuffManager
    {
        [SerializeField] PlayerManager playerManager;

        public override CharacterBaseManager GetCharacter()
        {
            return playerManager;
        }
    }
}
