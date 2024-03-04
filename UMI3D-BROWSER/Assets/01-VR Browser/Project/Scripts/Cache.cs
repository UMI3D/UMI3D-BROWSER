using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A listwith the comportement of a queue with a max Size.
/// </summary>
/// <typeparam name="T">Type of the elements inside the cache</typeparam>
public class Cache<T>
{
    private readonly int m_MaxElements;
    protected List<T> m_Values;

    /// <summary>
    /// Max elements this cache can contain.
    /// </summary>
    public int MaxElements => m_MaxElements;

    /// <summary>
    /// Actual number of element inside the cache
    /// </summary>
    public int Count => m_Values.Count;

    public T this[int index] => m_Values.ElementAt(index);

    /// <summary>
    /// Create a cache of <see cref="MaxElements"/> elements max.
    /// </summary>
    /// <param name="maxElements">Maximum number of element inside the cache</param>
    public Cache(int maxElements = 10)
    {
        m_MaxElements = maxElements;
        m_Values = new List<T>();
    }

    /// <summary>
    /// Add a value to the cache.
    /// The latest element of the cache will be removed if there is no more room.
    /// </summary>
    /// <param name="value">Object to be added to the cache</param>
    public void Add(T value)
    {
        if (m_Values.Count + 1 > m_MaxElements && m_Values.Count > 0)
            m_Values.RemoveAt(0);
        m_Values.Add(value);
    }

    /// <summary>
    /// Empty the cache
    /// </summary>
    public void Empty() => m_Values = new List<T>();

    public static implicit operator List<T>(Cache<T> cache) => cache.m_Values;
}
