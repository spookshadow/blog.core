using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Threading;

namespace Blog.Core.Controllers
{
    /// <summary>
    /// 创想客户端调用接口
    /// </summary>
    [Route("cx/win")]
    public class WinController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpHead]
        public void Index()
        {

        }
        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Ceshi")]
        public string Ceshi()
        {
            return "Hello测试Test";
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public ActionResult Login(string account, string pwd)
        {
            var resMsg = new ResultMsg();
            try
            {
                resMsg.StatusCode = (int)StatusCodeEnum.Success;
                resMsg.Info = "操作成功！";
                var token = new ApiToken
                {
                    AuthCode = account,
                    SignToken = Guid.NewGuid(),
                    ExpireTime = DateTime.Now.AddDays(1)
                };

                #region json
                var json = @"{
    'StatusCode': 200,
    'Info': '登录成功',
    'Data': {
        'AuthCode': '70d89e4f-f69b-4cf6-9b62-1ee6a5d7dcdb',
        'SignToken': '9d2ffe75-d0f8-4d7b-8059-d11b0dbb7afd',
        'ExpireTime': '2019-01-04T16:38:56.99+08:00',
        'Userinfo': {
            'realName': '邹冬梅',
            'account': 'F134',
            'shopId': 'fe908aaf-ada5-4cb6-ab16-7ac2f36972eb'
        },
        'ShopList': [
            {
                'shopId': 'fe908aaf-ada5-4cb6-ab16-7ac2f36972eb',
                'shopName': '西安雅居阁店'
            },
            {
                'shopId': 'xxxxxxxx-ada5-4cb6-ab16-xxxxxxxxxxxx',
                'shopName': 'xxxxxx店'
            }
        ]
    }
}";
                #endregion

                resMsg = json.ToObject<ResultMsg>();
                resMsg.Info = $"{resMsg.Info}, account={account},pwd={pwd}";
            }
            catch (Exception ex)
            {
                resMsg.StatusCode = (int)StatusCodeEnum.Error;
                resMsg.Info = ex.Message;
            }

            return Json(resMsg);
        }

        private Dictionary<string, List<Customer>> dic = new Dictionary<string, List<Customer>>();
        /// <summary>
        /// 获取我的出图单客户信息
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="intentOrderType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OrderInfo")]
        public ActionResult OrderInfo([FromBody]OrderInfoRequest request)
        {
            var resMsg = new ResultMsg();
            try
            {
                resMsg.StatusCode = (int)StatusCodeEnum.Success;
                resMsg.Info = "操作成功！";
                //Thread.Sleep(5000);
                var key = request.ShopId + request.IntentOrderType;
                if (!dic.ContainsKey(key))
                {
                    List<Customer> list = new List<Customer>();
                    Random random = new Random();
                    var max = random.Next(20);
                    for (int i = 0; i < max; i++)
                    {
                        Customer customer = Customer.Instance(
                        "客户" + i,
                        "导购" + i, i,
                        "状态" + i,
                        "1366666666" + i,
                        key + i + "号（地铁3号线龙归站附近）",
                        i % 2 == 0 ? "男" : "女",
                        Guid.NewGuid().ToString(),
                        "F134000" + i,
                        request.IntentOrderType
                        );
                        list.Add(customer);
                    }
                    dic.Add(key, list);
                }

                resMsg.Data = dic[key];
            }
            catch (Exception ex)
            {
                resMsg.StatusCode = (int)StatusCodeEnum.Error;
                resMsg.Info = ex.Message;
            }

            return Json(resMsg);
        }

        /// <summary>
        /// 附件上传
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadFile")]
        public ActionResult UploadFile(string token)
        {
            var resMsg = new ResultMsg();
            try
            {
                var httpRequest = HttpContext.Request;

                resMsg.StatusCode = (int)StatusCodeEnum.Success;
                resMsg.Info = "操作成功！";
            }
            catch (Exception ex)
            {
                resMsg.StatusCode = (int)StatusCodeEnum.Error;
                resMsg.Info = ex.Message;
            }
            resMsg.Data = "";
            return Json(resMsg);
        }

        [HttpPost]
        [Route("FileSave")]

        // [RequestSizeLimit(100_000_000)] //最大100m左右
        [DisableRequestSizeLimit]  //或者取消大小的限制
        public async Task<IActionResult> FileSave(string token)
        {
            var resMsg = new ResultMsg();
            var code = "200";
            try
            {
                var date = Request;
                var files = Request.Form.Files;
                long size = files.Sum(f => f.Length);
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var fileName = Encoding.UTF8.GetString(Convert.FromBase64String(formFile.FileName));
                        // code = fileName.Contains("002") ? "500" : "200";
                        string fileExt = Path.GetExtension(fileName);
                        long fileSize = formFile.Length; //获得文件大小，以字节为单位
                        string newFileName = System.Guid.NewGuid().ToString() + "." + fileExt; //随机生成新的文件名
                        var filePath = "C:\\fileAnnexes\\" + newFileName;
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                            resMsg.StatusCode = (int)StatusCodeEnum.Success;
                            resMsg.Data = System.Guid.NewGuid();
                        }
                    }
                }



                #region json
                var json = @"{
	'StatusCode': '{code}',
	'Data': [{
		'FileId': null,
		'FolderId': '0833c8fb-8da9-4e2d-88ce-8fdbc9a0de55',
		'FileName': '第一次打开时间.xls',
		'FilePath': 'c:\\attachment\\订单下单图纸\\2019\\1\\2019-01-03\\0833c8fb-8da9-4e2d-88ce-8fdbc9a0de55\\第一次打开时间.xls',
		'FileSize': '65536',
		'FileExtensions': '.xls',
		'FileType': 'xls'
	}],
	'Info': '请求(或处理)成功'
}";
                json = json.Replace("{code}", code);


                resMsg = json.ToObject<ResultMsg>();

                #endregion
            }
            catch (Exception ex)
            {
                resMsg.StatusCode = (int)StatusCodeEnum.Error;
                resMsg.Info = ex.Message;
            }

            return Json(resMsg);
        }

        [HttpPost]
        [Route("FileSave2")]
        public string FileSave2()
        {
            if (Request.Form.Files.Count > 0)
            {
                try
                {
                    //得到客户端上传的文件
                    var file = Request.Form.Files[0];
                    //服务器端要保存的路径
                    string newFileName = System.Guid.NewGuid().ToString() + "." + file.FileName;
                    var filePath = "C:\\fileAnnexes\\" + newFileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    //返回结果
                    return "Success";
                }
                catch
                {
                    return "Error";
                }
            }
            else
            {
                return "Error1";
            }
        }

        #region 断点

        [HttpGet]
        [Route("GetResumFile")]
        public long GetResumFile(string md5str)
        {
            //用于获取当前文件是否是续传。和续传的字节数开始点。
            var saveFilePath = string.Format("C:\\fileAnnexes\\Temp\\", md5str);
            if (System.IO.File.Exists(saveFilePath))
            {
                using (var fs = System.IO.File.OpenWrite(saveFilePath))
                {
                    return fs.Length;
                }
            }
            return 0;
        }

        [HttpPost]
        [Route("Rsume")]
        public ActionResult Rsume(string filename)
        {
            try
            {
                if (Request.Form.Files.Count == 1)
                {
                    var file = Request.Form.Files[0];

                    this.SaveAs("C:\\fileAnnexes\\" + filename, file);
                    return Json(new ResultMsg() { StatusCode = 200 });
                }
            }
            catch (System.Exception ex)
            {
                return Json(new ResultMsg() { StatusCode = 500, Info = ex.Message });
            }
            return Json(new ResultMsg() { StatusCode = 500, Info = "not file or parameter " });
        }

        private void SaveAs(string saveFilePath, IFormFile file)
        {
            using (Stream stream = new MemoryStream())
            {
                file.CopyTo(stream);

                long lStartPos = 0;
                int startPosition = 0;
                int endPosition = 0;
                var contentRange = Request.Headers["Content-Range"].ToString();
                //bytes 10000-19999/1157632
                if (!string.IsNullOrEmpty(contentRange))
                {
                    contentRange = contentRange.Replace("bytes", "").Trim();
                    contentRange = contentRange.Substring(0, contentRange.IndexOf("/"));
                    string[] ranges = contentRange.Split('-');
                    startPosition = int.Parse(ranges[0]);
                    endPosition = int.Parse(ranges[1]);
                }
                System.IO.FileStream fs;
                if (System.IO.File.Exists(saveFilePath))
                {
                    fs = System.IO.File.OpenWrite(saveFilePath);
                    lStartPos = fs.Length;

                }
                else
                {
                    fs = new System.IO.FileStream(saveFilePath, System.IO.FileMode.Create);
                    lStartPos = 0;
                }
                if (lStartPos > endPosition)
                {
                    fs.Close();
                    return;
                }
                else if (lStartPos < startPosition)
                {
                    lStartPos = startPosition;
                }
                else if (lStartPos > startPosition && lStartPos < endPosition)
                {
                    lStartPos = startPosition;
                }
                fs.Seek(lStartPos, System.IO.SeekOrigin.Current);
                byte[] nbytes = new byte[512];
                int nReadSize = 0;
                nReadSize = stream.Read(nbytes, 0, 512);
                while (nReadSize > 0)
                {
                    fs.Write(nbytes, 0, nReadSize);
                    nReadSize = stream.Read(nbytes, 0, 512);
                }
                fs.Close();
            }
        }

        #endregion


        #region 
        [HttpPost]
        [Route("UploadScale")]
        public string UploadScale(string name, long size_total, int file_index, int size)
        {

            string msg = "";
            string flag = "";
            try
            { //从Request中取参数，注意上传的文件在Requst.Files中
                var data = Request.Body;
                string dir = "C:\\fileAnnexes";
                string file_shard = Path.Combine(dir, name + "_" + file_index); //文件片路径及文件名
                string part = ""; //文件部分
                string file_name = Path.Combine(dir, name); //文件路径及文件名
                if (file_index == size_total)
                {
                    var fs = new FileStream(file_name, FileMode.Create);
                    for (int i = 1; i <= size_total; ++i)
                    {
                        part = Path.Combine(dir, name + "_" + i);
                        var bytes = System.IO.File.ReadAllBytes(part);
                        fs.Write(bytes, 0, bytes.Length);
                        bytes = null;
                        System.IO.File.Delete(part);
                    }
                    fs.Close();
                    flag = "ok";    //返回是否成功
                }
                else
                {
                    byte[] bytes = new byte[data.Length];
                    data.Read(bytes, 0, bytes.Length);
                    // 设置当前流的位置为流的开始   
                    data.Seek(0, SeekOrigin.Begin);

                    // 把 byte[] 写入文件   
                    FileStream fs = new FileStream(file_name, FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(bytes);
                    bw.Close();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                flag = "error";    //返回是否成功
            }
            msg = flag;
            return msg;
        }

        #endregion

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PushOrder")]
        public ActionResult PushOrder(string token)
        {
            var resMsg = new ResultMsg();
            try
            {
                var httpRequest = HttpContext.Request;

                resMsg.StatusCode = (int)StatusCodeEnum.Success;
                resMsg.Info = "操作成功！";
            }
            catch (Exception ex)
            {
                resMsg.StatusCode = (int)StatusCodeEnum.Error;
                resMsg.Info = ex.Message;
            }
            resMsg.Data = "";
            return Json(resMsg);
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        [HttpGet]
        [Route("GetServerVersion")]
        public ActionResult GetServerVersion()
        {
            var resMsg = new ResultMsg();
            try
            {
                resMsg.Data = "3.0.0.1";
                resMsg.StatusCode = (int)StatusCodeEnum.Success;
                resMsg.Info = "操作成功！";
            }
            catch (Exception ex)
            {
                resMsg.StatusCode = (int)StatusCodeEnum.Error;
                resMsg.Info = ex.Message;
            }
            return Json(resMsg);
        }

        /// <summary>AES加密</summary>
        /// <param name="text">明文</param>
        /// <param name="key">密钥,长度为16的字符串</param>
        /// <param name="iv">偏移量,长度为16的字符串</param>
        /// <returns>密文</returns>

        [HttpGet]
        [Route("EncodeAES")]
        public string EncodeAES(string text, string key, string iv)
        {
            text = "3b0eec76bcb33d416adf3597ce9ba25c&dfcaec3066a446b65fd63ea02f37fb81&YGZJAPP&00879676020&13623061080";
            key = "1547884092264";

            key = "1690a405da847e7f59cadfb5f3caa1f84cda33efc538dd579f2e67f5b73fae02";
            iv = "0000000000000000";


            Aes rijndaelCipher = Aes.Create();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(text);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

            var result = Convert.ToBase64String(cipherBytes);

            var equalss = result == "4hCxYM42W9NDxhmRqhZlQ3UtkmoKTMoFoy%2BwdsuVVbrpwzVAzWX4oOgG85BGX8pT4JCVe8s3vyeY3hYImDHga%2Bias5z2dp2E1H9QkSl9EHOckjNHw6W9A%2BW1%2BexNvZm7GkgdsNYsI6ulnxvXIl%2FoZg%3D%3D";

            return equalss + "";

        }


    }


}

public class OrderInfoRequest
{
    public string ShopId { get; set; }
    public string IntentOrderType { get; set; }
    public string Token { get; set; }
}