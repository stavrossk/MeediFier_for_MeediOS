MeediFier for MeediOS
======================

**Copyright (C) 2008-2013 Skamagkis Stavros**

## MeediFier - An intelligent Media Importing System for MeediOS.



### Summary

MeediFier is a multi-purpose entertainment media importer and updater plugin
which aims to provide a complete solution for Meedio and MeediOS users
to easily import and acquire meta-information for their media collections.

The plugin currently supports importing media files and downloading media information for:

* Movies
* tv series/tv shows
* Music
* RSS Feeds




### Introduction

```
Note: Information about basic concepts of media identification follows.
Experienced users of home theater applications may safely skip this chapter.
```


  An important feature of modern Home Theater applications is to be able
to not only playback the  user's entertainment (video or music),
but also present to the user key information about the media material in question
and it's associated art in an attractive graphical interface,
as well as provide advanced media organizing and searching capabilities.


  Depending on it's design, the frontend application and/or it's plugins
  may acquire this information and art by looking up and downloding information
  about the media product in question from online databases
  like TheMovieDb.org, TvDb.com, and/or OpenSubtitles.org.


  Successfull execution of this task however highly depends on accurate
  (and in some cases exact) naming of the user's media files.

  In most cases, adittional key information about the media product
  can greatly aid the Metadata Downloader in uniquely recognizing the media material in question.


  Conventional media identification engines mainly rely on the media filenames
  in order to successfully identify and download metadata for the user's media. 
  This identification method usually requires strictly stuctured
  directory and file naming conventions for the user's media collection.


  Additionally, extraction of this structure requires the definition
  of special text filters such as Tag Masks and Regural Expressions.

  This is where MeediFier comes in to rectify these problems
  by seamlessly integrating the following capabilities: 




### Capabilities & Features

#### Intelligent & adaptive media files importer:

Compared to traditional file importing plugins (tag mask importers),
MeediFier's built-in media files importer:

* Needs very minimal initial configuration (only needs to know the location of user's media collection on disk).

* Recognizes and imports only media files of interest within the specified root directories, ignoring non-media files or files which do not belong to the desired media type. (film, tv episode or music)

* The file importing algorithm works completely idependently from video/audio extensions ensuring that you 'll never have to specify any, and also that any media file which the system is able to playback will be imported regardless of it's file extension.  

* Distinguishes media types of interest inside directories which contain mixed content, eliminating the need of manual media collection organizing.



#### Audio Tag importer:

* Extracts the track information contained in the audio file's IDV3 tags and imports them in their respective library tags.

* Has the ability to embed downloaded information and/or album cover into the audio file's IDV3 tags.



#### Media format Information importer:

* Similarly to the popular MediaInfo plugin, this process will analyze your media files and import media information like video/audio formats and bitrate qualities. Most modern themes display this information graphically on the frontend's interface.



#### Built-in filenames parser and cleaner:

Inspired from the SmartMovieSorter and SmartSeriesSorter plugins, the purpose of this sub-process is to detect the media material's title from the filesystem and extract adittional information about the material which will later help the online identifier to recognize and import details for the media product More specifically:

* Cleans the filenames of media files from release format tags, and extracts the media's actual title.
* Locates media identification information like a film's production year and IMDb id or a tv episode's number and season number. This crucial information then helps the built-in online details downloader to automatically identify the media file without requiring user assistance.
* Parses .nfo files existing in a film's or music album's directory and extracts required media identification information further increasing matching acurracy.



#### Media details and cover art downloader:

* Matches the media file's unique hash or 'fingerprint' with online databases (OSdb and MusicIP) to identify films, tv episodes and music. Requires no previously existing information for the media file to recognize it and tremendously maximizes the identifier's acurracy and minimizes the need of identifying assistance from the user.

* Downloads media information from multiple online databases:
      - imdb.com, themoviedb.org and opensubtitles.org for films.
      - thetvdb.com and imdb.com for tv series.



* Downloads cover art and frontend backdrops fully unattended from the following online sources: (switching between them as necessary)
      - themoviedb.org, ImpAwards and IMDb for films.
      - thetvdb.com for tv shows & tv series.



* Provides ability to disable unattended operation on the fly, letting you select your preferred cover image from the found images on all sources. 

* Saves film description .xml files on sucessful identification. In case the user's Films media section is re-imported from scratch, the plugin will acquire the required information from these descriptors, skipping the online lookup and significantly decrease importing time.



#### Automated video subtitles downloader:  

* Downloads from OpenSubtitles.org synchronized subtitles for your films or tv episodes in your preferred language(s).

* The video fingerprint matching algorithm ensures that the subtitles will always be perfectly synchronized to the particular version of your film.

* Allows to specify an alternative language in case no subtitles are found for your primary language.




#### Media Organizer:

* Automatically sorts (renames and moves) identified media files to your specified locations for each media type according to your desired filesystem structure.



#### Library Cleaner:

* Automatically deletes library items which refer to non-existent (deleted) media files. The built-in network diagnostics will ensure that files residing on offline drives will not be accidentaly deleted.

* Optionally removes empty library section tags ensuring a cleaner media section and faster library accessing opearations.

* Scans and links together multiple video files which constitute a single film. This process ensures that those video files will be chained together even if a previously used file importer was unable to do so.



#### Archive Extractor:    

* Automatically extracts compressed archives contained in the specified media locations and imports any media files of interest they contain. Optionally deletes extracted archives. Is also able to mantain a user-specified free space amount.



#### Media Archiver:    

* Optionally compresses and stores in your specified location a compressed archive for each already watched film, (containing the entire film's directory) for disk space and archiving purposes. Each archived film is removed from the media library, ensuring that only new material is displayed in the frontend.


#### Miscellaneous:

* Intelligently detects media files which reside on a network disk and skips them if your file server or network disk is offline.

* Built-in internet connection and databases availability diagnostics, to cut down retrying delays and prevent connection errors.




#### Downloading Locations:

[Download from MeediOS OpenMAID plugin repository](http://www.meedios.com/OpenMAIDOS/detail.php?plugin_id=3AE69DCF-B3C6-4DD3-85A8-2744A3B50452)

[Download from Google Code](http://code.google.com/p/mediafairy-for-meedios)




**Important:**

Users with 64-bit operating sytem will need to seperately download
from [here](http://mediainfo.sourceforge.net/el/Download) the 64-bit version
of MediaInfo.dll and copy it to the plugin's directory replacing it's original 32-bit version.


###Documentation
For detailed installation, configuration and usage instructions 
please refer to the corresponding plugin's wiki pages on Google Code:

[Wiki Documentation](http://code.google.com/p/mediafairy-for-meedios/w/list)

