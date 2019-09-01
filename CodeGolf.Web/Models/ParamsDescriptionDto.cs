namespace CodeGolf.Web.Models
{
    public class ParamsDescriptionDto
    {
        public ParamsDescriptionDto(string type, string suggestedName)
        {
            this.Type = type;
            this.SuggestedName = suggestedName;
        }

        public string Type { get; }

        public string SuggestedName { get; }
    }
}
