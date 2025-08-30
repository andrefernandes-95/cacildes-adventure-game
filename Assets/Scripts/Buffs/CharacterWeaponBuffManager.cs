namespace AF
{
    using UnityEngine;

    public class CharacterWeaponBuffManager : CharacterBaseWeaponBuffManager
    {
        [SerializeField] CharacterManager characterManager;

        public override CharacterBaseManager GetCharacter()
        {
            return characterManager;
        }
    }
}
