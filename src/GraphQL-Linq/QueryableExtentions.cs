using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL_Linq
{
    /// <summary>
    /// GraphQL Linq Extentions
    /// </summary>
    public static class QueryableExtentions
    {
        /// <summary>
        /// Returns a <see cref="IAsyncEnumerable{T}" /> from a <see cref="IQueryable{T}"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The source to return the <see cref="IAsyncEnumerable{T}"/> from</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> which can be enumerated async</returns>
        private static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this IQueryable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var asyncEnumerable = source as IAsyncEnumerable<TSource>;
            if (asyncEnumerable != null)
            {
                return asyncEnumerable;
            }

            throw new InvalidOperationException("Queryable is not async");
        }

        /// <summary>
        ///     Asynchronously creates a <see cref="List{T}" /> from an <see cref="IQueryable{T}" /> by enumerating it
        ///     asynchronously.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <typeparam name="TSource">
        ///     The type of the elements of <paramref name="source" />.
        /// </typeparam>
        /// <param name="source">
        ///     An <see cref="IQueryable{T}" /> to create a list from.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains a <see cref="List{T}" /> that contains elements from the input sequence.
        /// </returns>
        public static async Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var list = new List<TSource>();

            using (var asyncEnumerator = source.AsAsyncEnumerable().GetEnumerator())
            {
                while (await asyncEnumerator.MoveNext(cancellationToken))
                {
                    list.Add(asyncEnumerator.Current);
                }
            }

            return list;
        }
    }
}
