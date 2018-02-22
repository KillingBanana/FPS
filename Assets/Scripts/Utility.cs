using UnityEngine;

public static class Utility {
	public static void SetLayerRecursively(GameObject gameObject, int layer) {
		gameObject.layer = layer;
		foreach (Transform childTransform in gameObject.transform) {
			SetLayerRecursively(childTransform.gameObject, layer);
		}
	}
}