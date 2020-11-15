/////////////////////////////////////////////////////////////////////
//    SW-001 Software, Arduino Script for wired OBS Tally Light    //
/////////////////////////////////////////////////////////////////////
//
//  Written by: Matthew P. Visnovsky
//  Adapted from https://boprograms.eu/obsTally/.
//  Includes code by Mark Kriegsman.
//
#include "FastLED.h" //Using FastLED Library https://github.com/FastLED/FastLED

// Dependency check
#if FASTLED_VERSION < 3001000
#error "Requires FastLED 3.1 or later; check github for latest code."
#endif

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
#define DUTY_CYCLE  255                 //PWM duty cycle initialization
#define NUM_LEDS    7                   //Number of LEDs per tally lamp
#define NUM_SETS    4                   //Total number of tally lamps
#define TOTAL_LEDS  NUM_LEDS*NUM_SETS   //Total number of LEDs

//FastLED Data Structure Setup
CRGB rawleds[TOTAL_LEDS];
CRGBSet leds(rawleds, TOTAL_LEDS);    //Use this if you want to do things with all LEDs at once
CRGBSet set1(leds(NUM_LEDS*0,NUM_LEDS*1-1));
CRGBSet set2(leds(NUM_LEDS*1,NUM_LEDS*2-1));
CRGBSet set3(leds(NUM_LEDS*2,NUM_LEDS*3-1));
CRGBSet set4(leds(NUM_LEDS*3,NUM_LEDS*4-1));
struct CRGB * ledarray[] ={set1, set2, set3, set4}; 

// State Variables
int SerialState = 0; int lastSerialState = 0;
int currentPreview1; int currentPreview2; int currentPreview3; int currentPreview4;
int lastPreview1; int lastPreview2; int lastPreview3; int lastPreview4;
int currentLive1; int currentLive2; int currentLive3; int currentLive4;
int lastLive1; int lastLive2; int lastLive3; int lastLive4;

// Serial Variables
char readString[15];
int startbit;
int Idx1;
int Idx2;
int Idx3;
int Idx4;

// Misc Variables
int numLive = 1; int numPreview = 1;

// ------------------------FUNCTIONS------------------------ //

// This function reads the incoming serial stream and parses it into respective variables.
// We expect a string like  51,2,3,4*  or  50,2,*. For now, this only allows for up to 4
// sources, but we can manually modify the code to allow for more.
//   Serial Key:
//   50  = Preview state change
//   51  = Live state change
//   0-3 = Live/Preview source range
//   4   = Source is out of range
//   ,   = Delimiter
//   *   = Stop bit
void readSerial () {

  char *ptr;
  int charsRead;

  charsRead = Serial.readBytesUntil(',*', readString, sizeof(readString) - 1);
  readString[charsRead] = '\0';

  //Serial.print("Captured string is : "); Serial.println(readString);
  //Serial.print("Length of string is : "); Serial.println(strlen(readString));

  ptr = strtok(readString, ","); if (ptr != '\0') { startbit = atoi(ptr); }
  ptr = strtok('\0', ",");       if (ptr != '\0') { Idx1 = atoi(ptr); } else { Idx1 = -1; }
  ptr = strtok('\0', ",");       if (ptr != '\0') { Idx2 = atoi(ptr); } else { Idx2 = -1; }
  ptr = strtok('\0', ",");       if (ptr != '\0') { Idx3 = atoi(ptr); } else { Idx3 = -1; }
  ptr = strtok('\0', ",");       if (ptr != '\0') { Idx4 = atoi(ptr); } else { Idx4 = -1; }
  
  readString[0] = '\0'; //clears variable for new input
  
}

// This function draws rainbows with an ever-changing, widely-varying set of parameters. 
// Function credit goes to Mark Kriegsman (2015).
void rainbow() {
  static uint16_t sPseudotime = 0;
  static uint16_t sLastMillis = 0;
  static uint16_t sHue16 = 0;
 
  uint8_t sat8 = beatsin88( 87, 220, 250);
  uint8_t brightdepth = beatsin88( 341, 96, 224);
  uint16_t brightnessthetainc16 = beatsin88( 203, (25 * 256), (40 * 256));
  uint8_t msmultiplier = beatsin88(147, 23, 60);

  uint16_t hue16 = sHue16;//gHue * 256;
  uint16_t hueinc16 = beatsin88(113, 1, 3000);
  
  uint16_t ms = millis();
  uint16_t deltams = ms - sLastMillis ;
  sLastMillis  = ms;
  sPseudotime += deltams * msmultiplier;
  sHue16 += deltams * beatsin88( 400, 5,9);
  uint16_t brightnesstheta16 = sPseudotime;
  
  for( uint16_t i = 0 ; i < NUM_LEDS*NUM_SETS; i++) {
    hue16 += hueinc16;
    uint8_t hue8 = hue16 / 256;

    brightnesstheta16  += brightnessthetainc16;
    uint16_t b16 = sin16( brightnesstheta16  ) + 32768;

    uint16_t bri16 = (uint32_t)((uint32_t)b16 * (uint32_t)b16) / 65536;
    uint8_t bri8 = (uint32_t)(((uint32_t)bri16) * brightdepth) / 65536;
    bri8 += (255 - brightdepth);
    
    CRGB newcolor = CHSV( hue8, sat8, bri8);
    
    uint16_t pixelnumber = i;
    pixelnumber = (NUM_LEDS*NUM_SETS-1) - pixelnumber;
    
    nblend( leds[pixelnumber], newcolor, 64);
  }
}

