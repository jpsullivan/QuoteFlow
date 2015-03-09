using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteFlow.Api.Entity.Property
{
    /// <summary>
    /// Type of the entity property.
    /// </summary>
    public class EntityPropertyType
    {
        public static readonly EntityPropertyType ASSET_PROPERTY = new EntityPropertyType("AssetProperty", "common.concepts.asset", "ASSETPROP", "asset.property");
        public static readonly EntityPropertyType CATALOG_PROPERTY = new EntityPropertyType("CatalogProperty", "common.concepts.catalog", "CATALOGPROP", "catalog.property");
        public static readonly EntityPropertyType MANUFACTURER_PROPERTY = new EntityPropertyType("ManufacturerProperty", "common.concepts.manufacturer", "MANUFACTURERPROP", "manufacturer.property");
        public static readonly EntityPropertyType COMMENT_PROPERTY = new EntityPropertyType("CommentProperty", "common.concepts.comment", "COMMENTPROP", "comment.property");

        private static readonly IDictionary<string, EntityPropertyType> JqlClauseToProperty = new Dictionary<string, EntityPropertyType>();

        public virtual string DbEntityName { get; private set; }
        public virtual string I18NKeyForEntityName { get; private set; }
        public virtual string JqlName { get; private set; }
        public virtual string IndexPrefix { get; private set; }
        static EntityPropertyType()
        {
            JqlClauseToProperty[ASSET_PROPERTY.JqlName] = ASSET_PROPERTY;
        }

        public EntityPropertyType(string dbEntityName, string i18NKeyForEntityName, string indexPrefix, string jqlName)
        {
            DbEntityName = dbEntityName;
            I18NKeyForEntityName = i18NKeyForEntityName;
            IndexPrefix = indexPrefix;
            JqlName = jqlName;
        }

        public static bool IsJqlClause(string clauseName)
        {
            return clauseName != null && JqlClauseToProperty.ContainsKey(clauseName);
        }
        public static EntityPropertyType GetEntityPropertyTypeForClause(string clauseName)
        {
            return JqlClauseToProperty[clauseName];
        }
    }
}
