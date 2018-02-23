using UnityEngine;
using UnityEngine.UI;

public class KillfeedItem : MonoBehaviour {
	[SerializeField] private Text text;

	public void SetText(string newText) {
		text.text = newText;
		Destroy(gameObject, 5f);
	}
}