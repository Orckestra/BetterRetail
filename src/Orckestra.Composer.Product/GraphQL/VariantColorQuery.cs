using Composite.Data;
using GraphQL;
using GraphQL.Types;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.Enums;
using Orckestra.Composer.GraphQL.Extensions;
using Orckestra.Composer.Product.GraphQL.Types;
using Orckestra.Composer.Services.Lookup;
using System;
using System.IO;
using System.Linq;

namespace Orckestra.Composer.Product.GraphQL
{
    internal class VariantColorQuery : ObjectGraphType<object>
    {

        public ILookupService LookupService { get; set; }

        public VariantColorQuery()
        {
            Name = "VariantColorQuery";



            this.FieldList<IVariantColorConfiguration>();


            Field<ListGraphType<StringGraphType>>("colorImages",
               resolve: (context) =>
               {
                   var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UI.Package/Images/VariantColors");
                   var files = Directory.GetFiles(path);
                   var result = from f in files
                                select Path.GetFileName(f);
                   return result;

               });

            Field<ListGraphType<VariantColorLookupValuesGraphType>>("colorValues",
                resolve: (context) =>
                {
                    using (var connection = new DataConnection(PublicationScope.Unpublished))
                    {
                        var varConfig = connection.Get<IVariantColorConfiguration>();


                        var lookupServise = context.RequestServices.GetService(typeof(ILookupService)) as LookupService;
                        var lookup = lookupServise.GetLookupAsync(LookupType.Product, "Colour").Result;

                        var QSOuterJoin = from emp in lookup.Values
                                          join add in varConfig
                                          on emp.Value equals add.LookupValue
                                          into EmployeeAddressGroup
                                          from address in EmployeeAddressGroup.DefaultIfEmpty()
                                          select new VariantColorLookupValues
                                          {
                                              LookupValue = emp.Value,
                                              Color = address?.Color,
                                              Image = address?.Image
                                          };

                        return QSOuterJoin;
                    }
                });

        }
    }
}