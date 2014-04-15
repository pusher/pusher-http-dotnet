using RestSharp.Serializers;

namespace PusherServer
{                                                                    
    /// <summary>
    /// An implementation of <see cref="IBodySerializer"/> that uses <see cref="JsonSerializer"/> for its implementation.
    /// </summary>
    public class DefaultJsonBodySerializer : IBodySerializer
    {
        /// <inheritdoc/>
        public string Serialize(object body)
        {
            var serializer = new JsonSerializer();
            return serializer.Serialize(body);
        }
    }
}