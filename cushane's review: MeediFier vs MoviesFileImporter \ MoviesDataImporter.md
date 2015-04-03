When I used Meedio I was a big fan of MediaFairy. Using Meedios I uncovered some MediaFairy bugs and started to shy away from it. The rapid advancement of as MoviesFileImporter and MovieDataImporter eventually reduced my use of MediaFairy to nothing but a subtitle downloader. 

In version 0.5.5 Enthalpient incorporated MetaDataFolders and MediaFolders so I decided to try it again. Rather than just testing it for the sake of testing it, I decided I would do a side-by-side comparison to the MFI/MDI combo I was currently using. Here is what I found: 

###MediaFairy: 
98% of trailers were correctly excluded (160/163) 
100% of movie files were imported (491/491) 
99% of movies were identified (488/491) 
97% of movies were correctly identified (473/491) 
47 minutes to complete the import 
Errors: 4 filename parsing errors & 1"series tags extractor" error 

###MFI/MDI: 
100% of trailers were correctly excluded (163/163) 
99% of movie files were imported (486/491) 
91% of movies were identified (444/491) 
84% of movies were correctly identified (408/491) 
113 minutes to complete the import 
Errors: none 

How I score it: 

###Trailers: 
MediaFairy pulled in three that it shouldn't have, but the new version that has just been posted is supposed to remedy this problem. So I won't hold it against MF. Winner: Tie 

###Importing: 
The fact that MoviesFileImporter skipped 5 movies completely is a real problem. But because it appears to have to do with file extensions I'm sure it will be easily remedied once I post it on the plugin's thread. So I'll still call it a wash. Winner: Tie 

###Identifying: 
I'll skip the number identified and go straight to the number correctly identified because that is what ultimately matters. MediaFairy clearly performed much better in this area. Whats even more, a MF update has just been released which should improve "the plugin's ability to import and automatically identify films" Winner: MediaFairy 

###Run Time: 
This is an area were MDI really needs to make some improvements. It took nearly 2.5x longer to complete! Winner: MediaFairy 

###Error Handling: 
My take on this one might surprise you given the results I posted. Even though MF threw 5 errors it is very important to note that the importer did not halt. Instead, a message was displayed in the system tray. After running this test, I reran MDI with poster resizing enabled. MDI generated 2 errors. I wouldn't penalize MDI for this because image resizing was not in the scope of this test; however, MDI's error handling gives it a significant disadvantage. MDI's importing actually stops until the user clicks "ok" on the error message. This is extremely annoying if you want to run a large unattended import -- you could come back to find that your import is nowhere close to being finished because it is waiting on the user to click "ok". Winner: MediaFairy 

#Final Score: MFI/MDI - 0, MF - 3 

###Closing Remarks: 
Don't rush out and dump MFI/MDI just yet, they still bring a lot to the table. MDI has much more flexibility in terms of scrapers. MDI is required to use some of MeePopupProvider's functionality, which is something I use regularly. MDI also supports downloading multiple fanart images -- MF currently does not. 

Ultimately it comes down to personal preference. I for one am glad that both of these plugins are out there. A little healthy competition benefits everyone. 



###Notes:
I have a library of 491 movies, each in their own folder. 163 of those folders also contain trailers (named using the *-trailer.* convention). None of the movies are in DVD/BluRay/Vob format.

I had both importers configured to only download IMDb data and posters (using one source). I did not have things such as trailers, subtitles, fanart, picture resizing, etc. enabled
Both importers were configured to run in full-automatic/unassisted mode
I have attached all of the data I collected (logs, which movies were not identified, which movies were misidentified, etc) in two zip files: one for each method of importing
