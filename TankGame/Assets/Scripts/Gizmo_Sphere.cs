using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo_Sphere : MonoBehaviour
{
    [SerializeField] private Color sphereColor;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}
