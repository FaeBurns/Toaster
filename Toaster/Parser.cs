using System.Text.RegularExpressions;

namespace Toaster
{
    public class Parser
    {
        public void Build()
        {

        }

        public void Toast(string program)
        {
            Regex r = new Regex(@"([0-9]+)");
            Match match = r.Match(program);
        }
    }
}