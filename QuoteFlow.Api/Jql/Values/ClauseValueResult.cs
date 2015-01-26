namespace QuoteFlow.Api.Jql.Values
{
    public class ClauseValueResult
    {
        public string Value { get; set; }
        public string[] DisplayNameParts { get; set; }

        public ClauseValueResult(string value)
        {
            Value = value;
            DisplayNameParts = new[] {value};
        }

        public ClauseValueResult(string value, string displayName)
        {
            Value = value;
            DisplayNameParts = new[] { displayName };
        }

        public ClauseValueResult(string value, string[] displayNameParts)
        {
            Value = value;
            DisplayNameParts = displayNameParts;
        }

        public override string ToString()
        {
            return string.Format("Result { value={0}, displayNameParts={1}", Value, DisplayNameParts);
        }
    }
}