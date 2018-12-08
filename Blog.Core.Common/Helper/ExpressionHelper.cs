using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Linq.Expressions;

namespace Blog.Core.Common.Helper
{
    /// <summary>
    /// TODO:// Expression 扩展
    /// 1. mono.linq.expressions, 问题：若传入参数未能获取该参数
    /// Expression<Func<Advertisement, bool>> exp = a => a.Id > 18 && a.ImgUrl.Contains("https");
    /// Expression<Func<Advertisement, bool>> exp2 = a => a.Id > id && a.ImgUrl.Contains("https");
    /// var s1 = exp.ToString(); 
    /// var s2 = exp2.ToString();
    /// 2. Extensions.cs 问题：看不懂代码
    /// 参考网页在 refer/Expression 谷歌收藏夹 core/express
    /// 

    /// </summary>
    public class ExpressionHelper
    {
        public static string ParsingToString<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            StringBuilder sb = new StringBuilder();
            expression.Evaluate(sb, 5);
            // now fix the capture class names (from a and b)
            string s = sb.ToString();
            s = Regex.Replace(s, @"value\([^)]+\)\.", "");
            return Regex.Replace(s, @"value\([^)]+\)\.", "");


            /*var parameters = new List<String>();
            foreach (var parameter in expression.Parameters)
            {
                BinaryExpression operation = (BinaryExpression)expression.Body;
                ParameterExpression left = (ParameterExpression)operation.Left;
                ConstantExpression right = (ConstantExpression)operation.Right;

                parameters.Add(String.Format("{0} => {1} {2} {3}", parameter.Name, left.Name, operation.NodeType, right.Value));
            }
            return String.Join(",", parameters.ToArray());
            */

        }
    }
}