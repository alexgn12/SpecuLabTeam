using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrototipoApi.BaseDatos;
using PrototipoApi.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PrototipoApi.Controllers
{
    [Route("api/request-status-history")]
    [ApiController]
    public class RequestStatusHistoryController : ControllerBase
    {
        private readonly ContextoBaseDatos _context;

        public RequestStatusHistoryController(ContextoBaseDatos context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRequestStatusHistories(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? requestId = null)
        {
            var query = _context.RequestStatusHistories
                .Include(rsh => rsh.Request)
                .Include(rsh => rsh.OldStatus)
                .Include(rsh => rsh.NewStatus)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(rsh => rsh.ChangeDate >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(rsh => rsh.ChangeDate <= toDate.Value);
            if (requestId.HasValue)
                query = query.Where(rsh => rsh.RequestId == requestId.Value);

            var totalCount = await query.CountAsync();
            var histories = await query
                .OrderByDescending(rsh => rsh.ChangeDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(rsh => new RequestStatusHistoryDto
                {
                    RequestId = rsh.RequestId,
                    RequestDescription = rsh.Request.Description,
                    OldStatusId = rsh.OldStatusId,
                    OldStatusType = rsh.OldStatus != null ? rsh.OldStatus.StatusType : null,
                    NewStatusId = rsh.NewStatusId,
                    NewStatusType = rsh.NewStatus.StatusType,
                    ChangeDate = rsh.ChangeDate,
                    Comment = rsh.Comment
                })
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalCount,
                histories
            });
        }
    }
}
