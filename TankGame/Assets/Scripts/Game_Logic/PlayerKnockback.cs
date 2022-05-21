using DapperDino.Tutorials.Lobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    [SerializeField] private PlayerScript playerScript;
    public void Knockback(Vector3 origin) 
    {
        playerScript.HandleKnockback(origin);
    }
}
