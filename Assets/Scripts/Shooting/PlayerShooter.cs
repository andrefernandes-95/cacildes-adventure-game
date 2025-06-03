using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using Cinemachine;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

namespace AF.Shooting
{
    public class PlayerShooter : CharacterBaseShooter
    {
        public readonly int hashFireBowLockedOn = Animator.StringToHash("Locked On - Shoot Bow");

        [Header("Stamina Cost")]
        public int minimumStaminaToShoot = 10;

        [Header("Achievements")]
        public Achievement achievementOnShootingBowForFirstTime;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        public GameSession gameSession;

        [Header("Aiming")]
        public GameObject aimingCamera;
        public LookAtConstraint lookAtConstraint;

        public float bowAimCameraDistance = 1.25f;
        public float crossbowAimCameraDistance = 1.5f;
        public float spellAimCameraDistance = 2.25f;

        [Header("Components")]
        public LockOnManager lockOnManager;
        public UIManager uIManager;
        public MenuManager menuManager;
        public NotificationManager notificationManager;

        [Header("Refs")]
        public Transform playerFeetRef;
        public Transform playerShootingHandRef;


        [Header("Flags")]
        public bool isAiming = false;
        public bool isShooting = false;

        [Header("Enums")]
        public ProjectileType arrowProjectileType;
        public ProjectileType boltProjectileType;
        public ProjectileType bulletProjectileType;

        // For cache purposes
        Spell previousSpell;

        [Header("Events")]
        public UnityEvent onSpellAim_Begin;
        public UnityEvent onBowAim_Begin;

        [Header("Cinemachine")]
        Cinemachine3rdPersonFollow cinemachine3RdPersonFollow;

        public CinemachineImpulseSource cinemachineImpulseSource;

        Coroutine FireDelayedProjectileCoroutine;

        public GameObject queuedProjectile;
        public Spell queuedSpell;

        public bool hasAimShotCooldown = false;

        [Header("Sounds")]
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip drawArrowSfx;

        public void ResetStates()
        {
            isShooting = false;

            queuedProjectile = null;
            queuedSpell = null;
        }

        void SetupCinemachine3rdPersonFollowReference()
        {
            if (cinemachine3RdPersonFollow != null)
            {
                return;
            }

            cinemachine3RdPersonFollow = aimingCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }

