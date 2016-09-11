using UnityEngine;
using System.Collections;

namespace EfrelGames
{
	/// <summary>
	/// Destination types when moving units. Its int value represents its
	/// priority level.
	/// </summary>
	public enum DestType
	{
		/// <summary>Player specifically set this destination.</summary>
		PlayerSet = 5,
		/// <summary>Player attacked enemy at this destination.</summary>
		PlayerAtt = 4,
		/// <summary>Auto attack enemy at this destination.</summary>
		AutoAtt = 3,
		/// <summary>Auto return original position after auto attack.</summary>
		AutoAttRetrun = 2
	}

	/// <summary>
	/// Destination for moving units.
	/// </summary>
	[System.Serializable]
	public class Destination
	{
		#region Public fields and properties
		//======================================================================

		/// <summary>Position to be reached.</summary>
		public Vector3 Position { get; set; }

		/// <summary>Distance from position to consider we arrived.</summary>
		public float Distance { get; set; }

		/// <summary>Destination type.</summary>
		public DestType Type { get; set; }

		#endregion


		#region Constructors
		//======================================================================

		public Destination (Vector3 position, DestType type)
			: this (position, 0, type)
		{
		}

		public Destination (Vector3 position, float distance, DestType type)
		{
			Position = position;
			Distance = distance;
			Type = type;
		}

		#endregion


		#region Object methods
		//======================================================================

		public override string ToString ()
		{
			return string.Format (
				"[Destination: Pos={0}, Dist={1}, Type={2}]",
				Position,
				Distance,
				Type
			);
		}

		#endregion
	}
}
