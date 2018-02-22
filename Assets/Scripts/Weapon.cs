[System.Serializable]
public class Weapon {
	public string name = "Pistol";
	public bool auto;
	public ushort damage = 10;
	public float fireRate = 5;
	public float range = 100;

	public WeaponModel model;
}