        bool IsRangeWeaponCompatibleWithProjectile()
        {
            Weapon currentRangeWeapon = equipmentDatabase.GetCurrentWeapon();
            Arrow arrow = equipmentDatabase.GetCurrentArrow();

            if (currentRangeWeapon == null || arrow == null)
            {
                return false;
            }

            if (currentRangeWeapon.projectileType == bulletProjectileType && arrow.projectileType != bulletProjectileType)
            {
                string alert = "";
                if (Utils.IsPortuguese())
                {
                    alert = "Precisas de balas para usar esta arma.";
                }
                else
                {
                    alert = "You need bullets to use this weapon.";
                }

                GetPlayerManager().uIDocumentAlert.ShowAlert(alert);
                return false;
            }

            if (currentRangeWeapon.projectileType == boltProjectileType && arrow.projectileType != boltProjectileType)
            {
                string alert = "";

                if (Utils.IsPortuguese())
                {
                    alert = "Precisas de virotes para usar esta besta.";
                }
                else
                {
                    alert = "You need bolts to use this crossbow.";
                }

                GetPlayerManager().uIDocumentAlert.ShowAlert(alert);
                return false;
            }

            if (currentRangeWeapon.projectileType == arrowProjectileType && arrow.projectileType != arrowProjectileType)
            {
                string alert = "";
                if (Utils.IsPortuguese())
                {
                    alert = "Precisas de flechas para usar este arco.";
                }
                else
                {
                    alert = "You need arrows to use this bow.";
                }

                GetPlayerManager().uIDocumentAlert.ShowAlert(alert);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnFireInput()
        {
            if (CanShoot())
            {
                if (equipmentDatabase.IsRangeWeaponEquipped() && equipmentDatabase.HasEnoughCurrentArrows())
                {
                    if (!IsRangeWeaponCompatibleWithProjectile())
                    {
                        return;
                    }

                    if (isAiming && hasAimShotCooldown)
                    {
                        return;
                    }

                    ShootBow(equipmentDatabase.GetCurrentArrow(), transform, lockOnManager.nearestLockOnTarget?.transform);
                    uIDocumentPlayerHUDV2.UpdateEquipment();
                    return;
                }

                PlayerManager playerManager = GetPlayerManager();

                if (
                   equipmentDatabase.IsStaffEquipped()
                   && equipmentDatabase.GetCurrentSpell() != null
                   && playerManager.manaManager.HasEnoughManaForSpell(equipmentDatabase.GetCurrentSpell()))
                {
                    playerManager.manaManager.DecreaseMana(equipmentDatabase.GetCurrentSpell().costPerCast);

                    HandleSpellCastAnimationOverrides();

                    playerManager.PlayBusyHashedAnimationWithRootMotion(hashCast);
                }
            }
        }

        void HandleSpellCastAnimationOverrides()
        {
            Spell currentSpell = equipmentDatabase.GetCurrentSpell();

            if (currentSpell == previousSpell)
            {
                return;
            }

            previousSpell = currentSpell;

            bool ignoreSpellsAnimationClips = false;
            if (
                currentSpell.animationCanNotBeOverriden == false &&
                equipmentDatabase.GetCurrentWeapon() != null &&
                equipmentDatabase.GetCurrentWeapon().ignoreSpellsAnimationClips)
            {
                ignoreSpellsAnimationClips = true;
            }

            if (currentSpell.castAnimationOverride != null && ignoreSpellsAnimationClips == false)
            {
                GetPlayerManager().UpdateAnimatorOverrideControllerClip("Cacildes - Spell - Cast", currentSpell.castAnimationOverride);
                GetPlayerManager().RefreshAnimationOverrideState();
            }
        }

        public void Aim_Begin()
        {
            if (!CanAim())
            {
                return;
            }

            isAiming = true;
            aimingCamera.SetActive(true);
            GetPlayerManager().thirdPersonController.rotateWithCamera = true;
            lockOnManager.DisableLockOn();

            SetupCinemachine3rdPersonFollowReference();

            if (equipmentDatabase.IsRangeWeaponEquipped())
            {
                OnAimingBowBegin();
            }
            else if (equipmentDatabase.IsStaffEquipped())
            {
                cinemachine3RdPersonFollow.CameraDistance = spellAimCameraDistance;
                onSpellAim_Begin?.Invoke();
            }

            GetPlayerManager().thirdPersonController.virtualCamera.gameObject.SetActive(false);
        }

        void OnAimingBowBegin()
        {
            GetPlayerManager().animator.SetBool(hashIsAiming, true);

            cinemachine3RdPersonFollow.CameraDistance = equipmentDatabase.GetCurrentWeapon().projectileType == boltProjectileType
                ? crossbowAimCameraDistance : bowAimCameraDistance;

            onBowAim_Begin?.Invoke();

            ShowArrowPlaceholder();
        }

        void ShowArrowPlaceholder()
        {
            if (equipmentDatabase.GetCurrentArrow() != null)
            {
                GetPlayerManager().projectileSpawner.ShowArrowPlaceholder(equipmentDatabase.GetCurrentArrow());

                if (drawArrowSfx != null)
                {
                    audioSource.PlayOneShot(drawArrowSfx);
                }
            }
        }

        public void Aim_End()
        {
            if (!isAiming)
            {
                return;
            }

            isAiming = false;
            aimingCamera.SetActive(false);
            lookAtConstraint.constraintActive = false;
            GetPlayerManager().thirdPersonController.rotateWithCamera = false;
            GetPlayerManager().animator.SetBool(hashIsAiming, false);
            GetPlayerManager().thirdPersonController.virtualCamera.gameObject.SetActive(true);

            GetPlayerManager().projectileSpawner.HideArrowPlaceholders();
        }

        private void Update()
        {
            lookAtConstraint.constraintActive = isAiming && equipmentDatabase.IsRangeWeaponEquipped();
        }

        public void ShootBow(Arrow arrow, Transform origin, Transform lockOnTarget)
        {
            ShowArrowPlaceholder();

            if (equipmentDatabase.IsRangeWeaponEquipped())
            {
                achievementOnShootingBowForFirstTime.AwardAchievement();
            }

            if (equipmentDatabase.GetCurrentArrow().loseUponFiring)
            {
                inventoryDatabase.RemoveItem(arrow, 1);
            }

            GetPlayerManager().staminaStatManager.DecreaseStamina(minimumStaminaToShoot);

            FireProjectile(arrow.arrowProjectile.gameObject, lockOnTarget, null);
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        public override void CastSpell()
        {
            ShootSpell(equipmentDatabase.GetCurrentSpell(), lockOnManager.nearestLockOnTarget?.transform);

            OnShoot();
        }

        public override void FireArrow()
        {
        }

        public void ShootSpell(Spell spell, Transform lockOnTarget)
        {
            if (spell == null)
            {
                return;
            }

            GetPlayerManager().staminaStatManager.DecreaseStamina(minimumStaminaToShoot);

            if (spell.projectile != null)
            {
                FireProjectile(spell.projectile.gameObject, lockOnTarget, spell);
            }
        }

        public void FireProjectile(GameObject projectile, Transform lockOnTarget, Spell spell)
        {
            if (lockOnTarget != null && lockOnManager.isLockedOn)
            {
                var rotation = lockOnTarget.transform.position - characterBaseManager.transform.position;
                rotation.y = 0;
                characterBaseManager.transform.rotation = Quaternion.LookRotation(rotation);
            }

            if (equipmentDatabase.IsRangeWeaponEquipped())
            {
                if (isAiming)
                {
                    characterBaseManager.PlayBusyHashedAnimation(hashFireBow);
                }
                else
                {
                    characterBaseManager.PlayBusyHashedAnimation(hashFireBowLockedOn);
                }
            }

            queuedProjectile = projectile;
            queuedSpell = spell;
        }

        public void ShootWithoutClearingProjectilesAndSpells(bool ignoreSpawnFromCamera)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
            Vector3 lookPosition = ray.direction;

            /*if (lookPosition.y > 0)
            {
                lookPosition.y *= -1f;
            }*/

            cinemachineImpulseSource.GenerateImpulse();
            isShooting = true;

            if (FireDelayedProjectileCoroutine != null)
            {
                StopCoroutine(FireDelayedProjectileCoroutine);
            }

            float distanceFromCamera = 0f;
            if (queuedProjectile != null)
            {
                distanceFromCamera = 1f;
            }

            Vector3 origin = ray.GetPoint(distanceFromCamera);

            if (equipmentDatabase.IsRangeWeaponEquipped())
            {
                HandleArrowProjectile(queuedProjectile);
            }
            else
            {
                HandleProjectile(
                    queuedProjectile,
                    origin,
                    ray,
                    0f,
                    queuedSpell,
                    ignoreSpawnFromCamera);
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnShoot()
        {
            ShootWithoutClearingProjectilesAndSpells(false);
            queuedProjectile = null;
            queuedSpell = null;
        }

        void HandleArrowProjectile(GameObject projectile)
        {
            if (projectile == null)
            {
                return;
            }

            // Origin: where the arrow starts

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, playerShootingHandRef.transform.position.z));
            Vector3 origin = isAiming ? ray.origin : playerShootingHandRef.transform.position;
            Vector3 lookPosition = ray.direction;

            // Instantiate projectile facing toward the screen center
            GameObject projectileInstance = Instantiate(projectile, origin, isAiming ? Quaternion.LookRotation(lookPosition) : playerFeetRef.transform.rotation);

            // If aiming, instantiate the projectile a bit forward as to not occupy the whole screen
            if (isAiming)
            {
                projectileInstance.transform.position = projectileInstance.transform.position + projectileInstance.transform.forward * 2;
            }

            IProjectile[] projectileComponents = GetProjectileComponentsInChildren(projectileInstance);

            foreach (IProjectile componentProjectile in projectileComponents)
            {
                Vector3 velocity = projectileInstance.transform.forward * componentProjectile.GetForwardVelocity();

                componentProjectile.Shoot(characterBaseManager,
                    velocity,
                    componentProjectile.GetForceMode());
            }

            HandleProjectileDamageManagers(projectileInstance, null);

            if (isAiming)
            {
                hasAimShotCooldown = true;
            }

            HandleShootingArrowSideEffects(GetPlayerManager(), equipmentDatabase.GetCurrentArrow());
        }


        void HandleProjectile(GameObject projectile, Vector3 origin, Ray ray, float delay, Spell spell, bool ignoreSpawnFromCamera)
        {
            Quaternion lookPosition = Quaternion.identity;

            // If shooting spell but not locked on, use player transform forward to direct the spell
            if (lockOnManager.isLockedOn == false && isAiming == false || spell != null && spell.ignoreSpawnFromCamera || ignoreSpawnFromCamera)
            {
                origin = lookAtConstraint.transform.position;
                ray.direction = characterBaseManager.transform.forward;

                Vector3 lookDir = ray.direction;
                lookDir.y = 0;
                lookPosition = Quaternion.LookRotation(lookDir);
            }

            if (spell != null)
            {
                if (lockOnManager.nearestLockOnTarget != null && spell.spawnOnLockedOnEnemies)
                {
                    origin = lockOnManager.nearestLockOnTarget.transform.position + lockOnManager.nearestLockOnTarget.transform.up;
                }
                else if (spell.spawnAtPlayerFeet)
                {
                    origin = playerFeetRef.transform.position + new Vector3(0, spell.playerFeetOffsetY, 0);
                }

                if (spell.statusEffects != null && spell.statusEffects.Length > 0)
                {
                    foreach (StatusEffect statusEffect in spell.statusEffects)
                    {
                        GetPlayerManager().statusController.statusEffectInstances.FirstOrDefault(x => x.Key == statusEffect).Value?.onConsumeStart?.Invoke();

                        // For positive effects, we override the status effect resistance to be the duration of the consumable effect
                        GetPlayerManager().statusController.statusEffectResistances[statusEffect] = spell.effectsDurationInSeconds;

                        GetPlayerManager().statusController.InflictStatusEffect(statusEffect, spell.effectsDurationInSeconds, true);
                    }
                }
            }

            if (projectile == null)
            {
                return;
            }

            GameObject projectileInstance = Instantiate(projectile, origin, lookPosition);

            if (spell != null && spell.parentToPlayer)
            {
                projectileInstance.transform.parent = GetPlayerManager().transform;
            }

            IProjectile[] projectileComponents = GetProjectileComponentsInChildren(projectileInstance);


            foreach (IProjectile componentProjectile in projectileComponents)
            {
                componentProjectile.Shoot(characterBaseManager, ray.direction * componentProjectile.GetForwardVelocity(), componentProjectile.GetForceMode());
            }

            HandleProjectileDamageManagers(projectileInstance, spell);
        }

        IProjectile[] GetProjectileComponentsInChildren(GameObject obj)
        {
            List<IProjectile> projectileComponents = new List<IProjectile>();

            IProjectile projectile;
            if (obj.TryGetComponent(out projectile))
            {
                projectileComponents.Add(projectile);
            }

            foreach (Transform child in obj.transform)
            {
                projectileComponents.AddRange(GetProjectileComponentsInChildren(child.gameObject));
            }

            return projectileComponents.ToArray();
        }

        void HandleProjectileDamageManagers(GameObject projectileInstance, Spell currentSpell)
        {
            // Assign the damage owner to the OnDamageCollisionAbstractManager of the projectile instance, if it exists
            if (projectileInstance.TryGetComponent(out OnDamageCollisionAbstractManager onDamageCollisionAbstractManager))
            {
                onDamageCollisionAbstractManager.damageOwner = GetPlayerManager();

                if (currentSpell != null)
                {
                    bool shouldDoubleDamage = false;

                    Weapon currentWeapon = GetPlayerManager().attackStatManager.equipmentDatabase.GetCurrentWeapon();

                    if (currentWeapon != null)
                    {
                        shouldDoubleDamage =
                            currentWeapon.doubleDamageDuringNightTime && gameSession.IsNightTime() ||
                            currentWeapon.doubleDamageDuringDayTime && !gameSession.IsNightTime();
                    }

                    onDamageCollisionAbstractManager.damage.ScaleSpell(
                        GetPlayerManager().attackStatManager,
                        currentWeapon,
                        playerStatsDatabase.GetCurrentReputation(),
                        currentSpell.isFaithSpell,
                        currentSpell.isHexSpell,
                        shouldDoubleDamage);
                }

                if (GetPlayerManager().statsBonusController.spellDamageBonusMultiplier > 0)
                {
                    onDamageCollisionAbstractManager.damage.ScaleDamage(GetPlayerManager().statsBonusController.spellDamageBonusMultiplier);
                }
            }

            // Assign the damage owner to all child OnDamageCollisionAbstractManagers of the projectile instance
            OnDamageCollisionAbstractManager[] onDamageCollisionAbstractManagers = GetAllChildOnDamageCollisionManagers(projectileInstance);
            foreach (var onChildDamageCollisionAbstractManager in onDamageCollisionAbstractManagers)
            {
                onChildDamageCollisionAbstractManager.damageOwner = GetPlayerManager();

                if (currentSpell != null)
                {
                    onChildDamageCollisionAbstractManager.damage.ScaleSpell(
                        GetPlayerManager().attackStatManager,
                        GetPlayerManager().attackStatManager.equipmentDatabase.GetCurrentWeapon(),
                        playerStatsDatabase.GetCurrentReputation(),
                        currentSpell.isFaithSpell,
                        currentSpell.isHexSpell,
                        gameSession);
                }

                if (GetPlayerManager().statsBonusController.spellDamageBonusMultiplier > 0)
                {
                    onChildDamageCollisionAbstractManager.damage.ScaleDamage(GetPlayerManager().statsBonusController.spellDamageBonusMultiplier);
                }
            }
        }

        public OnDamageCollisionAbstractManager[] GetAllChildOnDamageCollisionManagers(GameObject obj)
        {
            List<OnDamageCollisionAbstractManager> managers = new List<OnDamageCollisionAbstractManager>();

            foreach (Transform child in obj.transform)
            {
                managers.AddRange(GetAllChildOnDamageCollisionManagers(child.gameObject));
            }

            OnDamageCollisionAbstractManager[] childManagers = obj.GetComponents<OnDamageCollisionAbstractManager>();
            if (childManagers.Length > 0)
            {
                managers.AddRange(childManagers);
            }

            return managers.ToArray();
        }


        bool CanAim()
        {
            if (GetPlayerManager().IsBusy())
            {
                return false;
            }

            if (GetPlayerManager().thirdPersonController.isSwimming)
            {
                return false;
            }

            return equipmentDatabase.IsRangeWeaponEquipped() || equipmentDatabase.IsStaffEquipped();
        }

        public override bool CanShoot()
        {
            if (playerStatsDatabase.currentStamina < minimumStaminaToShoot)
            {
                return false;
            }

            if (menuManager.isMenuOpen)
            {
                return false;
            }

            if (uIManager.IsShowingGUI())
            {
                return false;
            }

            if (GetPlayerManager().IsBusy())
            {
                return false;
            }

            if (GetPlayerManager().characterBlockController.isBlocking)
            {
                return false;
            }

            // If not ranged weapons equipped, dont allow shooting
            if (
                !equipmentDatabase.IsRangeWeaponEquipped()
                && !equipmentDatabase.IsStaffEquipped())
            {
                return false;
            }

            if (GetPlayerManager().thirdPersonController.isSwimming)
            {
                return false;
            }

            return true;
        }

        PlayerManager GetPlayerManager()
        {
            return characterBaseManager as PlayerManager;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="projectile"></param>
        public void ShootProjectile(GameObject projectile)
        {

            if (lockOnManager.nearestLockOnTarget?.transform != null && lockOnManager.isLockedOn)
            {
                var rotation = lockOnManager.nearestLockOnTarget.transform.transform.position - characterBaseManager.transform.position;
                rotation.y = 0;
                characterBaseManager.transform.rotation = Quaternion.LookRotation(rotation);
            }

            this.queuedProjectile = projectile;

            ShootWithoutClearingProjectilesAndSpells(true);
            queuedProjectile = null;
            queuedSpell = null;
        }

        public void ResetHasAimShotCooldown()
        {
            if (isAiming && equipmentDatabase.IsRangeWeaponEquipped())
            {
                ShowArrowPlaceholder();
            }

            hasAimShotCooldown = false;
        }

        public bool IsAimingBowOrCrossbow()
        {
            if (!isAiming)
            {
                return false;
            }

            if (equipmentDatabase.GetCurrentWeapon() != null && equipmentDatabase.GetCurrentWeapon().projectileType == bulletProjectileType)
            {
                return false;
            }

            return true;
        }

        void HandleShootingArrowSideEffects(PlayerManager playerManager, Arrow arrowThatWasShot)
        {
            if (arrowThatWasShot == null)
            {
                return;
            }

            if (arrowThatWasShot.statusEffectsInflictedUponShootingArrow != null && arrowThatWasShot.statusEffectsInflictedUponShootingArrow.Length > 0)
            {
                foreach (var statusEffectEntry in arrowThatWasShot.statusEffectsInflictedUponShootingArrow)
                {
                    playerManager.statusController.InflictStatusEffect(statusEffectEntry.statusEffect, statusEffectEntry.amountPerHit, false);
                }
            }
        }

    }

}
