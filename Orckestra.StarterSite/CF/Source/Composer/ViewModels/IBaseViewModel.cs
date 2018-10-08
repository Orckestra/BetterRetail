using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace Orckestra.Composer.ViewModels
{
    public interface IBaseViewModel
    {
        /// <summary>
        /// Bag holding properties that will be added dynamically to the view model.
        /// </summary>
        Dictionary<string, object> Bag { get; }

        /// <summary>
        /// Bag holding properties that will be added dynamically to the Json Context
        /// </summary>
        Dictionary<string, object> Context { get; }

        /// <summary>
        /// Json representation of the Context.
        /// </summary>
//[MetadataIgnore]
        string JsonContext { get; }

        /// <summary>
        /// Sets the IViewModelMetadataRegistry to be used by the BaseViewModel.
        /// </summary>
        /// <param name="viewModelMetadataRegistry"></param>
        void SetViewModelMetadataRegistry(IViewModelMetadataRegistry viewModelMetadataRegistry);

        ///// <summary>
        ///// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:Syste m.Dynamic.DynamicObject"/> 
        ///// class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        ///// </summary>
        ///// <returns>
        ///// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines 
        ///// the behavior. (In most cases, a run-time exception is thrown.)
        ///// </returns>
        ///// <param name="binder">
        ///// Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which 
        ///// the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an 
        ///// instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase 
        ///// property specifies whether the member name is case-sensitive.</param><param name="result">The result of the get operation. For example, if the method 
        ///// is called for a property, you can assign the property value to <paramref name="result"/>.
        ///// </param>
        //bool TryGetMember(GetMemberBinder binder, out object result);

        ///// <summary>
        ///// Provides the implementation for operations that set member values. 
        ///// Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic 
        ///// behavior for operations such as setting a value for a property.
        ///// </summary>
        ///// <returns>
        ///// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder 
        ///// of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        ///// </returns>
        ///// <param name="binder">
        ///// Provides information about the object that called the dynamic operation. The binder.Name property provides 
        ///// the name of the member to which the value is being assigned. For example, for the statement 
        ///// sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the 
        ///// <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase 
        ///// property specifies whether the member name is case-sensitive.</param><param name="value">The value to set to the member. 
        ///// For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the 
        ///// <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".
        ///// </param>
        //bool TrySetMember(SetMemberBinder binder, object value);

        ///// <summary>
        ///// Returns the enumeration of all dynamic member names. 
        ///// </summary>
        ///// <returns>
        ///// A sequence that contains dynamic member names.
        ///// </returns>
        //IEnumerable<string> GetDynamicMemberNames();

        /// <summary>
        /// Produces a dictionary of key-value pairs based on the metadata of the <see cref="BaseViewModel"/>.
        /// </summary>
        /// <returns>Dictionnary of key-value pairs based on the metadata</returns>
        IDictionary<string, object> ToDictionary();

        //bool TryDeleteMember(DeleteMemberBinder binder);
        //bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result);
        //bool TryConvert(ConvertBinder binder, out object result);
        //bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result);
        //bool TryInvoke(InvokeBinder binder, object[] args, out object result);
        //bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result);
        //bool TryUnaryOperation(UnaryOperationBinder binder, out object result);
        //bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result);
        //bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value);
        //bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes);
        //DynamicMetaObject GetMetaObject(Expression parameter);
    }
}