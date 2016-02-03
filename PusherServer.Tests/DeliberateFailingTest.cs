using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PusherServer.Tests
{
    [TestFixture]
    public class DeliberateFailingTest
    {
        [Test]
        public void ThisTestWillFail()
        {
            Assert.AreEqual(1,2);
        }
    }
}
