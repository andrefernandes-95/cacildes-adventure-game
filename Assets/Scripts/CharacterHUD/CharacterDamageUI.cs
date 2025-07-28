using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace AF.Health
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CharacterDamageUI : MonoBehaviour
    {

        [Header("UI Options")]
        [SerializeField] CanvasGroup healthRestored;
        TextMeshProUGUI healthRestoredLabel => healthRestored.GetComponentInChildren<TextMeshProUGUI>();

        [SerializeField] CanvasGroup physical;
        TextMeshProUGUI physicalLabel => physical.GetComponentInChildren<TextMeshProUGUI>();

        [SerializeField] CanvasGroup fire;
        TextMeshProUGUI fireLabel => fire.GetComponentInChildren<TextMeshProUGUI>();

        [SerializeField] CanvasGroup frost;
        TextMeshProUGUI frostLabel => frost.GetComponentInChildren<TextMeshProUGUI>();

        [SerializeField] CanvasGroup lightning;
        TextMeshProUGUI lightningLabel => lightning.GetComponentInChildren<TextMeshProUGUI>();

        [SerializeField] CanvasGroup magic;
        TextMeshProUGUI magicLabel => magic.GetComponentInChildren<TextMeshProUGUI>();

        [SerializeField] CanvasGroup darkness;
        TextMeshProUGUI darknessLabel => darkness.GetComponentInChildren<TextMeshProUGUI>();

        [SerializeField] CanvasGroup water;
        TextMeshProUGUI waterLabel => water.GetComponentInChildren<TextMeshProUGUI>();


        [Header("Options")]
        [SerializeField] float popupDuration = 2f;
        [SerializeField] float canvasFadeSpeed = .5f;

        Coroutine healthRestoredCoroutine;
        Coroutine physicalCoroutine;
        Coroutine fireCoroutine;
        Coroutine frostCoroutine;
        Coroutine lightningCoroutine;
        Coroutine magicCoroutine;
        Coroutine darknessCoroutine;
        Coroutine waterCoroutine;

        public void SetupEvents(CharacterBaseManager characterBaseManager)
        {
            characterBaseManager.health.onHealthRestoredUI.AddListener(OnHealthRestoredUI);
            characterBaseManager.characterBaseDamageReceiver.onPhysicalDamageUI.AddListener(OnPhysicalDamageUI);
            characterBaseManager.characterBaseDamageReceiver.onFireDamageUI.AddListener(OnFireDamageUI);
            characterBaseManager.characterBaseDamageReceiver.onFrostDamageUI.AddListener(OnFrostDamageUI);
            characterBaseManager.characterBaseDamageReceiver.onLightningDamageUI.AddListener(OnLightningDamageUI);
            characterBaseManager.characterBaseDamageReceiver.onMagicDamageUI.AddListener(OnMagicDamageUI);
            characterBaseManager.characterBaseDamageReceiver.onDarknessDamageUI.AddListener(OnDarknessDamageUI);
            characterBaseManager.characterBaseDamageReceiver.onWaterDamageUI.AddListener(OnWaterDamageUI);

            HideAll();
        }

        void HideAll()
        {
            healthRestored.gameObject.SetActive(false);
            physical.gameObject.SetActive(false);
            fire.gameObject.SetActive(false);
            frost.gameObject.SetActive(false);
            lightning.gameObject.SetActive(false);
            magic.gameObject.SetActive(false);
            darkness.gameObject.SetActive(false);
            water.gameObject.SetActive(false);
        }

        void OnHealthRestoredUI(int value)
        {
            if (healthRestoredCoroutine != null)
            {
                StopCoroutine(healthRestoredCoroutine);
            }
            healthRestoredCoroutine = StartCoroutine(StartPopupCoroutine(healthRestored, healthRestoredLabel, value));
        }

        void OnPhysicalDamageUI(int damage)
        {
            if (physicalCoroutine != null)
            {
                StopCoroutine(physicalCoroutine);
            }
            physicalCoroutine = StartCoroutine(StartPopupCoroutine(physical, physicalLabel, damage));
        }

        void OnFireDamageUI(int damage)
        {
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
            }
            fireCoroutine = StartCoroutine(StartPopupCoroutine(fire, fireLabel, damage));
        }

        void OnFrostDamageUI(int damage)
        {
            if (frostCoroutine != null)
            {
                StopCoroutine(frostCoroutine);
            }
            frostCoroutine = StartCoroutine(StartPopupCoroutine(frost, frostLabel, damage));
        }

        void OnLightningDamageUI(int damage)
        {
            if (lightningCoroutine != null)
            {
                StopCoroutine(lightningCoroutine);
            }
            lightningCoroutine = StartCoroutine(StartPopupCoroutine(lightning, lightningLabel, damage));
        }

        void OnMagicDamageUI(int damage)
        {
            if (magicCoroutine != null)
            {
                StopCoroutine(magicCoroutine);
            }
            magicCoroutine = StartCoroutine(StartPopupCoroutine(magic, magicLabel, damage));
        }

        void OnDarknessDamageUI(int damage)
        {
            if (darknessCoroutine != null)
            {
                StopCoroutine(darknessCoroutine);
            }
            darknessCoroutine = StartCoroutine(StartPopupCoroutine(darkness, darknessLabel, damage));
        }

        void OnWaterDamageUI(int damage)
        {
            if (waterCoroutine != null)
            {
                StopCoroutine(waterCoroutine);
            }
            waterCoroutine = StartCoroutine(StartPopupCoroutine(water, waterLabel, damage));
        }

        IEnumerator StartPopupCoroutine(CanvasGroup canvasGroup, TextMeshProUGUI label, int damageAmount)
        {
            canvasGroup.alpha = 0;

            canvasGroup.gameObject.SetActive(true);
            label.text = damageAmount.ToString();

            // Reset scale before punch
            canvasGroup.transform.localScale = Vector3.one;

            // Slight "pop" animation
            canvasGroup.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 8, 1f);

            UIUtils.FadeIn(canvasGroup, canvasFadeSpeed);
            yield return new WaitForSeconds(popupDuration);
            UIUtils.FadeOut(canvasGroup, canvasFadeSpeed, () =>
            {
                canvasGroup.gameObject.SetActive(false);
            });

        }
    }
}
