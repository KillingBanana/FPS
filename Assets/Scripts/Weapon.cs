using UnityEngine;

[System.Serializable]
public class Weapon {
	public string name = "Pistol";
	public bool auto;
	public ushort damage = 10;
	[SerializeField] private float fireRate = 5;
	public float ShootCooldown => 1 / fireRate;
	public int maxBullets = 10;
	[HideInInspector] public int bullets;
	public float reloadTime = 2;
	public WeaponModel model;

	public void Reload() {
		bullets = maxBullets;
	}
}