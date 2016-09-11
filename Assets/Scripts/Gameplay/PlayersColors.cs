using UnityEngine;
using System.Collections;

/// <summary>
/// Static access to player colors resources.
/// </summary>
public class PlayersColors
{
	/// <summary>
	/// Materials for the units.
	/// </summary>
	public static Material[] playerMats = {
		Resources.Load<Material> ("BlackPlayerMat"),
		Resources.Load<Material> ("BluePlayerMat"),
		Resources.Load<Material> ("RedPlayerMat"),
	};

	/// <summary>
	/// Materials for the selections marks.
	/// </summary>
	public static Material[] selMarkMats = {
		Resources.Load<Material> ("BlackSelMarkMat"),
		Resources.Load<Material> ("BlueSelMarkMat"),
		Resources.Load<Material> ("RedSelMarkMat"),
	};
}
