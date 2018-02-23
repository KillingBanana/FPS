using UnityEngine;

public class Killfeed : MonoBehaviour {
	private static Killfeed instance;
	[SerializeField] private KillfeedItem killfeedItemPrefab;

	private void Awake() {
		instance = this;
	}

	public static void AddKill(HitInfo hitInfo) {
		KillfeedItem killfeedItem = Instantiate(instance.killfeedItemPrefab, instance.transform);
		killfeedItem.SetText(hitInfo.KillText);
		killfeedItem.transform.SetAsFirstSibling();
	}
}