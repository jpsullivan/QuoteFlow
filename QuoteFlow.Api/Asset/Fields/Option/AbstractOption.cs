using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace QuoteFlow.Api.Asset.Fields.Option
{
    public abstract class AbstractOption : IOption
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual string ImagePath
        {
            get { return null; }
            set { throw new System.NotImplementedException(); }
        }

        public virtual string ImagePathHtml
        {
            get { return HttpUtility.HtmlEncode(ImagePath); }
            set { throw new System.NotImplementedException(); }
        }

        public virtual string CssClass
        {
            get { return null; }
            set { throw new System.NotImplementedException(); }
        }

        public virtual IList ChildOptions
        {
            get { return new List<object>(); }
            set { throw new System.NotImplementedException(); }
        }

        protected bool Equals(AbstractOption other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && string.Equals(Description, other.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AbstractOption) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}