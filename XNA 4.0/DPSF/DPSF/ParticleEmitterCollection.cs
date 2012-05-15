using System;
using System.Collections.Generic;
using DPSF.Exceptions;

namespace DPSF
{
	public class ParticleEmitterCollection
	{
		/// <summary>
		/// The dictionary used to hold all of a particle system's emitters.
		/// </summary>
		private readonly Dictionary<int, ParticleEmitter> _emitters = new Dictionary<int, ParticleEmitter>(); 

		/// <summary>
		/// Adds a new ParticleEmitter to the list of emitters and returns the unique ID that can be used to retrieve it from the collection.
		/// </summary>
		/// <returns>Returns the unique ID that can be used to reference the ParticleEmitter in the collection.</returns>
		public int Add()
		{
			return Add(null);
		}

		/// <summary>
		/// Adds a copy of the given ParticleEmitter to the list of emitters and returns the unique ID that can be used to retrieve it from the collection.
		/// </summary>
		/// <param name="emitterToCopy">The ParticleEmitter to add to the collection.</param>
		/// <returns>Returns the unique ID that can be used to reference the ParticleEmitter in the collection.</returns>
		public int Add(ParticleEmitter emitterToCopy)
		{
			ParticleEmitter newEmitter;
			if (emitterToCopy == null)
				newEmitter = new ParticleEmitter();
			else
				newEmitter = new ParticleEmitter(emitterToCopy);

			_emitters.Add(newEmitter.ID, newEmitter);
			return newEmitter.ID;
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
		/// Returns all of the ParticleEmitters in this collection.
		/// </summary>
		/// <returns>Returns all of the ParticleEmitters in this collection.</returns>
		public IEnumerable<ParticleEmitter> Emitters()
		{
			foreach (ParticleEmitter emitter in _emitters.Values)
				yield return emitter;
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