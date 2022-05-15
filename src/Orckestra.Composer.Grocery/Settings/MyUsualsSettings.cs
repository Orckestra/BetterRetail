using Composite.Data;
using Orckestra.Composer.Services;
using System;
using System.Linq;

namespace Orckestra.Composer.Grocery.Settings
{
    public class MyUsualsSettings : IMyUsualsSettings
    {
        public IWebsiteContext WebsiteContext;
        private DataTypes.IMyUsualsSettingsMeta MyUsualsSettingsMeta;

        public MyUsualsSettings(IWebsiteContext websiteContext)
        {
            WebsiteContext = websiteContext;
            using (var con = new DataConnection())
            {
                MyUsualsSettingsMeta = con.Get<DataTypes.IMyUsualsSettingsMeta>().FirstOrDefault(g => g.PageId == WebsiteContext.WebsiteId);
            }
        }

        public int Frequency
        {
            get
            {
                return Convert.ToInt32(MyUsualsSettingsMeta.Frequency);

            }
        }

        public int TimeFrame
        {
            get
            {
                return Convert.ToInt32(MyUsualsSettingsMeta.TimeFrame);

            }
        }

        public Guid MyUsualsPageId
        {
            get
            {
                if (MyUsualsSettingsMeta != null && MyUsualsSettingsMeta.MyUsualsPage.HasValue)
                {
                    return MyUsualsSettingsMeta.MyUsualsPage.Value;
                }

                return Guid.Empty;
            }
        }
    }
}
