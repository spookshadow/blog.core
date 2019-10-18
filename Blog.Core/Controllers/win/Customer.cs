namespace Blog.Core.Controllers
{
    public class Customer
    {
        public static Customer Instance(string userName, string dg, int status, string statusText,
            string mobile, string address, string gender,
            string orderId, string orderCode, string orderType)
        {
            return new Customer()
            {
                UserName = userName,
                DG = dg,
                Status = status,
                StatusText = statusText,
                Mobile = mobile,
                Address = address,
                Gender = gender,
                OrderId = orderId,
                OrderCode = orderCode,
                IntentOrderType = orderType
            };
        }
        /// <summary>
        /// 客户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderCode { get; set; }
        /// <summary>
        /// 单类型
        /// </summary>
        public string IntentOrderType { get; set; }
        /// <summary>
        /// 导购名
        /// </summary>
        public string DG { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string StatusText { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 性别(0: 女， 1:男)
        /// </summary>
        public string Gender { get; set; }
    }
}
