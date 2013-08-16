#region File Description
//===================================================================
// DPSF.cs
// 
// This is the base class for the Dynamic Particle System Framework. This
// class should be inherited by another Particle System class, and 
// the provided functions will take care of Particle management.
//
// If you want DPSF Particle Systems to run as DrawableGameComponents,
// add the symbol "DPSFAsDrawableGameComponent" to the Conditional
// Compilation Symbols in the Build options in the Project Properties.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;       // Used for Conditional Attributes and Stopwatches.
using DPSF.Exceptions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF
{
	/// <summary>
	/// The Base Particle System Framework Class.
	/// This class contains the methods and properties needed to keep track of, update, and draw Particles
	/// </summary>
	/// <typeparam name="Particle">The Particle class used to hold a particle's information. The Particle class
	/// specified must be or inherit from the DPSFParticle class</typeparam>
	/// <typeparam name="Vertex">The Particle Vertex struct used to hold a vertex's information used for drawing</typeparam>
#if (WINDOWS)
	[Serializable]
#endif
#if (DPSFAsDrawableGameComponent)
	public class DPSF<Particle, Vertex> : DrawableGameComponent, IDPSFParticleSystem 
		where Particle : DPSFParticle, new()
		where Vertex : struct, IDPSFParticleVertex
#else
	public abstract class DPSF<Particle, Vertex> : IDPSFParticleSystem
		where Particle : DPSFParticle, new()
		where Vertex : struct, IDPSFParticleVertex
#endif
	{
		#region Delegate Prototypes and Class Declarations

		/// <summary>
		/// The function prototype that Particle System Events must follow
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How much time in seconds has elapsed since the last update</param>
		public delegate void UpdateParticleSystemDelegate(float fElapsedTimeInSeconds);

		/// <summary>
		/// The function prototype that the Particle Events must follow
		/// </summary>
		/// <param name="cParticle">The Particle to be updated</param>
		/// <param name="fElapsedTimeInSeconds">How much time in seconds has elapsed since the last update</param>
		public delegate void UpdateParticleDelegate(Particle cParticle, float fElapsedTimeInSeconds);

		/// <summary>
		/// The function prototype that the Particle Initialization Functions must follow
		/// </summary>
		/// <param name="cParticle">The Particle to be initialized</param>
		public delegate void InitializeParticleDelegate(Particle cParticle);

		/// <summary>
		/// The function prototype that the Vertex Update Functions must follow
		/// </summary>
		/// <param name="sParticleVertexBuffer">The vertex buffer array</param>
		/// <param name="iIndexInVertexBuffer">The index in the vertex buffer that the Particle properties should be written to</param>
		/// <param name="cParticle">The Particle whose properties should be copied to the vertex buffer</param>
		public delegate void UpdateVertexDelegate(ref Vertex[] sParticleVertexBuffer, int iIndexInVertexBuffer, Particle cParticle);
		
		/// <summary>
		/// Class to hold all of the Particle Events
		/// </summary>
	#if (WINDOWS)
		[Serializable]
	#endif
		public class CParticleEvents
		{
			/// <summary>
			/// The Particle Event Types
			/// </summary>
		#if (WINDOWS)
			[Serializable]
		#endif
			enum EParticleEventTypes
			{
				EveryTime = 0,
				OneTime = 1,
				Timed = 2,
				NormalizedTimed = 3
			}

			/// <summary>
			/// Class to hold a Particle Event's information
			/// </summary>
		#if (WINDOWS)
			[Serializable]
		#endif
			class CParticleEvent
			{
				public UpdateParticleDelegate cFunctionToCall;  // The Function to call when the Event fires
				public EParticleEventTypes eType;               // The Type of Event this is
				public int iExecutionOrder;                     // The Order, relative to other Events, of when this Event should Execute (i.e. Fire)
				public int iGroup;                              // The Group that this Event belongs to

				/// <summary>
				/// Explicit Constructor
				/// </summary>
				/// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
				/// <param name="_eType">The Type of Event this is</param>
				/// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should be Fired</param>
				/// <param name="_iGroup">The Group this Event should belong to</param>
				public CParticleEvent(UpdateParticleDelegate _cFunctionToCall, EParticleEventTypes _eType, int _iExecutionOrder, int _iGroup)
				{
					cFunctionToCall = _cFunctionToCall;
					eType = _eType;
					iExecutionOrder = _iExecutionOrder;
					iGroup = _iGroup;
				}

				/// <summary>
				/// Overload the == operator to test for value equality
				/// </summary>
				/// <returns>Returns true if the structures have the same values, false if not</returns>
				public static bool operator ==(CParticleEvent c1, CParticleEvent c2)
				{
					if (c1.cFunctionToCall == c2.cFunctionToCall &&
						c1.iExecutionOrder == c2.iExecutionOrder && 
						c1.eType == c2.eType && c1.iGroup == c2.iGroup)
					{
						// Return that the structures have the same values
						return true;
					}
					// Return that the structures have different values
					return false;
				}

				/// <summary>
				/// Overload the != operator to test for value equality
				/// </summary>
				/// <returns>Returns true if the structures do not have the same values, false if they do</returns>
				public static bool operator !=(CParticleEvent c1, CParticleEvent c2)
				{
					return !(c1 == c2);
				}

				/// <summary>
				/// Override the Equals method
				/// </summary>
				public override bool Equals(object obj)
				{
					if (!(obj is CParticleEvent))
					{
						return false;
					}
					return this == (CParticleEvent)obj;
				}

				/// <summary>
				/// Override the GetHashCode method
				/// </summary>
				public override int GetHashCode()
				{
					return iExecutionOrder;
				}
			}

			/// <summary>
			/// Class to hold a Timed Particle Event's information
			/// </summary>
		#if (WINDOWS)
			[Serializable]
		#endif
			class CTimedParticleEvent : CParticleEvent
			{
				public float fTimeToFire;   // The Time at which this Event should Fire

				/// <summary>
				/// Explicit Constructor
				/// </summary>
				/// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
				/// <param name="_eTimedType">The Type of Timed Event this is (Timed or NormalizedTimed)</param>
				/// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should Fire</param>
				/// <param name="_iGroup">The Group this Event should belong to</param>
				/// <param name="_fTimeToFire">The Time at which this Event should Fire</param>
				public CTimedParticleEvent(UpdateParticleDelegate _cFunctionToCall, EParticleEventTypes _eTimedType, 
										   int _iExecutionOrder, int _iGroup, float _fTimeToFire)
					: base(_cFunctionToCall, _eTimedType, _iExecutionOrder, _iGroup)
				{
					fTimeToFire = _fTimeToFire;
				}

				/// <summary>
				/// Overload the == operator to test for value equality
				/// </summary>
				/// <returns>Returns true if the structures have the same values, false if not</returns>
				public static bool operator ==(CTimedParticleEvent c1, CTimedParticleEvent c2)
				{
					if (c1.cFunctionToCall == c2.cFunctionToCall &&
						c1.iExecutionOrder == c2.iExecutionOrder && 
						c1.eType == c2.eType && c1.iGroup == c2.iGroup &&
						c1.fTimeToFire == c2.fTimeToFire)
					{
						// Return that the structures have the same values
						return true;
					}
					// Return that the structures have different values
					return false;
				}

				/// <summary>
				/// Overload the != operator to test for value equality
				/// </summary>
				/// <returns>Returns true if the structures do not have the same values, false if they do</returns>
				public static bool operator !=(CTimedParticleEvent c1, CTimedParticleEvent c2)
				{
					return !(c1 == c2);
				}

				/// <summary>
				/// Override the Equals method
				/// </summary>
				public override bool Equals(object obj)
				{
					if (!(obj is CTimedParticleEvent))
					{
						return false;
					}
					return this == (CTimedParticleEvent)obj;
				}

				/// <summary>
				/// Override the GetHashCode method
				/// </summary>
				public override int GetHashCode()
				{
					return iExecutionOrder;
				}
			}

			// Function used to sort the Events by their Execution Order
			private int EventSorter(CParticleEvent c1, CParticleEvent c2)
			{
				return c1.iExecutionOrder.CompareTo(c2.iExecutionOrder);
			}

			// List to hold all of the Events
			private List<CParticleEvent> mcParticleEventList = new List<CParticleEvent>();


			/// <summary>
			/// Adds a new EveryTime Event with a default Execution Order and Group of zero. 
			/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			public void AddEveryTimeEvent(UpdateParticleDelegate cFunctionToCall)
			{
				// Add the new EveryTime Event
				AddEveryTimeEvent(cFunctionToCall, 0, 0);
			}

			/// <summary>
			/// Adds a new EveryTime Event with a default Group of zero. 
			/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute. 
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
			/// order they are added.</para></param>
			public void AddEveryTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
			{
				// Add the new EveryTime Event
				AddEveryTimeEvent(cFunctionToCall, iExecutionOrder, 0);
			}

			/// <summary>
			/// Adds a new EveryTime Event. 
			/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
			/// order they are added.</para></param>
			/// <param name="iGroup">The Group that this Event should belong to</param>
			public void AddEveryTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
			{
				// Add the Event to the Events List and re-sort it
				mcParticleEventList.Add(new CParticleEvent(cFunctionToCall, EParticleEventTypes.EveryTime, iExecutionOrder, iGroup));
				mcParticleEventList.Sort(EventSorter);
			}

			/// <summary>
			/// Removes all EveryTime Events with the specified Function.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="cFunction">The Function of the EveryTime Event to remove</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveEveryTimeEvents(UpdateParticleDelegate cFunction)
			{
				// Remove all EveryTime Events with the specified Function
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Removes all EveryTime Events in the specified Group.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="iGroup">The Group to remove the EveryTime Events from</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveEveryTimeEvents(int iGroup)
			{
				// Remove all EveryTime Events that belong to the specified Group
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.EveryTime && cEvent.iGroup == iGroup); });
			}

			/// <summary>
			/// Removes an EveryTime Event with the specified Function, Execution Order, and Group.
			/// Returns true if the Event was found and removed, false if not.
			/// </summary>
			/// <param name="cFunction">The Function of the EveryTime Event to remove</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
			/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
			/// <returns>Returns true if the Event was found and removed, false if not.</returns>
			public bool RemoveEveryTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Remove the specified Event
				return mcParticleEventList.Remove(new CParticleEvent(cFunction, EParticleEventTypes.EveryTime, iExecutionOrder, iGroup));
			}

			/// <summary>
			/// Removes all EveryTime Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllEveryTimeEvents()
			{
				// Remove all Events with a Particle Event Type of EveryTime
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return cEvent.eType == EParticleEventTypes.EveryTime; });
			}

			/// <summary>
			/// Returns if there is an EveryTime Event with the specified Function or not.
			/// </summary>
			/// <param name="cFunction">The Function of the EveryTime Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
			public bool ContainsEveryTimeEvent(UpdateParticleDelegate cFunction)
			{
				// Return if there is an EveryTime Event with the specified Function
				return mcParticleEventList.Exists(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Returns if there is an EveryTime Event with the specifed Function, Execution Order, and Group or not.
			/// </summary>
			/// <param name="cFunction">The Function of the EveryTime Event to look for</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
			/// <param name="iGroup">The Group of the Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
			public bool ContainsEveryTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Return if there is an EveryTime Event with the specified criteria or not
				return mcParticleEventList.Contains(new CParticleEvent(cFunction, EParticleEventTypes.EveryTime, iExecutionOrder, iGroup));
			}

			/// <summary>
			/// Adds a new OneTime Event with a default Execution Order and Group of zero. 
			/// OneTime Events fire once then are automatically removed.
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			public void AddOneTimeEvent(UpdateParticleDelegate cFunctionToCall)
			{
				// Add the new OneTime Event with default Execution Order
				AddOneTimeEvent(cFunctionToCall, 0, 0);
			}

			/// <summary>
			/// Adds a new OneTime Event with a default Group of zero. 
			/// OneTime Events fire once then are automatically removed.
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
			/// order they are added.</para></param>
			public void AddOneTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
			{
				// Add the new OneTime Event with default Execution Order
				AddOneTimeEvent(cFunctionToCall, iExecutionOrder, 0);
			}

			/// <summary>
			/// Adds a new OneTime Event. 
			/// OneTime Events fire once then are automatically removed.
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
			/// order they are added.</para></param>
			/// <param name="iGroup">The Group that this Event should belong to</param>
			public void AddOneTimeEvent(UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
			{
				// Add the OneTime Event to the List and re-sort the List
				mcParticleEventList.Add(new CParticleEvent(cFunctionToCall, EParticleEventTypes.OneTime, iExecutionOrder, iGroup));
				mcParticleEventList.Sort(EventSorter);
			}

			/// <summary>
			/// Removes all OneTime Events with the specified Function.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="cFunction">The Function of the OneTime Event to remove</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveOneTimeEvents(UpdateParticleDelegate cFunction)
			{
				// Remove all OneTime Events with the specified Function
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.OneTime && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Removes all OneTime Events in the specified Group.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="iGroup">The Group to remove the OneTime Events from</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveOneTimeEvents(int iGroup)
			{
				// Remove all OneTime Events that belong to the specified Group
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.OneTime && cEvent.iGroup == iGroup); });
			}

			/// <summary>
			/// Removes a OneTime Event with the specified Function To Call, Execution Order, and Group.
			/// Returns true if the Event was found and removed, false if not.
			/// </summary>
			/// <param name="cFunction">The Function of the OneTime Event to remove</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
			/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
			/// <returns>Returns true if the Event was found and removed, false if not.</returns>
			public bool RemoveOneTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Remove the specified Event from the Event List
				return mcParticleEventList.Remove(new CParticleEvent(cFunction, EParticleEventTypes.OneTime, iExecutionOrder, iGroup));
			}

			/// <summary>
			/// Removes all OneTime Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllOneTimeEvents()
			{
				// Remove all Events with a Particle Event Type of OneTime
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return cEvent.eType == EParticleEventTypes.OneTime; });
			}

			/// <summary>
			/// Returns if there is an OneTime Event with the specified Function or not.
			/// </summary>
			/// <param name="cFunction">The Function of the OneTime Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
			public bool ContainsOneTimeEvent(UpdateParticleDelegate cFunction)
			{
				// Return if there is an OneTime Event with the specified Function
				return mcParticleEventList.Exists(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.OneTime && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Returns if there is an OneTime Event with the specifed Function, Execution Order, and Group or not.
			/// </summary>
			/// <param name="cFunction">The Function of the OneTime Event to look for</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
			/// <param name="iGroup">The Group of the Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
			public bool ContainsOneTimeEvent(UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Return if there is an OneTime Event with the specified criteria or not
				return mcParticleEventList.Contains(new CParticleEvent(cFunction, EParticleEventTypes.OneTime, iExecutionOrder, iGroup));
			}

			/// <summary>
			/// Adds a new Timed Event with a default Execution Order and Group of zero. 
			/// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fTimeToFire">The Time when the Event should fire
			/// (i.e. when the Function should be called)</param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			public void AddTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunctionToCall)
			{
				// Add the Timed Event with default Execution Order
				AddTimedEvent(fTimeToFire, cFunctionToCall, 0, 0);   
			}

			/// <summary>
			/// Adds a new Timed Event with a default Group of zero. 
			/// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fTimeToFire">The Time when the Event should fire
			/// (i.e. when the Function should be called)</param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute. 
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
			/// order they are added.</para></param>
			public void AddTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
			{
				// Add the Timed Event with default Execution Order
				AddTimedEvent(fTimeToFire, cFunctionToCall, iExecutionOrder, 0);
			}

			/// <summary>
			/// Adds a new Timed Event. 
			/// Timed Events fire when the Particle's Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fTimeToFire">The Time when the Event should fire
			/// (i.e. when the Function should be called)</param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
			/// order they are added.</para></param>
			/// <param name="iGroup">The Group that this Event should belong to</param>
			public void AddTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
			{
				// Add the Timed Event to the Event List and re-sort the List
				mcParticleEventList.Add(new CTimedParticleEvent(cFunctionToCall, EParticleEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
				mcParticleEventList.Sort(EventSorter);
			}

			/// <summary>
			/// Removes all Timed Events with the specified Function.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="cFunction">The Function that is called when the Event fires</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveTimedEvents(UpdateParticleDelegate cFunction)
			{
				// Remove all Timed Events with the specified Function
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.Timed && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Removes all Timed Events in the specified Group.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="iGroup">The Group to remove the Timed Events from</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveTimedEvents(int iGroup)
			{
				// Remove all Timed Events that belong to the specified Group
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.Timed && cEvent.iGroup == iGroup); });
			}

			/// <summary>
			/// Removes a Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
			/// Returns true if the Event was found and removed, false if not.
			/// </summary>
			/// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
			/// <param name="cFunction">The Function that is called when the Event fires</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
			/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
			/// <returns>Returns true if the Event was found and removed, false if not.</returns>
			public bool RemoveTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Remove the Timed Event from the Event List
				return mcParticleEventList.Remove(new CTimedParticleEvent(cFunction, EParticleEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
			}

			/// <summary>
			/// Removes all Timed Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllTimedEvents()
			{
				// Remove all Events with a Particle Event Type of Timed
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return cEvent.eType == EParticleEventTypes.Timed; });
			}

			/// <summary>
			/// Returns if there is a Timed Event with the specified Function or not.
			/// </summary>
			/// <param name="cFunction">The Function of the Timed Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
			public bool ContainsTimedEvent(UpdateParticleDelegate cFunction)
			{
				// Return if there is a Timed Event with the specified Function
				return mcParticleEventList.Exists(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.Timed && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Returns if there is a Timed Event with the specifed Timed To Fire, Function, Execution Order, and Group or not.
			/// </summary>
			/// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
			/// <param name="cFunction">The Function of the Timed Event to look for</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
			/// <param name="iGroup">The Group of the Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
			public bool ContainsTimedEvent(float fTimeToFire, UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Return if there is a Timed Event with the specified criteria or not
				return mcParticleEventList.Contains(new CTimedParticleEvent(cFunction, EParticleEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
			}

			/// <summary>
			/// Adds a new Normalized Timed Event with a default Execution Order and Group of zero. 
			/// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
			/// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunctionToCall)
			{
				// Add the Normalized Timed Event with default Execution Order
				AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, 0, 0);                
			}

			/// <summary>
			/// Adds a new Normalized Timed Event with a default Group of zero. 
			/// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
			/// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute. NOTE: Events with lower Execution Order are executed first. NOTE: Events
			/// with the same Execution Order are not guaranteed to be executed in the order they are added.</param>
			public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder)
			{
				// Add the Normalized Timed Event with default Execution Order
				AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, iExecutionOrder, 0);
			}

			/// <summary>
			/// Adds a new Normalized Timed Event. 
			/// Normalized Timed Events fire when the Particle's Normalized Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire
			/// (compared against the Particle's Normalized Elapsed Time). NOTE: This is clamped to the range of 0.0 - 1.0</param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the 
			/// order they are added.</para></param>
			/// <param name="iGroup">The Group that this Event should belong to</param>
			public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
			{
				// Clamp the NormalizedTimeToFire between 0.0 and 1.0
				fNormalizedTimeToFire = MathHelper.Clamp(fNormalizedTimeToFire, 0.0f, 1.0f);

				// Add the Normalized Timed Event to the Event List and re-sort the List
				mcParticleEventList.Add(new CTimedParticleEvent(cFunctionToCall, EParticleEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
				mcParticleEventList.Sort(EventSorter);
			}

			/// <summary>
			/// Removes all Normalized Timed Events with the specified Function.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="cFunction">The Function that is called when the Event fires</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveNormalizedTimedEvents(UpdateParticleDelegate cFunction)
			{
				// Remove all Normalized Timed Events with the specified Function
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Removes all Normalized Timed Events in the specified Group.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="iGroup">The Group to remove the Normalized Timed Events from</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveNormalizedTimedEvents(int iGroup)
			{
				// Remove all Normalized Timed Events that belong to the specified Group
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.NormalizedTimed && cEvent.iGroup == iGroup); });
			}

			/// <summary>
			/// Removes a Normalized Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
			/// Returns true if the Event was found and removed, false if not.
			/// </summary>
			/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
			/// <param name="cFunction">The Function that is called when the Event fires</param>
			/// <param name="iExectionOrder">The Execution Order of the Event to remove. Default is zero.</param>
			/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
			/// <returns>Returns true if the Event was found and removed, false if not.</returns>
			public bool RemoveNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunction, int iExectionOrder, int iGroup)
			{
				// Remove the Normalized Timed Event from the Event List
				return mcParticleEventList.Remove(new CTimedParticleEvent(cFunction, EParticleEventTypes.NormalizedTimed, iExectionOrder, iGroup, fNormalizedTimeToFire));
			}

			/// <summary>
			/// Removes all Normalized Timed Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllNormalizedTimedEvents()
			{
				// Remove all Events with a Particle Event Type of NormalizedTimed
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return cEvent.eType == EParticleEventTypes.NormalizedTimed; });
			}

			/// <summary>
			/// Returns if there is a NormalizedTimed Event with the specified Function or not.
			/// </summary>
			/// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
			public bool ContainsNormalizedTimedEvent(UpdateParticleDelegate cFunction)
			{
				// Return if there is a NormalizedTimed Event with the specified Function
				return mcParticleEventList.Exists(delegate(CParticleEvent cEvent)
				{ return (cEvent.eType == EParticleEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Returns if there is a NormalizedTimed Event with the specifed Normalized Time To Fire, Function, Execution Order, and Group or not.
			/// </summary>
			/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
			/// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
			/// <param name="iGroup">The Group of the Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
			public bool ContainsNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Return if there is a NormalizedTimed Event with the specified criteria or not
				return mcParticleEventList.Contains(new CTimedParticleEvent(cFunction, EParticleEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
			}

			/// <summary>
			/// Removes all Timed Events and Normalized Timed Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllTimedAndNormalizedTimedEvents()
			{
				return (RemoveAllTimedEvents() + RemoveAllNormalizedTimedEvents());
			}

			/// <summary>
			/// Removes all Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllEvents()
			{
				// Store the Number of Events that were in the Event List
				int iNumberOfEvents = mcParticleEventList.Count;

				// Remove all Events from the Event List
				mcParticleEventList.Clear();

				// Return the Number of Events removed
				return iNumberOfEvents;
			}

			/// <summary>
			/// Removes all Events in the specified Group.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="iGroup">The Group to remove all Events from</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllEventsInGroup(int iGroup)
			{
				// Remove all Events that belong to the specified Group
				return mcParticleEventList.RemoveAll(delegate(CParticleEvent cEvent)
				{ return (cEvent.iGroup == iGroup); });
			}

			/// <summary>
			/// Updates the given Particle according to the Particle Events. This is called automatically
			/// every frame by the Particle System.
			/// </summary>
			/// <param name="cParticle">The Particle to update</param>
			/// <param name="fElapsedTimeInSeconds">The amount of Time Elapsed since the last Update</param>
			public void Update(Particle cParticle, float fElapsedTimeInSeconds)
			{
				// If there are no Events to process
				if (mcParticleEventList.Count <= 0)
				{
					// Exit without doing anything
					return;
				}

				// Loop through all of the Events (which are pre-sorted by Execution Order).
				// We check against mcParticleEventList.Count instead of storing the value in 
				// a local variable incase an Event removes other Events (may throw an 
				// index out of range exception if this happens).
				for (int iIndex = 0; iIndex < mcParticleEventList.Count; iIndex++)
				{
					// If this is an EveryTime or OneTime Event
					if (mcParticleEventList[iIndex].eType == EParticleEventTypes.EveryTime ||
						mcParticleEventList[iIndex].eType == EParticleEventTypes.OneTime)
					{
						// Execute the Event's function
						mcParticleEventList[iIndex].cFunctionToCall(cParticle, fElapsedTimeInSeconds);
					}
					// Else this is a Timed Event
					else
					{
						// Get a handle to the Timed Event
						CTimedParticleEvent cTimedEvent = (CTimedParticleEvent)mcParticleEventList[iIndex];

						// If this is a normal Timed Event
						if (cTimedEvent.eType == EParticleEventTypes.Timed)
						{
							// Store the Last Elapsed Time of the Particle. 
							// If it's zero we need to change it to be before zero so that any Events set to fire at time zero will be fired.
							float fLastElapsedTime = (cParticle.LastElapsedTime == 0.0f) ? -0.1f : cParticle.LastElapsedTime;

							// If this Event should fire
							if (fLastElapsedTime < cTimedEvent.fTimeToFire &&
								cParticle.ElapsedTime >= cTimedEvent.fTimeToFire)
							{
								// Execute the Event's function
								cTimedEvent.cFunctionToCall(cParticle, fElapsedTimeInSeconds);
							}
						}
						// Else this is a Normalized Timed Event
						else
						{
							// Store the Last Normalized Elapsed Time of the Particle. 
							// If it's zero we need to change it to be before zero so that any Events set to fire at time zero will be fired.
							float fLastNormalizedElapsedTime = (cParticle.LastNormalizedElapsedTime == 0.0f) ? -0.1f : cParticle.LastNormalizedElapsedTime;

							// If this Event should fire
							if (fLastNormalizedElapsedTime < cTimedEvent.fTimeToFire &&
								cParticle.NormalizedElapsedTime >= cTimedEvent.fTimeToFire)
							{
								// Fire the Event
								cTimedEvent.cFunctionToCall(cParticle, fElapsedTimeInSeconds);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Class to hold all of the Particle System Events and related info
		/// </summary>
	#if (WINDOWS)
		[Serializable]
	#endif
		public class CParticleSystemEvents
		{
			/// <summary>
			/// The Particle System Event Types
			/// </summary>
		#if (WINDOWS)
			[Serializable]
		#endif
			enum EParticleSystemEventTypes
			{
				EveryTime = 0,
				OneTime = 1,
				Timed = 2,
				NormalizedTimed = 3
			}

			/// <summary>
			/// Class to hold a Particle System Event's information
			/// </summary>
		#if (WINDOWS)
			[Serializable]
		#endif
			class CParticleSystemEvent
			{
				public UpdateParticleSystemDelegate cFunctionToCall;    // The Function to call when the Event fires
				public EParticleSystemEventTypes eType;                 // The Type of Event this is
				public int iExecutionOrder;                             // The Order, relative to other Events, of when this Event should Execute (i.e. Fire)
				public int iGroup;                                      // The Group the Event belongs to

				/// <summary>
				/// Explicit Constructor
				/// </summary>
				/// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
				/// <param name="_eType">The Type of Event this is</param>
				/// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should be Fired</param>
				/// <param name="_iGroup">The Group this Event should belong to</param>
				public CParticleSystemEvent(UpdateParticleSystemDelegate _cFunctionToCall, EParticleSystemEventTypes _eType, int _iExecutionOrder, int _iGroup)
				{
					cFunctionToCall = _cFunctionToCall;
					eType = _eType;
					iExecutionOrder = _iExecutionOrder;
					iGroup = _iGroup;
				}

				/// <summary>
				/// Overload the == operator to test for value equality
				/// </summary>
				/// <returns>Returns true if the structures have the same values, false if not</returns>
				public static bool operator ==(CParticleSystemEvent c1, CParticleSystemEvent c2)
				{
					if (c1.cFunctionToCall == c2.cFunctionToCall &&
						c1.iExecutionOrder == c2.iExecutionOrder &&
						c1.eType == c2.eType && c1.iGroup == c2.iGroup)
					{
						// Return that the structures have the same values
						return true;
					}
					// Return that the structures have different values
					return false;
				}

				/// <summary>
				/// Overload the != operator to test for value equality
				/// </summary>
				/// <returns>Returns true if the structures do not have the same values, false if they do</returns>
				public static bool operator !=(CParticleSystemEvent c1, CParticleSystemEvent c2)
				{
					return !(c1 == c2);
				}

				/// <summary>
				/// Override the Equals method
				/// </summary>
				public override bool Equals(object obj)
				{
					if (!(obj is CParticleSystemEvent))
					{
						return false;
					}
					return this == (CParticleSystemEvent)obj;
				}

				/// <summary>
				/// Override the GetHashCode method
				/// </summary>
				public override int GetHashCode()
				{
					return iExecutionOrder;
				}
			}

			/// <summary>
			/// Class to hold a Timed Particle System Event's information
			/// </summary>
		#if (WINDOWS)
			[Serializable]
		#endif
			class CTimedParticleSystemEvent : CParticleSystemEvent
			{
				public float fTimeToFire;   // The Time at which this Event should Fire

				/// <summary>
				/// Explicit Constructor
				/// </summary>
				/// <param name="_cFunctionToCall">The Function the Event should call when it Fires</param>
				/// <param name="_eTimedType">The Type of Timed Event this is (Timed or NormalizedTimed)</param>
				/// <param name="_iExecutionOrder">The Order, relative to other Events, of when this Event should Fire</param>
				/// <param name="_iGroup">The Group this Event should belong to</param>
				/// <param name="_fTimeToFire">The Time at which this Event should Fire</param>
				public CTimedParticleSystemEvent(UpdateParticleSystemDelegate _cFunctionToCall, EParticleSystemEventTypes _eTimedType,
												 int _iExecutionOrder, int _iGroup, float _fTimeToFire)
					: base(_cFunctionToCall, _eTimedType, _iExecutionOrder, _iGroup)
				{
					fTimeToFire = _fTimeToFire;
				}

				/// <summary>
				/// Overload the == operator to test for value equality
				/// </summary>
				/// <returns>Returns true if the structures have the same values, false if not</returns>
				public static bool operator ==(CTimedParticleSystemEvent c1, CTimedParticleSystemEvent c2)
				{
					if (c1.cFunctionToCall == c2.cFunctionToCall &&
						c1.iExecutionOrder == c2.iExecutionOrder &&
						c1.eType == c2.eType && c1.iGroup == c2.iGroup &&
						c1.fTimeToFire == c2.fTimeToFire)
					{
						// Return that the structures have the same values
						return true;
					}
					// Return that the structures have different values
					return false;
				}

				/// <summary>
				/// Overload the != operator to test for value equality
				/// </summary>
				/// <returns>Returns true if the structures do not have the same values, false if they do</returns>
				public static bool operator !=(CTimedParticleSystemEvent c1, CTimedParticleSystemEvent c2)
				{
					return !(c1 == c2);
				}

				/// <summary>
				/// Override the Equals method
				/// </summary>
				public override bool Equals(object obj)
				{
					if (!(obj is CTimedParticleSystemEvent))
					{
						return false;
					}
					return this == (CTimedParticleSystemEvent)obj;
				}

				/// <summary>
				/// Override the GetHashCode method
				/// </summary>
				public override int GetHashCode()
				{
					return iExecutionOrder;
				}
			}

			/// <summary>
			/// The Options of what should happen when the Particle System reaches the end of its Lifetime
			/// </summary>
		#if (WINDOWS)
			[Serializable]
		#endif
			public enum EParticleSystemEndOfLifeOptions
			{
				/// <summary>
				/// When the Particle System reaches the end of its Lifetime nothing special happens; It just
				/// continues to operate as normal.
				/// </summary>
				Nothing = 0,

				/// <summary>
				/// When the Particle System reaches the end of its Lifetime its Elapsed Time is reset to zero,
				/// so that all of the Timed Events will be repeated again.
				/// </summary>
				Repeat = 1,

				/// <summary>
				/// When the Particle System reaches the end of its Lifetime it calls its Destroy() function, so
				/// the Particle System releases all its resources and is no longer updated or drawn.
				/// </summary>
				Destroy = 2
			}

			/// <summary>
			/// Class to hold the Lifetime information of the Particle System
			/// </summary>
		#if (WINDOWS)
			[Serializable]
		#endif
			public class CParticleSystemLifetimeData : DPSFParticle
			{
				private EParticleSystemEndOfLifeOptions meEndOfLifeOption;  // Tells what should happen when the Particle System reaches its Lifetime

				/// <summary>
				/// Get / Set what should happen when the Particle System reaches the end of its Lifetime
				/// </summary>
				public EParticleSystemEndOfLifeOptions EndOfLifeOption
				{
					get { return meEndOfLifeOption; }
					set { meEndOfLifeOption = value; }
				}

				/// <summary>
				/// Resets the class variables to their default values
				/// </summary>
				public override void Reset()
				{
					base.Reset();
					EndOfLifeOption = EParticleSystemEndOfLifeOptions.Nothing;
				}

				/// <summary>
				/// Deep copy the ParticleToCopy's values into this Particle
				/// </summary>
				/// <param name="ParticleToCopy">The Particle whose values should be Copied</param>
				public override void CopyFrom(DPSFParticle ParticleToCopy)
				{
					CParticleSystemLifetimeData cParticleToCopy = (CParticleSystemLifetimeData)ParticleToCopy;
					base.CopyFrom(cParticleToCopy);
					EndOfLifeOption = cParticleToCopy.EndOfLifeOption;
				}
			}

			// Function used to sort the Events by their Execution Order
			private int EventSorter(CParticleSystemEvent c1, CParticleSystemEvent c2)
			{
				return c1.iExecutionOrder.CompareTo(c2.iExecutionOrder);
			}

			// List to hold all of the Events
			private List<CParticleSystemEvent> mcParticleSystemEventList = new List<CParticleSystemEvent>();

			// Variable to hold the Lifetime Data of the Particle System
			private CParticleSystemLifetimeData mcParticleSystemLifetimeData = new CParticleSystemLifetimeData();

			/// <summary>
			/// Get / Set the Lifetime information of the Particle System
			/// </summary>
			public CParticleSystemLifetimeData LifetimeData
			{
				get { return mcParticleSystemLifetimeData; }
				set { mcParticleSystemLifetimeData = value; }
			}

			/// <summary>
			/// Adds a new EveryTime Event with a default Execution Order and Group of zero. 
			/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			public void AddEveryTimeEvent(UpdateParticleSystemDelegate cFunctionToCall)
			{
				// Add the new EveryTime Event
				AddEveryTimeEvent(cFunctionToCall, 0, 0);
			}

			/// <summary>
			/// Adds a new EveryTime Event with a default Group of zero. 
			/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
			/// the order they are added.</para></param>
			public void AddEveryTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
			{
				// Add the new EveryTime Event
				AddEveryTimeEvent(cFunctionToCall, iExecutionOrder, 0);
			}

			/// <summary>
			/// Adds a new EveryTime Event. 
			/// EveryTime Events fire every frame (i.e. every time the Update() function is called).
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
			/// the order they are added.</para></param>
			/// <param name="iGroup">The Group that this Event should belong to</param>
			public void AddEveryTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
			{
				// Add the Event to the Events List and re-sort it
				mcParticleSystemEventList.Add(new CParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.EveryTime, iExecutionOrder, iGroup));
				mcParticleSystemEventList.Sort(EventSorter);
			}

			/// <summary>
			/// Removes all EveryTime Events with the specified Function.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="cFunction">The Function of the EveryTime Event to remove</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveEveryTimeEvents(UpdateParticleSystemDelegate cFunction)
			{
				// Remove all EveryTime Events with the specified Function
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Removes all EveryTime Events in the specified Group.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="iGroup">The Group to remove the EveryTime Events from</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveEveryTimeEvents(int iGroup)
			{
				// Remove all EveryTime Events that belong to the specified Group
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.EveryTime && cEvent.iGroup == iGroup); });
			}

			/// <summary>
			/// Removes an EveryTime Event with the specified Function, Execution Order, and Group.
			/// Returns true if the Event was found and removed, false if not.
			/// </summary>
			/// <param name="cFunction">The Function of the EveryTime Event to remove</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
			/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
			/// <returns>Returns true if the Event was found and removed, false if not.</returns>
			public bool RemoveEveryTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Remove the specified Event
				return mcParticleSystemEventList.Remove(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.EveryTime, iExecutionOrder, iGroup));
			}

			/// <summary>
			/// Removes all EveryTime Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllEveryTimeEvents()
			{
				// Remove all Events with a Particle Event Type of EveryTime
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return cEvent.eType == EParticleSystemEventTypes.EveryTime; });
			}

			/// <summary>
			/// Returns if there is an EveryTime Event with the specified Function or not.
			/// </summary>
			/// <param name="cFunction">The Function of the EveryTime Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
			public bool ContainsEveryTimeEvent(UpdateParticleSystemDelegate cFunction)
			{
				// Return if there is an EveryTime Event with the specified Function
				return mcParticleSystemEventList.Exists(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.EveryTime && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Returns if there is an EveryTime Event with the specifed Function, Execution Order, and Group or not.
			/// </summary>
			/// <param name="cFunction">The Function of the EveryTime Event to look for</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
			/// <param name="iGroup">The Group of the Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
			public bool ContainsEveryTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Return if there is an EveryTime Event with the specified criteria or not
				return mcParticleSystemEventList.Contains(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.EveryTime, iExecutionOrder, iGroup));
			}            

			/// <summary>
			/// Adds a new OneTime Event with a default Execution Order and Group of zero. 
			/// OneTime Events fire once then are automatically removed.
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			public void AddOneTimeEvent(UpdateParticleSystemDelegate cFunctionToCall)
			{
				// Add the new OneTime Event with default Execution Order
				AddOneTimeEvent(cFunctionToCall, 0, 0);
			}

			/// <summary>
			/// Adds a new OneTime Event with a default Group of zero. 
			/// OneTime Events fire once then are automatically removed.
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
			/// the order they are added.</para></param>
			public void AddOneTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
			{
				// Add the new OneTime Event with default Execution Order
				AddOneTimeEvent(cFunctionToCall, iExecutionOrder, 0);
			}

			/// <summary>
			/// Adds a new OneTime Event. 
			/// OneTime Events fire once then are automatically removed.
			/// </summary>
			/// <param name="cFunctionToCall">The Function to Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in 
			/// the order they are added.</para></param>
			/// <param name="iGroup">The Group that this Event should belong to</param>
			public void AddOneTimeEvent(UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
			{
				// Add the OneTime Event to the List and re-sort the List
				mcParticleSystemEventList.Add(new CParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.OneTime, iExecutionOrder, iGroup));
				mcParticleSystemEventList.Sort(EventSorter);
			}

			/// <summary>
			/// Removes all OneTime Events with the specified Function.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="cFunction">The Function of the OneTime Event to remove</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveOneTimeEvents(UpdateParticleSystemDelegate cFunction)
			{
				// Remove all OneTime Events with the specified Function
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.OneTime && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Removes all OneTime Events in the specified Group.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="iGroup">The Group to remove the OneTime Events from</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveOneTimeEvents(int iGroup)
			{
				// Remove all OneTime Events that belong to the specified Group
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.OneTime && cEvent.iGroup == iGroup); });
			}

			/// <summary>
			/// Removes a OneTime Event with the specified Function To Call, Execution Order, and Group.
			/// Returns true if the Event was found and removed, false if not.
			/// </summary>
			/// <param name="cFunction">The Function of the OneTime Event to remove</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
			/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
			/// <returns>Returns true if the Event was found and removed, false if not.</returns>
			public bool RemoveOneTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Remove the specified Event from the Event List
				return mcParticleSystemEventList.Remove(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.OneTime, iExecutionOrder, iGroup));
			}

			/// <summary>
			/// Removes all OneTime Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllOneTimeEvents()
			{
				// Remove all Events with a Particle Event Type of OneTime
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return cEvent.eType == EParticleSystemEventTypes.OneTime; });
			}

			/// <summary>
			/// Returns if there is an OneTime Event with the specified Function or not.
			/// </summary>
			/// <param name="cFunction">The Function of the OneTime Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
			public bool ContainsOneTimeEvent(UpdateParticleSystemDelegate cFunction)
			{
				// Return if there is an OneTime Event with the specified Function
				return mcParticleSystemEventList.Exists(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.OneTime && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Returns if there is an OneTime Event with the specifed Function, Execution Order, and Group or not.
			/// </summary>
			/// <param name="cFunction">The Function of the OneTime Event to look for</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
			/// <param name="iGroup">The Group of the Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
			public bool ContainsOneTimeEvent(UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Return if there is an OneTime Event with the specified criteria or not
				return mcParticleSystemEventList.Contains(new CParticleSystemEvent(cFunction, EParticleSystemEventTypes.OneTime, iExecutionOrder, iGroup));
			}

			/// <summary>
			/// Adds a new Timed Event with a default Execution Order and Group of zero. 
			/// Timed Events fire when the Particle System's Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fTimeToFire">The Time when the Event should fire (i.e. when the Function should be called)</param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			public void AddTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunctionToCall)
			{
				// Add the Timed Event with default Execution Order
				AddTimedEvent(fTimeToFire, cFunctionToCall, 0, 0);
			}

			/// <summary>
			/// Adds a new Timed Event with a default Group of zero. 
			/// Timed Events fire when the Particle System's Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fTimeToFire">The Time when the Event should fire (i.e. when the Function should be called)</param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the order they are added.</para></param>
			public void AddTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
			{
				// Add the Timed Event with default Execution Order
				AddTimedEvent(fTimeToFire, cFunctionToCall, iExecutionOrder, 0);
			}

			/// <summary>
			/// Adds a new Timed Event. 
			/// Timed Events fire when the Particle System's Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fTimeToFire">The Time when the Event should fire (i.e. when the Function should be called)</param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the order they are added.</para></param>
			/// <param name="iGroup">The Group that this Event should belong to</param>
			public void AddTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
			{
				// Add the Timed Event to the Event List and re-sort the List
				mcParticleSystemEventList.Add(new CTimedParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
				mcParticleSystemEventList.Sort(EventSorter);
			}

			/// <summary>
			/// Removes all Timed Events with the specified Function.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="cFunction">The Function that is called when the Event fires</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveTimedEvents(UpdateParticleSystemDelegate cFunction)
			{
				// Remove all Timed Events with the specified Function
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.Timed && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Removes all Timed Events in the specified Group.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="iGroup">The Group to remove the Timed Events from</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveTimedEvents(int iGroup)
			{
				// Remove all Timed Events that belong to the specified Group
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.Timed && cEvent.iGroup == iGroup); });
			}

			/// <summary>
			/// Removes a Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
			/// Returns true if the Event was found and removed, false if not.
			/// </summary>
			/// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
			/// <param name="cFunction">The Function that is called when the Event fires</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to remove. Default is zero.</param>
			/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
			/// <returns>Returns true if the Event was found and removed, false if not.</returns>
			public bool RemoveTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Remove the Timed Event from the Event List
				return mcParticleSystemEventList.Remove(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
			}

			/// <summary>
			/// Removes all Timed Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllTimedEvents()
			{
				// Remove all Events with a Particle Event Type of Timed
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return cEvent.eType == EParticleSystemEventTypes.Timed; });
			}

			/// <summary>
			/// Returns if there is a Timed Event with the specified Function or not.
			/// </summary>
			/// <param name="cFunction">The Function of the Timed Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
			public bool ContainsTimedEvent(UpdateParticleSystemDelegate cFunction)
			{
				// Return if there is a Timed Event with the specified Function
				return mcParticleSystemEventList.Exists(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.Timed && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Returns if there is a Timed Event with the specified Timed To Fire, Function, Execution Order, and Group or not.
			/// </summary>
			/// <param name="fTimeToFire">The Time the Event is scheduled to fire at</param>
			/// <param name="cFunction">The Function of the Timed Event to look for</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
			/// <param name="iGroup">The Group of the Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
			public bool ContainsTimedEvent(float fTimeToFire, UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Return if there is a Timed Event with the specified criteria or not
				return mcParticleSystemEventList.Contains(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.Timed, iExecutionOrder, iGroup, fTimeToFire));
			}

			/// <summary>
			/// Adds a new Normalized Timed Event with a default Execution Order and Group of zero. 
			/// Normalized Timed Events fire when the Particle System's Normalized Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
			/// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunctionToCall)
			{
				// Add the Normalized Timed Event with default Execution Order
				AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, 0, 0);
			}

			/// <summary>
			/// Adds a new Normalized Timed Event with a default Group of zero. 
			/// Normalized Timed Events fire when the Particle System's Normalized Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire. 
			/// <para>NOTE: This is clamped to the range of 0.0 - 1.0.</para></param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute. NOTE: Events with lower Execution Order are executed first. NOTE: Events
			/// with the same Execution Order are not guaranteed to be executed in the order they are added.</param>
			public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder)
			{
				// Add the Normalized Timed Event with default Execution Order
				AddNormalizedTimedEvent(fNormalizedTimeToFire, cFunctionToCall, iExecutionOrder, 0);
			}

			/// <summary>
			/// Adds a new Normalized Timed Event. 
			/// Normalized Timed Events fire when the Particle System's Normalized Elapsed Time reaches the specified Time To Fire.
			/// </summary>
			/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) when the Event should fire
			/// (compared against the Particle System's Normalized Elapsed Time). NOTE: This is clamped to the range of 0.0 - 1.0</param>
			/// <param name="cFunctionToCall">The Function To Call when the Event fires</param>
			/// <param name="iExecutionOrder">The Order, relative to other Events, of when this Event 
			/// should Execute.
			/// <para>NOTE: Events with lower Execution Order are executed first.</para>
			/// <para>NOTE: Events with the same Execution Order are not guaranteed to be executed in the order they are added.</para></param>
			/// <param name="iGroup">The Group that this Event should belong to</param>
			public void AddNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunctionToCall, int iExecutionOrder, int iGroup)
			{
				// Clamp the NormalizedTimeToFire between 0.0 and 1.0
				fNormalizedTimeToFire = MathHelper.Clamp(fNormalizedTimeToFire, 0.0f, 1.0f);

				// Add the Normalized Timed Event to the Event List and re-sort the List
				mcParticleSystemEventList.Add(new CTimedParticleSystemEvent(cFunctionToCall, EParticleSystemEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
				mcParticleSystemEventList.Sort(EventSorter);
			}

			/// <summary>
			/// Removes all Normalized Timed Events with the specified Function.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="cFunction">The Function that is called when the Event fires</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveNormalizedTimedEvents(UpdateParticleSystemDelegate cFunction)
			{
				// Remove all Normalized Timed Events with the specified Function
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Removes all Normalized Timed Events in the specified Group.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="iGroup">The Group to remove the Normalized Timed Events from</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveNormalizedTimedEvents(int iGroup)
			{
				// Remove all Normalized Timed Events that belong to the specified Group
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.NormalizedTimed && cEvent.iGroup == iGroup); });
			}

			/// <summary>
			/// Removes a Normalized Timed Event with the specified Function, Time To Fire, Execution Order, and Group.
			/// Returns true if the Event was found and removed, false if not.
			/// </summary>
			/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
			/// <param name="cFunction">The Function that is called when the Event fires</param>
			/// <param name="iExectionOrder">The Execution Order of the Event to remove. Default is zero.</param>
			/// <param name="iGroup">The Group that the Event to remove is in. Default is zero.</param>
			/// <returns>Returns true if the Event was found and removed, false if not.</returns>
			public bool RemoveNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunction, int iExectionOrder, int iGroup)
			{
				// Remove the Normalized Timed Event from the Event List
				return mcParticleSystemEventList.Remove(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.NormalizedTimed, iExectionOrder, iGroup, fNormalizedTimeToFire));
			}

			/// <summary>
			/// Removes all Normalized Timed Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllNormalizedTimedEvents()
			{
				// Remove all Events with a Particle Event Type of NormalizedTimed
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return cEvent.eType == EParticleSystemEventTypes.NormalizedTimed; });
			}

			/// <summary>
			/// Returns if there is a NormalizedTimed Event with the specified Function or not.
			/// </summary>
			/// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function was found, false if not</returns>
			public bool ContainsNormalizedTimedEvent(UpdateParticleSystemDelegate cFunction)
			{
				// Return if there is a NormalizedTimed Event with the specified Function
				return mcParticleSystemEventList.Exists(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.eType == EParticleSystemEventTypes.NormalizedTimed && cEvent.cFunctionToCall == cFunction); });
			}

			/// <summary>
			/// Returns if there is a NormalizedTimed Event with the specifed Normalized Time To Fire, Function, Execution Order, and Group or not.
			/// </summary>
			/// <param name="fNormalizedTimeToFire">The Normalized Time (0.0 - 1.0) the Event is scheduled to fire at</param>
			/// <param name="cFunction">The Function of the NormalizedTimed Event to look for</param>
			/// <param name="iExecutionOrder">The Execution Order of the Event to look for</param>
			/// <param name="iGroup">The Group of the Event to look for</param>
			/// <returns>Returns true if an Event with the specified Function, Execution Order, and Group was found, false if not</returns>
			public bool ContainsNormalizedTimedEvent(float fNormalizedTimeToFire, UpdateParticleSystemDelegate cFunction, int iExecutionOrder, int iGroup)
			{
				// Return if there is a NormalizedTimed Event with the specified criteria or not
				return mcParticleSystemEventList.Contains(new CTimedParticleSystemEvent(cFunction, EParticleSystemEventTypes.NormalizedTimed, iExecutionOrder, iGroup, fNormalizedTimeToFire));
			}

			/// <summary>
			/// Removes all Timed Events and Normalized Timed Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllTimedAndNormalizedTimedEvents()
			{
				return (RemoveAllTimedEvents() + RemoveAllNormalizedTimedEvents());
			}

			/// <summary>
			/// Removes all Events.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllEvents()
			{
				// Store the Number of Events that were in the Event List
				int iNumberOfEvents = mcParticleSystemEventList.Count;

				// Remove all Events from the Event List
				mcParticleSystemEventList.Clear();

				// Return the Number of Events removed
				return iNumberOfEvents;
			}

			/// <summary>
			/// Removes all Events in the specified Group.
			/// Returns the number of Events that were removed.
			/// </summary>
			/// <param name="iGroup">The Group to remove all Events from</param>
			/// <returns>Returns the number of Events that were removed.</returns>
			public int RemoveAllEventsInGroup(int iGroup)
			{
				// Remove all Events that belong to the specified Group
				return mcParticleSystemEventList.RemoveAll(delegate(CParticleSystemEvent cEvent)
				{ return (cEvent.iGroup == iGroup); });
			}

			/// <summary>
			/// Updates the Particle System according to the Particle System Events. This is done automatically
			/// by the Particle System every frame (i.e. Everytime the Update() function is called).
			/// </summary>
			/// <param name="fElapsedTimeInSeconds">How much Time has passed, in seconds, 
			/// since the last Update</param>
			public void Update(float fElapsedTimeInSeconds)
			{
				// Update the Particle System
				Update(fElapsedTimeInSeconds, fElapsedTimeInSeconds);

				// Remove all of the OneTime Events now that they've been processed
				RemoveAllOneTimeEvents();
			}

			/// <summary>
			/// Updates the Particle System according to the Particle System Events
			/// </summary>
			/// <param name="fElapsedTimeThisPass">The amount of Elapsed Time to pass
			/// into the Event Functions being called on this Pass</param>
			/// <param name="fTotalElapsedTimeThisFrame">How much Time has passed, in seconds, 
			/// since the last Frame</param>
			private void Update(float fElapsedTimeThisPass, float fTotalElapsedTimeThisFrame)
			{
				// Variable to tell if the Lifetime data needs to be reset so the Events can be Repeated or not
				bool bEventsNeedToBeRepeated = false;
				float fNextPassElapsedTime = 0.0f;

				// Update the Particle System's Elapsed Time
				LifetimeData.UpdateElapsedTimeVariables(fElapsedTimeThisPass);

				// If there are no Events to process
				if (mcParticleSystemEventList.Count <= 0)
				{
					// Exit without doing anything
					return;
				}

				// If the Particle System has reached the end of its Lifetime AND 
				// the Particle System Events are set to Repeat
				if (LifetimeData.ElapsedTime >= LifetimeData.Lifetime &&
					LifetimeData.EndOfLifeOption == EParticleSystemEndOfLifeOptions.Repeat)
				{
					// Record that the Lifetime data will need to be reset
					bEventsNeedToBeRepeated = true;

					// Save how far past the Lifetime the Elapsed Time has gone
					fNextPassElapsedTime = LifetimeData.ElapsedTime - LifetimeData.Lifetime;

					// Adjust the Elapsed Time to be the amount of time it would take to reach the Lifetime
					fElapsedTimeThisPass -= fNextPassElapsedTime;

					// Make sure we do not have a negative Elapsed Time, as this would be the
					// case if the Elapsed Time was multiple times greater than the Lifetime
					fElapsedTimeThisPass = Math.Max(fElapsedTimeThisPass, 0.0f);

					// If the Elapsed Time was multiple times greater than the Lifetime, ignore all
					// of the full Lifetimes we would process and skip to the last one
					fNextPassElapsedTime = fNextPassElapsedTime % LifetimeData.Lifetime;

					// Set the Particle System's Elapsed Time to its Lifetime
					// We first set the Elapsed Time and then Update the Automatic Variables in order
					// to make sure that the Last Elapsed Time is correct as well, since we can't 
					// set it directly (we have to use the UpdateElapsedTimeVariables() function).
					LifetimeData.ElapsedTime = LifetimeData.LastElapsedTime;
					LifetimeData.UpdateElapsedTimeVariables(LifetimeData.Lifetime - LifetimeData.ElapsedTime);
				}

				// If we should process the Events this pass
				if (fElapsedTimeThisPass > 0.0f)
				{
					// Loop through all of the Events (which are pre-sorted by Execution Order).
					// We check against mcParticleSystemEventList.Count instead of storing the value 
					// in a local variable in case an Event removes other Events (may throw an 
					// index out of range exception if this happens).
					for (int iIndex = 0; iIndex < mcParticleSystemEventList.Count; iIndex++)
					{
						// If this is an EveryTime Event
						if (mcParticleSystemEventList[iIndex].eType == EParticleSystemEventTypes.EveryTime)
						{
							// Execute the Event's function
							mcParticleSystemEventList[iIndex].cFunctionToCall(fElapsedTimeThisPass);
						}
						// Else if this is a OneTime Event
						else if (mcParticleSystemEventList[iIndex].eType == EParticleSystemEventTypes.OneTime)
						{
							// If this is the last time this function will be called this frame.
							// This check is here because OneTime Events should only execute once, so we
							// only execute the function if we know this function will not be recursively
							// calling itself again, and we use the total Elapsed Time of this frame instead
							// of the Elapsed Time for this pass.
							if (!bEventsNeedToBeRepeated)
							{
								// Execute the OneTime Event using the Total Elapsed Time for this frame
								mcParticleSystemEventList[iIndex].cFunctionToCall(fTotalElapsedTimeThisFrame);
							}
						}
						// Else this is a Timed Event
						else
						{
							// Get a handle to the Timed Event
							CTimedParticleSystemEvent cTimedEvent = (CTimedParticleSystemEvent)mcParticleSystemEventList[iIndex];

							// If this is a normal Timed Event
							if (cTimedEvent.eType == EParticleSystemEventTypes.Timed)
							{
								// Store the Last Elapsed Time of the Particle. 
								// If it's zero we need to change it to be before zero so that any Events set to fire at time zero will be fired.
								float fLastElapsedTime = (LifetimeData.LastElapsedTime == 0.0f) ? -0.1f : LifetimeData.LastElapsedTime;

								// If this Event should fire
								if (fLastElapsedTime < cTimedEvent.fTimeToFire &&
									LifetimeData.ElapsedTime >= cTimedEvent.fTimeToFire)
								{
									// Execute the Event's function
									cTimedEvent.cFunctionToCall(fElapsedTimeThisPass);
								}
							}
							// Else this is a Normalized Timed Event
							else
							{
								// Store the Last Normalized Elapsed Time of the Particle. 
								// If it's zero we need to change it to be before zero so that any Events set to fire at time zero will be fired.
								float fLastNormalizedElapsedTime = (LifetimeData.LastNormalizedElapsedTime == 0.0f) ? -0.1f : LifetimeData.LastNormalizedElapsedTime;

								// If this Event should fire
								if (fLastNormalizedElapsedTime < cTimedEvent.fTimeToFire &&
									LifetimeData.NormalizedElapsedTime >= cTimedEvent.fTimeToFire)
								{
									// Fire the Event
									cTimedEvent.cFunctionToCall(fElapsedTimeThisPass);
								}
							}
						}
					}
				}

				// If the Particle System Lifetime should be reset and Timed Events repeated
				if (bEventsNeedToBeRepeated)
				{
					// Reset the Particle System's Elapsed Time
					LifetimeData.ElapsedTime = 0.0f;

					// Call this function again recursively to process the Timed Events properly
					Update(fNextPassElapsedTime, fTotalElapsedTimeThisFrame);
				}
			}
		}

		#endregion

		#region Fields

		// Define the Delegates used to Initialize a Particle's properties and Update a Vertex's properties
		private InitializeParticleDelegate mcParticleInitializationFunction = null;
		private UpdateVertexDelegate mcVertexUpdateFunction = null;

		private ParticleTypes meParticleType = ParticleTypes.None;   // Tells which type of Particle is being used so we know how to draw it (e.g. Point Sprite, Quad, etc.)
		private Particle[] mcParticles = null;      // Array used to hold all Particles
		private int miNumberOfParticlesToDraw = 0;  // Tells how many Particles in the Vertex Buffer should be drawn (i.e. How many are Active and Visible)

		private Vertex[] mcParticleVerticesToDraw = null;   // Array used to hold Particle Vertex information (i.e. The Vertex Buffer used to draw the Point Sprites and Quads)
		private int[] miaIndexBufferArray = null;           // The 32-bit Index Buffer used along with the Vertex Buffer for drawing Quads in the HiDef profile
		private short[] msaIndexBufferReachArray = null;    // The 16-bit Index Buffer used along with the Vertex Buffer for drawing Quads in the Reach profile
		private int miIndexBufferIndex = 0;                 // The current Index we are at in the Index Buffer
		private static bool? _SuseIntIndexBuffer = null;	// Tells whether the Int or Short Index Buffer should be used.
	#if (WINDOWS)
		[NonSerialized]
	#endif
		private SpriteBatch mcSpriteBatch = null;                   // The Sprite Batch used to draw Sprites
		private Particle[] mcParticleSpritesToDraw = null;			// The List of Sprites to Draw (i.e. The Vertex Buffer used to draw Sprites)
		private SpriteBatchSettings mcSpriteBatchSettings = null;	// Variable used to hold the Sprite Batch's drawing options

		private LinkedList<Particle> mcActiveParticlesList = null;      // List used to hold Active (alive) Particles
		private LinkedList<Particle> mcInactiveParticlesList = null;    // List used to hold Inactive (dead) Particles

	#if (WINDOWS)
		[NonSerialized]
	#endif
		private VertexDeclaration mcVertexDeclaration = null;   // Description of the Vertex Structure used to draw Particles
		private int miVertexSizeInBytes = 0;    // The Size of a Vertex in Bytes

	#if (WINDOWS)
		[NonSerialized]
	#endif
		private RenderProperties mcRenderProperties = null;
	#if (WINDOWS)
		[NonSerialized]
	#endif
		private Effect mcEffect = null;         // The Effect file to use to draw the Particles
	#if (WINDOWS)
		[NonSerialized]
	#endif
		private Texture2D mcTexture = null;     // The Texture used to draw Particles

		private int miMaxNumberOfParticlesAllowed = 0;  // The Max Number of Particles that this Particle System is Allowed to contain at a given time
		
	// If not inheriting from DrawableGameComponent, we need to define the variables that
	// are included in DrawableGameComponent our self
	#if (!DPSFAsDrawableGameComponent)
		private bool mbPerformDraws = true;         // Tells if the Particles should be Drawn or not
		private bool mbPerformUpdates = true;       // Tells if the Particle System and its Particles should be Updated or not

		private int miDrawOrder = 0;                // Tells where in the Order this particle system should be Drawn
		private int miUpdateOrder = 0;              // Tells where in the Order this particle system should be Updated

	#if (WINDOWS)
		[NonSerialized]
	#endif
		private Game mcGame = null;                 // The Game object passed into the constructor (Not used by DPSF at all; Just here for consistency with DrawableGameComponent)

	#if (WINDOWS)
		[NonSerialized]
	#endif
		private GraphicsDevice mcGraphicsDevice = null; // Handle to the Graphics Device to draw to

		/// <summary>
		/// Raised when the UpdateOrder property changes
		/// </summary>
		public event EventHandler<EventArgs> UpdateOrderChanged = null;

		/// <summary>
		/// Raised when the DrawOrder property changes
		/// </summary>
		public event EventHandler<EventArgs> DrawOrderChanged = null;

		/// <summary>
		/// Raised when the Enabled property changes
		/// </summary>
		public event EventHandler<EventArgs> EnabledChanged = null;

		/// <summary>
		/// Raised when the Visible property changes
		/// </summary>
		public event EventHandler<EventArgs> VisibleChanged = null;
	#endif

	#if (WINDOWS)
		[NonSerialized]
	#endif
		private ContentManager mcContentManager = null; // Handle to the Content Manager used to load Textures and Effects

		private float mfSimulationSpeed = 1.0f;         // How much to scale the Simulation Speed by (1.0 = normal speed, 0.5 = half speed, 2.0 = double speed)
		private float mfInternalSimulationSpeed = 1.0f; // How much to scale the Simulation Speed by to make the effect run at "normal" speed

		// Variables used to control how often the Particle System is updated
		private float mfTimeToWaitBetweenUpdates = 0.0f;    // How much time should elapse between Update()s
		private float mfTimeElapsedSinceLastUpdate = 0.0f;  // How much time has elapsed since the last Update()

		private CParticleEvents mcParticleEvents = null;            // Variable used to access the Particle Events
		private CParticleSystemEvents mcParticleSystemEvents = null;// Variable used to access the Particle System Events

		// Auto Memory Manager variables
		private AutoMemoryManagerSettings mcAutoMemoryManagerSettings = null;  // Auto Memory Manager Settings
		private float mfAutoMemoryManagersElapsedTime = 0.0f;                   // The Automatic Memory Manager's Timer
		private int miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds = 0; // The Max Number of Particles that were Active in a single frame Over The Last X Seconds (X is specified in the Auto Memory Manager Settings)

		private ParticleEmitter _emitter = null;			// The Emitter used to automatically generate Particles
		private ParticleEmitterCollection _emitters = null;	// The collection of emitters that this particle system uses.

