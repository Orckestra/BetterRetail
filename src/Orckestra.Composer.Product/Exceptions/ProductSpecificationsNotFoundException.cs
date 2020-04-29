using System;

namespace Orckestra.Composer.Product.Exceptions
{
	public sealed class ProductSpecificationsNotFoundException : Exception
	{
	    public ProductSpecificationsNotFoundException(string message) : base(message)
        {
            if (message == null) { throw new ArgumentNullException(nameof(message)); }
	    }

	    public ProductSpecificationsNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
            if (message == null) { throw new ArgumentNullException(nameof(message)); }
            if (innerException == null) { throw new ArgumentNullException(nameof(innerException)); }
	    }
	}
}