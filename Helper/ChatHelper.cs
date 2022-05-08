using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArtifactEvolutionPlusPlus
{
    public static class ChatHelper
    {
        public static bool Open = false;
        public static void DebugSend(string message)
        {
            if (Open)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = message
                });
            }
        }
        public static void Send(string message)
        {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = message
            });
        }
    }
}
