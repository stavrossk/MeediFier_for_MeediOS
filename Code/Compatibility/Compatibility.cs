using System.Windows.Forms;
using MeediOS;

// ReSharper disable CheckNamespace
namespace MediaFairy
// ReSharper restore CheckNamespace
{

    internal class Compatibility
    {

     
        public static void ReplaceTag(string tagName, string[] searchStrings, IMLSection section)
        {

            foreach (string searchReplaceString in searchStrings)
            {
                MessageBox.Show(searchReplaceString);
                string[] replaces = searchReplaceString.Split('|');
                string searchString = replaces[0];
                string replaceString = replaces[1];
                MessageBox.Show(@"Will replace with: " +  replaceString);

                IMLItemList itemList = section.SearchByTag(tagName, searchString);

                section.BeginUpdate();
                

                foreach (IMLItem t in itemList)
                {
                    MessageBox.Show(tagName);
                    t.Tags[tagName] = replaceString;
                    MessageBox.Show((string)t.Tags[tagName]);
                    t.SaveTags();
                }

                section.EndUpdate();

            }


        }
    
    }

}
