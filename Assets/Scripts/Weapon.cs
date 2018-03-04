using UnityEngine;

[System.Serializable]
public class Weapon {
	public string name = "Pistol";
	public bool auto;
	public ushort damage = 10;
	[SerializeField] private float fireRate = 5;
	public float reloadTime = 2;
	[SerializeField] private int maxBullets = 10;
	
	public WeaponModel model;

	public int MaxBullets => maxBullets;

	public int Bullets { get; private set; }
	
	public float ShootCooldown => 1 / fireRate;


	public void Reload() {
		Bullets = MaxBullets;
	}

	public void Shoot() {
		Bullets--;
	}
}