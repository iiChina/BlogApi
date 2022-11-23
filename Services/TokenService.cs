using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Models;
using BlogApi.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi.Services
{
    public class TokenService
    {
        //Método para gerar um token.
        public string GenerateToken(User user)
        {
            //Manipular do token - Classe que contém os métodos para construção de um token.
            var tokenHandler = new JwtSecurityTokenHandler();
            //Convertendo a key em um arr[Byte], pois o token é composto por esse array e não aceita string
            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

            var claims = user.GetClaims();
            //Criando um objeto que irá carregar as configurações do token.
            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                // Afirmações sobre o token.
                // Esse trecho traz novas funcionalidades para as controllers da aplicação, pois podemos faezr uso do objeto (ClaimPrincipal)User
                Subject = new ClaimsIdentity(claims),
                //Tempo de expiração do token - Dica: Entre 3 à 8 hrs
                Expires = DateTime.UtcNow.AddHours(8),
                //Como esse token vai ser gerado e lido.
                //SigningCredentials() - Ele pede a chave e o algoritmo para encriptar e desincriptar.
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            //Criando o token.
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //Retorna o token em formato string
            return tokenHandler.WriteToken(token);
        }
    }
}