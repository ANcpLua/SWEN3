namespace Agents.Configuration
{
    public class ElasticSearchConfig
    {
        public string Url { get; set; } = "http://localhost:9200";
        public string DefaultIndex { get; set; } = "documents";

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                throw new InvalidOperationException("ElasticSearch URL cannot be null, empty, or whitespace.");
            }

            if (string.IsNullOrWhiteSpace(DefaultIndex))
            {
                throw new InvalidOperationException("ElasticSearch DefaultIndex cannot be null, empty, or whitespace.");
            }
        }
    }
}