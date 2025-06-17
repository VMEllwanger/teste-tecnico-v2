using System.Diagnostics.CodeAnalysis;

namespace Thunders.TechTest.ApiService
{
    [ExcludeFromCodeCoverage]
    public class Features
    {
        public required bool UseMessageBroker { get; set; }
        public required bool UseEntityFramework { get; set; }

        public static Features BindFromConfiguration(IConfiguration section)
        {
            var features = section.GetSection("Features").Get<Features>() ??
                throw new InvalidOperationException("No 'Features' section found");

            return features;
        }
    }
}
