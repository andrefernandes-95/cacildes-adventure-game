namespace AF
{
    using UnityEngine;

    public class PlayerWeaponBuffManager : CharacterBaseWeaponBuffManager
    {
        [SerializeField] PlayerManager playerManager;

        public override CharacterBaseManager GetCharacter()
        {
            return playerManager;
        }
    }
}
