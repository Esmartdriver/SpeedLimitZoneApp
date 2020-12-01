using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace SpeedLimitZoneApp.App_Start
{
    public class CognitoUser : IdentityUser
    {
        public string Password { get; set; }
        //public UserStatusType Status { get; set; }
        public string IdToken { get; set; }
    }

    //public class CognitoUserStore : IUserStore<CognitoUser>
    ////,IUserLockoutStore<CognitoUser, string>,
    ////IUserTwoFactorStore<CognitoUser, string>
    //{
    //    private readonly AmazonCognitoIdentityProviderClient _client = new AmazonCognitoIdentityProviderClient();

    //    public Task CreateAsync(CognitoUser user)
    //    {
    //        // Register the user using Cognito
    //        var signUpRequest = new SignUpRequest
    //        {
    //            ClientId = ConfigurationManager.AppSettings["CLIENT_ID"],
    //            Password = user.Password,
    //            Username = user.Email,

    //        };

    //        var emailAttribute = new AttributeType
    //        {
    //            Name = "email",
    //            Value = user.Email
    //        };
    //        signUpRequest.UserAttributes.Add(emailAttribute);

    //        return _client.SignUpAsync(signUpRequest);
    //    }

    //    public Task DeleteAsync(CognitoUser user)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Dispose()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<CognitoUser> FindByIdAsync(string userId)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<CognitoUser> FindByNameAsync(string userName)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task UpdateAsync(CognitoUser user)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class CognitoUserManager : UserManager<CognitoUser>
    {
        public static CognitoUserManager CognitoManager = new CognitoUserManager(new UserStore<CognitoUser>());

        public CognitoUserManager(IUserStore<CognitoUser> store)
            : base(store)
        {
        }

        public override async Task<bool> CheckPasswordAsync(CognitoUser user, string password)
        {
            var IdToken = await CheckPasswordAsync(user.UserName, password);
            user.IdToken = IdToken;
            return !string.IsNullOrEmpty(IdToken);
        }

        private async Task<string> CheckPasswordAsync(string userName, string password)
        {
            try
            {
                string accessKey = WebConfigurationManager.AppSettings["AWSAccessKeyId"];
                string secretKey = WebConfigurationManager.AppSettings["AWSSecretAccessKey"];
                //string _aWSRegion = ConfigurationManager.AppSettings["AWSRegion"];

                AmazonCognitoIdentityProviderClient _client = new AmazonCognitoIdentityProviderClient(accessKey, secretKey, Amazon.RegionEndpoint.USEast1);
                var authReq = new AdminInitiateAuthRequest
                {
                    UserPoolId = WebConfigurationManager.AppSettings["USERPOOL_ID"],
                    ClientId = WebConfigurationManager.AppSettings["CLIENT_ID"],
                    AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
                };
                authReq.AuthParameters.Add("USERNAME", userName);
                authReq.AuthParameters.Add("PASSWORD", password);

                AdminInitiateAuthResponse authResp = await _client.AdminInitiateAuthAsync(authReq);
                string IdToken = authResp.AuthenticationResult.IdToken;
                return IdToken;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}