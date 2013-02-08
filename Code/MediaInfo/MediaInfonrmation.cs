//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//''    MediaFairy                                                               ''
//''    Copyright (C) 2008-2010  Stavros Skamagkis                               ''
//''                                                                             ''
//''    This program is free software: you can redistribute it and/or modify     ''
//''    it under the terms of the GNU General Public License as published by     ''
//''    the Free Software Foundation, either version 3 of the License, or        ''
//''    (at your option) any later version.                                      ''
//''                                                                             ''
//''    This program is distributed in the hope that it will be useful,          ''
//''    but WITHOUT ANY WARRANTY; without even the implied warranty of           ''
//''    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the            ''
//''    GNU General Public License for more details.                             ''
//''                                                                             ''
//''    You should have received a copy of the GNU General Public License        ''
//''    along with this program.  If not, see <http://www.gnu.org/licenses/>.    ''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaFairy;
using MediaInfoLib;
using System.Windows.Forms;

#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MeediOS;
#endif

namespace MediaFairy
{
    class MediaInformation
    {



        #region Media Info


        public static void ImportMediaInfoforVideo
            (MediaInfo mi, IMLItem item, string filesize, 
            string filecodec, int VideoDuration)
        {

            if (!Settings.WantMediaInfo) 
                return;


          

            Debugger.LogMessageToFile("Setting item's Media Information tags...");

            try
            {
                    
                Debugger.LogMessageToFile("filesize: " + filesize);
                item.Tags["filesize"] = filesize;
                item.SaveTags();
                Debugger.LogMessageToFile("filecodec: " + filecodec);
                item.Tags["Format"] = filecodec;
                item.SaveTags();

                Debugger.LogMessageToFile("VideoDuration: " + VideoDuration);
                if (VideoDuration > 0)
                    item.Tags["VideoDuration"] = VideoDuration + " min.";
                item.SaveTags();

                string VideoCodec = mi.Get(StreamKind.Video, 0, "CodecID/Hint");
                Debugger.LogMessageToFile("video codec: " + VideoCodec);
                item.Tags["video_codec"] = VideoCodec;
                item.SaveTags();

                string AudioCodec = mi.Get(StreamKind.Audio, 0, "CodecID/Hint");
                Debugger.LogMessageToFile("audio codec: " + AudioCodec);
                item.Tags["audio_codec"] = AudioCodec;
                item.SaveTags();

                string AudioChannels = mi.Get(StreamKind.Audio, 0, "Channel(s)");
                Debugger.LogMessageToFile("audio channels: " + AudioChannels);
                item.Tags["audio_channels"] = AudioChannels;
                item.SaveTags();

                string AudioLanguage = mi.Get(StreamKind.Audio, 0, "Language/String");
                Debugger.LogMessageToFile("audio language: " + AudioLanguage);
                item.Tags["Audio_Language"] = AudioLanguage;
                item.SaveTags();

                string AudioSamplingRate = mi.Get(StreamKind.Audio, 0, "SamplingRate/String");
                Debugger.LogMessageToFile("audio sampling rate: " + AudioSamplingRate);
                item.Tags["audio_samplingrate"] = AudioSamplingRate;
                item.SaveTags();

                string AudioResolution = mi.Get(StreamKind.Audio, 0, "BitDepth/String");
                Debugger.LogMessageToFile("audio resolution: " + AudioResolution);
                item.Tags["audio_resolution"] = AudioResolution;
                item.SaveTags();

                item.Tags["general_bitrate"] = mi.Get(0, 0, "OverallBitRate");
                item.SaveTags();

                #region Video bitrate
                string videoBitrate = mi.Get(StreamKind.Video, 0, "BitRate");
                if (!String.IsNullOrEmpty(videoBitrate))
                {
                    try
                    {
                        int videoBitrateInt = Convert.ToInt32(videoBitrate);
                        int videoKbps = videoBitrateInt / 1000;
                        item.Tags["video_bitrate"] = videoKbps + "kbps";
                    }
                    catch (Exception)
                    {
                    }

                }
                #endregion

                #region Audio bitrate
                string audioBitrate = mi.Get(StreamKind.Audio, 0, "BitRate");
                if (!String.IsNullOrEmpty(audioBitrate))
                {
                    try
                    {
                        int audioBitrateInt = Convert.ToInt32(audioBitrate);
                        int audioKbps = audioBitrateInt / 1000;
                        item.Tags["audio_bitrate"] = audioKbps + "kbps";
                    }
                    catch (Exception)
                    {
                    }

                }
                #endregion

                item.Tags["video_width"] = mi.Get(StreamKind.Video, 0, "Width");
                item.Tags["video_height"] = mi.Get(StreamKind.Video, 0, "Height");
                item.Tags["video_aspectratio"] = mi.Get(StreamKind.Video, 0, "DisplayAspectRatio/String");
                item.Tags["video_framerate"] = mi.Get(StreamKind.Video, 0, "FrameRate/String");
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An unexpected error ocurred while importing media information. The error was:" + Environment.NewLine + e.ToString());
            }

            item.SaveTags();
        }




