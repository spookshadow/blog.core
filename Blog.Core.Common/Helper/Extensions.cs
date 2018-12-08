using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Blog.Core.Common.Helper
{
    public static class Extensions
    {
        private static StringBuilder Indent(this StringBuilder builder, int depth)
        {
            for (int i = 0; i < depth; i++) builder.Append("  ");
            return builder;
        }

        public static void Evaluate(this LambdaExpression expr, StringBuilder builder, params object[] args)
        {
            var parameters = expr.Parameters.ToArray();
            if (args == null || parameters.Length != args.Length) throw new ArgumentException("args");
            Evaluate(expr.Body, 0, builder, parameters, args);
        }
        private static void Evaluate(this Expression expr, int depth, StringBuilder builder, ParameterExpression[] parameters, object[] args)
        {
            builder.Indent(depth).Append(expr).Append(" = "); // .Append(Expression.Lambda(expr, parameters).Compile().DynamicInvoke(args));

            UnaryExpression ue;
            BinaryExpression be;
            ConditionalExpression ce;

            if ((ue = expr as UnaryExpression) != null)
            {
                builder.AppendLine(" where");
                Evaluate(ue.Operand, depth + 1, builder, parameters, args);
            }
            if ((be = expr as BinaryExpression) != null)
            {
                builder.AppendLine(" where");
                Evaluate(be.Left, depth + 1, builder, parameters, args);
                Evaluate(be.Right, depth + 1, builder, parameters, args);
            }
            else if ((ce = expr as ConditionalExpression) != null)
            {
                builder.AppendLine(" where");
                Evaluate(ce.Test, depth + 1, builder, parameters, args);
                Evaluate(ce.IfTrue, depth + 1, builder, parameters, args);
                Evaluate(ce.IfFalse, depth + 1, builder, parameters, args);
            }
            else
            {
                builder.AppendLine();
            }
        }
    }
}
