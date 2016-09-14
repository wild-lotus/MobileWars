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

		/// <summary>Current destination for this unit (reactive).</summary>
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

		private Transform _trans;
		private NavMeshAgent _agent;
		private NavMeshObstacle _obstacle;

		#endregion


		#region Private fields
		//======================================================================

		/// <summary>
		/// Stream checking if current destination is reached.
		/// </summary>
		private IDisposable _pathCheckStream;
		/// <summary>
		/// Observable that checks if we have reached the destination.
		/// </summary>
		IObservable<bool> _pathCheckObs;
		/// <summary>
		/// Observable that start a path after NavMesh is available and then
		/// checks if we have reached the destination.
		/// </summary>
		IObservable<bool> _delayedStrartPathCheckObs;
		

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Cache components
			_trans = transform;
			_agent = GetComponent <NavMeshAgent> ();
			_obstacle = GetComponent <NavMeshObstacle> ();
			_reactiveDest = new ReactiveProperty<Destination> ();

			// Prepare observables
			_pathCheckObs = Observable.EveryUpdate ()
				.TakeUntilDestroy (gameObject)
				.TakeWhile (_ => Dest != null && !this.DestinationReached ())
				.ContinueWith (Observable.Return (Dest != null));
			
			_delayedStrartPathCheckObs = Observable.EveryUpdate()
				.Where(_ => {
					NavMeshHit hit;
					return NavMesh.SamplePosition(
							_trans.position, out hit, 0.01f, NavMesh.AllAreas
					);
				})
				.First()
				.Do (_ => {
					_agent.enabled = true;
					_agent.SetDestination (Dest.Position);
				})
				.ContinueWith (_pathCheckObs);
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
			if ((Dest == null || (int)Dest.Type <= (int)dest.Type)) {
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
			if (Dest != null && Dest == dest) {
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
			if (Dest != null && Dest.Type == type) {
				Dest = null;
				_agent.ResetPath ();
				return true;
			}
			return false;
		}

		#endregion


		#region Private methods
		//======================================================================

		/// <summary>Check agent reached the destination.</summary>
		private bool DestinationReached ()
		{
			return _agent.enabled
				&& !_agent.pathPending
				&& _agent.remainingDistance <= _agent.stoppingDistance
				&& (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
		}
			
		/// <summary>
		/// Starts a new path towards current destination. It deals with a
		/// combination of NavMeshAgent and NavMeshObstacle. It is required a
		/// delayed start of the path if obstable is enabled, so that 
		/// NavMeshAgent is on the NavMesh.
		/// </summary>
		/// <returns>
		/// Observable that emits true if we have reached the destination.
		/// </returns>
		private IObservable<bool> StartPath ()
		{
			IObservable<bool> pathCheckObs;
						
			if (_agent.isOnNavMesh) {
				// Start path and set regular path check observable.
				_agent.SetDestination (Dest.Position);
				pathCheckObs = _pathCheckObs;

			} else {
				// Disable obstacle and set path check observable that starts
				// the path when possible.
				_obstacle.enabled = false;
				pathCheckObs = _delayedStrartPathCheckObs;
			}

			if (_pathCheckStream == null) {
				// Subsribe to path check
				_pathCheckStream = pathCheckObs
					.Subscribe (reached => {
						if (reached) {
							Debug.Log (name + " Destination reached " + Dest);
							this.Remove (Dest);
						} else {
							Debug.Log (name + " Destination removed.");
						}
						// Enable the obstable again and clear the path check
						// stream.
						_agent.enabled = false;
						_obstacle.enabled = true;
						_pathCheckStream.Dispose();
						_pathCheckStream = null;
					});
			}

			_agent.stoppingDistance = Dest.Distance;

			return pathCheckObs;
		}

		#endregion
	}
}
