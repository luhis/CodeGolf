namespace CodeGolf.Service.Handlers
{
    using System;
    using MediatR;
    using Optional;

    public class GetGame : IRequest<Option<Guid>>
    {
        public GetGame(string accessKey)
        {
            this.AccessKey = accessKey;
        }

        public string AccessKey { get; }
    }
}
