using System.IO;
using System.Linq;
using System.Windows.Forms;
using MeediOS;

namespace MediaFairy.MediaImporters
{
    class BookImporter
    {


        internal static bool ImportBook(FileSystemInfo file, IMLSection booksSection, string fileName, string ext)
        {

            if (ext != ".pdf" && ext != ".PDF" && ext != ".djvu" && ext != ".DJVU")
                return false;

            if (!Settings.EnableBooksImporter)
                return false;


            bool itemfound = false;
            IMLItemList list = booksSection.SearchByLocation(file.FullName);


            foreach (IMLItem item in list.Where(item => item.Location == file.FullName))
                itemfound = true;


            if (itemfound)
                return true;

            Application.DoEvents();
            Debugger.LogMessageToFile("Importing book " + file.Name + "...");
            Importer.ThisProgress.Progress(Importer.CurrentProgress, "Importing book " + file.Name + "...");
            IMLItem Item;
            Helpers.AddFileToSection(out Item, booksSection, fileName, file.FullName, file.FullName);
            return true;



        }



    }



}
