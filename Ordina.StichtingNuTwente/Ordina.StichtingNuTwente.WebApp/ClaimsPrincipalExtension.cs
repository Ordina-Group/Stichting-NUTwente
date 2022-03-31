namespace Ordina.StichtingNuTwente.WebApp
{
    public static class ClaimsPrincipalExtension
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