        public static void ImportMediaInfoforAudio(MediaInfo MI, IMLItem Item, string filesize, string filecodec, int AudioDuration)
        {
            //MediaInfo MI = new MediaInfo();
            //int OpenResult = MI.Open(file.FullName);
  

            #region write MediaInfo tags 
            if (Settings.WantMediaInfo)
            {
                Debugger.LogMessageToFile("Setting item's Media Information tags...");

                try
                {
                    Item.Tags["filesize"] = filesize;
                    Item.Tags["Format"] = filecodec;

                    if (AudioDuration > 0)
                        Item.Tags["AudioDuration"] = AudioDuration;

                    Item.Tags["audio_codec"] = MI.Get(StreamKind.Audio, 0, "CodecID/Hint");
                    Item.Tags["general_bitrate"] = MI.Get(0, 0, "OverallBitRate");
                    //Item.Tags["audio_channels"] = MI.Get(StreamKind.Audio, 0, "Channel(s)");

                    #region Audio bitrate
                    string audioBitrate = MI.Get(StreamKind.Audio, 0, "BitRate");
                    if (!String.IsNullOrEmpty(audioBitrate))
                    {
                        try
                        {
                            int audioBitrateInt = Convert.ToInt32(audioBitrate);
                            int audioKbps = audioBitrateInt / 1000;
                            Debugger.LogMessageToFile("audio bitrate: " + audioKbps + "kbps");
                            Item.Tags["audio_bitrate"] = audioKbps + "kbps";
                        }
                        catch (Exception)
                        {
                        }

                    }
                    #endregion


                    string audioLanguage = MI.Get(StreamKind.Audio, 0, "Language/String");
                    Debugger.LogMessageToFile("audio language: " + audioLanguage);
                    Item.Tags["Audio_Language"] = audioLanguage;
                    Item.SaveTags();

                    string audioSamplingRate = MI.Get(StreamKind.Audio, 0, "SamplingRate/String");
                    Debugger.LogMessageToFile("audio sampling rate: " + audioSamplingRate);
                    Item.Tags["audio_samplingrate"] = audioSamplingRate;
                    Item.SaveTags();

                    string audioResolution = MI.Get(StreamKind.Audio, 0, "BitDepth/String");
                    Debugger.LogMessageToFile("audio resolution: " + audioResolution);
                    Item.Tags["audio_resolution"] = audioResolution;
                    Item.SaveTags();

                }
                catch (Exception e)
                {
                    Debugger.LogMessageToFile("An unexpected error ocurred while importing media information. The error was:" + Environment.NewLine + e.ToString());
                }

                Item.SaveTags();
            }
            #endregion

        }
        #endregion





    }
}
