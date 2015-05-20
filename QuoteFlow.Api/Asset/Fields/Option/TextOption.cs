using System.Collections;

namespace QuoteFlow.Api.Asset.Fields.Option
{
    public class TextOption : AbstractOption, IOption
    {
        public TextOption(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public TextOption(string id, string name, string cssClass)
        {
            Id = id;
            Name = name;
            CssClass = cssClass;
        }

        public override string ImagePath { get; set; }
        public override string ImagePathHtml { get; set; }
        public override string CssClass { get; set; }
        public override IList ChildOptions { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}