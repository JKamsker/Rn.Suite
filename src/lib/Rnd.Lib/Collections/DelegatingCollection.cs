using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Rnd.Collections
{
    public abstract class DelegatingCollection<T> : ICollection<T>
    {
        protected ICollection<T> UnderlyingCollection;

        public DelegatingCollection(ICollection<T> implementation)
        {
            this.UnderlyingCollection = implementation;
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.UnderlyingCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.UnderlyingCollection).GetEnumerator();
        }

        public virtual void Add(T item)
        {
            this.UnderlyingCollection.Add(item);
        }

        public virtual void Clear()
        {
            this.UnderlyingCollection.Clear();
        }

        public virtual bool Contains(T item)
        {
            return this.UnderlyingCollection.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            this.UnderlyingCollection.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(T item)
        {
            return this.UnderlyingCollection.Remove(item);
        }

        public virtual int Count => this.UnderlyingCollection.Count;

        public virtual bool IsReadOnly => this.UnderlyingCollection.IsReadOnly;

        //public void RemoveRange(IEnumerable<T> values) => values.ForEach(x => this.Remove(x));
        public virtual void RemoveRange(IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                this.Remove(item);
            }
        }
    }
}