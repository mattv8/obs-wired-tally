/////////////////////////////////////////////////////////////////////
//    SW-001 Software, Arduino Script for wired OBS Tally Light    //
/////////////////////////////////////////////////////////////////////
//
//  Original resp. engineer:
//  Matthew P. Visnovsky
//
//  Last modified by:
//  Matthew P. Visnovsky
//
//  #### CHANGE LOG ####
//  FROM | TO | BRIEF DESCRIPTION OF CHANGES
//  -    | A  | Initial release.
//       |    | 
//       |    |
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

  for( int i = 0 ; i < NUM_LEDS; i++) { leds[i] = CRGB::Red; }
    
  FastLED.show(); 

}//End Void Loop
