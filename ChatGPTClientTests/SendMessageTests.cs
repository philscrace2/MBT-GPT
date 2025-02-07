using NUnit.Framework;
using ChatGPT;

namespace ChatGPTClientTests
{
    [TestFixture]
    public class SendMessageTests
    {

        [Test]
        public void SendMessage()
        {
            ChatGPTClient client = new ChatGPTClient();

            Assert.That(client.SendMessage("What is the capital of France"), Is.EqualTo("The capital of France is Paris."));
        }

    }
}
