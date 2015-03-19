using System;
using System.Collections.Generic;
using QuoteFlow.Api.Index;

namespace QuoteFlow.Core.Index
{
    /// <summary>
    /// Manages search extractors.
    /// </summary>
    public interface ISearchExtractorRegistrationManager
    {
        /// <summary>
        /// Return all extractors that can be applied for specified class no subclasses are taken into account.
        /// </summary>
        /// <param name="entityClass">Class object to search for extractors.</param>
        /// <returns>List of extractors.</returns>
        IEnumerable<IEntitySearchExtractor<T>> findExtractorsForEntity<T>(Type entityClass);

        /// <summary>
        /// Registers extractor as capable o processing documents of specified class
        /// </summary>
        /// <param name="extractor">The extractor that will be registered for processing entities of <see cref="entityClass"/>.</param>
        /// <param name="entityClass">Class that is process by this extractor.</param>
        void Register<T, T1>(IEntitySearchExtractor<T1> extractor, Type entityClass);

        /// <summary>
        /// Remove this extractor (identified by equals method) from processing all registered classes.
        /// </summary>
        /// <param name="extractor">Instance of extractor to be unregistered.</param>
        /// <param name="entityClass">Class that this extractor should be unregistered from.</param>
        void Unregister<T, T1>(IEntitySearchExtractor<T1> extractor, Type entityClass);
    }
}