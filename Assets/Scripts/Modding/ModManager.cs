
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CI.QuickSave;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

namespace AF.ModTools
{
    public class ModManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] CursorManager cursorManager;
        [SerializeField] PlayerManager playerManager;
        [SerializeField] NavMeshSurface navMeshSurface;

        [Header("UI")]
        public UIDocumentMomentEditor uIDocumentMomentEditor;

        [Header("API")]
        public SelectionTool selectionTool;

        [Header("References")]
        public GameObject[] objectsToDisableInEditMode;

        public ModCameraController modCamera;
        public bool IsEditMode { get; private set; }

        public Dictionary<string, ModAsset> modAssetsLookup = new();
        public List<ModAsset> environmentModAssets = new();
        public List<ModAsset> objectModAssets = new();
        public List<ModAsset> entitiyModAssets = new();

        [Header("Enums")]
        public ModAssetType EnvironmentAsset;
        public ModAssetType ObjectAsset;
        public ModAssetType EntityAsset;

        // Events
        [HideInInspector] public UnityEvent onPlayModeEnter;
        [HideInInspector] public UnityEvent onEditModeEnter;

        public ModFile currentModFile;


        void Awake()
        {
            CreateModFile();

            LoadAssets();
        }

        void CreateModFile()
        {
            currentModFile = new();
            currentModFile.modName = $"Mod-{Guid.NewGuid()}";
        }

        void Start()
        {
            EnterEditMode();
        }

        void LoadAssets()
        {
            ModAsset[] modAssetsInScene = FindObjectsByType<ModAsset>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            modAssetsLookup.Clear();

            foreach (ModAsset modAsset in modAssetsInScene)
            {
                if (modAsset.modAssetType == EnvironmentAsset)
                {
                    environmentModAssets.Add(modAsset);
                }
                else if (modAsset.modAssetType == ObjectAsset)
                {
                    objectModAssets.Add(modAsset);
                }
                else if (modAsset.modAssetType == EntityAsset)
                {
                    entitiyModAssets.Add(modAsset);
                }

                modAssetsLookup.Add(modAsset.GetModAssetId(), modAsset);
            }
        }

        void Update()
        {
            if (IsEditMode)
            {
                cursorManager.ShowCursor();
                RenderSettings.fog = false;
            }

            // TEMP: toggle with F10
            if (Input.GetKeyDown(KeyCode.F10))
            {
                if (IsEditMode)
                {
                    ExitEditMode();
                }
                else
                {
                    EnterEditMode();
                }
            }
        }

        public void EnterEditMode()
        {
            IsEditMode = true;

            foreach (GameObject g in objectsToDisableInEditMode)
            {
                g.SetActive(false);
            }

            playerManager.playerComponentManager.DisablePlayerControl();
            playerManager.uIDocumentPlayerHUDV2.HideHUD();

            modCamera.EnableCamera();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            onEditModeEnter?.Invoke();
        }

        public void ExitEditMode()
        {
            IsEditMode = false;

            foreach (GameObject g in objectsToDisableInEditMode)
            {
                g.SetActive(true);
            }
            playerManager.playerComponentManager.EnablePlayerControl();
            playerManager.uIDocumentPlayerHUDV2.ShowHUD();

            modCamera.DisableCamera();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            navMeshSurface.BuildNavMesh();

            onPlayModeEnter?.Invoke();
        }

        public void SpawnModAsset(ModAsset modAsset)
        {
            if (modAsset == null || modAsset.prefab == null)
                return;

            Camera cam = Camera.main;
            if (cam == null)
                return;

            Vector3 spawnPosition = Vector3.zero;

            if (selectionTool.lastDragPosition != Vector3.zero)
            {
                spawnPosition = selectionTool.lastDragPosition;
            }

            // 4️⃣ Downward snap to surface below
            float rayStartHeight = Mathf.Max(spawnPosition.y + 5f, cam.transform.position.y + 5f);
            float maxDistance = 2000f;

            Vector3 rayStart = new Vector3(spawnPosition.x, rayStartHeight, spawnPosition.z);

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit downHit, maxDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                // Place on top of surface
                spawnPosition.y = downHit.collider.bounds.max.y;
            }
            else
            {
                spawnPosition.y = 0f;
            }


            InstantiateEditableObject(Guid.NewGuid().ToString(), spawnPosition, modAsset);
        }

        void InstantiateEditableObject(string id, Vector3 position, ModAsset modAsset)
        {
            // 3️⃣ Instantiate
            GameObject instance = Instantiate(
            modAsset.prefab,
            position,
            Quaternion.identity
            );

            // 4️⃣ Ensure it's editable
            if (!instance.TryGetComponent<EditableObject>(out _))
            {
                EditableObject ed = instance.AddComponent<EditableObject>();
                ed.modAsset = modAsset;
                ed.objectId = id;
            }

            // Get the integer value of the LayerMask
            LayerMask editableMask = selectionTool.editableLayers;

            // Convert LayerMask to actual layer number
            int layerNumber = 0;
            for (int i = 0; i < 32; i++)
            {
                if ((editableMask.value & (1 << i)) != 0)
                {
                    layerNumber = i;
                    break;
                }
            }

            instance.layer = layerNumber;

            // 5️⃣ Auto-select it
            selectionTool.Set(instance.GetComponent<EditableObject>(), selectionTool.selectionMaterial);
        }

        public void SaveMod()
        {
            string saveFileName = currentModFile.modName;
            string PreferencesFileName = saveFileName + ".json";
            string PreferencesFolderPath = Path.Combine(Application.persistentDataPath, "Mods");
            string PreferencesFilePath = Path.Combine(PreferencesFolderPath, PreferencesFileName);

            if (!Directory.Exists(PreferencesFolderPath))
            {
                Directory.CreateDirectory(PreferencesFolderPath);
            }

            QuickSaveWriter quickSaveWriter = QuickSaveWriter.Create(PreferencesFilePath);

            MonoBehaviour[] allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            IModSaveable[] saveables = allBehaviours.OfType<IModSaveable>().ToArray();

            List<SerializedModData<object>> data = new();
            foreach (IModSaveable s in saveables)
            {
                SerializedModData<object> savedItem = s.OnSaveData<object>();
                data.Add(savedItem);
            }

            quickSaveWriter.Write("modData", data.ToArray());
            quickSaveWriter.Write("modName", currentModFile.modName);
            quickSaveWriter.TryCommit();
        }

        public void LoadMod(QuickSaveReader quickSaveReader)
        {
            currentModFile = new ModFile();

            // Clear Editable Objects
            ClearEditableObjects();

            if (quickSaveReader.TryRead("modName", out string modName))
            {
                currentModFile.modName = modName;
            }

            LoadEditableObjectsFromMod(quickSaveReader);
        }

        void ClearEditableObjects()
        {
            EditableObject[] editableObjects = FindObjectsByType<EditableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (EditableObject e in editableObjects)
            {
                Destroy(e.gameObject);
            }
        }

        void LoadEditableObjectsFromMod(QuickSaveReader quickSaveReader)
        {
            if (quickSaveReader.TryRead("modData", out SerializedModData<object>[] modData))
            {
                foreach (var itemData in modData)
                {
                    if (modAssetsLookup.ContainsKey(itemData.modAssetId))
                    {
                        ModAsset modAsset = modAssetsLookup[itemData.modAssetId];
                        InstantiateEditableObject(itemData.id, itemData.worldPosition, modAsset);
                    }
                }
            }
        }

        public bool IsModalOpen()
        {
            if (uIDocumentMomentEditor.IsOpen())
            {
                return true;
            }

            return false;
        }
    }
}
