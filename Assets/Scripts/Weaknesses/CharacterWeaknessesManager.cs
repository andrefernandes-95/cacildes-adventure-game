namespace AF
{
    using UnityEngine;

    public class CharacterWeaknessesManager : CharacterBaseWeaknessesManager
    {
        [SerializeField] CharacterManager characterManager;

        public override CharacterBaseManager GetCharacter()
        {
            return characterManager;
        }
    }
}
