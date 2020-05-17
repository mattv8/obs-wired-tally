/////////////////////////////////////////////////////////////////////
//    SW-001 Software, Arduino Script for wired OBS Tally Light    //
/////////////////////////////////////////////////////////////////////
//
//  Written by: Matthew P. Visnovsky
//  Adapted from https://boprograms.eu/obsTally/.
//
//
#include "FastLED.h" //Using FastLED Library https://github.com/FastLED/FastLED

// -----------------------PINS------------------------- //
#define Vout     2   // Digital
#define DATA_PIN A2  // PWM

// -----------------------GLOBAL VARIABLES------------------------- //

//Time variables
unsigned long currentTime;  //Initialize master clock as global variable.
unsigned long pastTime = 0; // Set pastTime to 0. Don't redefine this anywhere or it will break the blink function.

//FastLED Variables
#define LED_TYPE    WS2811
#define COLOR_ORDER GRB
#define NUM_LEDS    7
#define BRIGHTNESS  255

CRGB leds[NUM_LEDS];

//OBS Variables
char currentScene;
char lastScene;

// ------------------------FUNCTIONS------------------------ //


// ------------------------SETUP LOOP------------------------------- //
void setup() {

  // tell FastLED about the LED strip configuration
  FastLED.addLeds<LED_TYPE,DATA_PIN,COLOR_ORDER>(leds, NUM_LEDS)
    .setCorrection(TypicalLEDStrip)
    .setDither(BRIGHTNESS < 255);

  // set master brightness control
  FastLED.setBrightness(BRIGHTNESS);
  
  // Pin mode initializations //
  pinMode(Vout, OUTPUT);

  // Pin digital writes //
  digitalWrite(Vout,HIGH); // Give power to the LED

}//End Setup Loop

// ------------------------VOID LOOP------------------------------- //
void loop(void) {
  
  ////////////// Perpetually Updating ///////////////
  currentTime = millis(); // Update the master clock

  //for( int i = 0 ; i < NUM_LEDS; i++) { leds[i] = CRGB::Red; }
  //FastLED.show(); 

  if(Serial.available()) { // Check if recieving data
	  
    currentScene = Serial.read(); //Read in serial bits
    
    if(currentScene != lastScene) { //If scene state changes
      
      FastLED.clear();  //Clear all pixel data
      
      // Live //
      if (currentScene == '1') {
        leds[1] = CRGB::Red;
      }
      else if (currentScene == '2') {
        leds[2] = CRGB::Red;
      }
      else if (currentScene == '3') {
        leds[3] = CRGB::Red;
      }
      else if (currentScene == '4') {
        leds[4] = CRGB::Red;
      }
      else if (currentScene == '5') {
        leds[5] = CRGB::Red;
      }

  	  // Preview Range //
      if (currentScene == '6') {
        leds[1] = CRGB::Green;
      }
      else if (currentScene == '7') {
        leds[2] = CRGB::Green;
      }
      else if (currentScene == '8') {
        leds[3] = CRGB::Green;
      }
      else if (currentScene == '9') {
        leds[4] = CRGB::Green;
      }
      else if (currentScene == 'n') {
        leds[5] = CRGB::Green;
      }
      
      lastScene = currentScene; // Update scene state
      FastLED.show(); // Update LED's
    
    }//End scene state check
  
  }//End Serial.available()
  
  delay(1); //Simple debounce delay
  
}//End Void Loop
