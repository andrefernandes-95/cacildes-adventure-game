using System.Collections.Generic;
using System.Linq;
using AF.Events;
using AYellowpaper.SerializedCollections;
using TigerForge;
using UnityEngine;

namespace AF
{
    public class SyntyCharacterModelManager : MonoBehaviour
    {
        public bool isUsingSyntyModularFantasyHeroModel = true;
        public SerializedDictionary<string, GameObject> syntyCharacterBodyParts = new();

        public CharacterBaseManager character;

        List<Material> characterMaterials = new();

        public List<string> maleFaces = new();
        public List<string> femaleFaces = new();
        public List<string> hairs = new();
        public List<string> eyebrows = new();
        public List<string> beards = new();
        public List<string> maleTorsos = new();
        public List<string> femaleTorsos = new();
        SkinnedMeshRenderer[] renderers => GetComponentsInChildren<SkinnedMeshRenderer>(true).ToArray();

        void Awake()
        {
            if (!isUsingSyntyModularFantasyHeroModel)
            {
                gameObject.SetActive(false);
                return;
            }

            CacheCharacterParts();
            InitializeDefaultBodyParts();
            LoadMaterialColorsAndApplyColors();

            CollectLists();
        }

        void CollectLists()
        {
            maleFaces.Clear();
            femaleFaces.Clear();
            hairs.Clear();
            eyebrows.Clear();
            beards.Clear();
            maleTorsos.Clear();
            femaleTorsos.Clear();

            foreach (var entry in syntyCharacterBodyParts)
            {
                if (entry.Key.Contains("Head_Male_"))
                {
                    maleFaces.Add(entry.Key);
                }
                else if (entry.Key.Contains("Head_Female_"))
                {
                    femaleFaces.Add(entry.Key);
                }
                else if (entry.Key.Contains("Chr_Hair_"))
                {
                    hairs.Add(entry.Key);
                }
                else if (entry.Key.Contains("Chr_Eyebrow_"))
                {
                    eyebrows.Add(entry.Key);
                }
                else if (entry.Key.Contains("FacialHair"))
                {
                    beards.Add(entry.Key);
                }
                else if (entry.Key.Contains("Torso_Male_") || entry.Key.Contains("ArmUpperRight_Male") || entry.Key.Contains("ArmUpperLeft_Male"))
                {
                    maleTorsos.Add(entry.Key);
                }
                else if (entry.Key.Contains("Torso_Female_") || entry.Key.Contains("ArmUpperRight_Female") || entry.Key.Contains("ArmUpperLeft_Female"))
                {
                    femaleTorsos.Add(entry.Key);
                }
            }
        }

        public void UpdateAvatar()
        {
            LoadMaterialColorsAndApplyColors();

            // Check if hair is hidden before toggling
            ToggleHair(true);
            ToggleBeard(true);
            ToggleEyebrows(true);
            ToggleFace(true);
            ToggleTorso(true);
        }

        void LoadMaterialColorsAndApplyColors()
        {
            characterMaterials.Clear();

            var appearance = character.characterBaseAppearance;

            var hairColor = appearance.GetHairColor();
            var skinColor = appearance.GetSkinColor();
            var eyesColor = appearance.GetEyesColor();
            var tattooColor = appearance.GetTattooColor();

            foreach (var renderer in renderers)
            {
                var originalMaterials = renderer.sharedMaterials;
                var clonedMaterials = new Material[originalMaterials.Length];

                for (int i = 0; i < originalMaterials.Length; i++)
                {
                    var clonedMat = new Material(originalMaterials[i]);

                    // Apply colors immediately
                    clonedMat.SetColor("_Color_Hair", hairColor);
                    clonedMat.SetColor("_Color_Skin", skinColor);
                    clonedMat.SetColor("_Color_Stubble", skinColor);
                    clonedMat.SetColor("_Color_Eyes", eyesColor);
                    clonedMat.SetColor("_Color_BodyArt", tattooColor);
                    clonedMat.SetColor("_Color_Scar", tattooColor);

                    clonedMaterials[i] = clonedMat;
                    characterMaterials.Add(clonedMat);
                }

                renderer.materials = clonedMaterials;
            }
        }

        void InitializeDefaultBodyParts()
        {
            ToggleHair(true);
            ToggleBeard(true);
            ToggleEyebrows(true);
            ToggleFace(true);
            ToggleTorso(true);
            ToggleHands(true);
            ToggleLegs(true);
        }

        void CacheCharacterParts()
        {
            syntyCharacterBodyParts.Clear();

            var syntyPieces = transform.GetChild(0).GetComponentsInChildren<Transform>(true);

            foreach (Transform t in syntyPieces)
            {
                if (!syntyCharacterBodyParts.ContainsKey(t.gameObject.name))
                {
                    syntyCharacterBodyParts.Add(t.gameObject.name, t.gameObject);

                    // Hide All Pieces By Default
                    t.gameObject.SetActive(false);
                }
            }

            if (syntyCharacterBodyParts.ContainsKey("Armature"))
            {
                // TODO: Improve this logic

                syntyCharacterBodyParts["Armature"].SetActive(true);
                foreach (Transform child in syntyCharacterBodyParts["Armature"].transform)
                {
                    SetActiveRecursively(child.gameObject, true);
                }
            }

            if (syntyCharacterBodyParts.ContainsKey("Exported Synty Character"))
            {
                syntyCharacterBodyParts["Exported Synty Character"].SetActive(true);
            }
        }
        void SetActiveRecursively(GameObject obj, bool isActive)
        {
            obj.SetActive(isActive);

            foreach (Transform child in obj.transform)
            {
                SetActiveRecursively(child.gameObject, isActive);
            }
        }

