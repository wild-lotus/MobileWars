using UnityEngine;
using System;
using System.Collections;
using UniRx;

namespace EfrelGames
{
	[RequireComponent (typeof(NavMeshAgent))]
	[RequireComponent (typeof(SelectableCtrl))]
	public class Movable : MonoBehaviour
	{
		#region Public fields and properties
		//======================================================================

		/// <summary>Current destination for this unit.</summary>

		private ReactiveProperty<Destination> _reactiveDest;
		public Destination Dest { 
			get { return _reactiveDest.Value; } 
			private set { _reactiveDest.Value = value; } 
		}
		public IObservable <Destination> DestObs {
			get { return _reactiveDest as IObservable<Destination>; }
		}

		#endregion


		#region Cached components
		//======================================================================

		private NavMeshAgent _agent;

		#endregion


		#region Private fields
		//======================================================================

		/// <summary>
		/// Stream checking if current destination is reached.
		/// </summary>
		private IDisposable _pathCheckStream;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Cache components
			_agent = GetComponent <NavMeshAgent> ();
			_reactiveDest = new ReactiveProperty<Destination> ();
		}

		#endregion


		#region Public methods
		//======================================================================

		/// <summary>
		/// Try to add a new destination and start the path. Succeeds if there 
		/// is no current destination with higher priority.
		/// </summary>
		/// <param name="dest">Destination to be added.</param>
		/// <returns>
		/// Observable that emits true if we have reached the destination.
		/// </returns>
		public IObservable<bool> Add (Destination dest)
		{
			if ((Dest == null || (int)Dest.Type <= (int)dest.Type) 
					&& _agent.enabled) {
//				Debug.Log ("Added destination " + dest);
				Dest = dest;
				return this.StartPath ();;
			}
			return null;
		}

		/// <summary>
		/// Remove current destination if it is the given one and stop moving.
		/// </summary>
		/// <param name="destination">Destination to be removed.</param>
		/// <returns>Whether the destionation has been removed.<returns>
		public bool Remove (Destination dest)
		{
			if (Dest != null && Dest == dest && _agent.enabled) {
				Dest = null;
				_agent.ResetPath ();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Remove the current destination if it is of the given type and stop
		/// moving.
		/// </summary>
		/// <param name="type">Destination type to be removed.</param>
		/// <returns>Whether the destionation has been removed.<returns>
		public bool Remove (DestType type)
		{
			if (Dest != null && Dest.Type == type && _agent.enabled) {
				Dest = null;
				_agent.ResetPath ();
				return true;
			}
			return false;
		}

		#endregion


		#region Private methods
		//======================================================================

		/// <summary>
		/// Starts a new path towards current destination.
		/// </summary>
		/// <returns>
		/// Observable that emits true if we have reached the destination.
		/// </returns>
		private IObservable<bool> StartPath ()
		{
			_agent.SetDestination (Dest.Position);
			_agent.stoppingDistance = Dest.Distance;

			// Observable that checks if we have reached the destination.
			IObservable<bool> pathCheckObs = Observable.EveryUpdate ()
				.TakeUntilDestroy (gameObject)
				.TakeWhile (_ =>
					Dest != null && !this.DestinationReached ()
				).ContinueWith (
					Observable.Return (1).Select (_ => Dest != null)
				);

			// Reset the path check stream
			if (_pathCheckStream != null) {
				_pathCheckStream.Dispose ();
			}
			_pathCheckStream = pathCheckObs.Subscribe (reached => {
				if (reached) {
					Debug.Log (name + " Destination reached " + Dest);
					this.Remove (Dest);
				} else {
					Debug.Log (name + " Destination removed. ");
				}
			});

			return pathCheckObs;
		}

		/// <summary>Check agent reached the destination.</summary>
		private bool DestinationReached ()
		{
			return _agent.enabled
				&& !_agent.pathPending
				&& _agent.remainingDistance <= _agent.stoppingDistance
				&& (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
		}

		#endregion
	}
}
