using Microsoft.AspNetCore.Mvc;
using SSKJ.RoadManageSystem.API.Controllers;
using SSKJ.RoadManageSystem.IBusines.System;
using System;
using System.Threading.Tasks;

namespace SSKJ.RoadManageSystem.API.Areas.SystemManage.Controllers
{
    [Route("api/Account/[action]")]
    [Area("SystemManage")]
    public class AccountController : BaseController
    {
        private readonly IUserBusines sysUserBll;
        private readonly IUserProjectBusines userProjectBll;
        private readonly IAreaBusines areaBll;

        public AccountController(IUserBusines sysUserBll, IUserProjectBusines userProjectBll, IAreaBusines areaBll)
        {
            this.sysUserBll = sysUserBll;
            this.userProjectBll = userProjectBll;
            this.areaBll = areaBll;
        }
        [HttpGet]
        public async Task<IActionResult> GetAccountData(int pageSize, int pageIndex)
        {
            try
            {
                var data = await sysUserBll.GetListAsync(e => true, e => e.CreateDate, true, pageSize, pageIndex);
                return SuccessData(new
                {
                    data = data.Item1,
                    count = data.Item2
                });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAreaData()
        {
            try
            {
                var data = await areaBll.GetListAsync();
                return SuccessData(data);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }
    }
}