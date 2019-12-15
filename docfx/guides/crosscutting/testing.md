# Integration Testing

add reference to nuget package: FunctionMonkey.Testing

```
public class GetEchoQueryTests : AbstractAcceptanceTest
    {
        [Fact]
        public async Task ReturnOkResultForQuery()
        {
            var response = await this.ExecuteHttpAsync(
                new GetEchoQuery(),
                HttpMethod.Get);

            response.StatusCode.ShouldBe(200);
        }
    }
```
