OBS Wired and Wireless Tally Project
==================================================

Wireless Tally -- Installed on Raspberry Pi -- RPi Directory
----------------------------

This project is to make a Raspberry pi into a wireless OBS tally light.
This is tested on a pi V1 with a wireless USB module for a wireless connection.
see https://www.raspberrypi.org/documentation/configuration/wireless/wireless-cli.md

For other pi models use other pin numbers and change tally.py
The red led and green led are connected with a 330ohm resistor.
see https://gpiozero.readthedocs.io/en/stable/

1. Install obs-websocket plugin to OBS

2. Install on the pi:
- gpiozero
	sudo apt-get install python-gpiozero
	(sudo apt-get install python3-gpiozero)
- nginx and php
	see https://www.raspberrypi.org/documentation/remote-access/web-server/nginx.md
- php-xml
	sudo apt-get install php-xml

install in /home/pi/
	tally.py
	tally.xml
install in /var/www/html/
	index.php
	gpio.php (for testing led)

set permissions in /var/www/
	sudo chown -R pi:www-data /var/www
	sudo chmod u+rwx,g+srw-x,o-rwx /var/www/
	sudo chmod u+rw,g+r-xw,o-rwx /var/www/html/index.php
set permissions in /home/pi/
	sudo chmod u+rw,g+rw-x,o-rwx /home/pi/tally.xml

3. To make the python script start at boot:
	see https://www.dexterindustries.com/howto/run-a-program-on-your-raspberry-pi-at-startup/
	sudo nano /etc/rc.local
	just above line 'exit 0' insert:
	sudo python /home/pi/tally.py &
	don't forget the & at the end for running it as a daemon
	
after starting the pi with the script running in background go to:
(Use ipscan24 to find the pi on the network.)
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

Wired Tally -- Installed on Arduino Pro Micro -- Arduino Directory
----------------------------

Software uese Windows app, OBS websocket and Arduino Pro Micro

Note: This is a .NET app and only runs on Windows

How to install
1. Wire up the LEDs to your Arduino
2. Scene 1 is PIN 12, Scene 2, 11, 3-10, 4-9 (pin 8 is for "out of range")
3. For preview use PINs 7, 6, 5, 4 (and pin 3 is for preview "out of range")

4. Content from folder "toArduino" should be compiled and uploaded onto your Arduino.
5. If not done before, install obs-websocket plugin to OBS.
6. Run the app in "toOBSComputer" folder.
7. Go through setup (enter names of your scenes and websocket password) IMPORTANT NOTE: the app won't work without password in websocket.
8. Select COM port your Arduino is connected to.

test: driverino

Source code can be found in Arduino/Source

(RPi portion forked from https://github.com/peterfdej/OBSpiTally and Arduino portion forked from https://boprograms.eu/obsTally/.)