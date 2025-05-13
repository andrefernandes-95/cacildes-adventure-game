using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{
    [RequireComponent(typeof(UIGameControls))]
    [RequireComponent(typeof(UIDocument))]
    public class UISettings_Controls : MonoBehaviour
    {
        UIDocument uIDocument => GetComponent<UIDocument>();
        VisualElement root => uIDocument.rootVisualElement;

        [Header("Key Rebinding")]
        VisualElement pressAnyKeyModal;
        public StarterAssetsInputs inputs;

        public Soundbank soundbank;

        UIGameControls gameControls => GetComponent<UIGameControls>();

        [Header("Databases")]
        public GameSettings gameSettings;

        void OnEnable()
        {
            SetupUI();

            pressAnyKeyModal = root.Q<VisualElement>("PressAnyKeyModal");
            HideRebindModal();
        }

        void SetupUI()
        {
            Slider cameraSensitivity = root.Q<Slider>("CameraSensitivity");

            cameraSensitivity.lowValue = gameSettings.minimumMouseSensitivity;
            cameraSensitivity.highValue = gameSettings.maximumMouseSensitivity;
            cameraSensitivity.value = gameSettings.GetCameraSensitivity();
            cameraSensitivity.label =
                (Utils.IsPortuguese() ? "Sensibilidade da Câmara: " : "Camera Sensitivity: ") + gameSettings.GetCameraSensitivity();

            cameraSensitivity.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetCameraSensitivity(ev.newValue);
                cameraSensitivity.label =
                    (Utils.IsPortuguese() ? "Sensibilidade da Câmara: " : "Camera Sensitivity: ") + gameSettings.GetCameraSensitivity();
            });


            AssignRebindListeners();
        }

        void HideRebindModal()
        {
            pressAnyKeyModal.style.display = DisplayStyle.None;
        }

        void AssignRebindListeners()
        {
            UIUtils.SetupButton(root.Q<VisualElement>("Sprint").Q<Button>("Rebind"), () =>
            {
                string label = Utils.IsPortuguese() ? "Redifinir Ação de Sprintar" : "Rebinding Sprint Action";

                StartCoroutine(SelectKeyBinding("Sprint", label, (bindingPayload) =>
                {
                    gameSettings.SetSprintOverrideBindingPayload(bindingPayload);
                }, () =>
                {
                    gameControls.UpdateInputsHUD();
                }));
            }, soundbank);

            UIUtils.SetupButton(root.Q<VisualElement>("Jump").Q<Button>("Rebind"), () =>
            {
                string label = Utils.IsPortuguese() ? "Redifinir Ação de Saltar" : "Rebinding Jump Action";

                StartCoroutine(SelectKeyBinding("Jump", label, (bindingPayload) =>
                {
                    gameSettings.SetJumpOverrideBindingPayload(bindingPayload);
                }, () =>
                {
                    gameControls.UpdateInputsHUD();
                }));
            }, soundbank);


            UIUtils.SetupButton(root.Q<VisualElement>("Dodge").Q<Button>("Rebind"), () =>
            {
                string label = Utils.IsPortuguese() ? "Redifinir Ação de Esquivar" : "Rebinding Dodge Action";

                StartCoroutine(SelectKeyBinding("Dodge", label, (bindingPayload) =>
                {
                    gameSettings.SetDodgeOverrideBindingPayload(bindingPayload);
                }, () =>
                {
                    gameControls.UpdateInputsHUD();
                }));
            }, soundbank);


            UIUtils.SetupButton(root.Q<VisualElement>("ToggleHands").Q<Button>("Rebind"), () =>
            {
                string label = Utils.IsPortuguese() ? "Redifinir Alternar Pega da Arma" : "Rebinding Toggle Weapon Grip";

                StartCoroutine(SelectKeyBinding("ToggleHands", label, (bindingPayload) =>
                {
                    gameSettings.SetTwoHandModeOverrideBindingPayload(bindingPayload);
                }, () =>
                {
                    gameControls.UpdateInputsHUD();
                }));
            }, soundbank);

            UIUtils.SetupButton(root.Q<VisualElement>("HeavyAttack").Q<Button>("Rebind"), () =>
            {
                string label = Utils.IsPortuguese() ? "Redifinir Ataque Pesado" : "Rebinding Heavy Attack";
                StartCoroutine(SelectKeyBinding("HeavyAttack", label, (bindingPayload) =>
                {
                    gameSettings.SetTwoHandModeOverrideBindingPayload(bindingPayload);
                }, () =>
                {
                    gameControls.UpdateInputsHUD();
                }));
            }, soundbank);
        }

        IEnumerator SelectKeyBinding(string actionName, string label, UnityAction<string> onRebindSuccessPayload, UnityAction onFinish)
        {
            pressAnyKeyModal.Q<Label>("Title").text = label;
            pressAnyKeyModal.Q<Label>("CurrentKeyLabel").text = (Utils.IsPortuguese() ? "Tecla Atual: " : "Current Key: ") + inputs.GetCurrentKeyBindingForAction(actionName);

            pressAnyKeyModal.style.display = DisplayStyle.Flex;

            yield return inputs.Rebind(actionName, (action) =>
            {
                onRebindSuccessPayload.Invoke(action);
            });

            pressAnyKeyModal.style.display = DisplayStyle.None;
            onFinish?.Invoke();
        }
    }
}
