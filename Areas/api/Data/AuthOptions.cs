using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MedBrat.Areas.api.Data
{
    public class AuthOptions
    {
        public const string ISSUER = "MedBratServer";
        public const string AUDIENCE = "MedBratClient";
        const string KEY = "23spef423FseF!s_srod_esr1123WERsr!s";
        public const int LIFETIME = 20;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
