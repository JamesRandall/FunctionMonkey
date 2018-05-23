# Philosophy - Why Develop this Framework?

I began work on the framework when I found myself facing some challenging constraints on projects including:

* Limited budgets.
* Very small teams.
* High expectations.
* Emerging requirements while searching for market fit.
* The need to address scale as the product became established - both technical (requests per second etc.) and development (larger and more teams)

Essentially I knew I would be heading towards a micro-service architecture but wouldn't be able to pay the [micro-service tax](https://martinfowler.com/bliki/MicroservicePremium.html) until the product was established.

Given that operations in a distributed system are essentially expressed as state - messages - then adopting that approach for in-process operations seemed to be a good starting point if I wanted to allow myself to move fairly easily from something more monolithic into something decomposed.

That led me to the idea of starting with a modular monolith with clear bounded contexts and many of the sub-systems hosted in shared process spaces that I would later be able to decompose into isolated components communicating with events, brokered messages and REST APIs.

I wanted to be able to construct this modular monolith in C# in a way that would be well supported by tooling and maximise productivity early on in development. At the same time I wanted to be able to decompose it without changing any business logic and with as little wider code change as possible. And I didn't want the decomposition to lead to the rewriting of hundreds of unit tests. To achieve that I settled on a configuration based approach making use of a commanding pattern with operations expressed as state and dispatched via a mediator to various handlers.

I looked at the popular [Mediatr](https://github.com/jbogard/MediatR) framework however given my own goals I was concerned that it did not pull apart dispatch and execution. This was something I felt that ought to be masked by the mediator when executing commands but ought to be exposed in a way that would allow for extension. With hindsight I'm not sure how much of this was luck or genuine forward thinking on my part but it was a realisation that proved to be key when I wanted to execute commands over HTTP connections and dispatch them to queues.

All that being the case I began work on this framework. Of course I didn't get all this implemented in version 1. It's rather been a process of steadily learning and evolving both the thinking behind the approach and the framework itself based on experience and feedback. If you look back at my initial commits things changed massively to begin with as I worked through concepts and ideas. Lots of things have been added and changed and a few ideas dropped but the core has been stable for a while now and recent work has been about adding additional extensions, smoothing out the developer experience, and reducing boilerplate.