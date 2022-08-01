using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public float spacing;
    public Image imagePrefab;
    public Image missileImage;
    private Image[] ammoImages;

    public void DeleteUI() 
    {
        foreach (Image img in ammoImages) 
        {
            Destroy(img.gameObject);
        }
    }
	public void CreateUI(int ammo) 
    {
        float minX = this.transform.position.x - ((ammo - 1) * spacing * 0.5f);
        ammoImages = new Image[ammo];
        for (int i = 0; i < ammo; i++) 
        {
            var img = Instantiate(imagePrefab, this.transform);
            var rect = this.GetComponent<RectTransform>();
            float x = minX + (spacing * i);
            float y = rect.position.y;
            float z = rect.position.z;
            img.rectTransform.position = new Vector3(x,y,z);
            ammoImages[i] = img;
        }
    }
    public void UpdateAmmo(int currentAmmo)
    {
        int ammoIndex = currentAmmo - 1;
        for (int i = 0; i < ammoImages.Length; i++) 
        {
            var img = ammoImages[i];
            if (i <= ammoIndex)
            {
                img.gameObject.SetActive(true);
            }
            else 
            {
                img.gameObject.SetActive(false);
            }
        }
    }
    public void UpdateMissile(int currentMissile) 
    {
        missileImage.enabled = (currentMissile > 0);
    }
}
