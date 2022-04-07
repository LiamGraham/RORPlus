using System.Linq;

namespace RORPlus
{
    internal class Utils
    {

        public static bool? TryGetBool(string arg)
        {
            string[] trueValues = { "yes", "true", "1" };
            string[] falseValues = { "no", "false", "0", "-1" };

            if (trueValues.Contains(arg.ToLower()))
            {
                return true;
            } else if (falseValues.Contains(arg.ToLower()))
            {
                return false;
            }
            return new bool?();
        }
    }
}
