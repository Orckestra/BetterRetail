using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Requests.Customers;
using System.Linq;
using Orckestra.Overture.ServiceModel.Requests;

namespace Orckestra.Composer.Extensions
{
    public static class CustomerExtensions
    {
        public static UpdateCustomerRequest ExtendUpdateCustomerRequest(this UpdateCustomerRequest request, Customer customer)
        {
            request.AccountStatus = customer.AccountStatus;
            if (customer.AddressIds != null)
            {
                request.AddressIds = customer.AddressIds.ToList();
            }
            request.Created = customer.Created;
            request.LastModified = customer.LastModified;
            request.Email = customer.Email;
            request.FirstName = customer.FirstName;
            request.LastName = customer.LastName;
            request.PasswordQuestion = customer.PasswordQuestion;
            request.PhoneExtension = customer.PhoneExtension;
            request.PhoneNumber = customer.PhoneNumber;
            request.CustomerType = customer.CustomerType;
            request.Username = customer.Username;
            request.CellNumber = customer.CellNumber;
            request.PhoneNumberWork = customer.PhoneNumberWork;
            request.PhoneExtensionWork = customer.PhoneExtensionWork;
            request.FaxNumber = customer.FaxNumber;
            request.FaxExtension = customer.FaxExtension;
            request.Language = customer.Language;
            request.CustomerId = customer.Id;
            if (customer.PropertyBag != null)
            {
                request.PropertyBag = new PropertyBag(customer.PropertyBag);
            }

            return request;
        } 
    }
}
