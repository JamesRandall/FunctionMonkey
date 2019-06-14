using System;
using System.Collections.Concurrent;
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
        public async Task AuthenticateWithBindingExpressionNegotiateAndSendAndRecieveMessage()
        {
            ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
            Guid markerId = Guid.NewGuid();

            string receivedMessage = null;
            SignalRToken token = await Settings.Host
                .AppendPathSegment("simpleNegotiate")
                .WithHeaders(new { x_ms_client_principal_id = Guid.NewGuid() })
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

        [Fact]
        public async Task AuthenticateWithBindingExpressionNegotiateWithUserIdInHeaderAndSendAndRecieveMessage()
        {
            ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
            Guid[] markerIds = { Guid.NewGuid()};
            string userId = Guid.NewGuid().ToString();

            SignalRToken token = await Settings.Host
                .AppendPathSegment("simpleNegotiate")
                .WithHeaders(new { x_ms_client_principal_id = userId })
                .GetJsonAsync<SignalRToken>();

            Assert.NotNull(token);

            HubConnection connection = new HubConnectionBuilder()
                .WithUrl(token.Url, options => options.AccessTokenProvider = () => Task.FromResult(token.AccessToken))
                .Build();

            connection.On<string>("sendMessageCollectionCommand", (message) =>
            {
                resetEvent.Set();
            });
            await connection.StartAsync();

            await Settings.Host
                .AppendPathSegment("signalR")
                .AppendPathSegment("messageCollectionToUser")
                .PostJsonAsync(new
                {
                    userId,
                    markerIds
                });

            bool didReceive = resetEvent.Wait(TimeSpan.FromSeconds(10));
            Assert.True(didReceive);
        }

        [Fact]
        public async Task CanRecieveMessageAfterAddingToGroupButNotAfterRemoving()
        {
            ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
            string userId = Guid.NewGuid().ToString();

            await Settings.Host
                .AppendPathSegment("signalR")
                .AppendPathSegment("addUserToGroup")
                .PutJsonAsync(new
                {
                    UserId = userId,
                    GroupName = "group1"
                });

            SignalRToken token = await Settings.Host
                .AppendPathSegment("negotiate")
                .SetQueryParam("userId", userId)
                .GetJsonAsync<SignalRToken>();

            HubConnection connection = new HubConnectionBuilder()
                .WithUrl(token.Url, options => options.AccessTokenProvider = () => Task.FromResult(token.AccessToken))
                .Build();

            connection.On<string>("sendMessageToGroupCommand", (message) =>
            {
                resetEvent.Set();
            });
            await connection.StartAsync();

            await Settings.Host
                .AppendPathSegment("signalR")
                .AppendPathSegment("messageToGroup")
                .AppendPathSegment("group1")
                .SetQueryParam("userId", userId)
                .GetAsync();

            bool didReceive = resetEvent.Wait(TimeSpan.FromSeconds(5));
            Assert.True(didReceive);
            resetEvent.Reset();

            await Settings.Host
                .AppendPathSegment("signalR")
                .AppendPathSegment("removeUserFromGroup")
                .PutJsonAsync(new
                {
                    UserId = userId,
                    GroupName = "group1"
                });

            await Settings.Host
                .AppendPathSegment("signalR")
                .AppendPathSegment("messageToGroup")
                .AppendPathSegment("group1")
                .SetQueryParam("userId", userId)
                .GetAsync();

            didReceive = resetEvent.Wait(TimeSpan.FromSeconds(5));
            Assert.False(didReceive);
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

        [Fact]
        public async Task AuthenticateSendAndRecieveMessagesForUser1234()
        {
            Guid[] markerIds = { Guid.NewGuid(), Guid.NewGuid() };
            ConcurrentBag<Guid> receivedGuids = new ConcurrentBag<Guid>();

            SignalRToken token = await Settings.Host
                .AppendPathSegment("negotiate")
                .SetQueryParam("userId", "1234")
                .GetJsonAsync<SignalRToken>();

            Assert.NotNull(token);

            HubConnection connection = new HubConnectionBuilder()
                .WithUrl(token.Url, options => options.AccessTokenProvider = () => Task.FromResult(token.AccessToken))
                .Build();

            connection.On<Guid>("sendMessageCollectionCommand", (markerId) =>
            {
                receivedGuids.Add(markerId);
                
            });
            await connection.StartAsync();

            await Settings.Host
                .AppendPathSegment("signalR")
                .AppendPathSegment("messageCollectionToUser")
                .SetQueryParam("message", markerIds.ToString())
                .PostJsonAsync(new
                {
                    userId = "1234",
                    markerIds
                });

            double timeTaken = 0;
            while (timeTaken < 20000 && receivedGuids.Count < 2)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                timeTaken += 500;
            }

            Assert.Contains(markerIds[0], receivedGuids);
            Assert.Contains(markerIds[1], receivedGuids);
        }

        [Fact]
        public async Task AuthenticateSendAndRecieveMessagesForUser345EnsuringUser678DoesNotReceiveMessage()
        {
            ConcurrentBag<string> usersThatRecievedMessages = new ConcurrentBag<string>();
            ManualResetEventSlim doneWaiting = new ManualResetEventSlim(false);
            WaitHandle[] listeningStarted = {
                new ManualResetEvent(false),
                new ManualResetEvent(false)
            };
            WaitHandle[] finishedListening =
            {
                new ManualResetEvent(false),
                new ManualResetEvent(false)
            };

#pragma warning disable 4014
            Task.Run(async () =>
#pragma warning restore 4014
            {
                await Task.WhenAll(ConnectAndWaitForMessages("345", usersThatRecievedMessages, (ManualResetEvent)listeningStarted[0], (ManualResetEvent)finishedListening[0]),
                    ConnectAndWaitForMessages("678", usersThatRecievedMessages, (ManualResetEvent)listeningStarted[1], (ManualResetEvent)finishedListening[1]));
                doneWaiting.Set();
            });
            
            bool areListening = WaitHandle.WaitAll(listeningStarted, TimeSpan.FromSeconds(20)); // wait for the clients to start listening
            Assert.True(areListening);

            Guid[] markerIds = new Guid[] { Guid.NewGuid() };
            await Settings.Host
                .AppendPathSegment("signalR")
                .AppendPathSegment("messageCollectionToUser")
                .PostJsonAsync(new
                {
                    userId = "345",
                    markerIds
                });

            WaitHandle.WaitAll(finishedListening, TimeSpan.FromSeconds(10));

            Assert.Contains("345", usersThatRecievedMessages);
            Assert.DoesNotContain("678", usersThatRecievedMessages);
        }

        private async Task ConnectAndWaitForMessages(string userId,
            ConcurrentBag<string> usersThatRecievedMessages, ManualResetEvent listeningStartedEvent, ManualResetEvent finishedListeningEvent)
        {
            ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
            
            SignalRToken token = await Settings.Host
                .AppendPathSegment("negotiate")
                .SetQueryParam("userId", userId)
                .GetJsonAsync<SignalRToken>();

            HubConnection connection = new HubConnectionBuilder()
                .WithUrl(token.Url, options => options.AccessTokenProvider = () => Task.FromResult(token.AccessToken))
                .Build();

            connection.On<Guid>("sendMessageCollectionCommand", (markerId) =>
            {
                resetEvent.Set();

            });
            await connection.StartAsync();


            listeningStartedEvent.Set();

            bool didReceive = resetEvent.Wait(TimeSpan.FromSeconds(10));
            if (didReceive)
            {
                usersThatRecievedMessages.Add(userId);
            }

            finishedListeningEvent.Set();
        }
    }
}
