using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using Control;
using UnityEditor.Analytics;
using UnityEngine.AI;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Vector2 hotspot;
            public Texture2D texture;
        }
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;
        [Range(0, 2)]
        [SerializeField] float runSpeed = 1;
        void Awake()
        {
            health = GetComponent<Health>();
        }
        void Update()
        {
            if (InteractWithUI())
            {
                SetCursor(CursorType.UI);
                return; 
            }
            if (health.IsDead())
            {
                SetCursor(CursorType.Dead);
                return;
            }
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
        private bool InteractWithComponent()
        {

            RaycastHit[] hits = RaycastAllSorted();

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }
        
        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(),raycastRadius);
            float[] distances = new float[hits.Length];
            for(int i=0;i<hits.Length;i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }
        private bool InteractWithMovement()
        {
            // RaycastHit hit;
            // bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            Vector3 target;
            bool hasHit = RayCastNavMesh(out target);

            if (hasHit)
            {
                if(!GetComponent<Mover>().CanMoveTo(target))return false;
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, runSpeed);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }
        private bool RayCastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            
            RaycastHit rayHit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out rayHit);
            if (!hasHit) return false;
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(rayHit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);

            if (!hasCastToNavMesh) return false;


            target = navMeshHit.position;

            return true;
        }

        
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }


        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture,mapping.hotspot,CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }
    }


}
