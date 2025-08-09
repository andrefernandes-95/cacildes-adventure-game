namespace AF
{
    using UnityEngine;

    public class CharacterBuffManager : CharacterBaseBuffManager
    {
        [SerializeField] CharacterManager characterManager;

        public override CharacterBaseManager GetCharacter()
        {
            return characterManager;
        }
    }
}
