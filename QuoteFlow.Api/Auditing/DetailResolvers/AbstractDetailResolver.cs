namespace QuoteFlow.Api.Auditing.DetailResolvers
{
    public class AbstractDetailResolver : IDetailResolver
    {
        public string Serialize()
        {
            return Jil.JSON.Serialize(this);
        }
    }
}