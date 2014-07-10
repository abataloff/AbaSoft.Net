using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbaSoft.Net.Rest
{
    public interface IRestController
    {
        void Proccess(IHttpMessage a_context);
        void Init();

        string Section { get; }
    }
}
