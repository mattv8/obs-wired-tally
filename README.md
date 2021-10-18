THIS PROJECT IS DEPRECATED. PLEASE SEE THE FOLLOWING REPOSITORY FOR THE LATEST: https://github.com/mattv8/open-tally
==================================================

Project Overview
----------------------------
This project utilizes a Windows .NET app, OBS websocket plugin, Arduino Pro Micro, WS2812 RGB LEDs, and 3D printed hardware to build an inexpensive OBS Tally Light system that is power efficient and relatively easy to get up and running. The tally lights themselves are designed to work with a standard hot (or cold) shoe found on most DSLR's and camcorders.

[![](http://img.youtube.com/vi/zPsItWoxoUQ/0.jpg)](http://www.youtube.com/watch?v=zPsItWoxoUQ "")


Requirments to Build the Project
----------------------------
1. A computer with [OBS](https://obsproject.com/download) and the [OBS Websockets Plugin](https://obsproject.com/forum/resources/obs-websocket-remote-control-obs-studio-from-websockets.466/) installed.
2. Access to a 3D printer.
	- I recommend [Solutech White filament](https://www.amazon.com/gp/product/B01B5KFRHO/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1) for best LED color results. It's a translucent white filament that makes for a good light diffusion effect on the "bulb" part.
3. An [Arduino Pro Micro](https://www.amazon.com/OSOYOO-ATmega32U4-arduino-Leonardo-ATmega328/dp/B012FOV17O?th=1).
4. [WS2812B Ring Lamp](https://www.amazon.com/gp/product/B0105VMT4S/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1) or similar 23mm diameter.
5. [3.5 mm surface-mount headphone connector](https://www.amazon.com/dp/B0833WYLWQ/ref=dp_cerb_1).
6. [3.5 mm male-to-male headphone wire](https://www.amazon.com/gp/product/B004JWIPKM/ref=ppx_yo_dt_b_asin_title_o00_s01?ie=UTF8&psc=1). 
	- Recommended: purchase in 25 ft lengths or as necessary for the distance between your cameras.
7. Soldering iron and solder.
8. [28 AWG copper wire (recommended)](https://www.amazon.com/Electrical-different-Insulated-Temperature-Resistance/dp/B07G2HFCS1/ref=sr_1_5?dchild=1&keywords=28+gauge+wire&qid=1598292006&sr=8-5).

Requirments to Contribute to the Project
----------------------------
I would welcome any software contributions! I have licensed the CAD parts and software as GPLv3. If you want to contribute to the software side, you will need the following:
1. Microsoft Visual Studio 2019
	- This project utilizes the [Costura.Fody](https://www.nuget.org/packages/Costura.Fody/) packager, which will have to be installed into MS Visual Studio with [NuGet](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio).
2. [Arduino IDE](https://www.arduino.cc/en/main/software).

If you want to contribute to the hardware side:
1. PTC Creo 5 for editing CAD files.
	- Unfortunately Creo is not free software. FOSS alternatives are [FreeCAD](https://www.freecadweb.org/), [OpenSCAD](https://www.openscad.org/) or [Blender](https://www.blender.org/download/).
	- I have provided STEP files in the "/CAD/STEP Files" folder that can be opened in other CAD programs.
	- I have provided STL files in the "/CAD/STL Files" that can be sent straight to your favorite slicer for 3D printing.

Credits
-----------------------------
- Project ideas from the following OBS forum thread: https://obsproject.com/forum/threads/live-cameara-led.73922/
- Arduino portion forked from https://boprograms.eu/obsTally/.
