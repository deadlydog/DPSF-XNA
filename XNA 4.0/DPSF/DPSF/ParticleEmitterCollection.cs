using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DPSF.Exceptions;

namespace DPSF
{
	/// <summary>
	/// Holds a collection of ParticleEmitters.
	/// </summary>
#if (WINDOWS)
	[Serializable]
#endif
	public class ParticleEmitterCollection
	{
		/// <summary>
		/// The dictionary used to hold the collection of emitters.
		/// We want to use a dictionary for this because one particle system may potentially end up with hundreds of emitters (e.g. every enemy has an emitter), so we want
		/// to be able to perform lookups very quickly, and dictionaries can do adds/removes/lookups in constant O(1) time.
		/// </summary>
		private readonly Dictionary<int, ParticleEmitter> _emitters = new Dictionary<int, ParticleEmitter>();

		/// <summary>
		/// The list of Emitter IDs to return back to clients when requested.
		/// This is essentially just a copy of the Emitter Dictionary's Keys, but we need it since the Keys collection can only be iterated over using an enumerator, which basically
		/// means using a foreach loop, which is bad for performance. To compensate, every time the Keys collection is modified we update this variable to hold all of the keys.
		/// This allows clients to use a regular for loop in order to iterate over all of the Emitter IDs, which means less garbage will be generated for the garbage collector.
		/// Essentially we are assuming that the IDs property will be accessed more often than the Add() and Remove() functions are called.
		/// </summary>
		private ReadOnlyCollection<int> _emitterIdsCache = new ReadOnlyCollection<int>(new List<int>());

		/// <summary>
		/// Fires anytime a ParticleEmitter is removed from the collection and the collection is left empty.
		/// </summary>
		public event EventHandler AllEmittersRemoved = delegate { };

		/// <summary>
		/// Adds a new ParticleEmitter to the list of emitters and returns it.
		/// </summary>
		/// <returns>Returns the ParticleEmitter added to the collection.</returns>
		public ParticleEmitter Add()
		{
			return Add(null);
		}

		/// <summary>
		/// Adds the given ParticleEmitter to the list of emitters and returns it.
		/// </summary>
		/// <param name="emitter">The ParticleEmitter to add to the collection.</param>
		/// <returns>Returns the ParticleEmitter added to the collection.</returns>
		public ParticleEmitter Add(ParticleEmitter emitter)
		{
			ParticleEmitter newEmitter = emitter ?? new ParticleEmitter();
			_emitters.Add(newEmitter.ID, newEmitter);

			// Update our IDs cache.
			_emitterIdsCache = new ReadOnlyCollection<int>(_emitters.Keys.ToList());

			return newEmitter;
		}

		/// <summary>
		/// Removes the ParticleEmitter with the given ID from the collection.
		/// Returns false if the ParticleEmitter was not found in the collection.
		/// </summary>
		/// <param name="id">The ID of the ParticleEmitter to remove.</param>
		/// <returns>Returns false if the ParticleEmitter was not found in the collection.</returns>
		public bool Remove(int id)
		{
			// Try and remove the emitter and record if it was found.
			bool found = _emitters.Remove(id);

			// If the ID was found and removed, update our IDs cache.
			if (found)
				_emitterIdsCache = new ReadOnlyCollection<int>(_emitters.Keys.ToList());

			// If the last emitter was just removed, fire the All Emitters Removed event.
			if (found && _emitters.Count <= 0)
				AllEmittersRemoved(this, EventArgs.Empty);

			// Return if the emitter was removed or not.
			return found;
		}

		/// <summary>
		/// Removes the given ParticleEmitter from the collection.
		/// Returns false if the ParticleEmitter was not found in the collection.
		/// </summary>
		/// <param name="emitter">The ParticlEmitter to remove from the collection.</param>
		/// <returns>Returns false if the ParticleEmitter was not found in the collection.</returns>
		public bool Remove(ParticleEmitter emitter)
		{
			if (emitter == null)
				return false;
			return Remove(emitter.ID);
		}

		/// <summary>
		/// Removes all ParticleEmitters from the collection.
		/// </summary>
		public void RemoveAll()
		{
			bool hadAtLeastOneEmitter = _emitters.Any();

			// Remove any emitters.
			_emitters.Clear();

			// Update our IDs cache.
			_emitterIdsCache = new ReadOnlyCollection<int>(_emitters.Keys.ToList());

			// If there was an emitter, we removed it, so fire the All Emitters Removed event.
			if (hadAtLeastOneEmitter)
				AllEmittersRemoved(this, EventArgs.Empty);
		}

		/// <summary>
		/// Returns if the ParticleEmitter with the given ID is in this collection or not.
		/// </summary>
		/// <param name="id">The ID of the ParticleEmitter to check for.</param>
		/// <returns>Returns true if the ParticleEmitter with the given ID is in the collection, false if not.</returns>
		public bool Contains(int id)
		{
			return _emitters.ContainsKey(id);
		}

		/// <summary>
		/// Returns if the given ParticleEmitter is in this collection or not.
		/// </summary>
		/// <param name="emitter">The ParticleEmitter to check for.</param>
		/// <returns>Returns true if the ParticleEmitter is in the collection, false if not.</returns>
		public bool Contains(ParticleEmitter emitter)
		{
			if (emitter == null)
				return false;
			return _emitters.ContainsKey(emitter.ID);
		}

		/// <summary>
		/// Gets how many ParticleEmitters are in this collection.
		/// </summary>
		public int Count { get { return _emitters.Count; } }

		/// <summary>
		/// Returns all of the ParticleEmitters in this collection.
		/// </summary>
		/// <returns>Returns all of the ParticleEmitters in this collection.</returns>
		public ICollection<ParticleEmitter> Emitters
		{
			get { return _emitters.Values; }
		}

		/// <summary>
		/// Returns a list of IDs for the ParticleEmitters that this collection contains.
		/// </summary>
		public ReadOnlyCollection<int> IDs
		{
			get { return _emitterIdsCache; }
		}

		/// <summary>
		/// Overload the [] operator to allow for direct access to ParticleEmitters in the collection using indexer syntax.
		/// Throws an exception if a ParticleEmitter with the given ID does not exist in the collection.
		/// </summary>
		/// <param name="id">The ID of the ParticleEmitter to retrieve.</param>
		/// <returns>Returns the ParticleEmitter with the given ID.</returns>
		public ParticleEmitter this[int id]
		{
			get
			{
				if (_emitters.ContainsKey(id))
					return _emitters[id];
				else
					throw new DPSFKeyNotFoundException("A ParticleEmitter with an ID of " + id + " does not exist in this collection.\n" +
						"Use the Contains() function to ensure the emitter is in this collection before trying to access it.");
			}
		}
	}
}