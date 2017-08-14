using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace CaptivePortal.API.Enums
{
    public enum ReturnCode
    {
        Success = 1,
        LoginSuccess = 202,
        CreateUserSuccess = 204,
        GetMacAddressSuccess = 206,
        DeleteUserSuccess = 208,
        UpdateMacAddressuccess = 210,
        UpdateUserSuccess = 212,
        Failure = 511,
        Warning = HttpStatusCode.Found
    }


    public enum ErrorCodeWarning
    {
        IncorrectPassword = 310,
        usernameisnotexist = 311,
        UserNameRequired = 312,
        PasswordRequired = 313,
        SiteIDRequired = 314,
        SiteIdNotExist = 315,
        UserIdRequired = 316,
        SessionIdRequired = 317,
        UserUniqueIdAlreadyExist = 318,
        MacAddressorUserNameExist = 319,
        NonAuthorize = 320,
        OperationTypeMissing = 321,
        IncorrectOperationtype = 322,
        IncorrectUserId = 323,
        MacAddressNotExist = 324
    }

    public enum OperationType
    {
        Add = 1,
        Delete = 2
    }

    public enum RTLSResult
    {
        Success=0,
        Failed=1
    }
}