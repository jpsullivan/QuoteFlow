using System.Collections.Generic;
using System.Text;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Asset.Editor
{
    /// <summary>
    /// Returns a list of ordered fields to be used to render the quick edit forms. 
    /// The fields are in the same order as on the normal edit screen.
    /// 
    /// This object also contains a list of field IDs that specify which fields should 
    /// be visible in the quick edit dialog.
    /// </summary>
    public class EditFields
    {
        public List<string> Fields { get; set; }
        public string XsrfToken { get; set; }
        public IErrorCollection ErrorCollection { get; set; }

        protected EditFields()
        {
        }

        public EditFields(string xsrfToken, IErrorCollection errorCollection)
        {
            Fields = new List<string>();
            XsrfToken = xsrfToken;
            ErrorCollection = errorCollection;
        }

        public EditFields(List<string> fields, string xsrfToken, IErrorCollection errorCollection)
        {
            Fields = fields;
            XsrfToken = xsrfToken;
            ErrorCollection = errorCollection;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Fields);
            sb.Append(ErrorCollection);

            return sb.ToString();
        }
    }
}