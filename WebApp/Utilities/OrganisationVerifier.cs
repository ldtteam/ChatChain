using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;

namespace WebApp.Utilities
{
    public static class OrganisationUserVerifier
    {
        public static async Task<Tuple<bool, Organisation>> VerifyOrganisationId(this PageModel pageModel, string organisationId, OrganisationService organisationService)
        {
            if (organisationId == null || !ObjectId.TryParse(organisationId, out ObjectId id))
                return new Tuple<bool, Organisation>(false, null);

            Organisation organisation = await organisationService.GetAsync(id);

            return organisation == null ? new Tuple<bool, Organisation>(false, null) : new Tuple<bool, Organisation>(true, organisation);
        }

        public static async Task<Tuple<bool, Organisation>> VerifyIsMember(this PageModel pageModel,
            string organisationId, OrganisationService organisationService)
        {
            Tuple<bool, Organisation> verificationTuple = await pageModel.VerifyOrganisationId(organisationId, organisationService);
            if (!verificationTuple.Item1)
                return verificationTuple;
            
            return !verificationTuple.Item2.Users.ContainsKey(pageModel.User.Claims.First(claim => claim.Type.Equals("sub")).Value) ? new Tuple<bool, Organisation>(false, verificationTuple.Item2) : new Tuple<bool, Organisation>(true, verificationTuple.Item2);
        }
        
        public static async Task<Tuple<bool, Organisation>> VerifyIsOwner(this PageModel pageModel,
            string organisationId, OrganisationService organisationService)
        {
            Tuple<bool, Organisation> verificationTuple = await pageModel.VerifyIsMember(organisationId, organisationService);
            if (!verificationTuple.Item1)
                return verificationTuple;
            
            return !verificationTuple.Item2.Owner.Equals(pageModel.User.Claims.First(claim => claim.Type.Equals("sub")).Value) ? new Tuple<bool, Organisation>(false, verificationTuple.Item2) : new Tuple<bool, Organisation>(true, verificationTuple.Item2);
        }
        
        public static async Task<Tuple<bool, Organisation>> VerifyUserPermissions(this PageModel pageModel, string organisationId, OrganisationService organisationService, OrganisationPermissions requiredPermission)
        {
            Tuple<bool, Organisation> verificationTuple = await pageModel.VerifyIsMember(organisationId, organisationService);
            if (!verificationTuple.Item1)
                return verificationTuple;

            IList<OrganisationPermissions> permissions = verificationTuple.Item2.Users[pageModel.User.Claims.First(claim => claim.Type.Equals("sub")).Value].Permissions;

            if (!(permissions.Contains(OrganisationPermissions.All) || permissions.Contains(requiredPermission)))
                return new Tuple<bool, Organisation>(false, verificationTuple.Item2);

            return new Tuple<bool, Organisation>(true, verificationTuple.Item2);
        }
    }
}