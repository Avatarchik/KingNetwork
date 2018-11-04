using System.Collections.Generic;

namespace KingNetwork.Shared
{
    /// <summary>
    /// This class is responsible for represents the king object pool.
    /// </summary>
    public class KingObjectPool<T>
    {
        #region private members 	

        /// <summary>
        /// The stack of objects to pool.
        /// </summary>
        private readonly Stack<T> _pool;

        #endregion

        #region properties

        /// <summary>
        /// The size value of stack.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// The count of king pool objects in stack.
        /// </summary>
        public int Count => _pool.Count;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="KingObjectPool"/>.
        /// </summary>
        public KingObjectPool(int size)
        {
            Size = size;
            _pool = new Stack<T>(Size);
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// This method is responsible for pop the last ojtect from stack.
        /// </summary>
        public T Pop()
        {
            lock (_pool)
                return _pool.Pop();
        }

        /// <summary>
        /// This method is responsible for push the object in stack.
        /// </summary>
        /// <param name="obj">The object item to push in stack.</param>
        public void Push(T obj)
        {
            lock (_pool)
                _pool.Push(obj);
        }

        #endregion
    }
}
