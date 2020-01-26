using System;
using System.Net;
using NUnit.Framework;
using RestSharp;
using SalesforceSharp.Security;
using TestSharp;

namespace SalesforceSharp.UnitTests.Security
{
    using Moq;

    [TestFixture]
    public class UsernamePasswordAuthenticationFlowTest
    {
        [Test]
        public void Constructor_NoRestClient_DefaultValues()
        {
            var target = new UsernamePasswordAuthenticationFlow("clientId", "clientSecret", "username", "password");
            Assert.IsNotNull(target);
        }

        [Test]
        public void Constructor_InvalidArgs_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentNullException("restClient"), () =>
            {
                new UsernamePasswordAuthenticationFlow((RestClient) null, "clientId", "clientSecret", "username", "password");
            });

            ExceptionAssert.IsThrowing(new ArgumentException("Argument 'clientId' can't be empty.", "clientId"), () =>
            {
                new UsernamePasswordAuthenticationFlow(new RestClient(), "", "clientSecret", "username", "password");
            });

            ExceptionAssert.IsThrowing(new ArgumentException("Argument 'clientSecret' can't be empty.", "clientSecret"), () =>
            {
                new UsernamePasswordAuthenticationFlow(new RestClient(), "clientId", "", "username", "password");
            });

            ExceptionAssert.IsThrowing(new ArgumentException("Argument 'username' can't be empty.", "username"), () =>
            {
                new UsernamePasswordAuthenticationFlow(new RestClient(), "clientId", "clientSecret", "", "password");
            });

            ExceptionAssert.IsThrowing(new ArgumentException("Argument 'password' can't be empty.", "password"), () =>
            {
                new UsernamePasswordAuthenticationFlow(new RestClient(), "clientId", "clientSecret", "username", "");
            });
        }

        [Test]
        public void Authenticate_Failed_Exception()
        {
            var response = new Mock<IRestResponse>();
            response.Setup(r => r.Content).Returns("{ error: 'authentication failure', error_description: 'authentication failed' }");
            response.Setup(r => r.StatusCode).Returns(HttpStatusCode.BadRequest);

            var restClient = new Mock<IRestClient>();
            restClient.Setup(r => r.BaseUrl).Returns(new Uri("http://tokenUrl"));
            restClient.Setup(r => r.Execute(It.IsAny<IRestRequest>(), Method.POST)).Returns(response.Object);

            var target = new UsernamePasswordAuthenticationFlow(restClient.Object, "clientId", "clientSecret", "userName", "password");
            target.TokenRequestEndpointUrl = "http://tokenUrl";

            ExceptionAssert.IsThrowing(new SalesforceException(SalesforceError.AuthenticationFailure, "authentication failed"), () =>
            {
                target.Authenticate();
            });
        }

        [Test]
        public void Authenticate_Success_AuthenticationInfo()
        {
            var response = new Mock<IRestResponse>();
            response.Setup(r => r.Content).Returns("{ access_token: 'access token 1', instance_url: 'instance url 2' }");
            response.Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);

            var restClient = new Mock<IRestClient>();
            restClient.Setup(r => r.BaseUrl).Returns(new Uri("https://login.salesforce.com/services/oauth2/token"));
            restClient.Setup(r => r.Execute(It.IsAny<IRestRequest>(), Method.POST)).Returns(response.Object);

            var target = new UsernamePasswordAuthenticationFlow(restClient.Object, "clientId", "clientSecret", "userName", "password");
            var actual = target.Authenticate();
            Assert.AreEqual("access token 1", actual.AccessToken);
            Assert.AreEqual("instance url 2", actual.InstanceUrl);
        }
    }
}