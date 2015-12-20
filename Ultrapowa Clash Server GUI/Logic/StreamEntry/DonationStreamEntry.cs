using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.Logic
{
    internal class DonationStreamEntry : StreamEntry
    {
        #region Private Fields

        private string m_vMessage;

        #endregion Private Fields

        #region Public Methods

        public override byte[] Encode()
        {
            var data = new List<byte>();

            data.AddRange(base.Encode());
            data.AddString(m_vMessage);

            return data.ToArray();
        }

        public string GetMessage()
        {
            return m_vMessage;
        }

        public override int GetStreamEntryType()
        {
            return 1;
        }

        public override void Load(JObject jsonObject)
        {
            base.Load(jsonObject);
            m_vMessage = jsonObject["message"].ToObject<string>();
        }

        public override JObject Save(JObject jsonObject)
        {
            jsonObject = base.Save(jsonObject);
            jsonObject.Add("message", m_vMessage);
            return jsonObject;
        }

        public void SetMessage(string message)
        {
            m_vMessage = message;
        }

        #endregion Public Methods
    }
}