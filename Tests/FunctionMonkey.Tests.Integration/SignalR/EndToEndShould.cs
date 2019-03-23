using System;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using FunctionMonkey.Tests.Integration.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace FunctionMonkey.Tests.Integration.SignalR
{
    public class EndToEndShould : AbstractHttpFunctionTest
    {
        private class SignalRToken
        {
            public string Url { get; set; }

            public string AccessToken { get; set; }
        }

        [Fact]
        public async Task AuthenticateSendAndRecieveMessage()
        {
            ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
            Guid markerId = Guid.NewGuid();
                
            string receivedMessage = null;
            SignalRToken token = await Settings.Host
                .AppendPathSegment("negotiate")
                .GetJsonAsync<SignalRToken>();

            Assert.NotNull(token);

            HubConnection connection = new HubConnectionBuilder()
                .WithUrl(token.Url, options => options.AccessTokenProvider = () => Task.FromResult(token.AccessToken))
                .Build();

            connection.On<string>("sendMessageCommand", (message) =>
            {
                receivedMessage = message;
                resetEvent.Set();
            });
            await connection.StartAsync();

            await Settings.Host
                .AppendPathSegment("signalR")
                .AppendPathSegment("messageToAll")
                .SetQueryParam("message", markerId.ToString())
                .GetAsync();

            bool didReceive = resetEvent.Wait(TimeSpan.FromSeconds(30));
            Assert.True(didReceive);
            Assert.Equal(markerId.ToString(), receivedMessage);
        }
    }
}
