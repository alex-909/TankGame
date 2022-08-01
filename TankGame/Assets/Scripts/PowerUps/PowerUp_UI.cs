using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp_UI : MonoBehaviour
{
    public GameObject[] imageSlots;

    public void UpdateUI(PowerUpInfo[] infos) 
    {
        foreach (GameObject slot in imageSlots) 
        {
            slot.GetComponent<Image>().enabled = false;
        }

        var qry = from s in infos
                  orderby s.powerUpType
                  select s;
        infos = qry.ToArray();

        for (int i = 0; i < infos.Length; i++) 
        {
            if ((i+2) > imageSlots.Length) { break; }

            imageSlots[i].GetComponent<Image>().enabled = true;
            imageSlots[i].GetComponent<Image>().sprite = infos[i].powerUpImage;
        }
    }
}
