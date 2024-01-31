using System.Collections.Generic;
using System.Threading.Tasks;
using P1S.Common.Interfaces;
using P1S.Common.Interfaces.RestClient;
using P1S.Common.Json;
using P1S.Common.Models.RestClient;
using P1S.Common.Services.RestClient;
using P1S.Common.Validation;
using P1S.Identifi.App.Service.Client;
using P1S.Identifi.App.Service.Client.Utils.Interfaces;
using P1S.Identifi.App.Service.Data.Model.Constants;
using P1S.Identifi.App.Service.Data.Model.Requests.Authentication;
using P1S.Identifi.App.Service.Data.Model.Views;

namespace OpenTelemetryTest
{
    public class IdentifiClient2
    {
        private readonly ISecurityManager _securityManager;
        private readonly RestClient _restClientForOrganizationCreation;
        private readonly RestClient _restClient;

        public IdentifiClient2(
            string serviceUrl,
            ISecurityManager securityManager,
            ILoggerService  loggerService)
        {
            Validate.StringAnyLength(serviceUrl, nameof(serviceUrl));
            Validate.NotNull<ISecurityManager>(securityManager, nameof(securityManager));
            this._securityManager = securityManager;
            this._restClient = new P1S.Common.Services.RestClient.RestClient((IWebRequestService) new WebRequestService(serviceUrl),
                (IStringSerializer) new JsonSerializer(), securityManager.GetHeaderProvider(), loggerService,
                (IRestClientExceptionFactory) new IdentifiServiceExceptionFactory());
     
        }

        public async Task<VirtualSessionView> ValidateTokenAsync(
            string sessionToken,
            string ipAddress,
            VirtualProducts? product = null,
            bool suppressSessionExtension = false,
            string  requestId = null)
        {
            if (string.IsNullOrWhiteSpace(sessionToken) || string.IsNullOrWhiteSpace(ipAddress))
                return (VirtualSessionView) null;
            ValidateTokenRequest payload = new ValidateTokenRequest()
            {
                Token = sessionToken,
                IpAddress = ipAddress,
                VirtualProductId = product
            };
            Dictionary<string, string> forValidateToken = new Dictionary<string, string> {{"sessionToken", sessionToken}};
            return await this._restClient.PostAndGetResponseNoExceptionAsync<ValidateTokenRequest, VirtualSessionView>("sso/auth/validate", payload,
                new RestCallOptions()
                {
                    NoLogOnSuccess = true,
                    Headers = forValidateToken
                });

        }
    }
}