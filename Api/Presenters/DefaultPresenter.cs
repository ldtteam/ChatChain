using Api.Core.Interfaces;
using Api.Serialization;
using Microsoft.AspNetCore.Http;

namespace Api.Presenters
{
    public class DefaultPresenter : IOutputPort<UseCaseResponseMessage>
    {
        public JsonContentResult ContentResult { get; }

        public DefaultPresenter()
        {
            ContentResult = new JsonContentResult();
        }
        
        public void Handle(UseCaseResponseMessage response)
        {
            ContentResult.StatusCode = response.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
            ContentResult.Content = JsonSerializer.SerializeObject(response);
        }
    }
}