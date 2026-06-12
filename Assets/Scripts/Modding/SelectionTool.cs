using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AF.ModTools
{
    public class SelectionTool : MonoBehaviour
    {
        [Header("Components")]
        public ModManager modManager;

        public Material selectionMaterial;

        [Header("Raycast")]
        public LayerMask editableLayers;
        public float maxDistance = 500f;

        [Header("Drag")]
        public float dragSpeed = 10f;

        Camera cam;
        bool dragging;
        Vector3 dragOffset;
        Plane dragPlane;

        public Vector3 lastDragPosition;

        public EditableObject Current { get; private set; }

        [HideInInspector] public UnityEvent<EditableObject> onObjectSelected;
        [HideInInspector] public UnityEvent<Vector3> onObjectDragged;
        [HideInInspector] public UnityEvent onObjectDeselected;


        public void Set(EditableObject obj, Material selectedMaterial)
        {
            if (Current != null)
            {
                Current.SetSelected(false, selectedMaterial);
            }

            Current = obj;

            if (Current != null)
            {
                Current.SetSelected(true, selectedMaterial);
            }

            onObjectSelected?.Invoke(Current);
        }

        public void Clear(Material selectedMaterial)
        {
            if (Current != null)
            {
                Current.SetSelected(false, selectedMaterial);
            }

            Current = null;
            onObjectDeselected?.Invoke();
        }

        public void DeleteCurrent()
        {
            if (Current != null)
            {
                GameObject tmp = Current.gameObject;
                Clear(null);
                Destroy(tmp);
            }
        }

        void Awake()
        {
            cam = Camera.main;
        }

        void Update()
        {
            if (modManager.IsEditMode && CanSelectOrDrag())
            {
                HandleSelection();
                HandleDragging();
            }
            else if (Current != null)
            {
                Clear(null);
            }
        }

        bool CanSelectOrDrag()
        {
            if (modManager.IsModalOpen())
            {
                return false;
            }

            return true;
        }

        void HandleSelection()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, editableLayers))
                {
                    EditableObject editable = hit.collider.GetComponentInParent<EditableObject>();

                    if (editable != null)
                    {
                        Set(editable, selectionMaterial);
                        BeginDrag(hit);
                        return;
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                Clear(selectionMaterial);
            }
        }

        void BeginDrag(RaycastHit hit)
        {
            if (Current == null)
                return;

            dragging = true;

            dragPlane = new Plane(Vector3.up, hit.point);

            dragOffset =
                Current.transform.position -
                hit.point;
        }

        void HandleDragging()
        {
            if (!dragging || Current == null)
                return;

            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 point = ray.GetPoint(enter);

                // 4️⃣ Downward snap to surface below
                float maxDistance = 2000f;

                Vector3 rayStart = new Vector3(Current.transform.position.x, Current.transform.position.y, Current.transform.position.z);
                Vector3 spawnPosition = point + dragOffset;

                if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit downHit, maxDistance, ~0, QueryTriggerInteraction.Ignore))
                {
                    spawnPosition.y = downHit.collider.bounds.max.y;
                }
                else
                {
                    spawnPosition.y = 0;
                }

                Current.transform.position =
                    Vector3.Lerp(
                        Current.transform.position,
                        spawnPosition,
                        Time.deltaTime * dragSpeed);

                lastDragPosition = Current.transform.position;

                onObjectDragged?.Invoke(Current.transform.position);
            }
        }
    }
}
