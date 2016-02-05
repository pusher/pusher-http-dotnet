using System.Web.Script.Serialization;

namespace PusherServer
{
    /// <summary>
    /// Default implmentation for serializing an object
    /// </summary>
    public class DefaultSerializer : ISerializeObjectsToJson
    {
        /// <inheritDoc/>
        public string Serialize(object objectToSerialize)
        {
            return new JavaScriptSerializer().Serialize(objectToSerialize);
        }
    }
}