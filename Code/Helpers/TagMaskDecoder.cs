using System;
using MeediOS;



namespace MediaFairy.Code.Helpers
{



    class TagMaskDecoder
    {




        internal static string DecodeTagMask(IMLItem item, string tagMask)
        {


            string decodedString = "";


            int j =0;
            for (int i = 0; i < tagMask.Length;i++ )
            {

                #region check char for tag opening and decode it
                if (tagMask[i] == '<')
                {
                    #region check if the tag closes
                    bool isValid = false;
                    for ( j =i + 1; j < tagMask.Length; j++)
                    {
                        if (tagMask[j] == '>')                        
                            isValid = true;                       
                    }
                    #endregion

                    #region get the rest of the tag and continue to next character
                    if (isValid)
                    {
                        #region get tag's name
                        string tagName = "";
                        
                        
                        for (j = i + 1; j < tagMask.Length; j++) //capture tag name until tag closes
                        {

                            if (tagMask[j] == '>')
                            {
                                i = j;
                                break;
                            }


                            tagName += tagMask[j];
                        }

                        #endregion

                        #region Get the tag's value

                        string TagValue = "";
                        try
                        {
                            TagValue = MediaFairy.Helpers.GetTagValueFromItem(item,tagName);
                        }
                        catch { }

                        if (String.IsNullOrEmpty(TagValue))
                        {
                            Debugger.LogMessageToFile("The value of the tag " + " is empty. Unable to download a backdrop for this item. ");
                            return "";
                        }
                        #endregion

                        decodedString += TagValue;
                    }
                    else
                    {
                        Debugger.LogMessageToFile("Invalid tag mask scheme detected. Unable to download backdrops.");
                        return "";
                    }
                    #endregion

                }
                else decodedString += tagMask[i];
                #endregion

            }

            return decodedString;

        }


    }



}
