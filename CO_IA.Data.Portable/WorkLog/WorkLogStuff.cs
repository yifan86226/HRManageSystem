using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class WorkLogStuff : AT_BC.Data.CheckableData<string>, AT_BC.Data.IFileDescription
    {
        public string WorkLogGuid
        {
            get; 
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public bool IsLoaded
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

        public bool ContentLoaded
        {
            get
            {
                return this.IsLoaded;
            }
        }

        public string Extension
        {
            get
            {
                int extPosition = this.Name.LastIndexOf('.');
                if (extPosition > 0)
                {
                    return this.Name.Substring(extPosition + 1);
                }
                return string.Empty;
            }
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

        public DataStateEnum DataState { get; set; }
    }
}
