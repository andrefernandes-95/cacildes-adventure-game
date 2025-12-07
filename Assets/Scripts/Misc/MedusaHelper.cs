namespace AF
{
    using System;
    using AF.Combat;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class MedusaSnakes : MonoBehaviour
    {
        [SerializeField] CharacterManager characterManager;
        [SerializeField] Slider healthSlider;
        [SerializeField] TextMeshProUGUI currentAndMaxHealthValue;

        [Header("Health Settings")]
        [SerializeField] int currentHealth;
        [SerializeField] int maxHealth = 5;
        [SerializeField] int damagePerHit = 1;

        [Header("VFX")]
        [SerializeField] DestroyableParticle damageBlood;

        [Header("Settings")]
        [SerializeField] float angleThreshold = 15f;     // Angle range for effect
        [SerializeField] float maxDistanceToInflictStatus = 5f;

        [SerializeField] StatusEffect statusEffectToApply;
        [SerializeField] float amount = 1f;

        [Header("Debug")]
        public bool debugAngle = true;

        private void Awake()
        {
            UpdateHealth(maxHealth, false);
        }

        void UpdateHealth(int nextValue, bool takeDamage)
        {
            this.currentHealth = Mathf.Clamp(nextValue, 0, maxHealth);
            healthSlider.value = (float)currentHealth / (float)maxHealth;
            currentAndMaxHealthValue.text = $"{(int)currentHealth}/{maxHealth}";

            if (takeDamage)
            {
                Instantiate(damageBlood, transform.position, Quaternion.identity);
            }

            if (currentHealth <= 0)
            {
                healthSlider.gameObject.SetActive(false);
                this.gameObject.SetActive(false);
            }
        }

        bool CanPetrify()
        {
            if (!this.isActiveAndEnabled)
            {
                return false;
            }

            if (currentHealth <= 0)
            {
                return false;
            }

            if (characterManager == null)
            {
                return false;
            }

            if (characterManager.health.GetCurrentHealth() <= 0)
            {
                return false;
            }

            if (!characterManager.GetTarget())
            {
                return false;
            }

            if (characterManager.characterPosture.isStunned)
            {
                return false;
            }

            return true;
        }

        void Update()
        {
            HandlePetrify();
        }

        void HandlePetrify()
        {
            if (!CanPetrify())
            {
                return;
            }

            CharacterBaseManager target = characterManager.GetTarget();

            // Calculate direction vectors
            Vector3 medusaForward = characterManager.transform.forward;
            Vector3 playerForward = target.transform.forward * -1f;

            // Check if the angle is within the threshold
            if (Vector3.Angle(medusaForward, playerForward) <= angleThreshold)
            {
                if (Vector3.Distance(transform.position, target.transform.position) <= maxDistanceToInflictStatus)
                {
                    ApplyStatusEffect(target);
                }
            }
        }

        void ApplyStatusEffect(CharacterBaseManager target)
        {
            target.statusController.InflictStatusEffect(statusEffectToApply, amount * Time.deltaTime);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnDamage()
        {
            UpdateHealth(currentHealth - damagePerHit, true);
        }
    }

}
