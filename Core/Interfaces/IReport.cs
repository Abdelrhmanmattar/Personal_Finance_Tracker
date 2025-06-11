using Core.DTO;
using Core.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReport
    {
        Task<Result<ReportSummaryDTO>> Summary(ClaimsPrincipal claims, DateTime? start = null, DateTime? end = null);
    }
}
