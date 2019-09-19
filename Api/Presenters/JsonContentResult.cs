using Microsoft.AspNetCore.Mvc;

namespace Api.Presenters
{
    public class JsonContentResult : ContentResult
    {
        public JsonContentResult()
        {
            ContentType = "application/json";
        }
    }
}