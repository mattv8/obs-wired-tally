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
#define BRIGHTNESS  255
#define NUM_LEDS    7                   //Number of LEDs per tally lamp
#define NUM_SETS    4                   //Total number of tally lamps
#define TOTAL_LEDS  NUM_LEDS*NUM_SETS   //Total number of LEDs

//FastLED Data Structure Setup
CRGB rawleds[TOTAL_LEDS];
CRGBSet leds(rawleds, TOTAL_LEDS);    //Use this if you want to do things with all LEDs
CRGBSet set1(leds(NUM_LEDS*0,NUM_LEDS*1-1));
CRGBSet set2(leds(NUM_LEDS*1,NUM_LEDS*2-1));
CRGBSet set3(leds(NUM_LEDS*2,NUM_LEDS*3-1));
CRGBSet set4(leds(NUM_LEDS*3,NUM_LEDS*4-1));
struct CRGB * ledarray[] ={set1, set2, set3, set4}; 

//OBS Variables
int SerialInt = 0;  // Global array for Serial integer. Initialize at 0.
int currentLive;
int lastLive;
int currentPreview;
int lastPreview;

// ------------------------FUNCTIONS------------------------ //


// ------------------------SETUP LOOP------------------------------- //
void setup() {
 
  // tell FastLED about the LED array configuration.
  FastLED.addLeds<LED_TYPE,DATA_PIN,COLOR_ORDER>(leds, TOTAL_LEDS)
    .setCorrection(TypicalLEDStrip)
    .setDither(BRIGHTNESS < 255);

  // set master brightness control
  FastLED.setBrightness(BRIGHTNESS);
  
  // Pin mode initializations //
  pinMode(Vout, OUTPUT);

  // Pin digital writes //
  digitalWrite(Vout,HIGH); // Give power to the LED

  delay(2000); //Safety delay

}//End Setup Loop

// ------------------------VOID LOOP------------------------------- //
void loop(void) {

  ////////////// Perpetually Updating ///////////////
  //currentTime = millis(); // Update the master clock

  ////////////// Serial Communication ///////////////
  if(Serial.available()) { // Check if recieving data
    
    //ReadSerial(); // Record serial bits using custom function.
    SerialInt = Serial.parseInt() - 1;
      
    // Live Range //
    if ((SerialInt>=1) && (SerialInt<=5)){ //If within live range
      currentLive = SerialInt; // Update live state
    }
    
    // Preview Range //
    if ((SerialInt>=6) && (SerialInt<=10)){ //If within preview range
      currentPreview = SerialInt-5; // Update preview state
    }
  }else { //If not recieving any data
    fill_solid( leds, TOTAL_LEDS, CRGB::Gray ); //Fill all LEDs gray
    FastLED.show();
  }//End Serial.available()


  ////////////// Tally Light States ///////////////
  // Preview //
  if(currentPreview != lastPreview && currentPreview != currentLive) { //If preview state changes & not currently live
      FastLED.clear(); // Clear all pixel data
      //if (currentPreview == 1) {
        //FastLED.clear(); // Clear all pixel data
        fill_solid( ledarray[currentPreview], NUM_LEDS, CRGB::Green );
        //Serial.print("Current preview:" ); Serial.println(currentPreview);
      //}
    }else{
      FastLED.clear(); // Clear all pixel data
    lastPreview = currentPreview; //Update preview state
  }

  // Live //
  //if(currentLive != lastLive) { //If live state changes
    //FastLED.clear(); // Clear all pixel data
      //if (currentLive == 1) {
        //FastLED.clear(); // Clear all pixel data
        fill_solid( ledarray[currentLive], NUM_LEDS, CRGB::Red );
        //Serial.print("Current scene:" ); Serial.println(currentLive);
      //}
    lastLive = currentLive; //Update live state
  //}
  
  FastLED.show(); //Update all LED states
  //delay(10); //Simple debounce delay
  
}//End Void Loop
