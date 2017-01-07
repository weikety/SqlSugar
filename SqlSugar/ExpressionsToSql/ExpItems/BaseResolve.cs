﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class BaseResolve
    {
        protected Expression Expression { get; set; }
        public ExpressionContext Context { get; set; }
        public string SqlWhere { get; set; }
        public bool IsFinished { get; set; }
        public bool? IsLeft { get; set; }
        public int ContentIndex { get { return this.Context.Index; } }
        public int Index { get; set; }
        private BaseResolve()
        {

        }
        public BaseResolve(ExpressionParameter parameter)
        {
            this.Expression = parameter.Expression;
            this.Context = parameter.Context;
        }

        public BaseResolve Start()
        {
            this.Index++;
            this.IsFinished = false;
            Expression exp = this.Expression;
            ExpressionParameter parameter = new ExpressionParameter()
            {
                Context = this.Context,
                Expression = exp,
                IsLeft = this.IsLeft,
                BaseExpression=this.Expression
            };
            if (exp is LambdaExpression)
            {
                return new LambdaExpressionResolve(parameter);
            }
            else if (exp is BinaryExpression)
            {
                return new BinaryExpressionResolve(parameter);
            }
            else if (exp is BlockExpression)
            {
                Check.ThrowNotSupportedException("BlockExpression");
            }
            else if (exp is ConditionalExpression)
            {
                Check.ThrowNotSupportedException("ConditionalExpression");
            }
            else if (exp is MethodCallExpression)
            {
                return new MethodCallExpressionResolve(parameter);
            }
            else if (exp is ConstantExpression)
            {
                return new ConstantExpressionResolve(parameter);
            }
            else if (exp is MemberExpression)
            {
                return new MemberExpressionResolve(parameter);
            }
            else if (exp is UnaryExpression)
            {
                return new UnaryExpressionResolve(parameter);
            }
            else if (exp != null && exp.NodeType.IsIn(ExpressionType.New, ExpressionType.NewArrayBounds, ExpressionType.NewArrayInit))
            {
                Check.ThrowNotSupportedException("ExpressionType.New、ExpressionType.NewArrayBounds and ExpressionType.NewArrayInit");
            }
            return null;
        }
        public void Continue()
        {
            if (!IsFinished)
            {
                this.Start();
            }
        }
    }
}