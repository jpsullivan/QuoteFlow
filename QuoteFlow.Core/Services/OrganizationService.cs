using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Core.Services
{
    public class OrganizationService : IOrganizationService
    {
        public Organization GetOrganization(int organizationId)
        {
            return Current.DB.Organizations.Get(organizationId);
        }

        public Organization GetOrganization(string organizationName)
        {
            const string sql = "select * from Organizations where OrganizationName = @organizationName";
            return Current.DB.Query<Organization>( sql, new { organizationName }).First();
        }

        public IEnumerable<Organization> GetOrganizations(int[] organizationIds)
        {
            const string sql = @"select * from Organizations where Id in @organizationIds";
            return Current.DB.Query<Organization>(sql, new { organizationIds }).ToList();
        }

        /// <summary>
        /// Gets a list of the organizations that a <see cref="User"/> is assigned to.
        /// </summary>
        /// <param name="userId">The <see cref="User"/> identifier.</param>
        /// <returns>A collection of <see cref="Organization"/> records.</returns>
        public IEnumerable<OrganizationUser> GetOrganizations(int userId)
        {
            const string sql = "select * from OrganizationUsers where UserId = @userId";
            return Current.DB.Query<OrganizationUser>(sql, new {@userId});
        }

        public Organization CreateOrganization(string organizationName, int ownerId)
        {
            if (!OrganizationExists(organizationName))
            {
                try
                {
                    var org = new Organization
                    {
                        OrganizationName = organizationName,
                        OwnerId = ownerId,
                        CreatorId = ownerId
                    };

                    var insert = Current.DB.Organizations.Insert(new
                    {
                        org.OrganizationName,
                        org.OwnerId,
                        org.CreatorId
                    });

                    if (insert != null)
                    {
                        org.Id = insert.Value;
                    }

                    return org;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                // display an error message or something
            }

            return null;
        }

        public bool OrganizationExists(string organizationName)
        {
            return Current.DB.Query<Organization>(
                "select 1 from Organizations where OrganizationName = @organizationName", new
                {
                    organizationName
                }).Any();
        }
    }
}