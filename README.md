# Chocobit Mario Maker 2 Overlay

I'm a streamer ([Chocobit on Twitch](https://www.twitch.tv/chocobitto)) and YouTuber ([Chocobit on YouTube](https://www.youtube.com/channel/UCaAfaDoOG80C1KfDI5_W9iw)) that wanted to add some extra interest to my content with a custom Mario Maker 2 overlay. I've been streaming for a little less than a year but I've been a programmer for 22 years and so I created my own. What was meant to just provide more interest to my content turned into something that's a huge support for me when I play as well.

The Chocobit Mario Maker 2 Overlay is a tool that can automatically display information about the current level you're playing. It will also automatically time the level and count your deaths. Just set it up, and turn it on and it does all the work for you. It's designed to be a stream overlay, but can be used just to give you that extra edge in endless or automatically track your play time and deaths even if you stop playing a level for a bit and come back to it. It remembers how long you've played a level and how many times you've died across play breaks.

![Application Screenshot!](https://github.com/chocobitto/Mario-Maker-2-Overlay/blob/master/screenshots/app.jpg)

## Clear Rate Comparison and Timer
First things first, this project allows you to track time invested in a level as well as number of deaths. Pressing the up or down arrows increase or decrease your death count. Moreso though, the overlay also estimates your current clear rate for the level so that you can compare it to the playerbase average. Additionally, when you move on to a new level and come back, it will restore the timer and the death count in case you want to stop playing a level for a bit, then come back to it.

## Nintendo API Integration
While developing the application it became clear the value of loading details for the current level directly from Nintendo. This makes comparing your clear rate performance against the community average much simpler and more accurate. Once we had API integration though we decided showing a lot more information would be interestging. So, we additionally display:

* Level Name
* Level Difficulty Categorization
* Likes
* Boos
* Like R  atio (e.g. 5:1)
* Clears
* Attempts
* Clear Rate
* World Record Time
* Clear Check Time
* Tags

Thank you to [tgrcode.com](https://tgrcode.com/) for providing us with such a convenient way to access these Nintendo APIs.

## Mario Maker OCR Integration

While researching how to connect to the Nintendo API, we came across another cool project called [Mario Maker OCR](https://github.com/dram55/MarioMaker2OCR). This project uses OCR to scan for the level being played and has a websocket server that feeds the new level code directly to us. It also does things like inform us of deaths and let us know when a level has been cleared. It's pretty sweet and we've integrated it as well.

# Setup
Running the application is simple, just build the project and run the executable. Without setting up the OCR integration you'll just have to type the level codes in manually, but the timer, death count, clear rate tracking and loading of level information from the Nintendo API all work out of the box.

## Setting up the OCR Integration
To get your deaths to be counted automatically and have the level loaded automatically you have to download and start the Mario Maker OCR tool before opening the Mario Maker 2 Overlay. Go to the [readme page](https://github.com/dram55/MarioMaker2OCR) for the project and follow the directions.

>The Mario Maker 2 OCR page gives instructions on set up for OBS. It so happens that there is something similar needed for `Streamlabs`, but it's much easier to do. If you're using Streamlabs just google `virtual camera` and Streamlabs to learn how it is done.
