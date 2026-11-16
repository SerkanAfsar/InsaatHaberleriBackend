using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Message;
using HaberApp.Core.Services;
using HaberApp.WebService.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HaberApp.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(CustomFilterAttribute<MessageRequestDto, MessageResponseDto>))]
    [CustomAuthorizationFilter<Message>]
    public class MessagesController : BaseController<MessageRequestDto, Message, MessageResponseDto>
    {
        private readonly IMessageService messageService;

        public MessagesController(IMessageService messageService)
        {
            this.messageService = messageService;
        }
        [AllowAnonymous]
        public override async Task<IActionResult> Post([FromBody] MessageRequestDto blogDto, CancellationToken cancellationToken = default)
        {
            var result = await this.messageService.CreateAsync(blogDto, cancellationToken);
            return Ok(result);
        }

        public override async Task<IActionResult> Put(int id, [FromBody] MessageRequestDto blogRequestDto, CancellationToken cancellationToken = default)
        {
            //return base.Put(id, blogRequestDto, cancellationToken);
            var result = await this.messageService.UpdateAsync(id, blogRequestDto, cancellationToken);
            return Ok(result);
        }
    }
}
