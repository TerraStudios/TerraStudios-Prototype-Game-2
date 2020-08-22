using System.Collections.Generic;

/// <summary>
/// Contains various extensions for <see cref="Dictionary{TKey, TValue}"/>s.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Retrieves an element from a dictionary, returning <paramref name="defaultValue"/> if not found.
    /// </summary>
    /// <typeparam name="T">The key of the Dictionary</typeparam>
    /// <typeparam name="V">The value of the Dictionary</typeparam>
    /// <param name="dictionary">The <see cref="Dictionary{TKey, TValue}"/> being accessed</param>
    /// <param name="key">The key to retrieve from</param>
    /// <param name="defaultValue">The default value if no key is found, defaulting to <code>default(V)</code>.</param>
    /// <returns>The value from the entry, or <code>default(V)</code> if not found.</returns>
    public static V GetOrDefault<T, V>(this IDictionary<T, V> dictionary, T key, V defaultValue = default) 
    {
        return dictionary.TryGetValue(key, out V value) ? value : defaultValue;
    }

    /// <summary>
    /// Retrieves an element from the dictionary, creating a new entry if it doesn't exist
    /// </summary>
    /// <typeparam name="T">The key of the Dictionary</typeparam>
    /// <typeparam name="V">The value of the Dictionary</typeparam>
    /// <param name="dict">The <see cref="Dictionary{T, V}"/> being referenced</param>
    /// <param name="key">The key to retrieve from</param>
    /// <returns>The value from the entry, or <code>new V</code> if not found.</returns>
    public static V GetOrPut<T, V>(this IDictionary<T, V> dict, T key)
    where V : new()
    {
        V val;

        if (!dict.TryGetValue(key, out val))
        {
            val = new V();
            dict.Add(key, val);
        }

        return val;
    }

}