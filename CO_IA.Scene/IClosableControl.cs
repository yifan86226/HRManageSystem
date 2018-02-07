using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Scene
{
    public interface IClosableControl
    {
        event Action Closed;
    }
}
