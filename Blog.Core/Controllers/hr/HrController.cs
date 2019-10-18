using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Core.Controllers.hr
{
    [Route("hr")]
    public class HrController : Controller
    {
        [HttpGet]
        [Route("getList")]
        public async void GetList()
        {
            using (HttpClient client = new HttpClient())
            {
                using (var response = await client.GetAsync("http://hr.aiegle.com:9999/signin?username=A001530&password=a12345678&locale=zh"))
                {
                    var cookies = response.Headers.GetValues("Set-Cookie").ToList()[0].Split('=');
                    string content = await response.Content.ReadAsStringAsync();

                    // 创建请求
                    //将刚才的cookies放入cookiescontainer并加入初始化
                    CookieContainer cookiescontainer = new CookieContainer();
                    cookiescontainer.Add(new Uri("http://hr.aiegle.com:9999"), new Cookie(cookies[0], cookies[1], "/", "hr.aiegle.com:9999"));
                    var handler = new HttpClientHandler() { CookieContainer = cookiescontainer, AllowAutoRedirect = false, UseCookies = true };

                    HttpClient httpClient = new HttpClient(handler);
                    httpClient.Timeout = TimeSpan.FromSeconds(10);

                    string url = "http://hr.aiegle.com:9999/";
                    var response2 = await httpClient.GetAsync(url);
                }
            }
        }
    }
}