// ------------------------SETUP LOOP------------------------------- //
void setup() {
 
  // Tell FastLED about the LED array configuration.
  FastLED.addLeds<LED_TYPE,DATA_PIN,COLOR_ORDER>(leds, TOTAL_LEDS)
    .setCorrection(TypicalSMD5050)
    .setDither(DUTY_CYCLE < 255);
  
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

  // Clear out all the LEDs upon serial state change
  SerialState = Serial.available();
  if (SerialState != lastSerialState) {
    FastLED.clear();
  }
  
  ////////////// Serial Communication ///////////////
  if(Serial.available()) { // Check if recieving data

    readSerial(); // Record serial bits from incoming serial stream.
    
    ////////////// Preview Range //////////////
    if (startbit == 50) { currentPreview1 = Idx1; currentPreview2 = Idx2; currentPreview3 = Idx3; currentPreview4 = Idx4; }
    
    ////////////// Live Range //////////////
    if (startbit == 51) { currentLive1 = Idx1; currentLive2 = Idx2; currentLive3 = Idx3; currentLive4 = Idx4; }

    ////////////// Tally Light States ///////////////
    // Preview State //
    if(lastPreview1 >= 0 || lastPreview1 != currentPreview1){ fill_solid( ledarray[lastPreview1], NUM_LEDS, CRGB::Black ); lastPreview1 = currentPreview1; }
    if(lastPreview2 >= 0 || lastPreview2 != currentPreview2){ fill_solid( ledarray[lastPreview2], NUM_LEDS, CRGB::Black ); lastPreview2 = currentPreview2; }
    if(lastPreview3 >= 0 || lastPreview3 != currentPreview3){ fill_solid( ledarray[lastPreview3], NUM_LEDS, CRGB::Black ); lastPreview3 = currentPreview3; }
    if(lastPreview4 >= 0 || lastPreview4 != currentPreview4){ fill_solid( ledarray[lastPreview4], NUM_LEDS, CRGB::Black ); lastPreview4 = currentPreview4; }
    
    // Live State //
    if(lastLive1 >= 0 || lastLive1 != currentLive1){ fill_solid( ledarray[lastLive1], NUM_LEDS, CRGB::Black ); lastLive1 = currentLive1; }
    if(lastLive2 >= 0 || lastLive2 != currentLive2){ fill_solid( ledarray[lastLive2], NUM_LEDS, CRGB::Black ); lastLive2 = currentLive2; }
    if(lastLive3 >= 0 || lastLive3 != currentLive3){ fill_solid( ledarray[lastLive3], NUM_LEDS, CRGB::Black ); lastLive3 = currentLive3; }
    if(lastLive4 >= 0 || lastLive4 != currentLive4){ fill_solid( ledarray[lastLive4], NUM_LEDS, CRGB::Black ); lastLive4 = currentLive4; }

    // Continually update current previews
    if(currentPreview1 >= 0){ fill_solid( ledarray[currentPreview1], NUM_LEDS, CRGB::Green ); numPreview = numPreview + 1; }
    if(currentPreview2 >= 0){ fill_solid( ledarray[currentPreview2], NUM_LEDS, CRGB::Green ); numPreview = numPreview + 1; }
    if(currentPreview3 >= 0){ fill_solid( ledarray[currentPreview3], NUM_LEDS, CRGB::Green ); numPreview = numPreview + 1; }
    if(currentPreview4 >= 0){ fill_solid( ledarray[currentPreview4], NUM_LEDS, CRGB::Green ); numPreview = numPreview + 1; }

    // Continually update current lives
    if(currentLive1 >= 0){ fill_solid( ledarray[currentLive1], NUM_LEDS, CRGB::Red ); numLive = numLive + 1; }
    if(currentLive2 >= 0){ fill_solid( ledarray[currentLive2], NUM_LEDS, CRGB::Red ); numLive = numLive + 1; }
    if(currentLive3 >= 0){ fill_solid( ledarray[currentLive3], NUM_LEDS, CRGB::Red ); numLive = numLive + 1; }
    if(currentLive4 >= 0){ fill_solid( ledarray[currentLive4], NUM_LEDS, CRGB::Red ); numLive = numLive + 1; }
        
    // Set duty cycle based on count of live and preview tallies
    if ( numPreview > 0 && numPreview < 2 || numLive > 0 && numLive < 2 ) { FastLED.setBrightness(DUTY_CYCLE*.10); }// Only 2 lights, use 10% duty cycle
    if ( numPreview >= 2 || numLive >= 2 ) { FastLED.setBrightness(DUTY_CYCLE*.05); }// 2 or more lights, use 5% duty cycle
    numLive = 0; numPreview = 0; // Reset live/preview counters

  }else { //If not recieving any data
    
    FastLED.setBrightness(DUTY_CYCLE*.05);//5% duty cycle for all lights at once
    rainbow(); //Fill all LEDs with beautiful dancing rainbows while we wait for Serial
    
  }//End Serial.available()

  FastLED.show(); //Send LED color states to the LEDs
  lastSerialState = SerialState; //Update serial state
  
}//End Void Loop
