using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class MatchListItem : MonoBehaviour {
	[SerializeField] private Text matchNameText;

	public delegate void JoinMatchDelegate(MatchInfoSnapshot matchInfoSnapshot);

	private JoinMatchDelegate joinMatchCallback;

	private MatchInfoSnapshot matchInfo;

	public void SetMatchInfo(MatchInfoSnapshot newMatchInfo, JoinMatchDelegate callback) {
		matchInfo = newMatchInfo;
		joinMatchCallback = callback;
		matchNameText.text = $"{newMatchInfo.name} ({newMatchInfo.currentSize}/{newMatchInfo.maxSize})";
	}

	public void JoinMatch() => joinMatchCallback.Invoke(matchInfo);
}