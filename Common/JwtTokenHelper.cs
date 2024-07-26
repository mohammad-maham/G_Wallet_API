using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace G_APIs.Common
{
    public static class JwtTokenHelper
    {
        //private static ConcurrentDictionary<int, string> onlineUsers = new ConcurrentDictionary<int, string>();
        //private static Timer timer;
        //static JwtTokenHelper()
        //{
        //    timer = new Timer(x =>
        //    {
        //        try
        //        {
        //            onlineUsers.Clear();
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }, null, 1000, 15 * 1000);
        //}

        private const string key = "/-*789][{]}]-=+F08E0C64-C2A7-44B1-B18F-F74CB542DE97_JWT_KEY_^^%%&&$#@#@#_046F1BB1-F74E-46EF-AAB3-FCDACBE5CFF4";
        private const string encryptionkey = "16CharEncryptKey"; //must be 16 character
        public static string GetToken(LoginParams login, int TimeToLiveInMinute = 0)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionkey)), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var permClaims = new List<Claim>();
            var sid = Guid.NewGuid().ToString();
            //onlineUsers.Where(x => x.Key == login.UserId).ToList().ForEach(x => onlineUsers.TryRemove(x.Key, out var _));
            //onlineUsers.TryAdd((int)login.UserId, sid);
            permClaims.Add(new Claim("gid", sid));
            permClaims.Add(new Claim("login", JsonConvert.SerializeObject(login,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore })));
            var descriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.Now,
                Expires = TimeToLiveInMinute == 0 ? DateTime.Now.AddDays(1) : DateTime.Now.AddMinutes(TimeToLiveInMinute),
                SigningCredentials = credentials,
                //EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(permClaims)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateEncodedJwt(descriptor);
            //var jwt_token = tokenHandler.(securityToken);
            return securityToken;
        }

        public static bool ValidateJwtToken(string token, out LoginParams login)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var validationParameters = new TokenValidationParameters();
                validationParameters.ValidIssuer = null;
                validationParameters.ValidAudience = null;
                validationParameters.IssuerSigningKey = securityKey;
                validationParameters.RequireExpirationTime = true;
                validationParameters.ClockSkew = new TimeSpan(0, 5, 0);
                validationParameters.ValidateIssuer = false;
                validationParameters.ValidateAudience = false;
                //validationParameters.TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionkey));

                var validator = new JwtSecurityTokenHandler();
                var res = validator.ValidateToken(token, validationParameters, out var securityToken);
                var loginClaim = res.Claims.FirstOrDefault(x => x.Type == "login");
                var sid = res.Claims.FirstOrDefault(x => x.Type == "gid");
                if (loginClaim == null || sid == null)
                {
                    throw new Exception("Claim not found");
                }
                login = JsonConvert.DeserializeObject<LoginParams>(loginClaim.Value);
                //if (onlineUsers.TryGetValue(login.UserId, out var prevSid))
                //{
                //    if (prevSid != sid.Value)
                //        throw new Exception("Duplicate login found");
                //}
                //else
                //{
                //    onlineUsers.TryAdd(login.UserId, sid.Value);
                //}
                return true;
            }
            catch (Exception e)
            {
                login = null;
                return false;
            }

        }

        public static string GetTokenForText(string text)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionkey)), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var permClaims = new List<Claim>();
            var sid = Guid.NewGuid().ToString();
            permClaims.Add(new Claim("gid", sid));
            permClaims.Add(new Claim("text", text));

            var descriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.AddDays(1).Date,
                SigningCredentials = credentials,
                //EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(permClaims)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);
            var jwt_token = tokenHandler.WriteToken(securityToken);
            return jwt_token;
        }

        public static bool ValidateJwtTokenForText(string token, out string text)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var validationParameters = new TokenValidationParameters();
                validationParameters.ValidIssuer = null;
                validationParameters.ValidAudience = null;
                validationParameters.IssuerSigningKey = securityKey;
                validationParameters.RequireExpirationTime = true;
                validationParameters.ClockSkew = new TimeSpan(0, 5, 0);
                validationParameters.ValidateIssuer = false;
                validationParameters.ValidateAudience = false;
                //validationParameters.TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionkey));

                var validator = new JwtSecurityTokenHandler();
                var res = validator.ValidateToken(token, validationParameters, out var securityToken);
                var textClaim = res.Claims.FirstOrDefault(x => x.Type == "text");
                var sid = res.Claims.FirstOrDefault(x => x.Type == "gid");
                if (textClaim == null || sid == null)
                {
                    throw new Exception("Claim not found");
                }

                text = textClaim.Value;

                return true;
            }
            catch (Exception)
            {
                text = null;
                return false;
            }

        }
    }
}