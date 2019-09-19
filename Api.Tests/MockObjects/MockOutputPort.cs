using Api.Core.Interfaces;

namespace Api.Tests.MockObjects
{
    public class MockOutputPort<TUseCaseResponseMessage> : IOutputPort<TUseCaseResponseMessage>
        where TUseCaseResponseMessage : UseCaseResponseMessage
    {
        public TUseCaseResponseMessage Response { get; private set; }

        public void Handle(TUseCaseResponseMessage response)
        {
            Response = response;
        }
    }
}