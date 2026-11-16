using HaberApp.Core.Domains;
using HaberApp.Core.DTOs.Message;

namespace HaberApp.Core.Services
{
    public interface IMessageService : IServiceBase<MessageRequestDto, Message, MessageResponseDto>
    {
    }
}
