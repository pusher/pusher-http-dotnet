namespace PusherDotNet
{
	public interface IPusherProvider
	{
		/// <summary>
		/// Sends the request. Will throw a WebException if anything other than a 202 response is encountered
		/// </summary>
		/// <param name="request"></param>
		void Trigger(PusherRequest request);
	}
}