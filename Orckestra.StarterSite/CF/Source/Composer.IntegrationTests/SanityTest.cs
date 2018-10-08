using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Orckestra.Composer.Tests.IntegrationTests
{
    //[TestFixture(Category = "Sanity")]
    public class SanityTest
    {
        private TimeSpan Duration = TimeSpan.FromMinutes(10);
        //private const string PageUrl = "https://composer-mvc-dev.orckestra.local/fr-CA/rechercher?keywords=tiger";
        private const string PageUrl = "https://composer-sitecore-cm-dev.orckestra.local/en-CA/search?keywords=*&sortDirection=asc&page=5";

        //[Test]
        public void ShouldAwlaysPass()
        {
           Assert.That(int.MaxValue, Is.GreaterThan(0));
        }

       // [Test]
        public async Task CoolTest()
        {
            var numberOfTask = 20;
            var tasks = new Task[numberOfTask];
            for (int i = 0; i < numberOfTask; i++)
            {
                tasks[i] = CreateRequest();
            }
            await Task.WhenAll(tasks);
        }

        private Task CreateRequest()
        {

            return Task.Run(() =>
            {
                var watch = Stopwatch.StartNew();
                
                   while (watch.Elapsed < Duration)
                   {
                       using (WebClient client = new WebClient())
                       {
                           var r = client.DownloadString(PageUrl);
                           //"https://composer-sitecore-cm-dev.orckestra.local/en-ca/search?keywords=tiger");
                       }
                   }
            });


        }
    }
}