// The Reach profile does not have access to the System.Diagnostics namespace, so it does not know about the Stopwatch class.
#if (!WINDOWS_PHONE)
    #if (WINDOWS)
            [NonSerialized]
    #endif
		private Stopwatch _performanceProfilingStopwatch = null; // Stopwatch used for diagnostic timers.
#endif
		private bool _performanceProfilingIsEnabled = false;

		private int miID = 0;                       // Set this Particle Systems unique ID
		private int miType = 0;                     // The Type of Particle System this is

		private Matrix mcWorld = Matrix.Identity;       // The World Matrix
		private Matrix mcView = Matrix.Identity;        // The View Matrix
		private Matrix mcProjection = Matrix.Identity;  // The Projection Matrix

		private ParticleSystemManager mcParticleSystemManager = null;	// The Manager whose properties should be used for this Particle System

		private static DPSFDefaultEffect SmcDPSFEffect = null;			// Handle to the default DPSF Effect

		/// <summary>
		/// A static int used to keep track of the total number of Particle Systems created
		/// </summary>
		private static int _totalNumberOfParticleSystemsCreated = 0;

		/// <summary>
		/// A static int used to keep track of how many DPSF particle systems are initialized at any given moment.
		/// </summary>
		private static int _numberOfParticleSystemsCurrentlyInitialized = 0;

