using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Base;
using HaberApp.Core.DTOs.Others;
using HaberApp.Core.Helpers;
using HaberApp.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace HaberApp.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TRequestDto, TDomain, TResponseDto> : ControllerBase
        where TRequestDto : BaseRequestDto
        where TDomain : BaseEntity
        where TResponseDto : BaseResponseDto
    {
        protected readonly IServiceBase<TRequestDto, TDomain, TResponseDto> serviceBase;

        public BaseController()
        {
            this.serviceBase = ServiceProviderHelper.GetRequiredService<IServiceBase<TRequestDto, TDomain, TResponseDto>>();
        }
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
        {
            var result = await this.serviceBase.FindByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var result = await this.serviceBase.FindAllAsync(null, null, cancellationToken);
            return Ok(result);
        }

        [HttpGet("Paginated")]
        public virtual async Task<IActionResult> GetAllPaginated([FromQuery] GetAllDataTypeRequestDto requestData, CancellationToken cancellationToken = default)
        {
            var paginatedResult = await this.serviceBase.GetAllByQueryAsync(requestData,
            a => string.IsNullOrEmpty(requestData.GlobalFilter)
            || a.Id.ToString() == requestData.GlobalFilter,
        null,
        cancellationToken
    );

            return Ok(paginatedResult);
        }


        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] TRequestDto blogDto, CancellationToken cancellationToken = default)
        {
            var result = await this.serviceBase.CreateAsync(blogDto, cancellationToken);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(int id, [FromBody] TRequestDto blogRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await this.serviceBase.UpdateAsync(id, blogRequestDto, cancellationToken);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var result = await this.serviceBase.DeleteAsync(id, cancellationToken);
            return Ok(result);
        }
    }
}
