using Composite.Core.Logging;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.DataTypes.Facets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.DataTypes
{
    static class DataTypesEventRegistrator
    {
        static DataTypesEventRegistrator()
        {
            DataEvents<IFacetConfiguration>.OnBeforeAdd += new DataEventHandler(ProcessFacetConfigurationData);
            DataEvents<IFacetConfiguration>.OnBeforeUpdate += new DataEventHandler(ProcessFacetConfigurationData);
        }

        public static void Initialize()
        {
        }

        private static void ProcessFacetConfigurationData(object sender, DataEventArgs dataEventArgs)
        {
            var config = (IFacetConfiguration)dataEventArgs.Data;

            if(config.IsDefault)
            {
                using (var connection = new DataConnection())
                {
                    var configurations = connection.Get<IFacetConfiguration>().Where(c => c.IsDefault).ToList();
                    foreach (var c in configurations)
                    {
                        c.IsDefault = false;
                    }
                    connection.Update(configurations);
                }
            }
        }
    }
}
