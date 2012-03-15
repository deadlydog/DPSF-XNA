namespace System.Collections.Generic
{
	/// <summary>
	/// Class used to extend other classes.
	/// </summary>
	public static class GenericCollectionsExtensions
	{
		/// <summary>
		/// Removes all elements from the List that match the conditions defined by the specified predicate.
		/// </summary>
		/// <typeparam name="T">The type of elements held by the List.</typeparam>
		/// <param name="list">The List to remove the elements from.</param>
		/// <param name="match">The Predicate delegate that defines the conditions of the elements to remove.</param>
		public static int RemoveAll<T>(this System.Collections.Generic.List<T> list, Func<T, bool> match)
		{
			int numberRemoved = 0;

			// Loop through every element in the List, in reverse order since we are removing items.
			for (int i = (list.Count - 1); i >= 0; i--)
			{
				// If the predicate function returns true for this item, remove it from the List.
				if (match(list[i]))
				{
					list.RemoveAt(i);
					numberRemoved++;
				}
			}

			// Return how many items were removed from the List.
			return numberRemoved;
		}

		/// <summary>
		/// Returns true if the List contains elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <typeparam name="T">The type of elements held by the List.</typeparam>
		/// <param name="list">The List to search for a match in.</param>
		/// <param name="match">The Predicate delegate that defines the conditions of the elements to match against.</param>
		public static bool Exists<T>(this System.Collections.Generic.List<T> list, Func<T, bool> match)
		{
			// Loop through every element in the List, until a match is found.
			for (int i = 0; i < list.Count; i++)
			{
				// If the predicate function returns true for this item, return that at least one match was found.
				if (match(list[i]))
					return true;
			}

			// Return that no matching elements were found in the List.
			return false;
		}
	}
}
