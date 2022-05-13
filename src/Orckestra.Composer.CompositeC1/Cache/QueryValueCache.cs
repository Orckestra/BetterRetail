﻿using System;
using System.Linq;
using System.Linq.Expressions;
using Composite.Core.Types;
using Composite.Data;
using Composite.Data.Caching;

namespace Orckestra.Composer.CompositeC1.Cache
{
	public class QueryCacheValue<TDataType, TPropertyType, TValueType> where TDataType : class, IData
	{
		private readonly Cache<string, ExtendedNullable<TValueType>> _innerCache;
		private readonly Expression<Func<TDataType, TPropertyType>> _propertyGetter;
		private readonly Expression<Func<TDataType, TValueType>> _valueGetter;
		private Func<TDataType, TPropertyType> _compiledPropertyExpression;
		private Func<TDataType, TValueType> _compiledValueExpression;
		private readonly bool _typeIsLocalizable;
		private readonly object _locker = new object();

		public QueryCacheValue(Expression<Func<TDataType, TPropertyType>> propertyGetter, Expression<Func<TDataType, TValueType>> valueGetter) :
			this("QueryCacheValue_" + typeof(TDataType).Name, propertyGetter, valueGetter, 10000) { }

		public QueryCacheValue(string name, Expression<Func<TDataType, TPropertyType>> propertyGetter, Expression<Func<TDataType, TValueType>> valueGetter, int size)
		{
			_innerCache = new Cache<string, ExtendedNullable<TValueType>>(name, size);
			_propertyGetter = propertyGetter;
			_valueGetter = valueGetter;

			DataEventSystemFacade.SubscribeToDataAfterAdd<TDataType>(OnDataChanged, true);
			DataEventSystemFacade.SubscribeToDataAfterUpdate<TDataType>(OnDataChanged, true);
			DataEventSystemFacade.SubscribeToDataDeleted<TDataType>(OnDataChanged, true);

			_typeIsLocalizable = DataLocalizationFacade.IsLocalized(typeof(TDataType));
		}

		private void OnDataChanged(object sender, DataEventArgs dataeventargs)
		{
			if (!(dataeventargs.Data is TDataType data)) { return; }

			string cacheKey = GetCacheKey(GetKey(data));
			lock (_locker)
			{
				_innerCache.Remove(cacheKey);
			}
		}

		public TValueType Get(TPropertyType key)
		{
			TValueType result;

			string cacheKey = GetCacheKey(key);

			var cacheRecord = _innerCache.Get(cacheKey);
			if (cacheRecord != null)
			{
				result = cacheRecord.Value;
			}
			else
			{
				lock (_locker)
				{
					cacheRecord = _innerCache.Get(cacheKey);
					if (cacheRecord != null)
					{
						result = cacheRecord.Value;
					}
					else
					{
						ParameterExpression parameter = _propertyGetter.Parameters[0];
						var newBody = Expression.Equal(_propertyGetter.Body, Expression.Constant(key, typeof(TPropertyType)));
						var newLabda = Expression.Lambda(newBody, new[] { parameter }) as Expression<Func<TDataType, bool>>;
						result = GetValue(DataFacade.GetData<TDataType>(false).Where(newLabda).FirstOrDefault());

						_innerCache.Add(cacheKey, new ExtendedNullable<TValueType> { Value = result });
					}
				}
			}

			return result;
		}

		public TValueType this[TPropertyType key]
		{
			get { return Get(key); }
		}

		private TPropertyType GetKey(TDataType dataItem)
		{
			if (_compiledPropertyExpression == null)
			{
				_compiledPropertyExpression = _propertyGetter.Compile();
			}

			return _compiledPropertyExpression.Invoke(dataItem);
		}

		private TValueType GetValue(TDataType dataItem)
		{
			if (dataItem == default(TDataType)) return default;

			if (_compiledValueExpression == null)
			{
				_compiledValueExpression = _valueGetter.Compile();
			}

			return _compiledValueExpression.Invoke(dataItem);
		}


		private string GetCacheKey(TPropertyType key)
		{
			string dataScope = DataScopeManager.MapByType(typeof(TDataType)).ToString();
			string result = key + dataScope;
			if (_typeIsLocalizable)
			{
				result += LocalizationScopeManager.MapByType(typeof(TDataType)).ToString();
			}
			return result;
		}
	}
}