OBS Wired Tally Project
==================================================

Project Overview
----------------------------
This project utilizes a Windows .NET app, OBS websocket plugin, Arduino Pro Micro, WS2812 RGB LEDs, and 3D printed hardware.
![Video Demonstration](https://r3---sn-a5meknsd.googlevideo.com/videoplayback?expire=1598301499&ei=GwlEX6HlA8Wc6QLBjIawDg&ip=50.255.205.61&id=fe6cec702ea6198b&itag=18&source=picasa&begin=0&requiressl=yes&sc=yes&susc=ph&app=fife&eaua=Fy9lgiZSPHo&mime=video/mp4&dur=55.960&lmt=1598293587294856&cpn=gccWQLD6pcQCgjxS&c=WEB_EMBEDDED_PLAYER&cver=20200821&sparams=expire,ei,ip,id,itag,source,requiressl,susc,app,eaua,mime,dur,lmt&sig=AOq0QJ8wRAIgJFi6bfdR6lW4pSvrWXkhB_XtqMgSWyTBGQq4eiuOHPICICpXbmftfmTdmNo3HAht6F0bfXSkStD6K6OQWTz_9voX&redirect_counter=1&cm2rm=sn-qxoe77s&req_id=f4819cae5aa2a3ee&cms_redirect=yes&mh=6t&mm=34&mn=sn-a5meknsd&ms=ltu&mt=1598294183&mv=m&mvi=3&pl=20&lsparams=mh,mm,mn,ms,mv,mvi,pl,sc&lsig=AG3C_xAwRQIgCc6-mdhJpQhApoHy72v3eMWeOTEmv4u7gLqZEHOqnPwCIQDPX8zZtx0GTsB4XPGg0QWaNvHsgiS94ihZKKhttysROQ%3D%3D)

Requirments to Build the Project
----------------------------
1. A computer with [OBS](https://obsproject.com/download) and the [OBS Websockets Plugin](https://obsproject.com/forum/resources/obs-websocket-remote-control-obs-studio-from-websockets.466/) installed.
2. Access to a 3D printer.
	- Recommended: [Solutech White filament](https://www.amazon.com/gp/product/B01B5KFRHO/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1) for best LED color results.
3. An [Arduino Pro Micro](https://www.amazon.com/OSOYOO-ATmega32U4-arduino-Leonardo-ATmega328/dp/B012FOV17O?th=1).
4. [WS2812B Ring Lamp](https://www.amazon.com/gp/product/B0105VMT4S/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1) or similar 23mm diameter.
5. [3.5 mm surface-mount headphone connector](https://www.amazon.com/dp/B0833WYLWQ/ref=dp_cerb_1).
6. [3.5 mm male-to-male headphone wire](https://www.amazon.com/gp/product/B004JWIPKM/ref=ppx_yo_dt_b_asin_title_o00_s01?ie=UTF8&psc=1). 
	- Recommended: purchase in 25 ft lengths or as necessary for the distance between your cameras.
7. Soldering iron and solder.
8. [28 AWG copper wire (recommended)](https://www.amazon.com/Electrical-different-Insulated-Temperature-Resistance/dp/B07G2HFCS1/ref=sr_1_5?dchild=1&keywords=28+gauge+wire&qid=1598292006&sr=8-5).

Requirments to Contribute to the Project
----------------------------
If you want to contribute to the software side:
1. Microsoft Visual Studio 2019
	- This project utilizes the [Costura.Fody](https://www.nuget.org/packages/Costura.Fody/) packager, which can be installed with NuGet.
2. Arduino IDE

If you want to contribute to the hardware side:
1. PTC Creo 5 for editing CAD files.
	- Other programs like Solidworks will work, but you will be working with STEP files which won't have model tree data.

Credits
-----------------------------
- Project ideas from the following OBS forum thread: https://obsproject.com/forum/threads/live-cameara-led.73922/
- Arduino portion forked from https://boprograms.eu/obsTally/.)