namespace PusherServer
{
    /// <summary>
    /// Responsible for serializing the message body to a string.
    /// </summary>
    public interface IBodySerializer
    {
        /// <summary>
        /// Serializes the body to a string.
        /// </summary>
        /// <param name="body">The object to serialize.</param>
        /// <returns>The serialized body.</returns>
        string Serialize(object body);
    }
}