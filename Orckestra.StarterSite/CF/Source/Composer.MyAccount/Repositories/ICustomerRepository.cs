using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.MyAccount.Repositories
{
    /// <summary>
    /// Repository for interacting with Customers
    /// Customers represents entities which have the ability to buy products. 
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Gets a single Customer based on it's unique identifier
        /// </summary>
        /// <param name="getCustomerByIdParam">The Repository call params <see cref="GetCustomerByIdParam"/></param>
        /// <returns>
        /// The Customer matching the requested ID, or null
        /// </returns>
        Task<Customer> GetCustomerByIdAsync(GetCustomerByIdParam getCustomerByIdParam);

        /// <summary>
        /// Gets the single Customer identified by a password reset ticket
        /// </summary>
        /// <param name="ticket">An encrypted password reset ticket</param>
        /// <returns>
        /// The Customer matching the ticket.
        /// </returns>
        Task<Customer> GetCustomerByTicketAsync(string ticket);

        /// <summary>
        /// Create a new Customer
        /// </summary>
        /// <param name="param">The Repository call params <see cref="CreateUserParam"/></param>
        /// <returns>
        /// The created Customer
        /// </returns>
        Task<Customer> CreateUserAsync(CreateUserParam param);

        /// <summary>
        /// Sends instructions to the given email address on how to reset it's Customer's password
        /// </summary>
        /// <param name="param"></param>
        Task SendResetPasswordInstructionsAsync(SendResetPasswordInstructionsParam param);

        /// <summary>
        /// Resets the password for a customer
        /// This will change the password without actually needing the current password at all.
        /// and is part of the ForgotPassword process
        /// </summary>
        /// <param name="username">The unique login Name of the customer to reset</param>
        /// <param name="newPassword">The new password to set</param>
        /// <param name="passwordAnswer">The answer to the password question</param>
        Task ResetPasswordAsync(string username, string newPassword, string passwordAnswer);

        /// <summary>
        /// Sets the new password for a customer.
        /// </summary>
        /// <param name="username">The unique login Name of the customer to update</param>
        /// <param name="oldPassword">The current login password of the customer to update</param>
        /// <param name="newPassword">The new password to set</param>
        Task ChangePasswordAsync(string username, string oldPassword, string newPassword);

        /// <summary>
        /// Update User infos
        /// </summary>
        /// <param name="param">The Repository call params <see cref="UpdateUserParam"/></param>
        /// <returns>
        /// The updated Customer.
        /// </returns>
        Task<Customer> UpdateUserAsync(UpdateUserParam param);

        /// <summary>
        /// Get the Payment methods available for a customer.
        /// </summary>
        /// <param name="param">GetPaymentMethodsParam</param>
        /// <returns>A List of PaymentMethod</returns>
        Task<List<PaymentMethod>> GetCustomerPaymentMethodsAsync(GetCustomerPaymentMethodsParam param);
    }
}
