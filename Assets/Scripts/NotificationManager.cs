using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour {
	
	public TextMeshProUGUI notif;



	public void ShowNotif(string text) {
		notif.text = text;
	}



}