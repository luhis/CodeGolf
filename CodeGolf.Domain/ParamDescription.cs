namespace CodeGolf.Domain
{
    using System;

    using EnsureThat;

    public class ParamDescription
    {
        public ParamDescription(Type type, string suggestedName)
        {
            this.Type = EnsureArg.IsNotNull(type, nameof(type));
            this.SuggestedName = EnsureArg.IsNotNull(suggestedName, nameof(suggestedName));
        }

        public Type Type { get; }

        public string SuggestedName { get; }
    }
}