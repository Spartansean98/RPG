using Control;
using UnityEngine;

namespace RPG.Control
{
    interface IRaycastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController callingController);
    }
}
