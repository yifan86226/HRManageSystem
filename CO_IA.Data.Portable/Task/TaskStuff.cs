using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class TaskStuff : AT_BC.Data.CheckableData<string>,AT_BC.Data.IFileDescription
    {
        public string TaskGuid
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        private byte[] content;

        public byte[] Content
        {
            get
            {
                return this.content;
            }
            set
            {
                if (value != null)
                {
                    this.content = value;
                    this.IsLoaded = true;
                }
            }
        }

        public long ContentLength
        {
            get;
            set;
        }

        public bool IsResultStuff
        {
            get;
            set;
        }

        public string OwnerGuid
        {
            get;
            set;
        }

        public string SubmitUser
        {
            get;
            set;
        }

        public DateTime SubmitTime
        {
            get;
            set;
        }

        public bool IsLoaded
        {
            get;
            set;
        }

        string AT_BC.Data.IFileDescription.Extension
        {
            get 
            {
                int extPosition=this.Name.LastIndexOf('.');
                if (extPosition > 0)
                {
                    return this.Name.Substring(extPosition+1);
                }
                return string.Empty;
            }
        }

        bool AT_BC.Data.IFileDescription.ContentLoaded
        {
            get
            {
                return this.IsLoaded;
            }
        }
    }
}
