using System.Collections.Generic;
using Api.Core.DTO;
using Api.Core.DTO.UseCaseResponses.Organisation;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Serialization;
using Microsoft.AspNetCore.Http;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Api.Presenters
{
    public class GetOrganisationPresenter : IOutputPort<GetOrganisationResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetOrganisationPresenter()
        {
            ContentResult = new JsonContentResult();
        }
        
        public void Handle(GetOrganisationResponse response)
        {
            ContentResult.StatusCode = response.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
            ContentResult.Content = JsonSerializer.SerializeObject(new ResponseObjectDTO
            {
                Organisation = response.Organisation.ToOrganisation(),
                Success = response.Success,
                Message = response.Message,
                CheckedPermissions = response.CheckedPermissions,
                Errors = response.Errors
            });
        }
    }
    
    public class ResponseObjectDTO
    {
        public Organisation Organisation { get; set; }
        
        public bool Success { get; set; }

        public string Message { get; set; }

        public bool CheckedPermissions { get; set;}

        public IEnumerable<Error> Errors { get; set; }        
    }
}