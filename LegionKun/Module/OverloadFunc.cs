using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegionKun.Module
{
    public static class OverloadFunc
    {
        public static async Task<IUserMessage> SendMessageAsync(this IMessageChannel channel, string text, bool isTTS = false, Embed embed = null, uint deleteAfter = 0, RequestOptions options = null)
        {
            var message = await channel.SendMessageAsync(text, isTTS, embed, options);
            if (deleteAfter > 0)
            {
                var _ = Task.Run(() => DeleteAfterAsync(message, deleteAfter));
            }
            return message;
        }

        private static async Task DeleteAfterAsync(IUserMessage message, uint deleteAfter)
        {
            await Task.Delay(TimeSpan.FromSeconds(deleteAfter));
            await message.DeleteAsync();
        }
    }
}
