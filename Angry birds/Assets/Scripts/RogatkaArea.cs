using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RogatkaArea : MonoBehaviour
{
    [SerializeField] private LayerMask _RogatkaAreaMask;

    public bool isMouseInRogatkaArea() 
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        if (Physics2D.OverlapPoint(worldPos, _RogatkaAreaMask)) 
        {
            return true;        
        }
        else 
        {
            return false;
        }
    }
}
