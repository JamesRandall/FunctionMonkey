using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FunctionMonkey.Extensions
{
    internal static class ExpressionExtensions
    {
        /// <summary>
        /// Gets a MemberInfo from a member expression.
        /// </summary>
        public static MemberInfo GetMember<TCommand, TProperty>(this Expression<Func<TCommand, TProperty>> expression)
        {
            if (!(RemoveUnary(expression.Body) is MemberExpression memberExp))
            {
                return null;
            }

            Expression currentExpr = memberExp.Expression;

            // Unwind the expression to get the root object that the expression acts upon. 
            while (true)
            {
                currentExpr = RemoveUnary(currentExpr);

                if (currentExpr != null && currentExpr.NodeType == ExpressionType.MemberAccess)
                {
                    currentExpr = ((MemberExpression)currentExpr).Expression;
                }
                else
                {
                    break;
                }
            }

            if (currentExpr == null || currentExpr.NodeType != ExpressionType.Parameter)
            {
                return null; // We don't care if we're not acting upon the model instance. 
            }

            return memberExp.Member;
        }


        private static Expression RemoveUnary(Expression toUnwrap)
        {
            if (toUnwrap is UnaryExpression expression)
            {
                return expression.Operand;
            }

            return toUnwrap;
        }
    }
}
