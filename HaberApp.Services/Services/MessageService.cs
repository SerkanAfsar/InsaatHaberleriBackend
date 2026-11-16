using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Message;
using HaberApp.Core.Exceptions;
using HaberApp.Core.Services;
using HaberApp.Core.Utils;
using System.Net;

namespace HaberApp.Services.Services
{
    public class MessageService : ServiceBase<MessageRequestDto, Message, MessageResponseDto>, IMessageService
    {
        private readonly IClaudFlareService claudFlareService;

        public MessageService(IClaudFlareService claudFlareService)
        {
            this.claudFlareService = claudFlareService;
        }

        public override async Task<ResponseResult<MessageResponseDto>> CreateAsync(MessageRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            await this.claudFlareService.ValidateToken(requestDto.Token, cancellationToken);
            return await base.CreateAsync(requestDto, cancellationToken);
        }

        public override async Task<ResponseResult<MessageResponseDto>> UpdateAsync(int id, MessageRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var result = await this.repository.GetByIdAsync(id, cancellationToken);
            if (result is null)
            {
                throw new CustomException("error", "Not Found", HttpStatusCode.NotFound);
            }

            result.IsReaded = true;
            var updatedResult = await this.repository.UpdateAsync(id, result, cancellationToken);
            await this.unitOfWork.CommitAsync(cancellationToken);

            return new ResponseResult<MessageResponseDto>()
            {
                Entity = this.mapper.Map<MessageResponseDto>(updatedResult)
            };

        }
    }
}
