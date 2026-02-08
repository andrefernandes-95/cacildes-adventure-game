using CI.QuickSave;
using UnityEngine;

namespace AF.ModTools
{
    public class EditableObject : MonoBehaviour, IModSaveable
    {
        [Header("Optional Metadata")]
        public ModAsset modAsset;
        public string objectId;
        public bool allowPositionEdit = true;

        [Header("Outline Settings")]
        private Material[] originalMaterials;  // Original materials
        private Renderer rend;

        void Awake()
        {
            rend = GetComponent<Renderer>();

            if (rend != null)
            {
                originalMaterials = rend.materials;
            }
        }

        public void SetHovered(bool hover, Material hoverMaterial)
        {
            if (hover)
                EnableOutline(hoverMaterial);
            else
                DisableOutline();
        }

        public void SetSelected(bool selected, Material outlineMaterial)
        {
            if (selected)
                EnableOutline(outlineMaterial);
            else
                DisableOutline();
        }

        private void EnableOutline(Material outlineMaterial)
        {
            if (rend == null)
            {
                return;
            }

            // Add outline material on top of the original materials
            Material[] mats = new Material[originalMaterials.Length + 1];
            originalMaterials.CopyTo(mats, 0);
            mats[mats.Length - 1] = outlineMaterial;
            rend.materials = mats;
        }

        private void DisableOutline()
        {
            if (rend == null)
            {
                return;
            }

            // Restore original materials
            rend.materials = originalMaterials;
        }

        public SerializedModData<Object> OnSaveData<Object>()
        {
            return modAsset.GetSerializedModData<Object>(this.gameObject);
        }
    }
}
