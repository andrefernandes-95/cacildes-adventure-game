namespace AF
{
    using UnityEngine;

    public class PlayerWeaknessesManager : CharacterBaseWeaknessesManager
    {
        [SerializeField] PlayerManager playerManager;

        public override CharacterBaseManager GetCharacter()
        {
            return playerManager;
        }
    }
}
