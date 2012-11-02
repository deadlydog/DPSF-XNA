using System;
using System.Collections.Generic;
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
		/// </summary>
		private readonly Dictionary<int, ParticleEmitter> _emitters = new Dictionary<int, ParticleEmitter>(); 

		/// <summary>
		/// Fires anytime a ParticleEmitter is removed from the collection and the collection is left empty.
		/// </summary>
		public event EventHandler AllEmittersRemoved = null;

		/// <summary>
		/// Adds a new ParticleEmitter to the list of emitters and returns the ParticleEmitter's unique ID that can be used to retrieve it from the collection.
		/// </summary>
		/// <returns>Returns the ParticleEmitter's unique ID that can be used to reference the ParticleEmitter in the collection.</returns>
		public ParticleEmitter Add()
		{
			return Add(null);
		}

		/// <summary>
		/// Adds the given ParticleEmitter to the list of emitters and returns its unique ID that can be used to retrieve it from the collection.
		/// </summary>
		/// <param name="emitter">The ParticleEmitter to add to the collection.</param>
		/// <returns>Returns the ParticleEmitter's unique ID that can be used to reference the ParticleEmitter in the collection.</returns>
		public ParticleEmitter Add(ParticleEmitter emitter)
		{
			ParticleEmitter newEmitter;
			if (emitter == null)
				newEmitter = new ParticleEmitter();
			else
				newEmitter = emitter;

			_emitters.Add(newEmitter.ID, newEmitter);
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
			return _emitters.Remove(id);
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
			return _emitters.Remove(emitter.ID);
		}

		/// <summary>
		/// Removes all ParticleEmitters from the collection.
		/// </summary>
		public void RemoveAll()
		{
			_emitters.Clear();
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
		public IList<ParticleEmitter> Emitters
		{
			get
			{
				return _emitters.Values.ToList();
			}
		}

		/// <summary>
		/// Returns a list of IDs for the ParticleEmitters that this collection contains.
		/// </summary>
		public IList<int> IDs
		{
			get { return _emitters.Keys.ToList(); }
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