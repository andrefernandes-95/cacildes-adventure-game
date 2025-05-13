using AF.UI;
using UnityEngine.UIElements;

namespace AF
{
    public class ViewSettingsMenu : ViewMenu
    {
        ViewComponent_GameSettings viewComponent_GameSettings => GetComponent<ViewComponent_GameSettings>();

        protected override void OnEnable()
        {
            base.OnEnable();

            root.Q<Label>("CurrentNewGameCounter").text = " " + gameSession.currentGameIteration;

            viewComponent_GameSettings.SetupRefs();
        }

    }
}
