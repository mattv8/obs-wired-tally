OBS Wired and Wireless Tally Project
==================================================

Wired Tally -- Installed on Arduino Pro Micro -- Arduino Directory
----------------------------

Software uese Windows app, OBS websocket and Arduino Pro Micro.

TO DO: add demo video of the project in use.

Source code can be found in Arduino/Source. .NET can be developed in Visual Studio.


Wireless Tally -- Installed on Raspberry Pi -- RPi Directory
----------------------------
NOTE: THIS IS A WORK IN PROGRESS.

This project is to make a Raspberry pi into a wireless OBS tally light.
This is tested on a Raspberry Pi Zero W running Raspbian (non-desktop).
See https://www.raspberrypi.org/documentation/configuration/wireless/wireless-cli.md

For other pi models use other pin numbers and change tally.py
The red led and green led are connected with a 330ohm resistor.
See https://gpiozero.readthedocs.io/en/stable/.

1. Install obs-websocket plugin to OBS

2. Install program dependencies on the pi:
```
sudo apt update
sudo apt install python-gpiozero php-xml nginx php-fpm -y
```
*For more info on nginx and php see https://www.raspberrypi.org/documentation/remote-access/web-server/nginx.md*

**Install Directories**

Place the following files in /home/pi/:
* tally.py
* tally.xml

Place the following files in /var/www/html/:
* index.php
* gpio.php (for testing led)

**Permissions**

Set permissions for /var/www/
```
sudo chown -R pi:www-data /var/www
sudo chmod u+rwx,g+srw-x,o-rwx /var/www/
sudo chmod u+rw,g+r-xw,o-rwx /var/www/html/index.php
```

Set permissions for /home/pi/
```
sudo chmod u+rw,g+rw-x,o-rwx /home/pi/tally.xml`
```

3. To make the python script start at boot (more info at https://www.dexterindustries.com/howto/run-a-program-on-your-raspberry-pi-at-startup/):
	
```
sudo nano /etc/rc.local
```
Just above line 'exit 0' insert:
```
sudo python /home/pi/tally.py &
```

Don't forget the & at the end for running it as a daemon. After starting the pi with the script running in background, in a browser go to (Use ipscan24 to find the pi on the network.):
    
http://<pi IP address>/gpio.php for testing the led.

http://<pi IP address>/index.php for configuring the 'tally'

After configuring the 'tally' reboot the pi.

Next project steps:
- Raspberry pi 3B+
- 10mm RGB led's on hot/cold shoe, wire connected to pi
	red = programm
	green = preview
	blue = not connected to OBS.
- film to show it working.
- publish a complete configured image for Raspberry pi 3B+

It would be nice to develop an intercom system with a USB headset and the Pi.

Credits
-----------------------------
- Project ideas from the following OBS forum thread: https://obsproject.com/forum/threads/live-cameara-led.73922/
- RPi portion forked from https://github.com/peterfdej/OBSpiTally
- Arduino portion forked from https://boprograms.eu/obsTally/.)