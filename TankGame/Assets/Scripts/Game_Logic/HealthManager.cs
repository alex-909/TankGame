using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
	public int maxHealth;
	public int currentHealth;

	private HealthBar healthBarUI;
	private HealthBar HealthBarUI
	{
		get
		{
			if (healthBarUI != null) { return healthBarUI; }
			Debug.Log("healthbar get");
			return healthBarUI = GameObject.FindGameObjectWithTag("HEALTHBAR").GetComponent<HealthBar>();
		}
	}
	public void Initialize(int maxHP)
	{
		maxHealth = maxHP;
		currentHealth = maxHP;

		HealthBarUI.SetMaxHealth(maxHP);
	}
	public void ResetHealth()
	{
		//Debug.Log($"current: {currentHealth}, max: {maxHealth}");
		currentHealth = maxHealth;
		RefreshHealthBar();
	}
	private void RefreshHealthBar()
	{
		HealthBarUI.SetHealth(currentHealth);
	}
	public void TakeDamage()
	{
		currentHealth -= 1;

		if (currentHealth < 0) currentHealth = 0;

		RefreshHealthBar();

	}
	public int GetHealth()
	{
		return currentHealth;
	}
}
