module FunctionMonkey.FSharp.Tests.Integration.Functions.Constants

module SignalR =
    let hubName = "simplehub"
    
module Storage =
    module Table =
        let markers = "markers"
        
    module Queue =
        let testQueue = "testqueue"
        let markerQueue = "markerqueue"
        
    module Blob =
        let blobCommandContainer = "blobcommands"
        let streamBlobCommandContainer = "streamblobcommands"
        let outputBlobContainer = "outputblobcontainer"
        let blobOutputCommandContainer = "outputbindingcontainer"
        
module ServiceBus =
    let markerQueue = "markerqueue"
    let markerQueueWithSessionId = "markerqueuesessionid"
    let markerTopicWithSessionId = "markertopicsessionid"
    let narkerTopic = "markertopic"
    let markerSubscription = "markersub"
    let queue = "testqueue"
    let sessionIdQueue = "sessionidtestqueue"
    let topicName = "testtopic"
    let subscriptionName = "testsub"
    let sessionIdTopicName = "sessionidtesttopic"
    let sessionIdSubscriptionName = "sessionidtestsub"
    let tableOutputQueue = "tableoutput"
    let signalRQueue = "signalr"
    
module Cosmos =
    let database = "testdatabase"
    let collection = "testcollection"
    let outputTableCollection = "outputtablecollection"
    let outputTableLeases = "outputtableleases"
