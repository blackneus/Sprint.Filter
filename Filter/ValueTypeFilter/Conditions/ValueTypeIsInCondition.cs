﻿// ReSharper disable CheckNamespace
namespace Sprint.Filter.Conditions
// ReSharper restore CheckNamespace
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public sealed class ValueTypeIsInCondition<TProperty> : IValueTypeCondition<TProperty> where TProperty : struct, IComparable, IComparable<TProperty>, IEquatable<TProperty>
    {
        private readonly string _title;

        public ValueTypeIsInCondition() { }

        public ValueTypeIsInCondition(string title)
        {
            _title = title;
        }

        public IFilterValue<TProperty?> Value { get; set; }

        public string Title
        {
            get { return _title; }
        }

        public Expression<Func<TModel, bool>> For<TModel>(Expression<Func<TModel, TProperty?>> property)
        {
            if (Value != null && Value.Values != null && Value.Values.Any(x=>x.HasValue))
            {
                var values = Value.Values.Where(x => x.HasValue).Select(x => x).ToList();

                var collectionType = values.GetType();

                var parametr = property.Parameters[0];

                var propertyBody = property.Body;

                var method = collectionType.GetMethod("Contains", new[] { property.ReturnType });

                var value = Expression.Constant(values, collectionType);

                var containsMethodExp = Expression.Call(value, method, new[] { propertyBody });

                return Expression.Lambda<Func<TModel, bool>>(containsMethodExp, parametr);
            }

            return null;
        }

        public Expression<Func<TModel, bool>> For<TModel>(Expression<Func<TModel, TProperty?>> left, Expression<Func<TModel, TProperty?>> right)
        {
            return For(left);
        }
    }
}
