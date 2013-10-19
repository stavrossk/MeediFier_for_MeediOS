using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeediFier.ToolBox.Utils
{
    class HtmlCleaners
    {
        static public String HtmlCleanerConventional(string vString, bool vClearHref)
        {

            Helpers.UpdateProgress("Updating Section...","Performing page cleaning...", null);
            //Thread.Sleep(1000);

            string sText1 = vString;


            try
            {

                sText1 = ClearScriptSections(sText1);
				
                sText1 = ClearStyleSections(sText1);

                sText1 = ClearHrefSections(vClearHref, sText1);

                return sText1.Replace
                    ("&nbsp;", " ")
                    .Replace("\n", " ")
                    .Replace("\r", " ")
                    .Replace("\t", " ")
                    .Replace("&amp;", "&")
                    .Trim();
			
            }
            catch
            {
                return vString.Trim();
            }
        }

        private static string ClearHrefSections(bool vClearHref, string sText1)
        {
            int HrefStartPosition = 0;

            int HrefEndPosition = 0;

            do
            {


                HrefStartPosition = sText1.IndexOf
                    ("<", HrefStartPosition,
                     StringComparison.Ordinal);


                if (HrefStartPosition <= -1) 
                    continue;


                if (vClearHref)
                {

                    HrefEndPosition = sText1.IndexOf
                        (">", HrefStartPosition, 
                         StringComparison.Ordinal);


                    if (HrefEndPosition > -1)
                    {
                        HrefEndPosition++;

                        sText1 = sText1.Replace
                            (sText1.Substring(HrefStartPosition, 
                                              (HrefEndPosition - HrefStartPosition)).Trim(), "").Trim();

                        sText1 += " ";
                    }
                    else
                    {
                        HrefStartPosition = -1;
                    }
                }
                else
                {

                    int num3 = sText1.ToLower().IndexOf
                        ("a href", HrefStartPosition,
                         StringComparison.Ordinal);

                    if (num3 == (HrefStartPosition + 1))
                    {
                        HrefStartPosition++;
                    }
                    else
                    {

                        num3 = sText1.ToLower().IndexOf
                            ("/a>", HrefStartPosition,
                             StringComparison.Ordinal);

                        if (num3 == (HrefStartPosition + 1))
                        {
                            HrefStartPosition++;
                        }
                        else
                        {
                            num3 = sText1.ToLower().IndexOf
                                ("p class=\"mt-0", HrefStartPosition,
                                 StringComparison.Ordinal);

                            if (num3 == (HrefStartPosition + 1))
                            {
                                HrefStartPosition++;
                            }
                            else
                            {
                                HrefEndPosition = sText1.IndexOf(">", HrefStartPosition, StringComparison.Ordinal);

                                if (HrefEndPosition > -1)
                                {
                                    HrefEndPosition++;

                                    sText1 = sText1.Replace
                                        (sText1.Substring(HrefStartPosition,
                                                          (HrefEndPosition - HrefStartPosition)).Trim(), "").Trim();

                                    sText1 += " ";
                                }
                                else
                                {
                                    HrefStartPosition = -1;
                                }

                            }

                        }

                    }

                }

            } 
            while (HrefStartPosition > -1);
		
            return sText1;
		
        }

        private static string ClearStyleSections(string sText1)
        {
            int styleStartPosition = 0;

            int styleEndPosition = 0;


            do
            {
                styleStartPosition = sText1.ToLower().IndexOf
                    ("<style", styleStartPosition, StringComparison.Ordinal);


                if (styleStartPosition <= -1)
                    continue;

                styleStartPosition++;

                styleEndPosition = sText1.ToLower().IndexOf
                    ("</style>", styleStartPosition, StringComparison.Ordinal);



                if (styleEndPosition > -1)
                {
                    styleEndPosition += 7;

                    sText1 = sText1.Replace
                        (sText1.Substring(styleStartPosition,
                                          (styleEndPosition - styleStartPosition)).Trim(), "").Trim();

                    sText1 += " ";
                }
                else
                {
                    styleStartPosition = -1;
                }
            } 
            while (styleStartPosition > -1);
		


            return sText1;
		
        }

        private static string ClearScriptSections(string sText1)
        {

            int scriptStartPosition = 0;
            int scriptEndPosition = 0;

            do
            {
                scriptStartPosition = sText1.ToLower().IndexOf
                    ("<script", scriptStartPosition, StringComparison.Ordinal);

                if (scriptStartPosition <= -1)
                    continue;

                scriptStartPosition++;

                scriptEndPosition = sText1.ToLower().IndexOf
                    ("</script>", scriptStartPosition, StringComparison.Ordinal);

                if (scriptEndPosition > -1)
                {
                    scriptEndPosition += 8;

                    sText1 = sText1.Replace
                        (sText1.Substring
                             (scriptStartPosition, (scriptEndPosition - scriptStartPosition)).Trim(), "").Trim();

                    sText1 += " ";
                }
                else
                {
                    scriptStartPosition = -1;
                }
            } 
            while (scriptStartPosition > -1);
		
            return sText1;
		
        }
    }
}
