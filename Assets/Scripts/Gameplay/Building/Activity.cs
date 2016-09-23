using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class Activity {

	/// <summary>
	/// Title of this activity for the player.
	/// </summary>
	public string title;

	/// <summary>
	/// Description of this activity for the player.
	/// </summary>
	public string description;

	/// <summary>
	/// The time it takes to complete this action.
	/// </summary>
	public float duration;

	/// <summary>
	/// The cost in resources to start this activity.
	/// </summary>
	public int cost;

	/// <summary>
	/// The action happening when the activity is complete.
	/// </summary>
	public UnityAction effect;
}
