using System;
using System.Collections.Specialized;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    public class UrlFormatterAppendQueryString
    {
        [TestCase(null, "test=test")]
        [TestCase("", "a=a")]
        [TestCase("http://google.ca", null)]
        public void WHEN_parameter_is_invalid_SHOULD_throw_ArgumentException(string url, string query)
        {
            //Arrange
            var qs = PopulateQueryString(query);

            //Act
            var action = new Action(() => UrlFormatter.AppendQueryString(url, qs));

            //Assert
            action.ShouldThrow<ArgumentException>();
        }

        [TestCase("http://google.ca", "a=a;b=b", "http://google.ca/?a=a&b=b")]
        [TestCase("http://google.ca/", "a=a;b=b", "http://google.ca/?a=a&b=b")]
        [TestCase("/About", "store=1;userid=25", "/About?store=1&userid=25")]
        [TestCase("/About?store=1", "store=2;userid=25", "/About?store=2&userid=25")]
        [TestCase("/About/?store=1", "store=2;userid=25", "/About/?store=2&userid=25")]
        public void WHEN_Given_Scenario_SHOULD_Return_Expected_Result(string url, string query, string expectedUrl)
        {
            //Arrange
            var qs = PopulateQueryString(query);

            //Act
            var finalUrl = UrlFormatter.AppendQueryString(url, qs);

            //Assert
            finalUrl.Should().BeEquivalentTo(expectedUrl);
        }

        private NameValueCollection PopulateQueryString(string query)
        {
            if (query == null) { return null; }

            var q = query.Split('=', ';');
            var queryStrings = new NameValueCollection();

            for (int i = 0; i < q.Length; i += 2)
            {
                var key = q[i];
                var value = q[i + 1];

                queryStrings.Set(key, value);
            }

            return queryStrings;
        }
    }
}
