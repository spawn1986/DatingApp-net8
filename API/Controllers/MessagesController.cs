using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, Mapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDtoRequest)
    {
        var username = User.GetUsername();

        if (username == createMessageDtoRequest.RecipientUsername.ToLower())
            return BadRequest("You cannot message yourself!");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDtoRequest.RecipientUsername);

        if (recipient == null || sender == null)
            return BadRequest("Cannot send message at this time.");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDtoRequest.Content
        };

        messageRepository.AddMessage(message);

        if (await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message));

        return BadRequest("Failed to save message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await messageRepository.GetMessagesForUserAsync(messageParams);

        Response.AddPaginationHeader(messages);

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();

        return Ok(await messageRepository.GetMessageThreadAsync(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await messageRepository.GetMessageAsync(id);

        if (message == null) return BadRequest("Cannot delete this message");

        if (message.SenderUsername != username && message.RecipientUsername != username) return Forbid();

        if (message.SenderUsername == username) message.SenderDeleted = true;

        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message is { SenderDeleted: true, RecipientDeleted: true }) //only in net 8
        {
            messageRepository.DeleteMessage(message);
        }

        if (await messageRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting the message");
    }
}
