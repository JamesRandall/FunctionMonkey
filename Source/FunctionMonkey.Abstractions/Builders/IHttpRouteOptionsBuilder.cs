namespace FunctionMonkey.Abstractions.Builders
{
    public interface IHttpRouteOptionsBuilder
    {
        IHttpRouteOptionsBuilder ClaimsPrincipalAuthorization<TClaimsPrincipalAuthorization>()
            where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization;
    }
}