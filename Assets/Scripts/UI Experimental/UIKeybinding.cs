namespace AF.UIExperimental
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIKeybinding : MonoBehaviour
    {
        [SerializeField] StarterAssetsInputs starterAssetsInputs;

        [SerializeField] Image image;

        [Header("Keys")]
        [SerializeField] Sprite keyboard;
        [SerializeField] Sprite ps4;
        [SerializeField] Sprite xbox;

        private void OnEnable()
        {
            if (starterAssetsInputs.IsPS4Controller())
            {
                image.sprite = ps4;
            }
            else if (starterAssetsInputs.IsXboxController())
            {
                image.sprite = xbox;
            }
            else if (starterAssetsInputs.IsKeyboardMouse())
            {
                image.sprite = keyboard;
            }
        }

    }
}
