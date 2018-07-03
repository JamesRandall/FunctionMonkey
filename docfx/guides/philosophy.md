# Philosophy - Why Develop this Framework?

For some of my underlying thinking (if you are interested!) its worth [reading the comments I made about the mediator framework Function Monkey makes use of](https://commanding.azurefromthetrenches.com/guides/philosophy.html).

Really what I wrote their also applies to Function Monkey and I wanted to apply the same philosophy to Azure Functions. 

Beyond that - I'm currently designing and implementing a number of serverless architectures and I wanted to accelerate my development and deal with some of the common pain points.

And I'm not very keen on the attribute model used by the Azure Functions team though I can see and appreciate why they went for that. In general I dislike the attribute model except in some very specific use cases as I find it makes for hard to read code that tends to start muddling concerns and bleed out into a broader codebase.

Its also very different putting together an API that abstracts itself from opinions about implementation compared to one that makes strong opinions about implementation.