using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MeediFier.Code.RegEx_Matchers
{
    class RegExMatchers
    {


        internal static string MatchRegexExpressionReturnFirstMatchFirstGroup(
            string textData, string regexPattern)
        {


            Regex regex = new Regex(regexPattern);
            
            string result = String.Empty;
            
            MatchCollection matches = regex.Matches(textData);

            if (matches.Count == 0)
                return String.Empty;

            try
            {

                result = matches[0].Groups[1].Value;
                if (String.IsNullOrEmpty(result))
                    result = matches[0].Value;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }



            return result;
        }








        internal static string[] MatchExpressionReturnFirstMatchAllGroups(string textData, string regexPattern)
        {
            Regex regex = new Regex(regexPattern);
            string[] results = new[] {String.Empty};
            MatchCollection matches = regex.Matches(textData);

            if (matches.Count == 0)
                return null;

            try
            {

                for (int i = 0; i < matches[0].Groups.Count; i++)
                    results[i] = matches[0].Groups[i].Value;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }



            return results;
        }



        internal static string[] MatchExpressionReturnAllMatchesFirstGroup(string textData, string regexPattern)
        {
            Regex regex = new Regex(regexPattern);
            MatchCollection matches = regex.Matches(textData);
            string[] result = new string[matches.Count];

            if (matches.Count == 0)
                return null;

            try
            {

                for (int i = 0; i < matches.Count; i++)
                {
                    result[i] = matches[i].Groups[1].Value;
                    if (String.IsNullOrEmpty(result[i]))
                        result[i] = matches[i].Value;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return result;
        }
    
    
    }


}
