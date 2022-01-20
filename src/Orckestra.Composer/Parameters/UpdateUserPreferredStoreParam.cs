using System;

namespace Orckestra.Composer.Parameters
{
	/// <summary>
	/// Parameters container to update preferred store of a customer
	/// </summary>
	public class UpdateUserPreferredStoreParam
	{
		/// <summary>
		/// Guid of a customer
		/// </summary>
		public Guid CustomerId { get; set; }
		/// <summary>
		/// Identifier of a scope
		/// </summary>
		public string ScopeId { get; set; }
		/// <summary>
		/// Guid of a preferred store
		/// </summary>
		public Guid StoreId { get; set; }
		/// <summary>
		/// Number of a preferred store
		/// </summary>
		public string StoreNumber { get; set; }

        public UpdateUserPreferredStoreParam Clone()
        {
            var param = (UpdateUserPreferredStoreParam)MemberwiseClone();
            return param;
        }
    }
}