// If we're running on the Xbox 360, declare some Xbox specific variables
#if (XBOX)
		private const int MAX_MEMORY_IN_BYTES_THAT_XBOX_CAN_DRAW = 524287;	// The max amount of memory (in bytes) that the Xbox 360 can use in a single Draw() call (2^19 = 524,288).
		private int miMaxParticlesThatXboxCanDrawAtOnce = 0;				// Will hold the max number of particles that the Xbox 360 is able to draw in a single Draw() call.
#endif

		#endregion

		#region Methods that cannot be overridden

	// If we are inheriting DrawableGameComponent
	#if (DPSFAsDrawableGameComponent)
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">The Game object to attach this Particle System to</param>
		public DPSF(Game cGame) : base(cGame)
		{
			// If the Game object given in the constructor was invalid
			if (Game == null)
			{
				// Throw an exception
				throw new DPSFArgumentNullException("cGame", "Game parameter specified in Particle System constructor is null. The constructor's Game parameter cannot be null when DPSF is inheriting from DrawableGameComponent.");
			}
	#else
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
		/// parameter if not using a Game object.</param>
		public DPSF(Game cGame)
		{
			// Save a handle to the Game if one was given
			mcGame = cGame;
	#endif
			// Set this Particle Systems unique ID
			miID = _totalNumberOfParticleSystemsCreated++;
		}

		/// <summary>
		/// Initializes a new No Display Particle System. This type of Particle System does not allow any of the Particles
		/// to be drawn to a Graphics Device (e.g. the screen).
		/// </summary>
		/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
		/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
		/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
		/// may also be adjusted manually at run-time.</param>
		/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
		/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
		/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
		/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
		/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
		/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
		/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
		/// may also be adjusted manually at run-time.</param>
		public void InitializeNoDisplayParticleSystem(int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow)
		{
			try
			{
				// Initialize the variables that all Particle Systems have in common
				InitializeCommonVariables(null, null, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.NoDisplay);
			}
			catch (Exception e)
			{
				// Specify that the Particle System is not yet Initialized and re-throw the exception
				ParticleType = ParticleTypes.None;
				throw new DPSFException("A problem occurred while Initializing the Particle System. Inner DPSFException: " + e.ToString(), e);
			}

			// Perform any user operations now that the Particle System is Initialized
			AfterInitialize();
		}

		/// <summary>
		/// Initializes a new Sprite Particle System
		/// </summary>
		/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
		/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
		/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
		/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
		/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
		/// may also be adjusted manually at run-time.</param>
		/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
		/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
		/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
		/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
		/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
		/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
		/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
		/// may also be adjusted manually at run-time.</param>
		/// <param name="sTexture">The asset name of the Texture to use to visualize the Particles</param>
		public void InitializeSpriteParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
								int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, string sTexture)
		{
			InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, sTexture, null);
		}

		/// <summary>
		/// Initializes a new Sprite Particle System
		/// </summary>
		/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
		/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
		/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
		/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
		/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
		/// may also be adjusted manually at run-time.</param>
		/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
		/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
		/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
		/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
		/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
		/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
		/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
		/// may also be adjusted manually at run-time.</param>
		/// <param name="sTexture">The asset name of the Texture to use to visualize the Particles</param>
		/// <param name="cSpriteBatchToDrawWith">The Sprite Batch that this particle system should use to draw its
		/// particles with.
		/// <para>If null, the particle system will use its own SpriteBatch to draw its particles.</para>
		/// <para>If not null, then you must call SpriteBatch.Begin() before calling ParticleSystem.Draw() to
		/// draw the particle system, and then call SpriteBatch.End() when done drawing the particle system.</para></param>
		public void InitializeSpriteParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
								int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, string sTexture, SpriteBatch cSpriteBatchToDrawWith)
		{
			try
			{
				// Initialize the variables that all Particle Systems have in common
				InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.Sprite);

				// Set the Texture to use
				SetTexture(sTexture);

				// If we should use the given Sprite Batch instead of our own, record it and save a handle to the SpriteBatch to use.
				if (cSpriteBatchToDrawWith != null)
				{
					mcSpriteBatch = cSpriteBatchToDrawWith;
					mcSpriteBatchSettings = null;
					this.UsingExternalSpriteBatchToDrawParticles = true;
				}
			}
			catch (Exception e)
			{
				// Specify that the Particle System is not yet Initialized and re-throw the exception
				ParticleType = ParticleTypes.None;
				throw new DPSFException("A problem occurred while Initializing the Particle System. Inner DPSFException: " + e.ToString(), e);
			}

			// Perform any user operations now that the Particle System is Initialized
			AfterInitialize();
		}

		/// <summary>
		/// Initializes a new Sprite Particle System
		/// </summary>
		/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
		/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
		/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
		/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
		/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
		/// may also be adjusted manually at run-time.</param>
		/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
		/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
		/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
		/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
		/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
		/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
		/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
		/// may also be adjusted manually at run-time.</param>
		/// <param name="cTexture">The Texture to use to visualize the Particles</param>
		public void InitializeSpriteParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
								int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, Texture2D cTexture)
		{
			InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, cTexture, null);
		}

		/// <summary>
		/// Initializes a new Sprite Particle System
		/// </summary>
		/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
		/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
		/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
		/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
		/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
		/// may also be adjusted manually at run-time.</param>
		/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
		/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
		/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
		/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
		/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
		/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
		/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
		/// may also be adjusted manually at run-time.</param>
		/// <param name="cTexture">The Texture to use to visualize the Particles</param>
		/// <param name="cSpriteBatchToDrawWith">The Sprite Batch that this particle system should use to draw its
		/// particles with.
		/// <para>If null, the particle system will use its own SpriteBatch to draw its particles.</para>
		/// <para>If not null, then you must call SpriteBatch.Begin() before calling ParticleSystem.Draw() to
		/// draw the particle system, and then call SpriteBatch.End() when done drawing the particle system.</para></param>
		public void InitializeSpriteParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
								int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, Texture2D cTexture, SpriteBatch cSpriteBatchToDrawWith)
		{
			try
			{
				// Initialize the variables that all Particle Systems have in common
				InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.Sprite);

				// Set the Texture to use
				Texture = cTexture;

				// If we should use the given Sprite Batch instead of our own, record it and save a handle to the SpriteBatch to use.
				if (cSpriteBatchToDrawWith != null)
				{
					mcSpriteBatch = cSpriteBatchToDrawWith;
					mcSpriteBatchSettings = null;
					this.UsingExternalSpriteBatchToDrawParticles = true;
				}
			}
			catch (Exception e)
			{
				// Specify that the Particle System is not yet Initialized and re-throw the exception
				ParticleType = ParticleTypes.None;
				throw new DPSFException("A problem occurred while Initializing the Particle System. Inner DPSFException: " + e.ToString(), e);
			}

			// Perform any user operations now that the Particle System is Initialized
			AfterInitialize();
		}

		/// <summary>
		/// Initializes a new Quad Particle System
		/// </summary>
		/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
		/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
		/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
		/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
		/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
		/// may also be adjusted manually at run-time.</param>
		/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
		/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
		/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
		/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
		/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
		/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
		/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
		/// may also be adjusted manually at run-time.</param>
		/// <param name="cVertexUpdateFunction">Function used to copy a Particle's drawable properties into the vertex buffer</param>
		public void InitializeQuadParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
								int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow,
								UpdateVertexDelegate cVertexUpdateFunction)
		{
			try
			{
				// Initialize the variables that all Particle Systems have in common
				InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.Quad);

				// Set the Vertex Update Function to use
				VertexUpdateFunction = cVertexUpdateFunction;

				// Make sure the Texture is null since it is not required
				Texture = null;
			}
			catch (Exception e)
			{
				// Specify that the Particle System is not yet Initialized and re-throw the exception
				ParticleType = ParticleTypes.None;
				throw new DPSFException("A problem occurred while Initializing the Particle System. Inner DPSFException: " + e.ToString(), e);
			}

			// Perform any user operations now that the Particle System is Initialized
			AfterInitialize();
		}

		/// <summary>
		/// Initializes a new Textured Quad Particle System
		/// </summary>
		/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
		/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
		/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
		/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
		/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
		/// may also be adjusted manually at run-time.</param>
		/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
		/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
		/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
		/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
		/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
		/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
		/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
		/// may also be adjusted manually at run-time.</param>
		/// <param name="cVertexUpdateFunction">Function used to copy a Particle's drawable properties into the vertex buffer</param>
		/// <param name="sTexture">The asset name of the Texture to use to visualize the Particles</param>
		public void InitializeTexturedQuadParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
								int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, 
								UpdateVertexDelegate cVertexUpdateFunction, string sTexture)
		{
			try
			{
				// Initialize the variables that all Particle Systems have in common
				InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.TexturedQuad);

				// Set the Vertex Update Function to use
				VertexUpdateFunction = cVertexUpdateFunction;

				// Set the Texture to use
				SetTexture(sTexture);
			}
			catch (Exception e)
			{
				// Specify that the Particle System is not yet Initialized and re-throw the exception
				ParticleType = ParticleTypes.None;
				throw new DPSFException("A problem occurred while Initializing the Particle System. Inner DPSFException: " + e.ToString(), e);
			}

			// Perform any user operations now that the Particle System is Initialized
			AfterInitialize();
		}

		/// <summary>
		/// Initializes a new Textured Quad Particle System
		/// </summary>
		/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
		/// <param name="cContentManager">Content Manager used to load Effect files and Textures</param>
		/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
		/// be Allocated for. If the Auto Memory Manager is enabled (default), this will be dynamically adjusted at
		/// run-time to make sure there is always roughly as much Memory Allocated as there are Particles. This value
		/// may also be adjusted manually at run-time.</param>
		/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
		/// Allowed at a single point in time. If the Auto Memory Manager will not be enabled to increase
		/// memory, this should be less than or equal to the Number Of Particles To Allocate Memory For, 
		/// as the Particle System can only handle as many Particles as it has Memory Allocated For. Also, the
		/// Auto Memory Manager will never increase the Allocated Memory to handle more Particles than this value. 
		/// If this is set to a value lower than the Number Of Particles To Allocate Memory For, then only this many
		/// Particles will be allowed, even though there is memory allocated for more Particles. This value 
		/// may also be adjusted manually at run-time.</param>
		/// <param name="cVertexUpdateFunction">Function used to copy a Particle's drawable properties into the vertex buffer</param>
		/// <param name="cTexture">The Texture to use to visualize the Particles</param>
		public void InitializeTexturedQuadParticleSystem(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
								int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow,
								UpdateVertexDelegate cVertexUpdateFunction, Texture2D cTexture)
		{
			try
			{
				// Initialize the variables that all Particle Systems have in common
				InitializeCommonVariables(cGraphicsDevice, cContentManager, iNumberOfParticlesToAllocateMemoryFor, iMaxNumberOfParticlesToAllow, ParticleTypes.TexturedQuad);

				// Set the Vertex Update Function to use
				VertexUpdateFunction = cVertexUpdateFunction;

				// Set the Texture to use
				Texture = cTexture;
			}
			catch (Exception e)
			{
				// Specify that the Particle System is not yet Initialized and re-throw the exception
				ParticleType = ParticleTypes.None;
				throw new DPSFException("A problem occurred while Initializing the Particle System. Inner DPSFException: " + e.ToString(), e);
			}

			// Perform any user operations now that the Particle System is Initialized
			AfterInitialize();
		}

		/// <summary>
		/// Initialize the variables common to all Particle Systems
		/// </summary>
		/// <param name="cGraphicsDevice">Graphics Device to draw to</param>
		/// <param name="cContentManager">Content Manager to use to load Effect files and Textures</param>
		/// <param name="iNumberOfParticlesToAllocateMemoryFor">The Number of Particles memory should
		/// be Allocated for. The Maximum Number Of Particles the Particle System should Allow is also
		/// set to this value initially.</param>
		/// <param name="iMaxNumberOfParticlesToAllow">The Maximum Number of Active Particles that are
		/// Allowed at a single point in time. This should be less than or equal to the Number Of Particles 
		/// To Allocate Memory For if the Auto Memory Manager will not be used, as the Particle System 
		/// can only handle as many Particles as it has Memory Allocated For.</param>
		/// <param name="eParticleType">The Type of Particles this Particle System should draw</param>
		private void InitializeCommonVariables(GraphicsDevice cGraphicsDevice, ContentManager cContentManager,
										int iNumberOfParticlesToAllocateMemoryFor, int iMaxNumberOfParticlesToAllow, 
										ParticleTypes eParticleType)
		{
			// Destroy the Particle System before Initializing to make sure it is a fresh Initialize
			Destroy();

			// Update the number of particle systems that are currently initialized
			_numberOfParticleSystemsCurrentlyInitialized++;

			// Set the Particle Type before anything else (Required for GraphicsDevice and ContentManager property checks)
			ParticleType = eParticleType;

		// If we inherit from DrawableGameComponent
		#if (DPSFAsDrawableGameComponent)
			// If the Game object given in the constructor was invalid
			if (Game == null)
			{
				// Throw an exception
				throw new DPSFArgumentNullException("The Game object given in the constructor is now null in the Initialization function and no longer valid.");
			}
			// Else a valid Game object was given in the constructor
			else
			{
				// Initialize the Drawable Game Component
				Initialize();

				// If the Game does not already Contain this Component
				if (!Game.Components.Contains(this))
				{
					// Add this Particle System to the Game Components to be Updated and Drawn
					Game.Components.Add(this);
				}
			}
		// Else we do not inherit from DrawableGameComponent
		#else
			// Save a handle to the Graphics Device
			GraphicsDevice = cGraphicsDevice;
		#endif

			// Save a handle to the Content Manager
			ContentManager = cContentManager;
			
			// If the Default Effect file hasn't been loaded yet
			if (SmcDPSFEffect == null && GraphicsDevice != null)
			{
		// If we are on Windows
		#if (WINDOWS)
			// If we should be using the Windows HiDef profile
			if (GraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef)
			{
				SmcDPSFEffect = new DPSFDefaultEffect(GraphicsDevice, DPSF.DPSFDefaultEffect.DPSFDefaultEffectConfigurations.WindowsHiDef);
			}
			// Else use the Windows Reach profile
			else
			{
				SmcDPSFEffect = new DPSFDefaultEffect(GraphicsDevice, DPSF.DPSFDefaultEffect.DPSFDefaultEffectConfigurations.WindowsReach);
			}
				
		// Else if we are on the XBox
		#elif (XBOX)
				SmcDPSFEffect = new DPSFDefaultEffect(GraphicsDevice, DPSF.DPSFDefaultEffect.DPSFDefaultEffectConfigurations.Xbox360HiDef);
		// Don't load anything for the Windows Phone, since it doesn't support custom Effects
		#endif
			}

			// Set the default Effect and Technique to use
			SetDefaultEffect();

			if (_SuseIntIndexBuffer == null && GraphicsDevice != null)
			{
				// If we should be using the Windows HiDef profile, then use the full size Int Index Buffer.
				if (GraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef)
				{
					_SuseIntIndexBuffer = true;
				}
				// Else we are using the Windows Reach profile, so use the Short Index Buffer.
				else
				{
					_SuseIntIndexBuffer = false;
				}

			// Using MonoGame for WinRT reports that it uses the HiDef profile, but doesn't actually support the Int Index Buffer, so tell it to use the Short Index Buffer for WinRT.
			#if (WIN_RT)
				_SuseIntIndexBuffer = false;
			#endif
			}

			// Specify the Vertex Declaration.
			Vertex vertex = new Vertex();
			mcVertexDeclaration = vertex.VertexDeclaration;
            miVertexSizeInBytes = (mcVertexDeclaration == null) ? 0 : mcVertexDeclaration.VertexStride;

			// If this is a Sprite Particle System
			if (ParticleType == ParticleTypes.Sprite)
			{
				// Initialize the Sprite Batch drawing Settings
				mcSpriteBatch = new SpriteBatch(GraphicsDevice);
				mcSpriteBatchSettings = new SpriteBatchSettings();
			}

			// Initialize the default render settings
			mcRenderProperties = new RenderProperties();

			// Apply any changes that the user wants to the Render settings.
			InitializeRenderProperties();

			// Set the Number of Particles to Allocate Memory for (which Initializes the Particle Arrays)
			NumberOfParticlesAllocatedInMemory = iNumberOfParticlesToAllocateMemoryFor;

			// Set the Max Number Of Particles To Allow
			MaxNumberOfParticlesAllowed = iMaxNumberOfParticlesToAllow;

			// Specify to Draw and Update Particles by default
			Visible = true;
			Enabled = true;

			/////////////////////////////////////////////////////////
			// Do not reset the Draw and Update Orders.
			// We don't want users to have to re-specify them every time they reinitialize the particle system.
			//UpdateOrder = 0;
			//DrawOrder = 0;
			/////////////////////////////////////////////////////////

			// Set the simulation to run at normal speed
			SimulationSpeed = 1.0f;
			InternalSimulationSpeed = 1.0f;

			// Default to the default Updates Per Second
			UpdatesPerSecond = DPSFDefaultSettings.UpdatesPerSecond;

			// Initialize the Particle Events
			mcParticleEvents = new CParticleEvents();

			// Initialize the Particle System Events
			mcParticleSystemEvents = new CParticleSystemEvents();

			// Initialize the Auto Memory Manager Settings
			mcAutoMemoryManagerSettings = new AutoMemoryManagerSettings(DPSFDefaultSettings.AutoMemoryManagementSettings);

			// Set if the Diagnostics should run or not
			this.PerformanceProfilingIsEnabled = DPSFDefaultSettings.PerformanceProfilingIsEnabled;

			// Initialize the Emitters
			_emitters = new ParticleEmitterCollection();
			_emitters.AllEmittersRemoved += new EventHandler(_emitters_AllEmittersRemoved);

			// Copy any properties from the Particle System Manager into this Particle System.
			// This includes things like the Updates Per Second and Simulation Speed.
			ParticleSystemManagerToCopyPropertiesFrom = ParticleSystemManagerToCopyPropertiesFrom;
		}

		/// <summary>
		/// Release all resources used by the Particle System and reset all properties to their default values
		/// </summary>
		public void Destroy()
		{
			// Perform any other user operations
			BeforeDestroy();

			// If this particle system was initialized, decrement the number of currently initialized particle systems.
			if (meParticleType != ParticleTypes.None)
				_numberOfParticleSystemsCurrentlyInitialized--;

			// Set the Particle Type to None first, as some other properties can only be set to null when this is None.
			meParticleType = ParticleTypes.None;

		// If we inherit from DrawableGameComponent
		#if (DPSFAsDrawableGameComponent)
			// If a valid Game object was given in the constructor
			if (Game != null)
			{
				// If the Game Contains this Component
				if (Game.Components.Contains(this))
				{
					// Remove this Particle System from the Game Components to be Updated and Drawn
					Game.Components.Remove(this);
				}
			}
		// Else we do not inherit from DrawableGameComponent
		#else
			// Release the handle to the Graphics Device
			mcGraphicsDevice = null;
		#endif

			// Release the Content Manager
			mcContentManager = null;

			// Release all data used by this class instance
			mcParticleInitializationFunction = null;
			mcVertexUpdateFunction = null;

			// Release the render settings
			mcRenderProperties = null;

			mcParticles = null;
			miNumberOfParticlesToDraw = 0;

			mcParticleVerticesToDraw = null;
			miaIndexBufferArray = null;
			msaIndexBufferReachArray = null;
			miIndexBufferIndex = 0;
			mcSpriteBatch = null;
			mcParticleSpritesToDraw = null;
			mcSpriteBatchSettings = null;

			// If the Active Particle List still exists
			if (mcActiveParticlesList != null)
			{
				mcActiveParticlesList.Clear();
			}
			mcActiveParticlesList = null;

			// If the Inactive Particle List still exists
			if (mcInactiveParticlesList != null)
			{
				mcInactiveParticlesList.Clear();
			}
			mcInactiveParticlesList = null;

			miMaxNumberOfParticlesAllowed = 0;

			mcVertexDeclaration = null;
			miVertexSizeInBytes = 0;

			mcEffect = null;
			mcTexture = null;

			DeserializationTexturePath = null;
			DeserializationEffectPath = null;
			DeserializationTechniqueName = null;

			Visible = false;
			Enabled = false;

			/////////////////////////////////////////////////////////
			// Do not reset the Draw and Update Orders.
			// We don't want users to have to re-specify them every time they reinitialize the particle system.
			//UpdateOrder = 0;
			//DrawOrder = 0;
			/////////////////////////////////////////////////////////

			SimulationSpeed = 1.0f;
			InternalSimulationSpeed = 1.0f;

			mfTimeToWaitBetweenUpdates = 0.0f;
			mfTimeElapsedSinceLastUpdate = 0.0f;

			World = Matrix.Identity;
			View = Matrix.Identity;
			Projection = Matrix.Identity;

			mcParticleEvents = null;
			mcParticleSystemEvents = null;

			mcAutoMemoryManagerSettings = null;

			_emitter = null;
			if (_emitters != null)
			{
				_emitters.AllEmittersRemoved -= new EventHandler(_emitters_AllEmittersRemoved);
				_emitters.RemoveAll();
			}
			_emitters = null;

			// Perform any other user operations
			AfterDestroy();
		}

		/// <summary>
		/// This function should be called immediately after deserializing a particle system in order to reinitialize the properties 
		/// that could not be serialized.
		/// <para>NOTE: If this type of particle system requires a Texture, this function will attempt to load the Texture specified
		/// by the DeserializationTexturePath property. If it is unable to load a texture, an DPSFArgumentNullException will be thrown, so 
		/// this function should be wrapped in a try block, and when an DPSFArgumentNullException is caught then the particle system's
		/// texture should be manually set.</para>
		/// <para>NOTE: This will attempt to load the Effect and Technique specified by the DeserializationEffectPath and
		/// DeserializationTechniqueName properties. If either of these are null, the DPSFDefaultEffect will be used, and the default
		/// Technique for this type of particle system will be loaded.</para>
		/// <para>NOTE: Particle systems can only be serialized (and thus, deserialized) if not inheriting from DrawableGameComponent
		/// (i.e. InheritsDrawableGameComponent == false. i.e. using the DPSF.dll, not DPSFAsDrawableGameComponent.dll).</para>
		/// </summary>
		/// <param name="cGame">Handle to the Game object being used. You may pass in null for this parameter if not using a Game object.</param>
		/// <param name="cGraphicsDevice">Graphics Device to draw to.</param>
		/// <param name="cContentManager">Content Manager used to load Effect files and Textures.</param>
		public void InitializeNonSerializableProperties(Game cGame, GraphicsDevice cGraphicsDevice, ContentManager cContentManager)
		{
			// Set this Particle Systems unique ID
			miID = _totalNumberOfParticleSystemsCreated++;

		// If we inherit from DrawableGameComponent
		#if (DPSFAsDrawableGameComponent)
			// If the Game object given in the constructor was invalid
			if (Game == null)
			{
				// Throw an exception
				throw new DPSFArgumentNullException("cGame", "The specified Game object is null. A valid Game object is required.");
			}
			// Else a valid Game object was given in the constructor
			else
			{
				// Initialize the Drawable Game Component
				Initialize();

				// If the Game does not already Contain this Component
				if (!Game.Components.Contains(this))
				{
					// Add this Particle System to the Game Components to be Updated and Drawn
					Game.Components.Add(this);
				}
			}
		// Else we do not inherit from DrawableGameComponent
		#else
			// Save a handle to the Game if one was given
			mcGame = cGame;

			// Save a handle to the Graphics Device
			GraphicsDevice = cGraphicsDevice;
		#endif

			// Save a handle to the Content Manager
			ContentManager = cContentManager;

			// Specify the Vertex Element
			Vertex vertex = new Vertex();
			mcVertexDeclaration = vertex.VertexDeclaration;

			// Assume the Sprite Batch isn't needed
			mcSpriteBatch = null;

			// If a Sprite Batch is required, initialize it
			if (meParticleType == ParticleTypes.Sprite)
				mcSpriteBatch = new SpriteBatch(GraphicsDevice);

			// If this is a particle system type that uses as Effect
			if (meParticleType != ParticleTypes.NoDisplay && meParticleType != ParticleTypes.None)
			{
				// If we have the Path to an Effect and Technique previously used in this particle system, load it.
				// NOTE: This is not guaranteed to be the last Effect and Technique used by the particle system.
				if (!string.IsNullOrEmpty(DeserializationEffectPath) && !string.IsNullOrEmpty(DeserializationTechniqueName))
				{
					SetEffectAndTechnique(DeserializationEffectPath, DeserializationTechniqueName);
				}
				// Else the Effect or Technique to use is null, so use the default ones
				else
				{
					SetDefaultEffect();
				}
			}

			// If this is a particle system type that uses a Texture
			if (meParticleType == ParticleTypes.Sprite || meParticleType == ParticleTypes.TexturedQuad)
			{
				// If we have the Path to a Texture used previously in this particle system, load it.
				// NOTE: This is not guaranteed to be the last Texture used by the particle system.
				if (!string.IsNullOrEmpty(DeserializationTexturePath))
				{
					SetTexture(DeserializationTexturePath);
				}
				// Else we do not have a Texture to use to draw the particles, so throw an exception about it.
				else
				{
					throw new DPSFArgumentNullException("DeserializationTexturePath", "The specified Texture to use is null. A valid Texture must be set to draw the current Type of Particles.");
				}
			}

            // Re-initialize the Render Properties.
            mcRenderProperties = new RenderProperties();
            InitializeRenderProperties();
		}

		/// <summary>
		/// The path used to load the Texture when the InitializeNonSerializableProperties() function is called.
		/// <para>NOTE: This is automatically set when the SetTexture() function is called.</para>
		/// </summary>
		public string DeserializationTexturePath { get; set; }

		/// <summary>
		/// The path used to load the Effect when the InitializeNonSerializableProperties() function is called.
		/// <para>NOTE: This is automatically set when the SetEffectAndTechnique(string, string) function is called. </para>
		/// </summary>
		public string DeserializationEffectPath { get; set; }

		/// <summary>
		/// The Name of the Technique to use when the InitializeNonSerializableProperties() function is called.
		/// <para>NOTE: This is automatically set when the SetEffectAndTechnique() and SetTechnique() functions are called.</para>
		/// </summary>
		public string DeserializationTechniqueName { get; set; }

		/// <summary>
		/// Returns true if the Particle System is Initialized, false if not.
		/// </summary>
		/// <returns>Returns true if the Particle System is Initialized, false if not.</returns>
		public bool IsInitialized
		{
			get { return (mcParticles != null && meParticleType != ParticleTypes.None && (this.GraphicsDevice != null || meParticleType == ParticleTypes.NoDisplay)); }
		}

		/// <summary>
		/// A custom effect provided by DPSF. In DPSF v2.1.0 and prior this effect was used as the default effect for all particle system types.
		/// Each particle system type now uses one of the built-in XNA 4 effects as its default effect in order to make all of the particle system types
		/// fully compatible with the Reach profile, and usable on the Windows Phone 7.
		/// <para>This effect may still be used for a particle system by calling the SetEffectAndTechnique() function from the particle system's overridden
		/// InitializeRenderProperties() function.</para>
		/// <para>This Effect has several techniques that may be used (<see cref="DPSF.DPSFDefaultEffectTechniques"/>).</para>
		/// </summary>
		public DPSFDefaultEffect DPSFDefaultEffect
		{
			get { return SmcDPSFEffect; }
		}

	// If not inheriting DrawableGameComponent, we need to define the Properties
	// implemented by DrawableGameComponent ourself.
	#if (!DPSFAsDrawableGameComponent)
		/// <summary>
		/// Get / Set if this Particle System should Draw its Particles or not.
		/// <para>NOTE: Setting this to false causes the Draw() function to not draw anything, including the 
		/// BeforeDraw() and AfterDraw() functions not to be called.</para>
		/// </summary>
		[DPSFViewerParameter(Description="Toggle whether the particle system should be visible or not.", Group="DPSF")]
		public bool Visible
		{
			get { return mbPerformDraws; }
			set
			{
				bool previousValue = mbPerformDraws;
				mbPerformDraws = value; 

				// If the Visibility was changed
				if (previousValue != mbPerformDraws)
				{
					// If there is a function to catch the event
					if (VisibleChanged != null)
					{
						// Throw the event that the Visibility was changed
						VisibleChanged(this, null);
					}
				}
			}
		}

		/// <summary>
		/// Get / Set if this Particle System should Update itself and its Particles or not.
		/// <para>NOTE: Setting this to false causes the Update() function to not update anything, including the
        /// BeforeUpdate() and AfterUpdate() function not to be called.</para>
		/// </summary>
		[DPSFViewerParameter(Description="Toggle whether the particle system should be updated or not.", Group="DPSF")]
		public bool Enabled
		{
			get { return mbPerformUpdates; }
			set
			{ 
				mbPerformUpdates = value;

				// If there is a function to catch the event
				if (EnabledChanged != null)
				{
					// Throw the event that the Enabled state was changed
					EnabledChanged(this, null);
				}
			}
		}

		/// <summary>
		/// The Order in which the Particle System should be Updated relative to other 
		/// DPSF Particle Systems in the same Particle System Manager. Particle Systems 
		/// are Updated in ascending order according to their Update Order (i.e. lowest first).
		/// <para>NOTE: The Update Order is one of the few properties that is not reset when
		/// the particle system is initialized or destroyed.</para>
		/// </summary>
		public int UpdateOrder
		{
			get { return miUpdateOrder; }
			set
			{ 
				miUpdateOrder = value;

				// If there is a function to catch the event
				if (UpdateOrderChanged != null)
				{
					// Throw the event that the Update Order was changed
					UpdateOrderChanged(this, null);
				}
			}
		}

		/// <summary>
		/// The Order in which the Particle System should be Drawn relative to other
		/// DPSF Particle Systems in the same Particle System Manager. Particle Systems
		/// are Drawn in ascending order according to their Draw Order (i.e. lowest first).
		/// <para>NOTE: The Draw Order is one of the few properties that is not reset when
		/// the particle system is initialized or destroyed.</para>
		/// </summary>
		public int DrawOrder
		{
			get { return miDrawOrder; }
			set 
			{ 
				miDrawOrder = value; 

				// If there is a function to catch the event
				if (DrawOrderChanged != null)
				{
					// Throw the event that the Draw Order was changed
					DrawOrderChanged(this, null);
				}
			}
		}

		/// <summary>
		/// Get the Game object set in the constructor, if one was given.
		/// </summary>
		public Game Game
		{
			get { return mcGame; }
		}

		/// <summary>
		/// Get / Set the Graphics Device to draw to
		/// </summary>
		public GraphicsDevice GraphicsDevice
		{
			get { return mcGraphicsDevice; }
			set
			{
				// If a valid Graphics Device was specified
				if (value != null || ParticleType == ParticleTypes.NoDisplay)
				{
					mcGraphicsDevice = value;
				}
				// Else an invalid Graphics Device was specified
				else
				{
					throw new DPSFArgumentNullException("GraphicsDevice", "The specified Graphics Device is null. A valid Graphics Device is required.");
				}
			}
		}
	// Else we are inheriting DrawableGameComponent
	#else
		// Hide the Initialize() function
		private new void Initialize() { base.Initialize(); }
	#endif

		/// <summary>
		/// Sets the Graphics Device to use to the given graphics device.
		/// <para>NOTE: This only has an effect if the particle system does not inherit from DrawableGameComponent
		/// (i.e. InheritsDrawableGameComponent == false. i.e. using the DPSF.dll, not DPSFAsDrawableGameComponent.dll), since 
		/// the Graphics Device is read-only when inheriting from DrawableGameComponent. The Game object's Graphics Device
		/// is always used when inheriting from DrawableGameComponent.</para>
		/// </summary>
		/// <param name="graphicsDevice">The graphics device to use.</param>
		public void SetGraphicsDevice(GraphicsDevice graphicsDevice)
		{
		// If not inheriting DrawableGameComponent
		#if (!DPSFAsDrawableGameComponent)
			this.GraphicsDevice = graphicsDevice;
		#endif
		}

		/// <summary>
		/// Get if the Particle System is inheriting from DrawableGameComponent or not.
		/// <para>If inheriting from DrawableGameComponent, the Particle Systems
		/// are automatically added to the given Game object's Components and the
		/// Update() and Draw() functions are automatically called by the
		/// Game object when it updates and draws the rest of its Components.
		/// If the Update() and Draw() functions are called by the user anyways,
		/// they will exit without performing any operations, so it is suggested
		/// to include them anyways to make switching between inheriting and
		/// not inheriting from DrawableGameComponent seemless; just be aware
		/// that the updates and draws are actually being performed when the
		/// Game object is told to update and draw (i.e. when base.Update() and base.Draw()
		/// are called), not when these functions are being called.</para>
		/// </summary>
		public bool InheritsDrawableGameComponent
		{
		// If inheriting DrawableGameComponent
		#if (DPSFAsDrawableGameComponent)
			get { return true; }
		// Else not inheriting DrawableGameComponent
		#else
			get { return false; }
		#endif
		}

		/// <summary>
		/// Get the unique ID of this Particle System.
		/// <para>NOTE: Each Particle System is automatically assigned a unique ID when it is instanciated.</para>
		/// </summary>
		public int ID
		{
			get { return miID; }
		}

		/// <summary>
		/// Get / Set the Type of Particle System this is. This is a user provided value that you can use for whatever
		/// purpose you want; it is not used by the built-in DPSF functionality in any way.
		/// </summary>
		public int Type
		{
			get { return miType; }
			set { miType = value; }
		}

		/// <summary>
		/// Get the Name of the Class that this Particle System is using. This can be used to 
		/// check what type of Particle System this is at run-time.
		/// </summary>
		[DPSFViewerParameter(Description = "Get the Name of the Class that this Particle System is using.", Group = "DPSF")]
		public string ClassName
		{
			get { return GetType().Name; }
		}

		/// <summary>
		/// Get / Set the Content Manager to use to load Textures and Effects
		/// </summary>
		public ContentManager ContentManager
		{
			get 
			{
				if (mcContentManager != null)
					return mcContentManager;
				else
					throw new DPSFNullReferenceException("The Content Manager is trying to be accessed, but is null. Be sure you have Initialized the particle system and provided a valid Content Manager.");
			}

			set
			{
				// If a valid Content Manager was given
				if (value != null || ParticleType == ParticleTypes.NoDisplay)
				{
					mcContentManager = value;
				}
				// Else an invalid Content Manager was given
				else
				{
					throw new DPSFArgumentNullException("ContentManager", "The specified Content Manager is null. A valid Content Manager is required.");
				}
			}
		}

		/// <summary>
		/// Get if we will be using the regular 32-bit Index Buffer (Integer) or the 16-bit Index Buffer (Short).
		/// </summary>
		protected bool IsUsingIntegerIndexBuffer
		{
			get { return _SuseIntIndexBuffer ?? false; }
		}

		/// <summary>
		/// Get / Set the 32-bit Index Buffer values. 
		/// <para>The Index Buffer is typically used when drawing Quads in the HiDef profile (except with MonoGame WinRT, which uses HiDef but only supports IndexBufferShort).</para>
		/// </summary>
		protected int[] IndexBuffer
		{
			get { return miaIndexBufferArray; }
			set { miaIndexBufferArray = value; }
		}

		/// <summary>
		/// Get / Set the 16-bit Index Buffer values.
		/// <para>The Index Buffer Short is typically used when drawing Quads in the Reach profile.</para>
		/// </summary>
		protected short[] IndexBufferShort
		{
			get { return msaIndexBufferReachArray; }
			set { msaIndexBufferReachArray = value; }
		}

		/// <summary>
		/// Get / Set the current position in the Index Buffer
		/// </summary>
		protected int IndexBufferIndex
		{
			get { return miIndexBufferIndex; }
			set { miIndexBufferIndex = value; }
		}

		/// <summary>
		/// Particle Events may be used to update Particles
		/// </summary>
		public CParticleEvents ParticleEvents
		{
			get 
			{
				if (mcParticleEvents != null)
					return mcParticleEvents; 
				else
					throw new DPSFNullReferenceException("The ParticleEvents property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
			}
		}

		/// <summary>
		/// Particle System Events may be used to update the Particle System
		/// </summary>
		public CParticleSystemEvents ParticleSystemEvents
		{
			get 
			{ 
				if (mcParticleSystemEvents != null)
					return mcParticleSystemEvents; 
				else
					throw new DPSFNullReferenceException("The ParticleSystemEvents property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
			}
		}

		/// <summary>
		/// Get the render properties used to draw the particles.
		/// </summary>
		public RenderProperties RenderProperties
		{
			get { return mcRenderProperties; }
		}

		/// <summary>
		/// Returns if this particle system is dependent on an external Sprite Batch to draw its particles or not.
		/// <para>If false, the particle system will use its own SpriteBatch to draw its particles.</para>
		/// <para>If true, then you must call SpriteBatch.Begin() before calling ParticleSystem.Draw() to
		/// draw the particle system, and then call SpriteBatch.End() when done drawing the particle system, where
		/// the SpriteBatch referred to here is the one you passed into the InitializeSpriteParticleSystem() function.</para>
		/// <para>NOTE: This property only applies to Sprite particle systems.</para>
		/// </summary>
		public bool UsingExternalSpriteBatchToDrawParticles { get; private set; }

		/// <summary>
		/// Returns the SpriteBatch used to draw the Sprite particles.
		/// <para>NOTE: If this is not a Sprite particle system, this will return null.</para>
		/// </summary>
		public SpriteBatch SpriteBatch
		{
			get { return mcSpriteBatch; }
		}

		/// <summary>
		/// The Sprite Batch drawing Settings used in the Sprite Batch's Begin() function call.
		/// <para>NOTE: These settings are only available for Sprite particle systems, and only for
		/// the Sprite particle systems using their own SpriteBatch (i.e. UsingExternalSpriteBatchToDrawParticles = false).</para>
		/// </summary>
		public SpriteBatchSettings SpriteBatchSettings
		{
			get
			{
				if (mcSpriteBatchSettings != null)
					return mcSpriteBatchSettings;
				else
					throw new DPSFNullReferenceException("The SpriteBatchSettings property is trying to be accessed, but is null. Be sure you have Initialized the particle system. Also, this property is only available when using a Sprite particle system, and not Using and External Sprite Batch To Draw the Particles.");
			}
		}

		/// <summary>
		/// The Settings used to control the Automatic Memory Manager
		/// </summary>
		public AutoMemoryManagerSettings AutoMemoryManagerSettings
		{
			get 
			{ 
				if (mcAutoMemoryManagerSettings != null)
					return mcAutoMemoryManagerSettings; 
				else
					throw new DPSFNullReferenceException("The AutoMemoryManagerSettings property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
			}
		}

		/// <summary>
		/// The Emitter is used to automatically generate new Particles.
		/// <para>NOTE: This is just a pointer to one of the ParticleEmitters in the Emitters ParticleEmitterCollection.</para>
		/// <para>NOTE: If you set this to a ParticleEmitter that is not in the Emitters collection, it will be added to it.</para>
		/// <para>During the particle system Update() this Emitter property is updated to point to the ParticleEmitter in the Emitters collection that is being updated.
        /// This allows you to use this property to reference the ParticleEmitter that is actually being used to add particles to the particle system.</para>
		/// </summary>
		[DPSFViewerParameter(Description = "The Emitter used by the particle system.", Group = "DPSF")]
		public ParticleEmitter Emitter
		{
			get 
			{
				if (_emitter != null)
					return _emitter;

				if (_emitters.Count <= 0)
					_emitters.Add();

				_emitter = _emitters[_emitters.IDs[0]];
				return _emitter;
			}

			set 
			{
				if (value == null)
				    throw new DPSFArgumentNullException("Emitter", "An invalid Emitter was specified. The Emitter cannot be null.");

				_emitter = value;

				// Add the given emitter to our collection of Emitters if it isn't already in there.
				if (!_emitters.Contains(_emitter))
					_emitters.Add(_emitter);
			}
		}

		/// <summary>
		/// The Emitters used to automatically generate new Particles for this Particle System.
		/// <para>Each particle system Update() will loop through all Emitters in this collection and add their new particles to this particle system.</para>
		/// <para>During the particle system Update() the Emitter property is updated to point to the ParticleEmitter in this collection that is being updated.</para>
		/// </summary>
		public ParticleEmitterCollection Emitters
		{
			get
			{
				if (_emitters == null)
					throw new DPSFNullReferenceException("The Emitters property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");

				return _emitters;
			}
		}

		/// <summary>
		/// Handles the AllEmittersRemoved event of the _emitters control to make sure we set the Emitter to null whenever the Emitters collection is cleared.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void _emitters_AllEmittersRemoved(object sender, EventArgs e)
		{
			// The Emitter is just a pointer to a ParticleEmitter in the Emitters collection, and since the collection is now empty, the Emitter should not be pointing to anything.
			_emitter = null;
		}

		/// <summary>
		/// Get a RandomNumbers object used to generate Random Numbers
		/// </summary>
		public RandomNumbers RandomNumber
		{
			get { return DPSFHelper.RandomNumber; }
		}

		/// <summary>
		/// Get / Set the World Matrix to use for drawing 3D Particles
		/// </summary>
		public Matrix World
		{
			get { return mcWorld; }
			set { mcWorld = value; }
		}

		/// <summary>
		/// Get / Set the View Matrix to use for drawing 3D Particles
		/// </summary>
		public Matrix View
		{
			get { return mcView; }
			set { mcView = value; }
		}

		/// <summary>
		/// Get / Set the Projection Matrix to use for drawing 3D Particles
		/// </summary>
		public Matrix Projection
		{
			get { return mcProjection; }
			set { mcProjection = value; }
		}

		/// <summary>
		/// Gets the result of multiplying the World, View, and Projection matrices.
		/// </summary>
		public Matrix WorldViewProjection
		{
			get { return Matrix.Multiply(Matrix.Multiply(this.World, this.View), this.Projection); }
		}

		/// <summary>
		/// Set the World, View, and Projection matrices for this Particle System.
		/// <para>NOTE: Sprite particle systems are not affected by the World, View, and Projection matrices.</para>
		/// </summary>
		/// <param name="cWorld">The World matrix</param>
		/// <param name="cView">The View matrix</param>
		/// <param name="cProjection">The Projection matrix</param>
		public void SetWorldViewProjectionMatrices(Matrix cWorld, Matrix cView, Matrix cProjection)
		{
			World = cWorld;
			View = cView;
			Projection = cProjection;
		}

		/// <summary>
		/// Set the function to use to copy a Particle's renderable properties into the Vertex Buffer.
		/// <para>NOTE: VertexUpdateFunction will not be changed if null value is given.</para>
		/// </summary>
		public UpdateVertexDelegate VertexUpdateFunction
		{
			set
			{
				// If an invalid Vertex Update Function is given
				if (value == null)
				{
					// If this Type of Particle doesn't require a Vertex Update Function
					if (ParticleType == ParticleTypes.None || ParticleType == ParticleTypes.NoDisplay || ParticleType == ParticleTypes.Sprite)
					{
						// Set the Vertex Update Function to null
						mcVertexUpdateFunction = null;
					}
					// Else a valid Vertex Update Function is required
					else
					{
						throw new DPSFArgumentNullException("VertexUpdateFunction", "The specified Vertex Update Function is null. A valid Vertex Update Function is required to draw the current Type of Particles.");
					}
				}
				// Else a valid Vertex Update Function was given
				else
				{
					// Set the function used to Update a Vertex
					mcVertexUpdateFunction = new UpdateVertexDelegate(value);
				}
			}
		}

		/// <summary>
		/// Sets the function to use to Initialize a Particle's properties.
		/// </summary>
		public InitializeParticleDelegate ParticleInitializationFunction
		{
			set
			{
				// If a valid Function was given
				if (value != null)
				{
					// Set the Function to use to Initialize Particles
					mcParticleInitializationFunction = new InitializeParticleDelegate(value);
				}
				// Else no Function was given
				else
				{
					// So set to not use a Particle Initialization Function
					mcParticleInitializationFunction = null;
				}
			}
		}

		/// <summary>
		/// Sets the Effect to be the default type for this type of particle system.
		/// This is done automatically when the particle system is initialized.
		/// <para>Default effect for each particle type is:</para>
		/// <list type="number">
		///     <item><description>NoDisplay and Sprite - null.</description></item>
		///     <item><description>Quad - BasicEffect.</description></item>
		///     <item><description>TexturedQuad - AlphaTestEffect.</description></item>
		/// </list>
		/// </summary>
		public void SetDefaultEffect()
		{
			// Assign the default Effect to use based on the type of particle system this is
			switch (meParticleType)
			{
				// Leave the Effect as null with no technique if this is a No Display or Sprite particle system	
				default: break;

				// AlphaTestEffect requires a texture, so use the BasicEffect for Quad particle systems.
				case ParticleTypes.Quad:
					this.Effect = DPSFDefaultSettings.UseSharedEffectForAllParticleSystems ? DPSFDefaultSettings.GetSharedBasicEffect(this.GraphicsDevice) : new BasicEffect(this.GraphicsDevice);
					break;

				case ParticleTypes.TexturedQuad:
					this.Effect = DPSFDefaultSettings.UseSharedEffectForAllParticleSystems ? DPSFDefaultSettings.GetSharedAlphaTestEffect(this.GraphicsDevice) : new AlphaTestEffect(this.GraphicsDevice); 
					break;
			}
		}

		/// <summary>
		/// Sets the Effect and Technique to use to draw the Particles.
		/// <para>NOTE: This will automatically set the DeserializationEffectPath property to the given sEffect.</para>
		/// <para>NOTE: This will automatically set the DeserializationTechniqueName property to the given sTechnique.</para>
		/// </summary>
		/// <param name="sEffect">The Asset Name of the Effect to use</param>
		/// <param name="sTechnique">The name of the Effect's Technique to use</param>
		public void SetEffectAndTechnique(string sEffect, string sTechnique)
		{
			// If the Effect to use is invalid, throw an exception about it
			if (string.IsNullOrEmpty(sEffect))
				throw new DPSFArgumentNullException("sEffect", "The Effect string supplied is null or an empty string. The Effect to use cannot be null.");

			// Load the Effect
			Effect cEffect = ContentManager.Load<Effect>(sEffect);

			// Record which Effect was loaded
			DeserializationEffectPath = sEffect;

			// Set the Effect and Technique to use
			SetEffectAndTechnique(cEffect, sTechnique);
		}

		/// <summary>
		/// Sets the Effect and Technique to use to draw the Particles.
		/// <para>NOTE: This will automatically set the DeserializationTechniqueName property to the given sTechnique.</para>
		/// </summary>
		/// <param name="cEffect">The Effect to use</param>
		/// <param name="sTechnique">The name of the Effect's Technique to use</param>
		public void SetEffectAndTechnique(Effect cEffect, string sTechnique)
		{
			// Set the Effect
			Effect = cEffect;

			// Set the Technique
			SetTechnique(sTechnique);
		}

		/// <summary>
		/// Get / Set the Effect to use to draw the Particles
		/// </summary>
		public Effect Effect
		{
			get { return mcEffect; }
			set
			{
				// If the Effect should be set to null
				if (value == null)
				{
					// If this Type of Particle does not require an Effect to be used
					if (ParticleType == ParticleTypes.None || ParticleType == ParticleTypes.NoDisplay || ParticleType == ParticleTypes.Sprite)
					{
						// Set the Effect to null
						mcEffect = null;
					}
					// Else an Effect must be specified for this Type of Particle
					else
					{
						throw new DPSFArgumentNullException("Effect", "Specified Effect to use is null. A valid Effect must be used to draw the current Type of Particles.");
					}
				}
				// Else a valid Effect was specified
				else
				{
					// If we have several particle systems, the content manager will return
					// a single shared effect instance to them all. But we want to pre-configure
					// the effect with parameters that are specific to this particular
					// particle system. By cloning the effect, we prevent one particle system
					// from stomping over the parameter settings of another.
					mcEffect = value.Clone();
				}
			}
		}

		/// <summary>
		/// Set which Technique of the current Effect to use to draw the Particles.
		/// <para>NOTE: This will automatically set the DeserializationTechniqueName property to the given sTechnique.</para>
		/// </summary>
		/// <param name="sTechnique">The name of the Effect's Technique to use</param>
		public void SetTechnique(string sTechnique)
		{
			// If the Effect hasn't been set yet, throw an exception about it.
			if (mcEffect == null)
				throw new DPSFInvalidOperationException("Effect is null when trying to specify the Technique to use. The Effect must be set before specifying the Technique.");

			// If the specified Technique is invalid, throw an exception about it.
			if (string.IsNullOrEmpty(sTechnique))
				throw new DPSFArgumentNullException("sTechnique", "The specified Technique to use is invalid. This parameter cannot be null or an empty string.");

			// Else both the Effect and Technique to use are valid
			// Store which Technique to use to draw
			mcEffect.CurrentTechnique = mcEffect.Techniques[sTechnique];

			// Record which Technique was specified
			DeserializationTechniqueName = sTechnique;
		}

		/// <summary>
		/// Get / Set which Technique of the current Effect to use to draw the Particles
		/// </summary>
		public EffectTechnique Technique
		{
			get 
			{
				// If the Effect has been set already
				if (mcEffect != null)
				{
					return mcEffect.CurrentTechnique;
				}
				// Else the Effect has not been set yet, so return null
				return null;
			}
			set 
			{
				// If the Effect hasn't been set yet, throw an exception about it.
				if (mcEffect == null)
					throw new DPSFInvalidOperationException("Effect is null when trying to specify the Technique to use. The Effect must be set before specifying the Technique.");

				// If the Technique to use is invalid, throw an exception about it.
				if (value == null)
					throw new DPSFArgumentNullException("Technique", "An invalid Technique to use was specified. The Technique to use cannot be null.");

				// Set the Technique to use
				mcEffect.CurrentTechnique = value;
			}
		}

		/// <summary>
		/// Set the Texture to use to draw the Particles
		/// </summary>
		/// <param name="sTexture">The Asset Name of the texture file to use (found in
		/// the XNA Properties of the file)</param>
		public void SetTexture(string sTexture)
		{
			// If the Texture to use is invalid
			if (string.IsNullOrEmpty(sTexture))
			{
				// If a Texture is required for this Type of Particle
				if (ParticleType == ParticleTypes.Sprite || ParticleType == ParticleTypes.TexturedQuad)
				{
					throw new DPSFArgumentNullException("sTexture", "Specified Texture to use is null. A valid Texture must be set to draw the current Type of Particles.");
				}
				// Else a Texture is not required
				else
				{
					// Set the Texture to null
					Texture = null;
					DeserializationTexturePath = null;
				}
			}
			// Else the Texture to use is valid
			else
			{
				try
				{
					// Save a handle to the Texture and save the Name of the Texture
					mcTexture = ContentManager.Load<Texture2D>(sTexture);
					mcTexture.Name = sTexture;

					// Record which Texture was loaded
					DeserializationTexturePath = sTexture;
				}
				catch (System.Collections.Generic.KeyNotFoundException e)
				{
					throw new DPSFInvalidOperationException("There was a problem loading the texture \"" + sTexture + "\". Did you Dispose() this resource earlier somewhere else by accident?", e);
				}
			}
		}

		/// <summary>
		/// Get / Set the Texture to use to draw the Particles
		/// </summary>
		public Texture2D Texture
		{
			get { return mcTexture; }
			set
			{
				// If the Texture should be set to null
				if (value == null)
				{
					// If a Texture is required for this Type of Particle
					if (ParticleType == ParticleTypes.Sprite || ParticleType == ParticleTypes.TexturedQuad)
					{
						throw new DPSFArgumentNullException("sTexture", "Specified Texture to use is null. A valid Texture must be set to draw the current Type of Particles.");
					}
					// Else a Texture is not required
					else
					{
						// Set the Texture to null
						mcTexture = null;
					}
				}
				// Else a valid Texture was specified
				else
				{
					mcTexture = value;
				}
			}
		}

		/// <summary>
		/// Get / Set how fast the Particle System Simulation should run.
		/// <para>Example: 1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.</para>
		/// <para>NOTE: If a negative value is specified, the Speed Scale is set 
		/// to zero (pauses the simulation; has same effect as Enabled = false).</para>
		/// </summary>
		public float SimulationSpeed
		{
			get { return mfSimulationSpeed; }
			set
			{
				// Make sure the Simulation Speed is not negative
				if (value < 0.0f)
				{
					mfSimulationSpeed = 0.0f;
				}
				else
				{
					mfSimulationSpeed = value;
				}
			}
		}

		/// <summary>
		/// Get / Set how fast the Particle System Simulation should run to look "normal".
		/// <para>1.0 = normal speed, 0.5 = half speed, 2.0 = double speed.</para>
		/// <para>This is provided as a way of speeding up / slowing down the simulation to have 
		/// it look as desired, without having to rescale all of the particle velocities, etc. This allows
		/// you to use the exact same particle system class to create two particle systems, and then have one run
		/// slower or faster than the other, creating two different effects. If you then wanted to speed up or slow down
		/// both effects (i.e. particle systems), you could adjust the SimulationSpeed property on both particle systems 
		/// without having to worry about adjusting this property at all to get the effects back to normal speed; just reset 
		/// the SimulationSpeed property you changed back to 1.0.</para>
		/// <para>NOTE: If a negative value is specified, the Internal Simulation Speed is set to zero 
		/// (pauses the simulation; has the same effect as Enabled = false).</para>
		/// </summary>
		public float InternalSimulationSpeed
		{
			get { return mfInternalSimulationSpeed; }
			set
			{
				// Make sure the Internal Simulation Speed is not negative
				if (value < 0.0f)
				{
					mfInternalSimulationSpeed = 0.0f;
				}
				else
				{
					mfInternalSimulationSpeed = value;
				}
			}
		}

		/// <summary>
		/// Specify how often the Particle System should be Updated.
		/// <para>NOTE: Specifying a value of zero (default) will cause the Particle 
		/// System to be Updated every time the Update() function is called 
		/// (i.e. as often as possible).</para>
		/// <para>NOTE: If the Update() function is not called often enough to
		/// keep up with this specified Update rate, the Update function
		/// updates the Particle Systems as often as possible.</para>
		/// </summary>
		public int UpdatesPerSecond
		{
			get
			{
				// If the Particle Systems should be Updated as often as possible
				if (mfTimeToWaitBetweenUpdates == 0.0f)
				{
					return 0;
				}
				// Else return how many times the Particle Systems are being Updated Per Second
				else
				{
					return (int)(1.0f / mfTimeToWaitBetweenUpdates);
				}
			}

			set
			{
				// If the Particle Systems should be Updated as often as possible
				if (value <= 0)
				{
					mfTimeToWaitBetweenUpdates = 0.0f;
				}
				// Else calculate how much Time should elapse between Particle System Updates
				else
				{
					mfTimeToWaitBetweenUpdates = (1.0f / value);
				}
			}
		}

		/// <summary>
		/// Get / Set if performance timings should be measured or not, such as how long it takes to perform updates and draws.
		/// <para>This should be disabled before building a release version of your application.</para>
		/// <para>Note: Performance profiling is not available on the Reach profile, so this will always return False on the Reach profile.</para>
		/// </summary>
		public bool PerformanceProfilingIsEnabled
		{
			get { return _performanceProfilingIsEnabled; }
			set 
			{
				// Performance profiling is not available on the Reach Profile.
				if (GraphicsDevice != null && GraphicsDevice.GraphicsProfile == GraphicsProfile.Reach)
					_performanceProfilingIsEnabled = false;
				else
					_performanceProfilingIsEnabled = value;
// The Reach profile does not have access to the System.Diagnostics namespace, so it does not know about the Stopwatch class.
#if (!WINDOWS_PHONE)
				// If Performance Profiling is enabled
				if (_performanceProfilingIsEnabled)
				{
					if (_performanceProfilingStopwatch == null)
						_performanceProfilingStopwatch = new Stopwatch();
				}
				else
				{
					_performanceProfilingStopwatch = null;
				}
#endif
			}
		}

		/// <summary>
		/// Get how long (in milliseconds) it took to perform the last Update() function call.
		/// <para>Returns 0 if Performance Profiling is not Enabled.</para>
		/// </summary>
		public double PerformanceTimeToDoUpdateInMilliseconds { get; private set; }

		/// <summary>
		/// Get how long (in milliseconds) it took to perform the last Draw() function call.
		/// <para>Returns 0 if Performance Profiling is not Enabled.</para>
		/// </summary>
		public double PerformanceTimeToDoDrawInMilliseconds { get; private set; }

		/// <summary>
		/// The Particle System Manager whose properties (SimulationSpeed and 
		/// UpdatesPerSecond) this particle system should follow.  If null is not specified,
		/// the Manager's properties will be copied into this particle system immediately.
		/// <para>NOTE: This Particle System's properties will only clone the Manager's properties
		/// if the Manager's properties are Enabled. For example, the Manager's SimulationSpeed
		/// will only be copied to this Particle System if the Manager's SimulationSpeedIsEnabled
		/// property is true.</para>
		/// <para>NOTE: This value is automatically set to the last Particle System Manager the 
		/// Particle System is added to.</para>
		/// </summary>
		public ParticleSystemManager ParticleSystemManagerToCopyPropertiesFrom
		{
			get { return mcParticleSystemManager; }
			set
			{
				// Save a handle to the Particle System Manager to copy from
				mcParticleSystemManager = value;

				// If there is a Particle System Manager whose properties should be copied
				if (mcParticleSystemManager != null)
				{
					// If the Particle System Manager's SimulationSpeed should be copied to this Particle System
					if (mcParticleSystemManager.SimulationSpeedIsEnabled)
					{
						SimulationSpeed = mcParticleSystemManager.SimulationSpeed;
					}

					// If the Particle System Manager's UpdatesPerSecond should be copied to this Particle System
					if (mcParticleSystemManager.UpdatesPerSecondIsEnabled)
					{
						UpdatesPerSecond = mcParticleSystemManager.UpdatesPerSecond;
					}
				}
			}
		}

		/// <summary>
		/// Get the type of Particles that this Particle System should draw.
		/// </summary>
		public ParticleTypes ParticleType
		{
			get { return meParticleType; }
			private set { meParticleType = value; }
		}

		/// <summary>
		/// This allocates the proper amount of space for the Particles and initializes the variables used to draw the Type of Particles specified. 
		/// For example, if using Textured Quads extra space will need to be allocated to hold the Particles, as each Quad Particle requires four 
		/// vertices, not one like Point Sprites. Also, the Index Buffer would be initialized, as it is required to draw Quads.
		/// </summary>
		private bool InitializeParticleArrays()
		{
			// If the Number of Particles To Allocate Memory for has not been specified yet, we can't initialize the arrays so just return false.
			if (mcParticles == null)
				return false;

			// Get the maximum Number of Particles this Particle System can hold right now
			int iMaxNumberOfParticles = mcParticles.Length;

			// Initialize the proper arrays based on the type of Particles to draw
			switch (meParticleType)
			{
				// If the Particle Type is not set or is set to NoDisplay
				default:
				case ParticleTypes.None:
				case ParticleTypes.NoDisplay:
					break;

				// If we are using Quads or Textured Quads
				case ParticleTypes.Quad:
				case ParticleTypes.TexturedQuad:
					// Initialize the Vertex and Index Buffer
					mcParticleVerticesToDraw = new Vertex[iMaxNumberOfParticles * 4];
					// Only initialize the Index Buffer that we will be using.
					if (IsUsingIntegerIndexBuffer)
					{
						miaIndexBufferArray = new int[iMaxNumberOfParticles * 6];
					}
					else
					{
						msaIndexBufferReachArray = new short[iMaxNumberOfParticles * 6];
					}

// If we're on the Xbox, calculate the max number of particles it can display in a single Draw() call.
// This only needs to be done for Quads since NoDisplay particles aren't drawn, and Sprite particles are drawn by a SpriteBatch.
#if (XBOX)
                    if (miVertexSizeInBytes > 0)
					    miMaxParticlesThatXboxCanDrawAtOnce = MAX_MEMORY_IN_BYTES_THAT_XBOX_CAN_DRAW / (miVertexSizeInBytes * 4);
                    // The only time this Else clause should be hit is when using a NoDisplay or Sprite particle system (since Sprite uses a SpriteBatch to draw and not a Vertex), so 
                    // assign an arbitrary value to the MaxParticlesThatXboxCanDrawAtOnce since it should never actually be used to draw anything.
                    else
                        miMaxParticlesThatXboxCanDrawAtOnce = 100;
#endif
                    break;

				// If we are using the Sprite Batch to draw Particles
				case ParticleTypes.Sprite:
					// Initialize the array used to hold the Sprites to draw
					mcParticleSpritesToDraw = new Particle[iMaxNumberOfParticles];
					break;
			}

			// Initialize all Particles and add them to the Inactive Particles List
			int iNumberOfParticles = mcParticles.Length;
			for (int iIndex = 0; iIndex < iNumberOfParticles; iIndex++)
			{
				// Initialize the Particle
				mcParticles[iIndex] = new Particle();
				mcInactiveParticlesList.AddFirst(mcParticles[iIndex]);
			}

			// If the Vertex Buffer is used for the current Type of Particles
			if (mcParticleVerticesToDraw != null)
			{
				// Initialize the Vertex Buffer
				int iNumberOfVertices = mcParticleVerticesToDraw.Length;
				for (int iIndex = 0; iIndex < iNumberOfVertices; iIndex++)
				{
					// Initialize the Particle's Vertex
					mcParticleVerticesToDraw[iIndex] = new Vertex();
				}
			}

			// Make sure we don't try to Draw any Particles until the Vertex Buffer has been refilled
			miNumberOfParticlesToDraw = 0;

			// Return that the particle arrays were resized successfully
			return true;
		}

		/// <summary>
		/// Get / Set the absolute Number of Particles to Allocate Memory for.
		/// <para>NOTE: This value must be greater than or equal to zero.</para>
		/// <para>NOTE: Even if this many particles aren't used, the space for this many Particles 
		/// is still allocated in memory.</para>
		/// </summary>
		public int NumberOfParticlesAllocatedInMemory
		{
			// Return the Max Number Of Particles  we can hold right now if it has been set, otherwise return zero.
			get { return mcParticles != null ? mcParticles.Length : 0; }
			set
			{
				// Temporarily store the New Max Number of Particles
				int iNewMaxNumberOfParticles = value;

				// If a valid Max Number of Particles was specified
				if (iNewMaxNumberOfParticles >= 0)
				{
					// Variable to tell if there are Particles that need to be copied to the new Active Particles List or not
					bool bThereAreOldParticlesToCopy = (mcActiveParticlesList != null && mcActiveParticlesList.Count > 0);

					// If we need to copy Particles over to the new Active Particles List, save a handle to
					// the current Active Particles List so that they aren't garbage collected right away.
					LinkedList<Particle> cOldActiveParticles = bThereAreOldParticlesToCopy ? mcActiveParticlesList : null;

					// If we need to copy Particles over to the new Active Particles List, save handles 
					// to the Vertex and Index Buffers as well to restore any vertices that
					// are currently being drawn. This is done to avoid a flicker, since these buffers
					// won't be populated again until the next Update() call, but Draw() may get called
					// several times before then.
					Vertex[] cOldParticleVertices = bThereAreOldParticlesToCopy ? mcParticleVerticesToDraw : null;
					int[] iaOldIndexBuffer = bThereAreOldParticlesToCopy ? miaIndexBufferArray : null;
					short[] saOldIndexBufferReach = bThereAreOldParticlesToCopy ? msaIndexBufferReachArray : null;
					int iNumberOfParticlesBeingDrawn = miNumberOfParticlesToDraw;
					Particle[] cOldParticleSpritesToDraw = bThereAreOldParticlesToCopy ? mcParticleSpritesToDraw : null;


					// Initialize the size of the array to the maximum number of Particles
					mcParticles = new Particle[iNewMaxNumberOfParticles];

					// Initialize the Particle lists
					mcActiveParticlesList = new LinkedList<Particle>();
					mcInactiveParticlesList = new LinkedList<Particle>();

					// Allocate the right amount of space for this Type of Particle
					// and initialize the variables to hold the Particle vertices.
					InitializeParticleArrays();


					// If there are Particles to copy to the new Active Particle List
					if (bThereAreOldParticlesToCopy)
					{
						// If there are more Particles Being Drawn than there are now allowed
						if (iNumberOfParticlesBeingDrawn > iNewMaxNumberOfParticles)
						{
							// Set the Number Of Particles Being Drawn to the new Max Number Of Particles
							iNumberOfParticlesBeingDrawn = iNewMaxNumberOfParticles;
						}

						// Loop through all of the old Active Particles and add them to the
						// new Active Particles List. We loop through them in reverse order
						// to maintain the order of the Active Particles, since the AddParticle() 
						// function adds new Particles to the front of the List.
						LinkedListNode<Particle> cNode = cOldActiveParticles.Last;
						while (cNode != null && AddParticle(cNode.Value))
						{
							// Move to the previous Active Particle
							cNode = cNode.Previous;
						}

						// If there are Particles currently being drawn
						if (iNumberOfParticlesBeingDrawn > 0)
						{
							// Copy the old draw information to the new draw buffers
							switch (meParticleType)
							{
								// If the Particle Type is not set
								default:
								case ParticleTypes.None:
									// Do nothing
								break;

								// If we are using Quads or Textured Quads
								case ParticleTypes.Quad:
								case ParticleTypes.TexturedQuad:
									// Copy the old Vertices into the new Vertices array
									int iNumberOfVerticesToCopy = iNumberOfParticlesBeingDrawn * 4;
									for (int iIndex = 0; iIndex < iNumberOfVerticesToCopy; iIndex++)
									{
										mcParticleVerticesToDraw[iIndex] = cOldParticleVertices[iIndex];
									}

									// If we're using the full Int Index Buffer, copy it.
									if (IsUsingIntegerIndexBuffer)
									{
										// Copy the old Index Buffer into the new one
										int lengthOfIndexBuffer = (miaIndexBufferArray == null) ? 0 : miaIndexBufferArray.Length;
										int iNumberOfIndicesToCopy = Math.Min(iNumberOfParticlesBeingDrawn * 6, lengthOfIndexBuffer);
										for (int iIndex = 0; iIndex < iNumberOfIndicesToCopy; iIndex++)
										{
											miaIndexBufferArray[iIndex] = iaOldIndexBuffer[iIndex];
										}
									}
									// Else we're using the Short Index Buffer, so copy it.
									else
									{
										// Copy the old Reach Index Buffer into the new one
										int lengthOfIndexBuffer = (msaIndexBufferReachArray == null) ? 0 : msaIndexBufferReachArray.Length;
										int iNumberOfIndicesToCopy = Math.Min(iNumberOfParticlesBeingDrawn * 6, lengthOfIndexBuffer);
										for (int iIndex = 0; iIndex < iNumberOfIndicesToCopy; iIndex++)
										{
											msaIndexBufferReachArray[iIndex] = saOldIndexBufferReach[iIndex];
										}
									}
								break;

								// If we are using the Sprite Batch to draw Particles
								case ParticleTypes.Sprite:
									// Copy the old Sprite Particles into the new Sprite Particles array
									for (int iIndex = 0; iIndex < iNumberOfParticlesBeingDrawn; iIndex++)
									{
										mcParticleSpritesToDraw[iIndex] = cOldParticleSpritesToDraw[iIndex];
									}
								break;
							}

							// Record how many Particles should still be drawn
							miNumberOfParticlesToDraw = iNumberOfParticlesBeingDrawn;
						}
					}
				}
				// Else an invalid Max Number of Particles was specified
				else
				{
					throw new DPSFArgumentException("MaxNumberOfParticles", "The specified Max Number Of Particles is less than or equal to zero. The Max Number Of Particles must be greater than zero.");
				}
			}
		}

        /// <summary>
        /// Get the number of particles that memory has been allocated for, for both this particle system and any particle systems contained within this one.
        /// <para>NOTE: Because a particle system may contain other particle systems, this is a virtual function that may be overridden to return the 
        /// NumberOfParticlesAllocatedInMemory for both this particle system and any child particle systems that are contained within this one.</para>
        /// <para>NOTE: By default this just returns this particle system's NumberOfParticlesAllocatedInMemory.</para>
        /// </summary>
        public virtual int TotalNumberOfParticlesAllocatedInMemory { get { return NumberOfParticlesAllocatedInMemory; } }

		/// <summary>
		/// Get / Set the Max Number of Particles this Particle System is Allowed to contain at any given time.
		/// <para>NOTE: The Automatic Memory Manager will never allocate space for more Particles than this.</para>
		/// <para>NOTE: This value must be greater than or equal to zero.</para>
		/// </summary>
		public int MaxNumberOfParticlesAllowed
		{
			get { return miMaxNumberOfParticlesAllowed; }
			set
			{
				// Get the specified Number of Allowed Particles
				int iNewNumberOfParticlesAllowed = value;

				// Make sure the New Virtual Max is valid
				if (iNewNumberOfParticlesAllowed < 0)
				{
					iNewNumberOfParticlesAllowed = 0;
				}

				// Set the new Number of Particles Allowed
				miMaxNumberOfParticlesAllowed = iNewNumberOfParticlesAllowed;
			}
		}

		/// <summary>
		/// Get the number of Particles that are currently Active.
		/// </summary>
		public int NumberOfActiveParticles
		{
			get
			{
				// If the Particle System is Initialized
				if (this.IsInitialized)
				{
					// Return the number of Active Particles
					return mcActiveParticlesList.Count;
				}
				// Else the Active Particles List does not exist
				else
				{
					// Return that there are no Active Particles
					return 0;
				}
			}
		}

        /// <summary>
        /// Get the number of particles that are currently Active, in both this particle system and any particle systems contained within this one.
        /// <para>NOTE: Because a particle system may contain other particle systems, this is a virtual function that may be overridden to return the 
        /// NumberOfActiveParticles for both this particle system and any child particle systems that are contained within this one.</para>
        /// <para>NOTE: By default this just returns this particle system's NumberOfActiveParticles.</para>
        /// </summary>
        public virtual int TotalNumberOfActiveParticles { get { return NumberOfActiveParticles; } }

		/// <summary>
		/// Get the number of Particles being Drawn. That is, how many Particles are both Active AND Visible.
		/// </summary>
		public int NumberOfParticlesBeingDrawn
		{
			get { return miNumberOfParticlesToDraw; }
		}

        /// <summary>
        /// Get the number of particles that are being Drawn, in both this particle system and any particle systems contained within this one.
        /// <para>NOTE: Because a particle system may contain other particle systems, this is a virtual function that may be overridden to return the 
        /// NumberOfParticlesBeingDrawn for both this particle system and any child particle systems that are contained within this one.</para>
        /// <para>NOTE: By default this just returns this particle system's NumberOfParticlesBeingDrawn.</para>
        /// </summary>
        public virtual int TotalNumberOfParticlesBeingDrawn { get { return NumberOfParticlesBeingDrawn; } }

		/// <summary>
		/// Get the number of Particles that may still be added before reaching the
		/// Max Number Of Particles Allowed. If the Max Number Of Particles Allowed is 
		/// greater than the Number Of Particles Allocated In Memory AND the Auto Memory Manager is
		/// set to not increase the amount of Allocated Memory, than this returns the number 
		/// of Particles that may still be added before running out of Memory.
		/// </summary>
		public int NumberOfParticlesStillPossibleToAdd
		{
			get 
			{
				// If the Memory Allocated is less than the Max Number Of Particles Allowed AND
				// the Particle System cannot automatically increase the amount of Memory Allocated
				if (NumberOfParticlesAllocatedInMemory < MaxNumberOfParticlesAllowed &&
					AutoMemoryManagerSettings.MemoryManagementMode != AutoMemoryManagerModes.IncreaseAndDecrease &&
					AutoMemoryManagerSettings.MemoryManagementMode != AutoMemoryManagerModes.IncreaseOnly)
				{
					// Return how many Particles can be added before we run out of Memory
					return NumberOfParticlesAllocatedInMemory - NumberOfActiveParticles;
				}
				// Else we have enough Memory, or the Memory can be increased if needed
				else
				{
					// Return how many Particles can be added before reaching the Max Number Of Paticles Allowed
					return (MaxNumberOfParticlesAllowed - NumberOfActiveParticles);
				}
			}
		}

		/// <summary>
		/// Get / Protected Set a Linked List whose Nodes point to the Active Particles.
        /// <para>NOTE: New particles are added to the start of the Active Particles list, so by default the oldest particles should be at the end.</para>
		/// <para>NOTE: The Protected Set option is only provided to allow the order of the LinkedListNodes to be changed, changing the update and drawing 
		/// order of the Particles. Be sure that all of the original LinkedListNodes (and only the original LinkedListNodes, no more) obtained from the 
		/// Get are included; they may only be rearranged. If they are not, there may (and probably will) be unexpected results.</para>
		/// </summary>
		public LinkedList<Particle> ActiveParticles
		{
			get 
			{ 
				if (mcActiveParticlesList != null)
					return mcActiveParticlesList;
				else
					throw new DPSFNullReferenceException("The ActiveParticles property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
			}
			protected set { mcActiveParticlesList = value; }
		}

		/// <summary>
		/// Returns a Linked List whose Nodes point to the Inactive Particles
		/// </summary>
		public LinkedList<Particle> InactiveParticles
		{
			get 
			{ 
				if (mcInactiveParticlesList != null)
					return mcInactiveParticlesList;
				else
					throw new DPSFNullReferenceException("The InactiveParticles property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
			}
		}

		/// <summary>
		/// Returns the array of all Particle objects
		/// </summary>
		public Particle[] Particles
		{
			get 
			{
				if (mcParticles != null)
					return mcParticles;
				else
					throw new DPSFNullReferenceException("The Particles property is trying to be accessed, but is null. Be sure you have Initialized the particle system.");
			}
		}

		/// <summary>
		/// Initialize the given Particle using the current Initialization Function
		/// </summary>
		/// <param name="cParticle">The Particle to Initialize</param>
		public void InitializeParticle(Particle cParticle)
		{
			mcParticleInitializationFunction(cParticle);
		}

		/// <summary>
		/// Adds a new Particle to the particle system, at the start of the Active Particle List. 
		/// This new Particle is initialized using the particle system's Particle Initialization Function
		/// </summary>
		/// <returns>True if a particle was added, False if there is not enough memory for another Particle</returns>
		public bool AddParticle()
		{
			return AddParticle(null);
		}

		/// <summary>
		/// Adds a new Particle to the particle system, at the start of the Active Particle List. Returns true if
		/// the Particle was added, false if there is not enough memory for another Particle.
		/// </summary>
		/// <param name="cParticleToCopy">The Particle to add to the Particle System. If this is null then a
		/// new Particle is initialized using the particle system's Particle Initialization Function</param>
		/// <returns>True if a particle was added, False if there is not enough memory for another Particle</returns>
		public bool AddParticle(Particle cParticleToCopy)
		{
#if (DEBUG)
			if (DPSFDefaultSettings.ThrowDebugWarningExceptions)
			{
				// If we want to use DisabledWithEarlyRecycling mode, but don't have things configured properly for it to take effect, warn the user.
				if (AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.DisabledWithEarlyRecycling &&
					miMaxNumberOfParticlesAllowed <= NumberOfParticlesAllocatedInMemory)
				{
					string message = "The particle system call '" + this.ClassName + "' is configured with the AutoMemoryManagerSettings.MemoryManagementMode " +
						"set to DisabledWithEarlyRecycling, but has the MaxNumberOfParticlesAllowed set to less than or equal to the NumberOfParticlesAllocatedInMemory " +
						"(" + miMaxNumberOfParticlesAllowed + " <= " + NumberOfParticlesAllocatedInMemory + ")." +
						"This means that particles will never be recycled, since the DisabledWithEarlyRecycling property only takes effect when the NumberOfParticlesAllocatedInMemory " +
						"has been reached and not the MaxNumberOfParticlesAllowed. If you do want particles to be recycled you should set the MaxNumberOfParticlesAllowed " +
						"to be greater than the NumberOfParticlesAllocatedInMemory. These variables are typically set in the AutoInitialize() function.";
					throw new DPSFDebugWarningException(message);
				}

				if (AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.Disabled ||
					AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.DisabledWithEarlyRecycling)
				{
					// If we have allocated memory that may never get used, warn the user.
					if (miMaxNumberOfParticlesAllowed < NumberOfParticlesAllocatedInMemory)
					{
						string message = "The particle system class '" + this.ClassName + "' is configured with the AutoMemoryManagerSettings.MemoryManagementMode " + 
							"set to Disabled, but has the MaxNumberOfParticlesAllowed set to less than the NumberOfParticlesAllocatedInMemory. " +
							"(" + miMaxNumberOfParticlesAllowed + " < " + NumberOfParticlesAllocatedInMemory + "). This means that you have allocated memory for " + 
							"more particles than are allowed to be created. Unless you are changing the MaxNumberOfParticlesAllowed at run-time, " +
							"it is recommended that you change the MaxNumberOfParticlesAllowed to match the NumberOfParticlesAllocatedInMemory when using one of the " +
							"'Disabled' Memory Management Modes. These variables are typically set in the AutoInitialize() function.";
						throw new DPSFDebugWarningException(message);
					}
				}
			}
#endif
			// If we have reached the Max Number Of Particles Allowed.
			if (mcActiveParticlesList.Count >= miMaxNumberOfParticlesAllowed)
			{
				// Return that we cannot add a new Particle at this time.
				return false;
			}

			// If we have reached the Number Of Particles Allocated In Memory.
			if (mcInactiveParticlesList.Count <= 0)
			{
				// If we should recycle the oldest particle to make room for this new particle.
				if (AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.DisabledWithEarlyRecycling)
				{
					MoveActiveParticleToInactiveParticleList(this.ActiveParticles.Last);
				}
				// Else If Auto Memory Management is Enabled to allow the increasing of memory.
				else if (AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.IncreaseAndDecrease ||
					AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.IncreaseOnly)
				{
					// Calculate what the new increased Number Of Particles Allocated In Memory should be..
					int iNewNumberOfParticlesAllocatedInMemory = (int)(Math.Ceiling((float)(NumberOfParticlesAllocatedInMemory * AutoMemoryManagerSettings.IncreaseAmount)));

					// Make sure we do not allocate more than the Max Number Of Particles.
					iNewNumberOfParticlesAllocatedInMemory = (int)MathHelper.Min(iNewNumberOfParticlesAllocatedInMemory, MaxNumberOfParticlesAllowed);

					// Set the new Number Of Particles Allocated In Memory.
					NumberOfParticlesAllocatedInMemory = iNewNumberOfParticlesAllocatedInMemory;
				}
				// Else we cannot add another Particle at this time.
				else
				{
					// Return that we cannot add a new Particle at this time.
					return false;
				}
			}

			// Do any user operations before Adding the new Particle
			BeforeAddParticle();

			// Get an Inactive Particle Node and remove it from the Inactive List
			LinkedListNode<Particle> cNode = mcInactiveParticlesList.Last;
			mcInactiveParticlesList.RemoveLast();

			// Get a handle to the Particle 
			Particle cParticle = cNode.Value;

			// Initialize the Particle
			cParticle.Reset();

			// If a Particle was not given to add
			if (cParticleToCopy == null)
			{
				// If a Particle Initialization Function has been specified
				if (mcParticleInitializationFunction != null)
				{
					// Initialize the Particle using the Initialization Function
					mcParticleInitializationFunction(cParticle);
				}
			}
			// Else a specific Particle was given to add
			else
			{
				// So Copy the given Particle properties into this Particle
				cParticle.CopyFrom(cParticleToCopy);
			}

			// Add the Particle to the Active Particles List
			mcActiveParticlesList.AddFirst(cNode);

			// Do any user operations after Adding the new Particle
			AfterAddParticle();

			// Return that a Particle was added
			return true;
		}

		/// <summary>
		/// Adds the specified number of new Particles to the particle system. 
		/// <para>These new Particles are initialized using the particle system's Particle Initialization Function.</para>
		/// </summary>
		/// <param name="numberOfParticlesToAdd">How many Particles to Add to the particle system.</param>
		/// <returns>Returns how many Particles were able to be added to the particle system.</returns>
		public int AddParticles(int numberOfParticlesToAdd)
		{
			return AddParticles(numberOfParticlesToAdd, (Particle)null);
		}

		/// <summary>
		/// Adds the specified number of new Particles to the particle system, copying the properties of the given Particle To Copy.
		/// <para>NOTE: The Particle Initialization Function is not called when copying from an existing particle.</para>
		/// </summary>
		/// <param name="numberOfParticlesToAdd">How many copies of the Particle To Copy to Add to the particle system.</param>
		/// <param name="particleToCopy">The Particle to copy from when Adding the Particles to the  Particle System. If this is null then the new 
		/// Particles will be initialized using the particle system's Particle Initialization Function.</param>
		/// <returns>Returns how many Particles were able to be added to the particle system.</returns>
		public int AddParticles(int numberOfParticlesToAdd, Particle particleToCopy)
		{
			// The number of Particles added to the Particle System.
			int numberOfNewParticlesAdded = 0;

			// While we haven't added the specified number of Particles, and the Particle System is not full.
			while (numberOfNewParticlesAdded < numberOfParticlesToAdd && AddParticle(particleToCopy))
			{
				// Increment the number of Particles we were able to add so far.
				numberOfNewParticlesAdded++;
			}

			// Return how many Particles we were able to add to the Particle System.
			return numberOfNewParticlesAdded;
		}

		/// <summary>
		/// Adds the specified number of new Particles to the particle system, linearly interpolating (Lerp) the Emitter's 
		/// Position/Orientation to be between its Previous Position/Orientation and current Position/Orientation when adding new particles.
		/// <para>Returns how many Particles were able to be added to the particle system.</para>
		/// <para>These new Particles are initialized using the particle system's Particle Initialization Function.</para>
		/// </summary>
		/// <param name="numberOfParticlesToAdd">How many Particles to Add to the particle system.</param>
		/// <param name="emitterToLerp">The Emitter to copy the position and orientation data from.</param>
		/// <param name="elapsedTimeInSeconds">The elapsed time in seconds since the last frame so we know how much to update each Lerp'd particle.</param>
		/// <returns>Returns how many Particles were able to be added to the particle system.</returns>
		public int AddParticles(int numberOfParticlesToAdd, ParticleEmitter emitterToLerp, float elapsedTimeInSeconds)
		{
			// Build a new Lerp Info from the given Emitter.
			var lerpInfo = new ParticleEmitterLerpInfo()
							{
								PreviousPosition = emitterToLerp.PreviousPosition,
								CurrentPosition = emitterToLerp.PositionData.Position,
								PreviousOrientation = emitterToLerp.PreviousOrientation,
								CurrentOrientation = emitterToLerp.OrientationData.Orientation,
								ElapsedTimeInSeconds = elapsedTimeInSeconds
							};

			return AddParticles(numberOfParticlesToAdd, lerpInfo);
		}

		/// <summary>
		/// Adds the specified number of new Particles to the particle system, linearly interpolating (Lerp) the Emitter's 
		/// Position/Orientation to be between its Previous Position/Orientation and current Position/Orientation when adding new particles.
		/// <para>Returns how many Particles were able to be added to the particle system.</para>
		/// <para>These new Particles are initialized using the particle system's Particle Initialization Function.</para>
		/// </summary>
		/// <param name="numberOfParticlesToAdd">How many Particles to Add to the particle system.</param>
		/// <param name="lerpInfo">The info to use to Linearly Interpolate the Emitter's Position/Orientation with.</param>
		/// <returns>Returns how many Particles were able to be added to the particle system.</returns>
		public int AddParticles(int numberOfParticlesToAdd, ParticleEmitterLerpInfo lerpInfo)
		{
			// Make sure they specified a valid number of particles to add.
			if (numberOfParticlesToAdd <= 0)
				return 0;

			// Make sure they actually gave us some Lerp Info to use.
			if (lerpInfo == null)
				throw new DPSFException("AddParticles() was called with a null value for the 'lerpInfo' parameter, which is not allowed.");
		
			int numberOfNewParticlesAdded = 0;

			// Backup the Emitter's Position and Orientation before we start messing with it.
			Vector3 emittersPositionBackup = _emitter.PositionData.Position;
			Quaternion emittersOrientationBackup = _emitter.OrientationData.Orientation;
		
			// Calculate the Step Size between releasing Particles.
			float stepSizeFraction = 1.0f / numberOfParticlesToAdd;
			
			// Emit the Particles.
			int particlesEmitted = 0;
			bool canStillAddParticles = true;
			while (particlesEmitted < numberOfParticlesToAdd && canStillAddParticles)
			{
				// Update the Number of Particles Emitted (even though we haven't actually added it yet).
				particlesEmitted++;

				// Update the Interpolated Position of the Emitter for the next Particle Emitted.
				_emitter.PositionData.Position = Vector3.Lerp(lerpInfo.PreviousPosition, lerpInfo.CurrentPosition, stepSizeFraction * particlesEmitted);

				// Update the Interpolated Orientation of the Emitter for the next Particle Emitted.
				_emitter.OrientationData.Orientation = Quaternion.Slerp(lerpInfo.PreviousOrientation, lerpInfo.CurrentOrientation, stepSizeFraction * particlesEmitted);

				// Add the new Particle, and record if we can still add more Particles or not.
				canStillAddParticles = AddParticle();

				// If the Particle was added successfully.
				if (canStillAddParticles)
				{
					// Get a handle to the newly added Particle.
					Particle particle = ActiveParticles.First.Value;

					// Calculate how much the just added Particle should be updated ("oldest" particles are added first).
					float fUpdateAmount = MathHelper.Lerp(lerpInfo.ElapsedTimeInSeconds, 0.0f, stepSizeFraction * particlesEmitted);

					// Update the newly added Particle.
					particle.UpdateElapsedTimeVariables(fUpdateAmount);
					ParticleEvents.Update(particle, fUpdateAmount);

					// If the Particle is no longer Active already.
					if (!particle.IsActive())
					{
						// Remove the Particle from the Active Particle List.
						ActiveParticles.RemoveFirst();
					}
					// Else the Particle is still Active.
					else
					{
						// Add the Particle to the list of Particles to be drawn.
						AddParticleToVertexBuffer(particle);

						// Increment the Number Of New Particles Added this frame.
						numberOfNewParticlesAdded++;
					}
				}
			}

			// Restore the Emitter's Position and Orientation now that we are done messing with it.
			_emitter.PositionData.Position = emittersPositionBackup;
			_emitter.OrientationData.Orientation = emittersOrientationBackup;

			return numberOfNewParticlesAdded;
		}

		/// <summary>
		/// Removes all Active Particles from the Active Particle List and adds them 
		/// to the Inactive Particle List
		/// </summary>
		public void RemoveAllParticles()
		{
			// Loop until all Active Particles have been removed
			while (mcActiveParticlesList.Count > 0)
			{
				// Save a handle to the Node to remove
				LinkedListNode<Particle> cNodeToRemove = mcActiveParticlesList.First;

				// Remove the Node from the Active Particles List
				mcActiveParticlesList.Remove(cNodeToRemove);

				// Add it to the Inactive Particles List
				mcInactiveParticlesList.AddFirst(cNodeToRemove);
			}
		}

	// If inheriting from DrawableGameComponent, we need to override the Update() function
	#if (DPSFAsDrawableGameComponent)
		/// <summary>
		/// Overloaded DrawableGameComponent Update function.
		/// <para>Updates the Particle System. This involves executing the Particle System
		/// Events, updating all Active Particles according to the Particle Events, and 
		/// adding new Particles according to the Emitter settings.</para>
		/// <para>NOTE: This function should never be called manually by the user. It should
		/// only be called automatically by the Game object.</para>
		/// </summary>
		/// <param name="cGameTime">GameTime object used to determine how
		/// much time has elapsed since the last frame</param>
		public override void Update(GameTime cGameTime)
		{
			// Update the Particle System
			base.Update(cGameTime);
			Update((float)cGameTime.ElapsedGameTime.TotalSeconds, true);
		}
#endif

		/// <summary>
		/// Updates the Particle System. This involves executing the Particle System
		/// Events, updating all Active Particles according to the Particle Events, and 
		/// adding new Particles according to the Emitter settings.
		/// <para>NOTE: This will only Update the Particle System if it does not inherit from DrawableGameComponent, 
		/// since if it does it will be updated automatically by the Game object.</para>
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How much time in seconds has 
		/// elapsed since the last time this function was called</param>
		public void Update(float fElapsedTimeInSeconds)
		{
			Update(fElapsedTimeInSeconds, false);
		}

		/// <summary>
		/// Updates the Particle System, even if the the Particle Systems inherits from DrawableGameComponent.
		/// <para>Updating the Particle System involves executing the Particle System Events, updating all Active 
		/// Particles according to the Particle Events, and adding new Particles according to the Emitter settings.</para>
		/// <para>NOTE: If inheriting from DrawableGameComponent and this is called, the Particle System will be updated
		/// twice per frame; once when it is called here, and again when automatically called by the Game object.
		/// If not inheriting from DrawableGameComponent, this acts the same as calling Update().</para>
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How much time in seconds has 
		/// elapsed since the last time this function was called</param>
		public void UpdateForced(float fElapsedTimeInSeconds)
		{
			Update(fElapsedTimeInSeconds, true);
		}

		/// <summary>
		/// Updates the Particle System. This involves executing the Particle System
		/// Events, updating all Active Particles according to the Particle Events, and 
		/// adding new Particles according to the Emitter's settings.
		/// </summary>
		/// <param name="fElapsedTimeInSeconds">How much time in seconds has 
		/// elapsed since the last time this function was called</param>
		/// <param name="bCalledByDrawableGameComponent">Indicates if this function was
		/// called manually by the user or called automatically by the Drawable Game Component.
		/// If this function Inherits Drawable Game Component, but was not called by
		/// Drawable Game Component, nothing will be updated since the Particle System will
		/// automatically be updated when the Game Component's Update() function is called.</param>
		private void Update(float fElapsedTimeInSeconds, bool bCalledByDrawableGameComponent)
		{
			// If the Particle System is not Initialized OR no Particles should be updated OR
			// the Particle System is a Drawable Game Component and was called manually by the user
			// (we do nothing in this case because the Particle System will automatically be Updated 
			// by the Drawable Game Component).
			if (!this.IsInitialized || !this.Enabled || 
				(this.InheritsDrawableGameComponent && !bCalledByDrawableGameComponent))
			{
				// Exit the function without updating anything
				return;
			}

			// Add the Elapsed Time since the last frame to our cumulative Time Elapsed Since Last Update
			mfTimeElapsedSinceLastUpdate += fElapsedTimeInSeconds;

			// If it's not time for the Particle System to be Updated yet
			if (mfTimeElapsedSinceLastUpdate < mfTimeToWaitBetweenUpdates)
			{
				// Exit the function without updating anything
				return;
			}

// The Reach profile does not have access to the System.Diagnostics namespace, so it does not know about the Stopwatch class.
#if (!WINDOWS_PHONE)
			// We are going to be doing the Update for sure if we have gotten here, so start the performance profiling if it is enabled.
			if (_performanceProfilingIsEnabled)
			{
				_performanceProfilingStopwatch.Reset();
				_performanceProfilingStopwatch.Start();
			}
#endif

			// Update the cumulative Elapsed Time
			mfTimeElapsedSinceLastUpdate -= mfTimeToWaitBetweenUpdates;

			// Calculate by how much Time the Particle System should be Updated.
			float fParticleSystemUpdateTime = 0.0f;

			// If the Particle System is supposed to be Updated as often as possible.
			if (mfTimeToWaitBetweenUpdates <= 0.0f)
			{
				// Use the actual Elapsed Time for the Particle System Update Time.
				fParticleSystemUpdateTime = fElapsedTimeInSeconds;
			}
			// Else If this function is not being called fast enough to keep up
			// with how often the Particle Systems are supposed to be Updated
			// (i.e. There is a very low frame-rate, or the user specified a
			// very large number of Updates Per Second, such as 1000).
			else if (mfTimeElapsedSinceLastUpdate >= mfTimeToWaitBetweenUpdates)
			{
				// Use the amount of Time that has passed since the last time this function
				// was called for the Particle System Update Time, and reset the cumulative Elapsed Time.
				fParticleSystemUpdateTime = (mfTimeElapsedSinceLastUpdate + mfTimeToWaitBetweenUpdates);
				mfTimeElapsedSinceLastUpdate = 0.0f;
			}
			// Else this function is being called often enough to keep up with 
			// the specified Update rate
			else
			{
				// Use the specified Update rate for the Particle System Update Time.
				fParticleSystemUpdateTime = mfTimeToWaitBetweenUpdates;
			}
			

			// Calculate the scaled Elapsed Time
			float fScaledElapsedTimeInSeconds = fParticleSystemUpdateTime * SimulationSpeed * InternalSimulationSpeed;

			// Perform other functions before this Particle System is updated.
			BeforeUpdate(fScaledElapsedTimeInSeconds);

			// Reset the count of how many Particles are still Active and Visible.
			miNumberOfParticlesToDraw = 0;

			// Reset the Index Buffer Index in case we are drawing Quads.
			miIndexBufferIndex = 0;

			// Variable to keep track of how many Particles are added this frame.
			int numberOfNewParticlesAdded = 0;


			// Update the Particle System according to its Events (before updating the Particles).
			ParticleSystemEvents.Update(fScaledElapsedTimeInSeconds);

			// If the Particle System Destroyed itself, just exit.
			if (!this.IsInitialized)
				return;

			// Get the IDs of the Emitters to run through and add particles from.
			var emitterIDs = _emitters.IDs;

			// Backup the emitter that Emitter is currently pointing to.
			var previousEmitter = _emitter;

			// Loop through each Emitter and add their particles to the particle system.
			for (int index = 0; index < emitterIDs.Count; index++)
			{
				int emitterID = emitterIDs[index];

				// Make sure this emitter still exists in the list, as it may have been removed by a previous emitter's BustCompleted event.
				if (!_emitters.Contains(emitterID))
					continue;

				// Update the Emitter to point to his emitter.
				_emitter = _emitters[emitterID];

				// Update the Emitter and get how many Particles should be emitted.
				int iNumberOfParticlesToEmit = _emitter.UpdateAndGetNumberOfParticlesToEmit(fScaledElapsedTimeInSeconds);

				// If the Particle System Destroyed itself from the Emitter's BurstCompleted event, just exit.
				if (!this.IsInitialized)
					return;

				// If the Emitter's Position and Orientation should not be Lerped this time.
				if (!_emitter.LerpEmittersPositionAndOrientationOnNextUpdate)
				{
					// Reset the Previous Position and Orientation to the current Position and Orientation.
					_emitter.PreviousPosition = _emitter.PositionData.Position;
					_emitter.PreviousOrientation = _emitter.OrientationData.Orientation;

					// Reset variable to allow Lerping on the next Update().
					_emitter.LerpEmittersPositionAndOrientationOnNextUpdate = true;
				}

				// If some Particles should be emitted
				if (iNumberOfParticlesToEmit > 0)
				{
					// If the emitter is moving very fast, or the frame rate drops for
					// some reason, then many particles may be released in one location
					// (i.e. the emitter's new location) when they really should be spread
					// out between the emitter's old location and the new one. To spread the
					// particles out, the location that particles are released is 
					// Linearly Interpolated (Lerp) between the emitter's old and new location.
					// The orientation of the emitter is also Spherically Linearly Interpolated (Slerp)
					// for the similar case that the emitter is rotating very fast.

					// If we should not be Lerping the Emitter's Position and Orientation.
					if (!_emitter.LerpEmittersPositionAndOrientation)
					{
						// Reset the Previous Position and Orientation to the current Position and Orientation.
						_emitter.PreviousPosition = _emitter.PositionData.Position;
						_emitter.PreviousOrientation = _emitter.OrientationData.Orientation;
					}

					// Add the new particles to the particle system.
					numberOfNewParticlesAdded += AddParticles(iNumberOfParticlesToEmit, _emitter, fScaledElapsedTimeInSeconds);

					// Add the number of particles added to the particle system to the emitter's total number of particles emitted.
					_emitter.NumberOfParticlesEmitted += numberOfNewParticlesAdded;
				}

				// Store the Emitter's Position and Orientation so that we can Lerp from it on the next Update().
				// We save it here instead of before Updating the Emitter in case the user manually moved or rotated 
				// the Emitter (before this Update() function was called).
				_emitter.PreviousPosition = _emitter.PositionData.Position;
				_emitter.PreviousOrientation = _emitter.OrientationData.Orientation;
			}

			// Restore the Emitter variable to point back to the same emitter it was pointing to before the Update().
			_emitter = previousEmitter;

			// Get a handle to the first Active Particle Node
			LinkedListNode<Particle> cNode = mcActiveParticlesList.First;            

			// Skip any Particles that were just added to the Particle System, since they've already been updated
			for (int iIndex = 0; iIndex < numberOfNewParticlesAdded; iIndex++)
			{
				// Move to the Next Particle in the list
				cNode = cNode.Next;
			}

			// Loop until all Nodes in the Active Particle List have been processed
			while (cNode != null)
			{
				// Get a handle to the Particle
				Particle cParticle = cNode.Value;

				// Update the Particle's Elapsed Time Variables
				cParticle.UpdateElapsedTimeVariables(fScaledElapsedTimeInSeconds);

				// Update the Particle according to the Particle Events
				ParticleEvents.Update(cParticle, fScaledElapsedTimeInSeconds);

				// If the Particle is no longer Active
				if (!cParticle.IsActive())
				{
					// Save a handle to the Node to remove
					LinkedListNode<Particle> cNodeToRemove = cNode;

					// Move to the Next Node in the Active Particle List
					cNode = cNode.Next;

                    MoveActiveParticleToInactiveParticleList(cNodeToRemove);
				}
				// Else the Particle is still Active
				else
				{
					// Add the Particle to the list of Particles to be drawn
					AddParticleToVertexBuffer(cParticle);

					// Move to the Next Node in the Active Particle List
					cNode = cNode.Next;
				}
			}

			// Remove all OneTime Events now that they've been executed
			ParticleEvents.RemoveAllOneTimeEvents();

			// If the Auto Memory Manager is Enabled to allow Decrease of Memory
			if (AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.IncreaseAndDecrease ||
				AutoMemoryManagerSettings.MemoryManagementMode == AutoMemoryManagerModes.DecreaseOnly)
			{
				// Update the Auto Memory Manager Timer
				mfAutoMemoryManagersElapsedTime += fElapsedTimeInSeconds;

				// If there are more Active Particles than our previous Max
				if (NumberOfActiveParticles > miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds)
				{
					// Save the new Max value and reset the Timer
					miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds = NumberOfActiveParticles;
					mfAutoMemoryManagersElapsedTime = 0.0f;
				}

				// If it is time to check if the memory should be reduced
				if (mfAutoMemoryManagersElapsedTime >= AutoMemoryManagerSettings.SecondsMaxNumberOfParticlesMustExistForBeforeReducingSize)
				{
					// Calculate what the new reduced Max Number Of Particles should be
					int iNewMaxNumberOfParticles = (int)(Math.Ceiling((float)(miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds * AutoMemoryManagerSettings.ReduceAmount)));

					// Make sure we do not Decrease it less than the Absolute Min
					iNewMaxNumberOfParticles = (int)MathHelper.Max(iNewMaxNumberOfParticles, AutoMemoryManagerSettings.AbsoluteMinNumberOfParticles);

					// If the new Max Number Of Particles is less than what we have currently
					if (iNewMaxNumberOfParticles < NumberOfParticlesAllocatedInMemory)
					{
						// Set the new Max Number Of Particles
						NumberOfParticlesAllocatedInMemory = iNewMaxNumberOfParticles;
					}

					// Reset the Max Number Of Active Particles Over The Last X Seconds
					miAutoMemoryManagerMaxNumberOfParticlesActiveAtOnceOverTheLastXSeconds = 0;
				}
			}

			// If the Particle System has reached the end of its Lifetime and is supposed to Destroy itself
			if (ParticleSystemEvents.LifetimeData.NormalizedElapsedTime >= 1.0 &&
				ParticleSystemEvents.LifetimeData.EndOfLifeOption == CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Destroy)
			{
				// Destroy the Particle System
				Destroy();
			}

			// Perform any other functions now that this Particle System has been Updated
			AfterUpdate(fScaledElapsedTimeInSeconds);

// The Reach profile does not have access to the System.Diagnostics namespace, so it does not know about the Stopwatch class.
#if (!WINDOWS_PHONE)
			// If performance profiling is enabled, log how long it took to do the Update
			if (_performanceProfilingIsEnabled)
			{
				_performanceProfilingStopwatch.Stop();
				PerformanceTimeToDoUpdateInMilliseconds = _performanceProfilingStopwatch.Elapsed.TotalMilliseconds;
			}
#endif
		}

        /// <summary>
        /// Moves the Active Particle's Linked List node to the Inactive Particle Linked List.
        /// </summary>
        /// <param name="nodeToRemove">The Active Particle Linked List's node to move to the Inactive Particle Linked List.</param>
        protected void MoveActiveParticleToInactiveParticleList(LinkedListNode<Particle> nodeToRemove)
        {
            // Remove the Node from the Active Particles List
            mcActiveParticlesList.Remove(nodeToRemove);

            // Add it to the Inactive Particles List
            mcInactiveParticlesList.AddFirst(nodeToRemove);
        }

		/// <summary>
		/// Adds the given Particle to the list of Particles to be Drawn (i.e. the Vertex Buffer), if it is Visible
		/// </summary>
		/// <param name="cParticle">The Particle to add to the Vertex Buffer</param>
		private void AddParticleToVertexBuffer(Particle cParticle)
		{
			// If the Particle should be drawn
			if (cParticle.Visible)
			{
				switch (meParticleType)
				{
					default:
					case ParticleTypes.NoDisplay:
						// Do Nothing
					break;

					case ParticleTypes.Quad:
					case ParticleTypes.TexturedQuad:
						// Set the Quad's 4 Vertex properties
						// A Quad uses 4 Vertices, so we must offset the index for this
						mcVertexUpdateFunction(ref mcParticleVerticesToDraw, miNumberOfParticlesToDraw * 4, cParticle);
					break;

					case ParticleTypes.Sprite:
						// Copy this Particle into the list of Sprites To Draw
						mcParticleSpritesToDraw[miNumberOfParticlesToDraw] = cParticle;
					break;
				}

				// Increment the number of Particles found to still be Active and Visible
				miNumberOfParticlesToDraw++;
			}
		}

	// If inheriting from DrawableGameComponent, we need to override the Draw() function
	#if (DPSFAsDrawableGameComponent)
		/// <summary>
		/// Overloaded DrawableGameComponent Draw function.
		/// Draws all of the Active Particles to the Graphics Device.
		/// <para>NOTE: This function should never be called manually by the user. It should
		/// only be called automatically by the Game object.</para>
		/// </summary>
		/// <param name="cGameTime">GameTime object used to determine
		/// how much time has elapsed since the last frame</param>
		public override void Draw(GameTime cGameTime)
		{
			// Draw the Particle System
			base.Draw(cGameTime);
			Draw(true);
		}
#endif

		/// <summary>
		/// Draws all of the Active Particles to the Graphics Device.
		/// <para>NOTE: This will only Draw the Particle System if it does not inherit from DrawableGameComponent, 
		/// since if it does it will be drawn automatically by the Game object.</para>
		/// </summary>
		public void Draw()
		{
			// Draw the Particle System
			TryDraw(false);
		}

		/// <summary>
		/// Draws all of the Active Particles to the Graphics Device, even if the the Particle Systems inherits
		/// from DrawableGameComponent.
		/// <para>NOTE: If inheriting from DrawableGameComponent and this is called, the Particle System will be drawn
		/// twice per frame; once when it is called here, and again when automatically called by the Game object.
		/// If not inheriting from DrawableGameComponent, this acts the same as calling Draw().</para>
		/// </summary>
		public void DrawForced()
		{
			// Force the drawing of the Particle System
			TryDraw(true);
		}

		/// <summary>
		/// Perform the Draw operation, catching any expected errors that we want to eat.
		/// </summary>
		/// <param name="calledByDrawableGameComponent">Indicates if this function was called manually by the user or called automatically by the Drawable Game Component.
		/// If this function Inherits Drawable Game Component, but was not called by Drawable Game Component, nothing will be drawn since the Particle System will
		/// automatically be drawn when the Game Component's Draw() function is called.</param>
		private void TryDraw(bool calledByDrawableGameComponent)
		{
			try
			{
				Draw(calledByDrawableGameComponent);
			}
			// Eat any Object Disposed Exceptions, as they are often thrown from within the Draw function when moving the application from one monitor to another.
			catch (ObjectDisposedException)
			{
				// Make sure we End the Sprite Batch, as calling Begin on it again without Ending it will cause another exception.
				if (meParticleType == ParticleTypes.Sprite && !UsingExternalSpriteBatchToDrawParticles)
				{
					try
					{
						// Start the SpriteBatch for drawing before we start the Pass.
						mcSpriteBatch.End();
					}
					// Eat the exception for the case where .Begin() had not been called yet.
					catch (InvalidOperationException)
					{ }
				}
			}
		}

		/// <summary>
		/// Draws all of the Active Particles to the Graphics Device
		/// </summary>
		/// <param name="bCalledByDrawableGameComponent">Indicates if this function was
		/// called manually by the user or called automatically by the Drawable Game Component.
		/// If this function Inherits Drawable Game Component, but was not called by
		/// Drawable Game Component, nothing will be drawn since the Particle System will
		/// automatically be drawn when the Game Component's Draw() function is called.</param>
		private void Draw(bool bCalledByDrawableGameComponent)
		{
			// If the Particle System is not Initialized OR no Particles should be drawn OR
			// the Particle System is a Drawable Game Component and was called manually by the user
			// (we do nothing in this case because the Particle System will automatically be Drawn 
			// by the Drawable Game Component).
			if (!this.IsInitialized || !Visible ||
				(InheritsDrawableGameComponent && !bCalledByDrawableGameComponent))
			{
				// Exit the function without drawing anything.
				return;
			}

// The Reach profile does not have access to the System.Diagnostics namespace, so it does not know about the Stopwatch class.
#if (!WINDOWS_PHONE)
			// We are going to be doing the Draw for sure if we have gotten here, so start the performance profiling if it is enabled.
			if (_performanceProfilingIsEnabled)
			{
				_performanceProfilingStopwatch.Reset();
				_performanceProfilingStopwatch.Start();
            }
#endif

// There's an XNA 4 bug on the Xbox that will throw a run-time error if we don't clear out the Render State before drawing, so clear them out.
#if (XBOX)
            // Reset all of the render states.
			if (ParticleType != ParticleTypes.NoDisplay)
				ClearRenderStatesOnXbox();
#endif

            // Perform other functions before this Particle System is drawn.
			BeforeDraw();

			// If there are no Particles to draw.
			if (miNumberOfParticlesToDraw <= 0 || ParticleType == ParticleTypes.NoDisplay)
			{
				// Perform the AfterDraw() operations before exiting.
				AfterDraw();

// The Reach profile does not have access to the System.Diagnostics namespace, so it does not know about the Stopwatch class.
#if (!WINDOWS_PHONE)
				// If performance profiling is enabled, log how long it took to do the Draw.
				if (_performanceProfilingIsEnabled)
				{
					_performanceProfilingStopwatch.Stop();
					PerformanceTimeToDoDrawInMilliseconds = _performanceProfilingStopwatch.Elapsed.TotalMilliseconds;
				}
#endif

				// Exit without drawing anything.
				return;
			}

			// If an Effect is not being used (can only be done with the SpriteBatch).
			if (meParticleType == ParticleTypes.Sprite && (Effect == null || Technique == null))
			{
				// If we are using our own SpriteBatch to draw the particles, set it up.
				if (!UsingExternalSpriteBatchToDrawParticles)
				{
					// Start the SpriteBatch for drawing.
					mcSpriteBatch.Begin(SpriteBatchSettings.SortMode, RenderProperties.BlendState, RenderProperties.SamplerState, RenderProperties.DepthStencilState, RenderProperties.RasterizerState, this.Effect, SpriteBatchSettings.TransformationMatrix);
				}

				// Loop through all of the Sprites to Draw.
				for (int index = 0; index < miNumberOfParticlesToDraw; index++)
				{
					// Draw this Sprite using the overloaded Draw Sprite function.
					DrawSprite(mcParticleSpritesToDraw[index], mcSpriteBatch);
				}

				// If we are using our own SpriteBatch to draw the particles, shut it down.
				if (!UsingExternalSpriteBatchToDrawParticles)
				{
					// End the SpriteBatch since we are done drawing.
					mcSpriteBatch.End();
				}
			}
			// Else an Effect is being used.
			else
			{
				// Set the Render State for drawing.
				ApplyRenderState();

				// Set the Effect Parameters.
				SetEffectParameters();

				// Loop through each Pass of the Current Technique (using a for loop instead of foreach for less memory consumption).
				int numberOfPasses = mcEffect.CurrentTechnique.Passes.Count;
				for (int passIndex = 0; passIndex < numberOfPasses; passIndex++)
				{
					// Get a handle to the pass.
					EffectPass cPass = mcEffect.CurrentTechnique.Passes[passIndex];

					// If we are using our own SpriteBatch to draw the particles, set it up.
					if (meParticleType == ParticleTypes.Sprite && !UsingExternalSpriteBatchToDrawParticles)
					{
						// Start the SpriteBatch for drawing before we start the Pass.
						mcSpriteBatch.Begin(SpriteBatchSettings.SortMode, RenderProperties.BlendState, RenderProperties.SamplerState, RenderProperties.DepthStencilState, RenderProperties.RasterizerState, this.Effect, SpriteBatchSettings.TransformationMatrix);
					}

					// Apply the Pass
					cPass.Apply();

// If this is running on the XBox.
#if (XBOX)
					try
					{
#endif
					// Draw the Particles based on what Type of Particles they are.
					switch (meParticleType)
					{
						default: break;

						case ParticleTypes.Quad:
						case ParticleTypes.TexturedQuad:
// If we are running on the XBox 360, catch the case of overloading the graphics buffer.
#if (XBOX)
							// Setup our variables before drawing
							int numberOfParticlesRemainingToDraw = miNumberOfParticlesToDraw;
							int vertexOffset = 0;
							int indexOffset = 0;

							// Keep looping while there are still particles to draw
							while (numberOfParticlesRemainingToDraw > 0)
							{
								// Calculate how many particles to draw during this iteration of the loop
								int particlesToDrawThisIteration = Math.Min(numberOfParticlesRemainingToDraw, miMaxParticlesThatXboxCanDrawAtOnce);

								// Calculate the total number of Primitives and Vertices to draw in this iteration of the loop
								int primitiveCount = particlesToDrawThisIteration * 2;		// 2 triangles are required to make a quad
								int numberOfVertices = particlesToDrawThisIteration * 4;	// Each quad uses 4 vertices
								
								// Draw the Particles as Quads (Xbox supports the HiDef profile, so always use the HiDef Index Buffer)
								GraphicsDevice.DrawUserIndexedPrimitives<Vertex>(PrimitiveType.TriangleList, mcParticleVerticesToDraw, vertexOffset, numberOfVertices, miaIndexBufferArray, indexOffset, primitiveCount, mcVertexDeclaration);

								// Update our vertex and index offsets based on how many particles have been drawn so far
								vertexOffset += numberOfVertices;
								indexOffset += particlesToDrawThisIteration * 6;	// 6 indices are used per quad particle

								// Calculate how many particles are still left to draw
								numberOfParticlesRemainingToDraw -= particlesToDrawThisIteration;
							}
// Else we are running on Windows, so just draw the Quads
#else
							// Draw the Particles as Quads according to the type of Index Buffer we are using.
							// If we are using the full Int Index Buffer.
							if (IsUsingIntegerIndexBuffer)
							{
								GraphicsDevice.DrawUserIndexedPrimitives<Vertex>(PrimitiveType.TriangleList, mcParticleVerticesToDraw, 0, miNumberOfParticlesToDraw * 4, miaIndexBufferArray, 0, miNumberOfParticlesToDraw * 2, mcVertexDeclaration);
							}
							// Else we are using the Short Index Buffer.
							else
							{
								GraphicsDevice.DrawUserIndexedPrimitives<Vertex>(PrimitiveType.TriangleList, mcParticleVerticesToDraw, 0, miNumberOfParticlesToDraw * 4, msaIndexBufferReachArray, 0, miNumberOfParticlesToDraw * 2, mcVertexDeclaration);
							}
#endif
						break;

						case ParticleTypes.Sprite:
							// Loop through all of the Sprites to Draw.
							for (int index = 0; index < miNumberOfParticlesToDraw; index++)
							{
								// Draw this Sprite using the overloaded Draw Sprite function.
								DrawSprite(mcParticleSpritesToDraw[index], mcSpriteBatch);
							}

							// If we are using our own SpriteBatch to draw the particles, shut it down.
							if (!UsingExternalSpriteBatchToDrawParticles)
							{
								// End the SpriteBatch since we are done drawing for this pass.
								mcSpriteBatch.End();
							}
						break;
					}

// If we are running on the XBox 360, catch the case of overloading the graphics buffer.
#if (XBOX)
					}
					catch (InvalidOperationException e)
					{
						// Calculate how much memory will be allocated to perform the Draw
						int iSizeOfMemoryToAllocate = miVertexSizeInBytes * miNumberOfParticlesToDraw;

						// If we are drawing Quads
						if (meParticleType == ParticleTypes.Quad || meParticleType == ParticleTypes.TexturedQuad)
						{
							// Quads require 4 times as many vertices as Point Sprites
							iSizeOfMemoryToAllocate *= 4;
						}

						throw new DPSFInvalidOperationException("Not enough video memory to draw the particle system. The XBox 360 can only allocate about " + MAX_MEMORY_IN_BYTES_THAT_XBOX_CAN_DRAW.ToString("###,###,###") + " bytes in video memory when using DrawUserPrimitives(). You are trying to allocate " + iSizeOfMemoryToAllocate.ToString("###,###,###") + " bytes.\n\nInner exception: " + e.ToString(), e);
					}
#endif
				}
			}

			// Perform any other functions now that this Particle System has been Drawn.
			AfterDraw();

// The Reach profile does not have access to the System.Diagnostics namespace, so it does not know about the Stopwatch class.
#if (!WINDOWS_PHONE)
			// If performance profiling is enabled, log how long it took to do the Draw.
			if (_performanceProfilingIsEnabled)
			{
				_performanceProfilingStopwatch.Stop();
				PerformanceTimeToDoDrawInMilliseconds = _performanceProfilingStopwatch.Elapsed.TotalMilliseconds;
			}
#endif
		}

		/// <summary>
		/// Resets all of the Sampler States and Vertex Sampler States on the Graphics Device.
		/// This must be done before any rendering to prevent an XNA 4 bug that causes the graphics device
		/// to incorrectly track state information, which manifests itself as run-time errors.
		/// https://connect.microsoft.com/site226/feedback/details/586216/cloned-effect-
        /// <para>It looks like this bug is only on the Xbox 360, so we only need to call this function on that platform.
        /// I've removed all of the checks that were required to make this code run properly on other platforms.</para>
		/// </summary>
		private void ClearRenderStatesOnXbox()
		{
			// If we don't have a handle to a Graphics Device (e.g. NoDisplay particle system), just exit.
			if (this.GraphicsDevice == null)
				return;

			// Reset all of the Sampler States
            for (int index = 0; index < 16; index++)
				GraphicsDevice.SamplerStates[index] = SamplerState.PointClamp;
// Apparently MonoGame doesn't know about the VertexSamplerStates property, so we can't update it when porting other platforms via MonoGame (e.g. Android, iOS, and WinRT).
// Even though this code will only be called on the Xbox, we still need this conditional compilation check in order for the project to compile on other platforms.
#if (XBOX)
			//  Reset all of the Vertex Sampler States
			for (int index = 0; index < 2; index++)
				GraphicsDevice.VertexSamplerStates[index] = SamplerState.PointWrap;
#endif
		}

		/// <summary>
		/// Applies the Particle System's Render State properties to the Graphics Device.
		/// </summary>
		private void ApplyRenderState()
		{
			GraphicsDevice.BlendState = this.RenderProperties.BlendState;
			GraphicsDevice.DepthStencilState = this.RenderProperties.DepthStencilState;
			GraphicsDevice.RasterizerState = this.RenderProperties.RasterizerState;
			GraphicsDevice.SamplerStates[0] = this.RenderProperties.SamplerState;
		}

		#endregion

		#region Virtual Methods that may be overridden

		/// <summary>
		/// Virtual function to Initialize the Particle System with default values.
		/// Particle system properties should not be set until after this is called, as 
		/// they are likely to be reset to their default values.
		/// </summary>
		/// <param name="cGraphicsDevice">The Graphics Device the Particle System should use</param>
		/// <param name="cContentManager">The Content Manager the Particle System should use to load resources</param>
		/// <param name="cSpriteBatch">The Sprite Batch that the Sprite Particle System should use to draw its particles.
		/// If this is not initializing a Sprite particle system, or you want the particle system to use its own Sprite Batch,
		/// pass in null.</param>
		public virtual void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
		{ }

		/// <summary>
		/// Sets the Camera Position of the particle system, so that the particles know how to make themselves face the camera if needed.
		/// This virtual function does not do anything unless overridden, and all it should do is set an internal Vector3 variable
		/// (e.g. public Vector3 CameraPosition { get; set; }) to match the given Vector3.
		/// </summary>
		/// <param name="cameraPosition">The position that the camera is currently at.</param>
		public virtual void SetCameraPosition(Vector3 cameraPosition)
		{ }

		/// <summary>
		/// Virtual function to draw a Sprite Particle. This function should be used to draw the given
		/// Particle with the provided SpriteBatch.
		/// </summary>
		/// <param name="Particle">The Particle Sprite to Draw</param>
		/// <param name="cSpriteBatch">The SpriteBatch to use to doing the Drawing</param>
		protected virtual void DrawSprite(DPSFParticle Particle, SpriteBatch cSpriteBatch)
		{ }

		/// <summary>
		/// Virtual function that is called at the end of the Initialize() function.
		/// This may be used to perform operations after the Particle System has been Initialized, such as 
		/// initializing other Particle Systems nested within this Particle System.
		/// </summary>
		protected virtual void AfterInitialize()
		{ }

		/// <summary>
		/// Virtual function that is called at the beginning of the Destroy() function.
		/// This may be used to perform operations before the Destroy() code is executed.
		/// </summary>
		protected virtual void BeforeDestroy()
		{ }

		/// <summary>
		/// Virtual function that is called at the end of the Destroy() function.
		/// This may be used to perform operations after the Particle System has been Destroyed, such as 
		/// to destroy other Particle Systems nested within this Particle System.
		/// </summary>
		protected virtual void AfterDestroy()
		{ }

		/// <summary>
		/// Virtual function that is called at the beginning of the Update() function.
		/// This may be used to perform operations before the Update() code is executed.
		/// </summary>
		protected virtual void BeforeUpdate(float fElapsedTimeInSeconds)
		{ }

		/// <summary>
		/// Virtual function that is called at the end of the Update() function.
		/// This may be used to perform operations after the Particle System has been updated, such as 
		/// to Update Particle Systems nested within this Particle System.
		/// </summary>
		protected virtual void AfterUpdate(float fElapsedTimeInSeconds)
		{ }

		/// <summary>
		/// Virtual function that is called at the beginning of the Draw() function.
		/// This may be used to perform operations before the Draw() code is executed.
		/// </summary>
		protected virtual void BeforeDraw()
		{ }

		/// <summary>
		/// Virtual function that is called at the end of the Draw() function.
		/// This may be used to perform operations after the Particle System has been drawn, such as 
		/// to Draw Particle Systems nested within this Particle System.
		/// </summary>
		protected virtual void AfterDraw()
		{ }

		/// <summary>
		/// Virtual function that is called at the beginning of the AddParticle() function.
		/// This may be used to execute some code before a new Particle is initialized and added.
		/// </summary>
		protected virtual void BeforeAddParticle()
		{ }

		/// <summary>
		/// Virtual function that is called at the end of the AddParticle() function.
		/// This may be used to execute some code after a new Particle is initialized and added.
		/// </summary>
		protected virtual void AfterAddParticle()
		{ }

		/// <summary>
		/// Virtual function to setup the Render Properties (i.e. BlendState, DepthStencilState, RasterizerState, and SamplerState)
		/// which will be applied to the Graphics Device before drawing the Particle System's Particles.
		/// <para>This function is only called once when initializing the particle system.</para>
		/// </summary>
		protected virtual void InitializeRenderProperties()
		{ }

		/// <summary>
		/// Virtual function to Set the Effect's Parameters before drawing the Particles.
		/// <para>This is called every time before the particle system is drawn.</para>
		/// </summary>
		protected virtual void SetEffectParameters()
		{ }

		#endregion
	}
}
