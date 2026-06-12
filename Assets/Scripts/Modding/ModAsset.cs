namespace AF.ModTools
{
    using System;
    using EditorAttributes;
    using UnityEngine;
    using UnityEngine.Localization;

    [RequireComponent(typeof(MonoBehaviourID))]
    public class ModAsset : MonoBehaviour
    {
        public LocalizedString customDisplayName;
        public ModAssetType modAssetType;

        [Header("Prefab")]
        public GameObject prefab;

        [Header("UI")]
        [AssetPreview]
        public Texture2D thumbnail;

        public virtual string GetName()
        {
            if (customDisplayName.IsEmpty)
            {
                return name;
            }

            return customDisplayName.GetLocalizedString();
        }

        public SerializedModData<T> GetSerializedModData<T>(GameObject instance)
        {
            SerializedModData<T> serializedModData = new();

            serializedModData.id = instance.GetComponent<EditableObject>().objectId;
            serializedModData.worldPosition = instance.transform.position;
            serializedModData.worldRotation = instance.transform.eulerAngles;
            serializedModData.worldScale = instance.transform.localScale;

            serializedModData.modAssetId = GetModAssetId();

            return serializedModData;
        }

        public virtual string GetResourcePath()
        {
            return "";
        }

        public string GetModAssetId()
        {
            if (TryGetComponent<MonoBehaviourID>(out var monoBehaviourID))
            {
                return monoBehaviourID.ID;
            }

            return Guid.NewGuid().ToString();
        }
    }

    [System.Serializable]
    public class SerializedModData<T>
    {
        public string id;
        public Vector3 worldPosition;
        public Vector3 worldRotation;
        public Vector3 worldScale;

        public string modAssetId;
    }
}
