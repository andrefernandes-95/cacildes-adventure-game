using System;
using System.Collections.Generic;
using AF.Dialogue;
using UnityEngine;

namespace AF.ModTools
{
    [System.Serializable]
    public abstract class ModEvent
    {
        public string uuid;
        public string action;
        public object data;

        public abstract void BuildEvent(GameObject root);

        public abstract string GetActionType();
    }

    public class DialogueModEvent : ModEvent
    {
        public DialogueModEvent()
        {
            uuid = Guid.NewGuid().ToString();
            SetData("Sample text...");
        }

        public class Payload
        {
            public string message;
        }

        public void SetData(string message)
        {
            Payload payload = new Payload();
            payload.message = message;
            this.data = payload;
        }

        public string GetMessage()
        {
            return (this.data as Payload).message;
        }

        public override void BuildEvent(GameObject root)
        {
            EV_SimpleMessage eV_SimpleMessage = root.AddComponent<EV_SimpleMessage>();
            eV_SimpleMessage.responses = new List<Response>().ToArray();
            eV_SimpleMessage.message = GetMessage();
        }

        public override string GetActionType()
        {
            if (Utils.IsPortuguese())
            {
                return "Diálogo";
            }

            return "Dialogue";
        }
    }
}
