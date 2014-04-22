using System.Collections;
using System.Collections.Generic;
using QuoteFlow.Models;

namespace QuoteFlow.Services.Interfaces
{
    public interface IOrganizationService
    {
        Organization GetOrganization(int organizationId);

        Organization GetOrganization(string organizationName);

        IEnumerable<Organization> GetOrganizations(int[] organizationIds);

        /// <summary>
        /// Gets a list of the organizations that a user is assigned to.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<OrganizationUser> GetOrganizations(int userId);

        Organization CreateOrganization(string organizationName, int ownerId);

        bool OrganizationExists(string organizationName);
    }
}