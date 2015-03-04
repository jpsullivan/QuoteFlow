namespace QuoteFlow.Api.Asset.Fields.Layout.Column
{
    /// <summary>
    /// Represents the cause or source of columns in an asset table, e.g. whether the were 
    /// requested explicitly, configured as the columns of a filter or the user's 
    /// configured defaults.
    /// </summary>
    public enum ColumnConfig
    {
        /// <summary>
        /// Columns come from the system defaults.
        /// </summary>
        System,

        /// <summary>
        /// Columns were explicitly listed in the asset table request.
        /// </summary>
        Explicit,

        /// <summary>
        /// Columns come from the filter.
        /// </summary>
        Filter,

        /// <summary>
        /// Columns come from the user's default column config.
        /// </summary>
        User,

        /// <summary>
        /// No columns are used. The default value.
        /// </summary>
        None
    }
}