        public void ShowPiece(string gameObjectName)
        {
            if (syntyCharacterBodyParts.ContainsKey(gameObjectName))
            {
                syntyCharacterBodyParts[gameObjectName].SetActive(true);
            }
        }

        public void HidePiece(string gameObjectName)
        {
            if (syntyCharacterBodyParts.ContainsKey(gameObjectName))
            {
                syntyCharacterBodyParts[gameObjectName].SetActive(false);
            }
        }

        void TogglePiece(List<string> pieces, bool show)
        {
            foreach (string gameObjectName in pieces)
            {
                if (show)
                {
                    ShowPiece(gameObjectName);
                }
                else
                {
                    HidePiece(gameObjectName);
                }
            }
        }

        public void ToggleHair(bool show)
        {
            hairs.ForEach(hair =>
            {
                if (syntyCharacterBodyParts.ContainsKey(hair))
                {
                    syntyCharacterBodyParts[hair].gameObject.SetActive(false);
                }
            });

            TogglePiece(character.characterBaseAppearance.GetHairs(), show);
        }
        public void ToggleEyebrows(bool show)
        {
            eyebrows.ForEach(eyebrow =>
            {
                if (syntyCharacterBodyParts.ContainsKey(eyebrow))
                {
                    syntyCharacterBodyParts[eyebrow].gameObject.SetActive(false);
                }
            });

            TogglePiece(character.characterBaseAppearance.GetEyebrows(), show);
        }
        public void ToggleBeard(bool show)
        {
            beards.ForEach(beard =>
            {
                if (syntyCharacterBodyParts.ContainsKey(beard))
                {
                    syntyCharacterBodyParts[beard].gameObject.SetActive(false);
                }
            });

            TogglePiece(character.characterBaseAppearance.GetBeard(), show);
        }
        public void ToggleFace(bool show)
        {
            maleFaces.ForEach(maleFace =>
            {
                if (syntyCharacterBodyParts.ContainsKey(maleFace))
                {
                    syntyCharacterBodyParts[maleFace].gameObject.SetActive(false);
                }
            });

            femaleFaces.ForEach(femaleFace =>
            {
                if (syntyCharacterBodyParts.ContainsKey(femaleFace))
                {
                    syntyCharacterBodyParts[femaleFace].gameObject.SetActive(false);
                }
            });

            TogglePiece(character.characterBaseAppearance.GetFace(), show);
        }
        public void ToggleTorso(bool show)
        {
            maleTorsos.ForEach(maleTorso =>
            {
                if (syntyCharacterBodyParts.ContainsKey(maleTorso))
                {
                    syntyCharacterBodyParts[maleTorso].gameObject.SetActive(false);
                }
            });
            femaleTorsos.ForEach(femaleTorso =>
            {
                if (syntyCharacterBodyParts.ContainsKey(femaleTorso))
                {
                    syntyCharacterBodyParts[femaleTorso].gameObject.SetActive(false);
                }
            });

            TogglePiece(character.characterBaseAppearance.GetTorso(), show);
        }
        public void ToggleHands(bool show)
        {
            TogglePiece(character.characterBaseAppearance.GetHands(), show);
        }
        public void ToggleLegs(bool show)
        {
            TogglePiece(character.characterBaseAppearance.GetLegs(), show);
        }

        public void EnableArmorPiece(
           List<string> pieces,
           Material armorMaterial
       )
        {
            foreach (string gameObjectName in pieces)
            {
                if (!syntyCharacterBodyParts.ContainsKey(gameObjectName))
                {
                    continue;
                }

                GameObject bodyPieces = syntyCharacterBodyParts[gameObjectName];

                // Set Materials
                if (bodyPieces.TryGetComponent(out SkinnedMeshRenderer skinnedMeshRenderer))
                {

                    // Clone the material to avoid modifying the shared one
                    Material clonedMaterial = new Material(armorMaterial);

                    clonedMaterial.SetColor("_Color_Hair", character.characterBaseAppearance.GetHairColor());
                    clonedMaterial.SetColor("_Color_Skin", character.characterBaseAppearance.GetSkinColor());
                    clonedMaterial.SetColor("_Color_Stubble", character.characterBaseAppearance.GetSkinColor());
                    clonedMaterial.SetColor("_Color_Eyes", character.characterBaseAppearance.GetEyesColor());
                    clonedMaterial.SetColor("_Color_BodyArt", character.characterBaseAppearance.GetTattooColor());
                    clonedMaterial.SetColor("_Color_Scar", character.characterBaseAppearance.GetTattooColor());

                    // Set the new cloned materials back to the renderer
                    skinnedMeshRenderer.material = clonedMaterial;
                }

                // Enable body piece
                bodyPieces.SetActive(true);
            }
        }

        public void DisablePieces(List<string> pieces)
        {
            foreach (string gameObjectName in pieces)
            {
                if (syntyCharacterBodyParts.ContainsKey(gameObjectName))
                {
                    syntyCharacterBodyParts[gameObjectName].SetActive(false);
                }
            }
        }
    }
}
