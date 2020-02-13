using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.MyAccount.Parameters
{
    public class SendResetPasswordInstructionsParam
    {
        public string Email { get; set; }
        public string Scope { get; set; }
    }
}
