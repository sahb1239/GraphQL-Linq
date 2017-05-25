using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL_Linq.Queryable
{
    /// <summary>
    /// Returns a <see cref="IAsyncEnumerable{T}"/> from a Task which return a <see cref="IEnumerable{T}"/>
    /// The <see cref="Task{TResult}"/> is invoked when the first element is accessed using a <see cref="IAsyncEnumerator{T}"/> using the <see cref="GetEnumerator"/> method
    /// </summary>
    /// <typeparam name="T">The type of the elements to contain</typeparam>
    internal class AsyncEnumerableTask<T> : IAsyncEnumerable<T>
    {
        private Task<IEnumerable<T>> _enumerable;

        public AsyncEnumerableTask(Task<IEnumerable<T>> enumerable)
        {
            _enumerable = enumerable;
        }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new IEnumerableTaskAsyncEnumerator<T>(_enumerable);
        }

        private class IEnumerableTaskAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly Task<IEnumerable<T>> _enumerable;
            private IEnumerator<T> _enumeratorLoaded;

            public IEnumerableTaskAsyncEnumerator(Task<IEnumerable<T>> enumerable)
            {
                _enumerable = enumerable;
            }

            protected async Task<IEnumerator<T>> Enumerator()
            {
                return _enumeratorLoaded ??
                       (_enumeratorLoaded = (await _enumerable).GetEnumerator());
            }


            public void Dispose()
            {
                _enumeratorLoaded?.Dispose();
            }

            public async Task MoveNext(CancellationToken cancellationToken)
            {
                (await Enumerator()).MoveNext();
            }

            async Task<bool> IAsyncEnumerator<T>.MoveNext(CancellationToken cancellationToken)
            {
                return (await Enumerator()).MoveNext();
            }

            public T Current => _enumeratorLoaded != null ? _enumeratorLoaded.Current : default(T);
        }
    }
}