using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Services
{
    public class UsersServices : IUsersServices
    {
        private IUsersRepository _userRepository;
        private IConfiguration _configuration;

        public UsersServices(IUsersRepository usersRepository, IConfiguration configuration)
        {
            _userRepository = usersRepository;
            _configuration = configuration;
        }

        public int CheckPassword(string password)
        {
            var result = Zxcvbn.Core.EvaluatePassword(password);
            return result.Score;

        }


        public async Task<User> GetById(int id)
        {
            return await _userRepository.GetById(id);
        }

        public async Task<User> Register(User user)
        {

            if (CheckPassword(user.Password) <= 2)
                return null;
            return await _userRepository.Register(user);
        }

        public async Task<User> Login(User userLogin)
        {
           var user= await _userRepository.Login(userLogin);

            user.Token = generateJwtToken(userLogin);
            return user;
        }

        public async Task<User> Update(int id, User userToUpdate)
        {
            return await _userRepository.Update(id, userToUpdate);
        }

        private string generateJwtToken(User user)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("key").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                   // new Claim("roleId", 7.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

    }
}

