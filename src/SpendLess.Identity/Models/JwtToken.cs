using Microsoft.IdentityModel.Tokens;
using SpendLess.Application.Models.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SpendLess.Identity.Models
{
    public class JwtToken : JwtSecurityToken, IToken
    {
        public JwtToken(string issuer = null, string audience = null, IEnumerable<Claim> claims = null, DateTime? notBefore = null, DateTime? expires = null, SigningCredentials signingCredentials = null)
        : base(issuer: issuer, audience: audience, claims: claims, notBefore: notBefore, expires: expires, signingCredentials: signingCredentials)
        {
            
        }
    }
}
