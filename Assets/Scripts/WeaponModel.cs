using UnityEngine;

public class WeaponModel : MonoBehaviour {
	[SerializeField] private ParticleSystem muzzleFlash;
	public GameObject impactPrefab;

	public void OnShoot() {
		muzzleFlash?.Play();
	}
}