using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace PusherServer.Tests.Helpers
{
    internal static class DataHelper
    {
        internal static List<Event> CreateEvents(int numberOfEvents)
        {
            return CreateEvents(numberOfEvents, eventSizeInBytes: "{}".Length);
        }

        internal static List<Event> CreateEvents(int numberOfEvents, int eventSizeInBytes)
        {
            Assert.IsTrue(eventSizeInBytes >= 2, "The event size must be greater than or equal to 2.");

            List<Event> events = new List<Event>(numberOfEvents);

            // We need to account for braces {} around the text when serialized.
            string data = new string('Q', eventSizeInBytes - "{}".Length);
            for (int i = 0; i < numberOfEvents; i++)
            {
                events.Add(new Event { Channel = "testChannel", EventName = "testEvent", Data = data });
            }

            return events;
        }

        internal static byte[] GenerateEncryptionMasterKey()
        {
            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
            byte[] key = new byte[32];
            random.GetBytes(key);
            return key;
        }
    }
}
