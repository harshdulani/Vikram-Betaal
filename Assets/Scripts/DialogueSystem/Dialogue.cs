using System;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
	public enum Character { Player, Betaal, Oldie}
	public List<string> betaalDialogues, oldieDialogues, playerDialogues;

	public string GetDialogue(Character type, int index)
	{
		var targetList = type switch
						 {
							 Character.Player => playerDialogues,
							 Character.Betaal => betaalDialogues,
							 Character.Oldie => oldieDialogues,
							 _ => throw new ArgumentOutOfRangeException(nameof (type), type, null)
						 };

		if (index > targetList.Count) return null;

		return targetList[index];
	}
}