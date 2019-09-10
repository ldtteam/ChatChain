using System;

namespace Api.Models
{
    public class CreateClientResponse
    {
        public string Password { get; set; }
        public Guid Id { get; set; }
    }
}