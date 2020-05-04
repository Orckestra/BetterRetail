using System;
using System.Linq;
using System.Linq.Expressions;
using Composite.Core.Types;
using Composite.Data;
using Composite.Data.Caching;
using Composite.Data.Foundation;

namespace Orckestra.Composer.CompositeC1.Cache
{
	public class QueryCache<TDataType, TPropertyType> where TDataType : class, IData
	{
		private readonly Cache<string, ExtendedNullable<TDataType>> _innerCache;
		private readonly Expression<Func<TDataType, TPropertyType>> _propertyGetter;
		private Func<TDataType, TPropertyType> _compiledExpression;
		private readonly bool _typeIsLocalizable;
		private readonly object _locker = new object();

		public QueryCache(Expression<Func<TDataType, TPropertyType>> propertyGetter) :
			this("Unnamed cache", propertyGetter, 1000) { }

		public QueryCache(string name, Expression<Func<TDataType, TPropertyType>> propertyGetter, int size)
		{
			_innerCache = new Cache<string, ExtendedNullable<TDataType>>(name, size);
			_propertyGetter = propertyGetter;

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

		public TDataType Get(TPropertyType key, bool forReadOnlyUsage)
		{
			TDataType result;

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
						result = DataFacade.GetData<TDataType>(false).Where(newLabda).FirstOrDefault();

						_innerCache.Add(cacheKey, new ExtendedNullable<TDataType> { Value = result });
					}
				}
			}

			return result == null || forReadOnlyUsage ? result : DataWrappingFacade.Wrap(result);
		}

		public TDataType this[TPropertyType key]
		{
			get { return Get(key, false); }
		}

		private TPropertyType GetKey(TDataType dataItem)
		{
			if (_compiledExpression == null) { _compiledExpression = _propertyGetter.Compile(); }

			return _compiledExpression.Invoke(dataItem);
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