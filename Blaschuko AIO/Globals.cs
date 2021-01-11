using System;
using System.Collections.Generic;
using System.Linq;
using SafeRequest;
using System.Text;
using System.Threading.Tasks;

namespace Blaschuko_AIO
{
    class Globals
    {

        public static Response Response;

        public static string RootUrl = "https://vdorks.xyz/api/";

        private static string EncryptionKey = "XK8xbXb%O+Ni40Jja@!A&o_hZjaytqcP+e5RhjG$1le7#vkWCOR^P5Eq0cfHe+5VgGcl+PFKeJWF=01u_XA4XjUO3YT#YykL=#050Sxj976YjKPKLJJd6!u5yKZG_mM76AiClN+$&i$fC$%!!!pz9Iw7vh+P7oIB6u8Xf-?Z$7q-2zR%$fzs+GPQE-ZQJoAw@yc8NnIsc56JfS$PWfvbbDsSABFl&9Vs5ZEl-0=uQ7BG+2u997=zT?9cGKOcIwz1";

        public static SafeRequest.SafeRequest SafeRequest = new SafeRequest.SafeRequest(EncryptionKey);

        public static User User;

        public static string UserHwid = "";

        public static List<string> Lines = new List<string>();

        public static List<string> NewLines = new List<string>();

    }
}
