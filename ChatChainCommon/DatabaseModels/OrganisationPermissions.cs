using System.ComponentModel.DataAnnotations;

namespace ChatChainCommon.DatabaseModels
{
    public enum OrganisationPermissions
    {
        //EDIT
        [Display(Name = "Edit Clients")]
        EditClients,
        [Display(Name = "Edit Groups")]
        EditGroups,
        [Display(Name = "Edit Organisation Users")]
        EditOrgUsers,
        [Display(Name = "Edit Organisation")]
        EditOrg,
        
        //CREATE
        [Display(Name = "Create Clients")]
        CreateClients,
        [Display(Name = "Create Groups")]
        CreateGroups,
        [Display(Name = "Create Organisation Users")]
        CreateOrgUsers,
        
        //DELETE
        [Display(Name = "Delete Clients")]
        DeleteClients,
        [Display(Name = "Delete Groups")]
        DeleteGroups,
        [Display(Name = "Delete Organisation Users")]
        DeleteOrgUsers,

        //ALL
        [Display(Name = "All Permissions")]
        All
    }
}