using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ActivityMessage
    {
        public string MessageGuid
        {
            get;
            set;
        }

        public string ActivityGuid
        {
            get;
            set;
        }

        public string MessageType
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public string AssistantInfo
        {
            get;
            set;
        }
    }
}
