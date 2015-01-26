using System.Text;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Api.Asset.Search
{
    /// <summary>
    /// A shareable representation of a search for assets. Officially known as a "Filter" or "Saved Filter".
    /// 
    /// This class binds the <see cref="IQuery"/>, which is used to perform the actual search, and
    /// the saved information (such as name, description), and any permissions that may be associated with the saved search
    /// together.
    /// </summary>
    public class SearchRequest
    {
        [Inject]
        public IUserService UserService { private get; set; }

        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                Modified = true;
                _name = value;
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                Modified = true;
                _description = value;
            }
        }

        public int OwnerId { get; set; }

        private User _owner;
        public User Owner
        {
            get { return UserService.GetUser(OwnerId); }
            set
            {
                Modified = true;
                _owner = value;
            }
        }

        private IQuery _query;
        public IQuery Query
        {
            get { return _query; }
            set
            {
                Modified = true;
                _query = value;
            }
        }

        public bool Modified { get; set; }

        private bool _loaded;
        public bool Loaded
        {
            get { return Id != 0; }
            set { _loaded = value; }
        }

        /// <summary>
        /// Whether to use the search requests specific columns
        /// </summary>
        public bool UseColumns { get; set; }

        /// <summary>
        /// A no-arg constructor that will build a SearchRequest with an empty <see cref="IQuery"/>, this
        /// will be a search that will find all issues with the default system sorting . You can then use the setter methods
        /// to set the attributes you wish this SearchRequest to contain.
        /// </summary>
        public SearchRequest()
        {
            Query = new Query(null, new OrderBy(), null);
            Modified = false;
            UseColumns = true;
        }

        /// <summary>
        /// Creates a SearchRequest with the specified <seealso cref="IQuery"/> and no other attributes.
        /// This can be used to create a programtic SearchRequest that can be used to perform a search but is not ready to
        /// be saved.
        /// </summary>
        /// <param name="query">Provides the details of the search that will be performed with this SearchRequest.</param>
        public SearchRequest(IQuery query)
        {
            Query = query;
            Modified = false;
            UseColumns = true;
        }

        /// <summary>
        /// Used to create a SearchRequest that copies all the information from the old search request. This includes
        /// the name, description, author, id, favCount and the SearchQuery.
        /// </summary>
        /// <param name="oldRequest">Defines all the attributes that this SearchRequest will contain.</param>
        public SearchRequest(SearchRequest oldRequest)
            : this(oldRequest.Query, oldRequest.Owner, oldRequest.Name, oldRequest.Description, oldRequest.Id)
        {
            UseColumns = oldRequest.UseColumns;
            Modified = oldRequest.Modified;
        }

        /// <summary>
        /// Build a SearchRequest with the provided attributes, this can be used if you want to create a SearchRequest that
        /// can be persisted.
        /// </summary>
        /// <param name="query">Defines what this SearchRequest will search for. </param>
        /// <param name="owner">The owner, user who initially create the request. </param>
        /// <param name="name">The name associated with this SearchRequest, can be set even if this is not persistent yet.</param>
        /// <param name="description">The description associated with this SearchRequest, can be set even if this is not persistent yet.</param>
        public SearchRequest(IQuery query, User owner, string name, string description)
            : this()
        {
            this.OwnerId = owner.Id;
            this.Name = name;
            this.Description = description;
            this.Query = query;
        }

        /// <summary>
        /// Build a SearchRequest with the provided attributes.
        /// </summary>
        /// <param name="query">Defines what this SearchRequest will search for. </param>
        /// <param name="owner">The owner, user who initially create the request. </param>
        /// <param name="name">The name associated with this SearchRequest, can be set even if this is not persistent yet.</param>
        /// <param name="description">The description associated with this SearchRequest, can be set even if this is not persistent yet.</param>
        /// <param name="id">The persistent id of the SearchRequest, null if the SearchRequest is not persistent.</param>
        public SearchRequest(IQuery query, User owner, string name, string description, int id)
            : this()
        {
            this.OwnerId = owner.Id;
            this.Name = name;
            this.Description = description;
            this.Query = query;
            this.Id = id;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Search Request: name: {0}\n", Name);

            if (Query != null && Query.ToString().HasValue())
            {
                sb.AppendFormat("query = {0}", Query.ToString());
            }

            return sb.ToString();
        }
    }
}