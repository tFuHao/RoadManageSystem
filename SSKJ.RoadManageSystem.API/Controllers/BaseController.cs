using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using SSKJ.RoadManageSystem.API.Models;

namespace SSKJ.RoadManageSystem.API.Controllers
{
    public class BaseController : Controller
    {
        public UserTokenInfoModel userTokenInfo;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                string strToken = "";
                if (Request.Headers.TryGetValue("x-access-token", out StringValues token))
                    strToken = token.ToString();
                var userInfo = Utility.Tools.TokenUtils.ToObject<UserTokenInfoModel>(strToken);

                userTokenInfo = userInfo;
            }
            catch (Exception)
            {

            }
            base.OnActionExecuting(context);
        }

        public UserTokenInfoModel GetUserInfo()
        {
            return userTokenInfo;
        }
    }
}