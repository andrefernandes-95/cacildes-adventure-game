using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;
using System.Collections;
using AF.UI.EquipmentMenu;
using AF.Events;
using TigerForge;

namespace AF
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public UnityEvent onMoveInput;

		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool toggleWalk;

		public bool consumeFavoriteItem;
		public UnityEvent onConsumeFavoriteItem;

		public UnityEvent onDodgeInput;
		public UnityEvent onLightAttackInput;

		public bool block;
		public UnityEvent onBlock_Start;
		public UnityEvent onBlock_End;

		public UnityEvent onLockOnInput;

		public UnityEvent onHeavyAttackInput;

		public bool interact;
		public UnityEvent onInteract;
		public UnityEvent onSwitchSpellInput;
		public UnityEvent onSwitchWeaponInput;

		public UnityEvent onSwitchShieldInput;

		public UnityEvent onSwitchConsumableInput;
		public UnityEvent onToggleTwoHandsInput;

		[Header("UI")]
		public UnityEvent onMenuEvent;
		public UnityEvent onCustomizeCharacter;
		[SerializeField] MenuManager menuManager;
		public UnityEvent onMainMenuUnequipSlot;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorInputForLook = true;

		[Header("Main Menu")]
		public UnityEvent onNextMenu;
		public UnityEvent onPreviousMenu;

		[Header("Day Night")]
		public UnityEvent onAdvanceOneHour;
		public UnityEvent onGoBackOneHour;

		// Abilities
		[HideInInspector] public UnityEvent onUseAbility;

		[Header("System")]
		public GameSettings gameSettings;

		Vector2 scaleVector = new(1, 1);

		[Header("Rebindings")]
		public PlayerInput playerInput;

		[Header("Components")]
		public EquipmentSlots equipmentSlots;

		[SerializeField] InputActionAsset inputActions;


		private void Awake()
		{
			EventManager.StartListening(EventMessages.ON_CAMERA_SENSITIVITY_CHANGED, UpdateScaleVector);
			EventManager.StartListening(EventMessages.ON_INVERT_Y_AXIS, UpdateScaleVector);
			EventManager.StartListening(EventMessages.ON_INPUT_BINDINGS_CHANGED, Refresh);

			UpdateScaleVector();
		}

		void Refresh()
		{
			gameObject.SetActive(false);
			gameObject.SetActive(true);
		}

		void OnEnable()
		{
			var actionMap = inputActions.FindActionMap("Player", true);

			// Apply Custom Bindings
			HandleCustomBindings(actionMap);

			var sprintAction = actionMap.FindAction("Sprint", true);
			sprintAction.performed += OnSprintPerformed;
			sprintAction.canceled += OnSprintCanceled;
			sprintAction.Enable();

			var dodgeAction = actionMap.FindAction("Dodge", true);
			dodgeAction.performed += OnDodgePerformed;
			dodgeAction.Enable();
		}

		void OnDisable()
		{
			var actionMap = inputActions.FindActionMap("Player", true);

			var sprintAction = actionMap.FindAction("Sprint", true);
			sprintAction.performed -= OnSprintPerformed;
			sprintAction.canceled -= OnSprintCanceled;
			sprintAction.Disable();

			var dodgeAction = actionMap.FindAction("Dodge", true);
			dodgeAction.performed -= OnDodgePerformed;
			dodgeAction.Disable();
		}

		void OnSprintPerformed(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				sprint = true;
			}
		}

		void OnSprintCanceled(InputAction.CallbackContext context)
		{
			if (context.canceled)
			{
				sprint = false;
			}
		}

		void OnDodgePerformed(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				onDodgeInput?.Invoke();
			}
		}

		private void UpdateScaleVector()
		{
			Vector2 newScale = new(gameSettings.GetCameraSensitivity(), gameSettings.GetCameraSensitivity());

			if (gameSettings.GetInvertYAxis())
			{
				newScale.y *= -1;
			}

			scaleVector = newScale;
		}

		public void OnMove(InputValue value)
		{
			move = value.Get<Vector2>();

			onMoveInput?.Invoke();
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				look = value.Get<Vector2>();
			}

			look.Scale(scaleVector);
		}

		public void OnJump(InputValue value)
		{
			jump = value.isPressed;
		}

		public void OnSprint(InputValue value)
		{
		}

		public void OnToggleWalk(InputValue value)
		{
			toggleWalk = !toggleWalk;

		}

		public void OnMenu(InputValue value)
		{
			if (value.isPressed)
			{
				onMenuEvent?.Invoke();
			}
		}

		public void OnCustomizeCharacter(InputValue value)
		{
			if (value.isPressed)
			{
				onCustomizeCharacter?.Invoke();
			}
		}

		public void OnTab(InputValue value)
		{
			onToggleTwoHandsInput?.Invoke();
		}

		public void OnLightAttack(InputValue value)
		{
			onLightAttackInput?.Invoke();
		}

		public void OnBlock(InputValue value)
		{
			bool previousState = block;
			block = value.isPressed;

			if (previousState != block)
			{
				if (block)
				{
					onBlock_Start?.Invoke();
				}
				else
				{
					onBlock_End?.Invoke();
				}
			}

		}

		public void OnLockOn(InputValue value)
		{
			onLockOnInput?.Invoke();

		}

		public void OnHeavyAttack(InputValue value)
		{
			onHeavyAttackInput?.Invoke();
		}

		public void OnInteract(InputValue value)
		{
			interact = value.isPressed;

			if (value.isPressed)
			{
				onInteract?.Invoke();
			}
		}

		public void OnSwitchSpell(InputValue value)
		{
			if (value.isPressed)
			{
				onSwitchSpellInput?.Invoke();
			}
		}

		public void OnSwitchConsumable(InputValue value)
		{
			if (value.isPressed)
			{
				onSwitchConsumableInput?.Invoke();
			}
		}

		public void OnSwitchWeapon(InputValue value)
		{
			if (value.isPressed)
			{
				onSwitchWeaponInput?.Invoke();
			}
		}

		public void OnSwitchShield(InputValue value)
		{
			if (value.isPressed)
			{
				onSwitchShieldInput?.Invoke();
			}
		}

		public void OnConsumeFavoriteItem(InputValue value)
		{
			consumeFavoriteItem = value.isPressed;

			onConsumeFavoriteItem?.Invoke();
		}

		public void OnQuickSave(InputValue value)
		{
		}
		public void OnQuickLoad(InputValue value)
		{
		}

		public void OnNextMenu(InputValue value)
		{
			onNextMenu?.Invoke();
		}
		public void OnPreviousMenu(InputValue value)
		{
			onPreviousMenu?.Invoke();
		}

		public void OnAdvanceOneHour()
		{
			onAdvanceOneHour?.Invoke();
		}
		public void OnGoBackOneHour()
		{
			onGoBackOneHour?.Invoke();
		}

		public void OnZoomIn(InputValue inputValue)
		{
			if (menuManager.isMenuOpen)
			{
				return;
			}

			gameSettings.IncreaseCameraDistance(inputValue.Get<float>());
		}

		public void OnZoomOut(InputValue inputValue)
		{
			if (menuManager.isMenuOpen)
			{
				return;
			}

			gameSettings.DecreaseCameraDistance(inputValue.Get<float>());
		}

		public RebindingOperation ChangeInput(InputAction inputAction)
		{
			return inputAction
				.PerformInteractiveRebinding()
				.WithControlsExcluding("<Mouse>/leftButton")
				.WithControlsExcluding("<Mouse>/rightButton")
				.WithControlsExcluding("<Keyboard>/f5")
				.WithControlsExcluding("<Keyboard>/f9")
				.WithControlsExcluding("<Keyboard>/escape")
				.WithCancelingThrough("<Keyboard>/escape")
				.Start();
		}

		public IEnumerator Rebind(string actionName, UnityAction<string> onRebindSuccessfull)
		{
			InputAction inputAction = playerInput
				.actions
				.FindAction(actionName);

			if (inputAction == null)
			{
				yield break;
			}

			inputAction.Disable();

			RebindingOperation rebindingOperation = ChangeInput(inputAction);

			yield return new WaitUntil(() =>
			{
				return rebindingOperation.completed;
			});

			onRebindSuccessfull.Invoke(rebindingOperation.action.bindings[0].overridePath);

			rebindingOperation.Dispose();
			inputAction.Enable();
		}

		public string GetCurrentKeyBindingForAction(string actionName)
		{
			InputAction inputAction = playerInput
				.actions
				.FindAction(actionName);

			if (inputAction == null)
			{
				return "";
			}

			return inputAction.GetBindingDisplayString();
		}

		public void RestoreDefaultKeyBindings()
		{
			playerInput.actions.RemoveAllBindingOverrides();
		}

		public void ApplyBindingOverride(string actionName, string overridePayload)
		{
			try
			{
				playerInput.actions[actionName].ApplyBindingOverride(overridePayload);
			}
			catch (System.Exception e)
			{
				Debug.LogError($"Failed to apply override for {actionName}: {e.Message}");
				return;
			}

			Debug.Log($"Applied override for {actionName}: {overridePayload}");
		}

		public void OnMainMenuUnequipSlot()
		{
			if (equipmentSlots.isActiveAndEnabled)
			{
				equipmentSlots.OnUnequip();
			}

			onMainMenuUnequipSlot?.Invoke();
		}

		public void OnUseAbility(InputValue inputValue)
		{
			if (inputValue.isPressed)
			{
				onUseAbility?.Invoke();
			}
		}

		public bool IsPS4Controller()
		{
			return playerInput.currentControlScheme == "PS4Controller";
		}

		public bool IsXboxController()
		{
			return playerInput.currentControlScheme == "XboxController";
		}

		public bool IsKeyboardMouse()
		{
			return playerInput.currentControlScheme == "KeyboardMouse";
		}

		public bool IsGamepad()
		{
			return playerInput.currentControlScheme == "Gamepad";
		}

		void HandleCustomBindings(InputActionMap actionMap)
		{
			InputAction sprintAction = actionMap.FindAction("Sprint", true);
			if (!string.IsNullOrEmpty(gameSettings.sprintBinding))
			{
				sprintAction.ApplyBindingOverride(gameSettings.sprintBinding);
			}
			else
			{
				sprintAction.RemoveAllBindingOverrides();
			}

			InputAction dodgeAction = actionMap.FindAction("Dodge", true);
			if (!string.IsNullOrEmpty(gameSettings.dodgeBinding))
			{
				dodgeAction.ApplyBindingOverride(gameSettings.dodgeBinding);
			}
			else
			{
				dodgeAction.RemoveAllBindingOverrides();
			}


			InputAction jumpAction = actionMap.FindAction("Jump", true);
			if (!string.IsNullOrEmpty(gameSettings.jumpBinding))
			{
				jumpAction.ApplyBindingOverride(gameSettings.jumpBinding);
			}
			else
			{
				jumpAction.RemoveAllBindingOverrides();
			}

			InputAction useAbility = actionMap.FindAction("UseAbility", true);
			if (!string.IsNullOrEmpty(gameSettings.useAbilityBinding))
			{
				useAbility.ApplyBindingOverride(gameSettings.useAbilityBinding);
			}
			else
			{
				useAbility.RemoveAllBindingOverrides();
			}

			InputAction toggleCombatStance = actionMap.FindAction("Tab", true);
			if (!string.IsNullOrEmpty(gameSettings.toggleCombatStanceBinding))
			{
				toggleCombatStance.ApplyBindingOverride(gameSettings.toggleCombatStanceBinding);
			}
			else
			{
				toggleCombatStance.RemoveAllBindingOverrides();
			}

			InputAction heavyAttack = actionMap.FindAction("HeavyAttack", true);
			if (!string.IsNullOrEmpty(gameSettings.heavyAttackBinding))
			{
				heavyAttack.ApplyBindingOverride(gameSettings.toggleCombatStanceBinding);
			}
			else
			{
				heavyAttack.RemoveAllBindingOverrides();
			}
		}
	}
}
