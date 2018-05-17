using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;

namespace RuinvestUtils.VK
{
    public class VKLogic
    {
        private const int appID = 6456519;
        private const string login = "+79601605946";
        private const string pass = "maxellpess";
        private readonly Settings scope = Settings.All;
        private readonly VkApi vk;

        private static VKLogic instance;

        public static VKLogic GetInstance()
        {
            instance = instance ?? new VKLogic();

            return instance;
        }

        private VKLogic()
        {
            vk = new VkApi();
            Authorize();
        }

        private void Authorize()
        {
            vk.Authorize(new ApiAuthParams
            {
                ApplicationId = appID,
                Login = login,
                Password = pass,
                Settings = scope
            });
        }

        public void SendMessage(string message)
        {
            var send = vk.Messages.Send(new MessagesSendParams
            {
                UserId = 151438995,
                Message = message
            });
        }

    }
}