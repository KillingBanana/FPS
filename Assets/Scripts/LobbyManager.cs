using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour {
	[SerializeField] private uint matchSize = 10;
	[SerializeField] private Text statusText;
	[SerializeField] private Toggle publicGameToggle;
	[SerializeField] private InputField matchNameInput;
	[SerializeField] private Transform matchListParent;
	[SerializeField] private MatchListItem matchButtonPrefab;

	private NetworkManager networkManager;

	private readonly List<MatchListItem> matchList = new List<MatchListItem>();

	private string Status {
		get { return statusText.text; }
		set { statusText.text = value; }
	}

	private void Start() {
		networkManager = NetworkManager.singleton;
		if (networkManager.matchMaker == null) networkManager.StartMatchMaker();
		FetchMatchList();
	}

	[UsedImplicitly]
	public void CreateMatch() {
		string matchName = matchNameInput.text;
		if (string.IsNullOrEmpty(matchName)) {
			Status = "Can't create match: name can't be empty";
		} else {
			Status = "Creating match...";
			networkManager.matchMaker.CreateMatch(matchName, matchSize, publicGameToggle.isOn, "", "", "", 0, 0, OnMatchCreated);
		}
	}

	private void OnMatchCreated(bool success, string extendedInfo, MatchInfo matchInfo) => networkManager.OnMatchCreate(success, extendedInfo, matchInfo);

	private void JoinMatch(MatchInfoSnapshot matchInfoSnapshot) {
		Status = "Joining match...";
		networkManager.matchMaker.JoinMatch(matchInfoSnapshot.networkId, "", "", "", 0, 0, OnMatchJoined);
	}

	private void OnMatchJoined(bool success, string extendedinfo, MatchInfo matchInfo) => networkManager.OnMatchJoined(success, extendedinfo, matchInfo);

	[UsedImplicitly]
	public void FetchMatchList() {
		ClearMatchList();
		networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchListReceived);
		Status = "Fetching match list...";
	}

	private void OnMatchListReceived(bool success, string extendedinfo, List<MatchInfoSnapshot> matchInfoList) {
		if (!success) {
			Status = "Match list could not be fetched";
		} else if (matchInfoList == null || matchInfoList.Count == 0) {
			Status = "No matches found";
		} else {
			Status = "Matches found";
			foreach (MatchInfoSnapshot matchInfoSnapshot in matchInfoList) {
				MatchListItem matchButton = Instantiate(matchButtonPrefab, matchListParent);

				matchButton.SetMatchInfo(matchInfoSnapshot, JoinMatch);

				matchList.Add(matchButton);
			}
		}
	}

	private void ClearMatchList() {
		foreach (MatchListItem match in matchList) {
			Destroy(match.gameObject);
		}

		matchList.Clear();
	}
}