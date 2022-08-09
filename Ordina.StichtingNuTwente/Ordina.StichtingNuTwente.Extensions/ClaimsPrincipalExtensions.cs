namespace Ordina.StichtingNuTwente.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasClaims(this System.Security.Claims.ClaimsPrincipal value, string type, params string[] values)
        {
            foreach (var typeValue in values)
            {
                if (value.HasClaim(type, typeValue))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
