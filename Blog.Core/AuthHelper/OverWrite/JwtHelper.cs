using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.AuthHelper.OverWrite
{
    /// <summary>
    /// 权限Helper
    /// </summary>
    public class JwtHelper
    {
        /// <summary>
        /// key
        /// </summary>
        public static string secretKey { get; set; } = "sdfsdfsrty45634kkhllghtdgdfss345t678fs";
        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public static string IssueJWT(TokenModelJWT tokenModel)
        {
            var dateTime = DateTime.UtcNow;
            var claims = new Claim[]
            {
               new Claim(JwtRegisteredClaimNames.Jti,tokenModel.Uid.ToString()),//Id
               new Claim("Role", tokenModel.Role),//角色
               new Claim(JwtRegisteredClaimNames.Iat,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
               new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddSeconds(10)).ToUnixTimeSeconds()}")
            };

            //秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtHelper.secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: "Blog.Core",    // jwt颁发者，非必须
                claims: claims,         // 声明集合
                expires: dateTime.AddHours(2),  // 指定token的生命周期，unix时间戳格式，非必须
                signingCredentials: creds);     // 使用私钥进行签名加密

            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            return encodedJwt;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static TokenModelJWT SerializeJWT(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            object role = new object(); ;
            try
            {
                //修改 ClaimTypes.Role => "Role" : 根据字符串"Role"获取解析后的角色
                jwtToken.Payload.TryGetValue("Role", out role);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var tm = new TokenModelJWT
            {
                Uid = (jwtToken.Id).ObjToInt(),
                Role = role != null ? role.ObjToString() : "",
            };
            return tm;
        }
    }
}