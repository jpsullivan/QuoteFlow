using Lucene.Net.Documents;
using QuoteFlow.Models.Assets.Context;

namespace QuoteFlow.Models.Assets.Index.Indexers
{
    public interface IFieldIndexer
    {
        /// <summary>
        /// The string representation of the field id that this indexer is indexing, this must be unique for
        /// each independant FieldIndexer. If the Indexer does not represent a System or Custom field in QuoteFlow this
        /// should still return a unique string that describes the indexer.
        /// </summary>
        string Id { get; }

        /// <returns> the String representation of the primary field id that will be added to the
        /// <seealso cref="Document"/> as a result of a successful call to the
        /// <seealso cref="AddIndex(Document, Asset)"/> method. </returns>
        string DocumentFieldId { get; }

        /// <summary>
        /// This method allows an indexer the opportunity to modifiy the provided Lucene document (by reference) such
        /// that it will contain fields that are relevant for searching and storage of the portion of the issue that
        /// the FieldIndexer handles.
        /// 
        /// If calling <see cref="IsFieldVisibleAndInScope(Asset)"/> returns false then
        /// this method should create fields that have an Indexed type of <see cref="Field.Index.NO"/>.
        /// This allows us to store the value in the index but renderes its value unsearchable.
        /// 
        /// If, for example, the indexer handles indexing an issues summary then this indexer will add a field to
        /// the document that represents the stored and searchable summary of the issue.
        /// </summary>
        /// <param name="doc">The lucene document that should be modified by adding relevant fields to.</param>
        /// <param name="asset">
        /// The asset that contains the data that will be indexed and which can be used to determine
        /// the project/issue type context that will allow you to determine if we should add the value as searchable
        /// or unsearchable.
        /// </param>
        void AddIndex(Document doc, Asset asset);

        /// <summary>
        /// This method is used to determine if the indexer is relevant for the provided issue. This method must check
        /// the fields visibility, in relation to the field configuration scheme, must check any global flags that would
        /// enable or disable a field (such as enable votes flag), and must check, if the field is a custom field, if
        /// the custom field is relevant for this issue.
        /// 
        /// All these checks should take into account the <seealso cref="AssetContext"/> as defined by
        /// the passed in issue.
        /// 
        /// If this method returns false then the FieldIndexer, when performing addIndex, should make sure to make the
        /// indexed values have an Indexed type of <seealso cref="Field.Index.NO"/>.
        /// 
        /// The result of this method is used to determine the correct values that should be returned when performing
        /// an empty search.
        /// </summary>
        /// <param name="asset">That is having a document created from.</param>
        /// <returns> if true then this field is relevant for the issue, otherwise it is not.</returns>
        bool IsFieldVisibleAndInScope(Asset asset);
    }

    public class FieldIndexerTypes
    {
        /// <summary>
        /// General empty token.
        /// </summary>
        public static string NoValueIndexValue = "-1";

        /// <summary>
        /// Empty token specific to LabelsIndexer.
        /// </summary>
        public static string LabelsNoValueIndexValue = "<EMPTY>";
    }
}