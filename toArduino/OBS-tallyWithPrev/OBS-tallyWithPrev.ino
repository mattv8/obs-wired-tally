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

//OBS Variables
int SerialInt;   // Global array for Serial integer.
int currentLive;
int lastLive;
int currentPreview;
int lastPreview;
int SerialState = 0;
int lastSerialState = 0;

// ------------------------FUNCTIONS------------------------ //

// This function draws rainbows with an ever-changing, widely-varying set of parameters. 
// Function credit goes to Mark Kriegsman (2015).
void rainbow() 
{
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
    //fill_solid( leds, TOTAL_LEDS, CRGB::Black ); //Fill all LEDs black
  }
  
  ////////////// Serial Communication ///////////////
  if(Serial.available()) { // Check if recieving data
        
    //Bump up duty cycle for only 2 lights
    FastLED.setBrightness(DUTY_CYCLE*.1);//10% duty cycle
    
    //ReadSerial(); // Record serial bits using custom function.
    SerialInt = Serial.parseInt();
      
    // Live Range //
    if ((SerialInt>=0) && (SerialInt<=4)){ //If within live range
      currentLive = SerialInt; // Update live state
    }
    
    // Preview Range //
    if ((SerialInt>=5) && (SerialInt<=9)){ //If within preview range
      currentPreview = SerialInt - 5; // Update preview state & subtract 5 so it matches live range
    }
    
    ////////////// Tally Light States ///////////////
    // Preview State //
    if(currentPreview != lastPreview) { //If preview state changes
        fill_solid( ledarray[lastPreview], NUM_LEDS, CRGB::Black ); // Clear pixel data if state changes
        lastPreview = currentPreview; //Update preview state
    }
    
    // Live State //
    if(currentLive != lastLive) { //If live state changes
      fill_solid( ledarray[lastLive], NUM_LEDS, CRGB::Black ); // Clear pixel data if state changes
      lastLive = currentLive; //Update live state
    }

    // Update LED Color States //
    // NOTE: Order is important here. Live is last so it "wins out" over Preview color.
    fill_solid( ledarray[currentPreview], NUM_LEDS, CRGB::Green );
    fill_solid( ledarray[currentLive], NUM_LEDS, CRGB::Red );

  }else { //If not recieving any data
    
    FastLED.setBrightness(DUTY_CYCLE*.05);//5% duty cycle for all lights at once
    rainbow(); //Fill all LEDs with beautiful dancing rainbows while we wait for Serial
    
  }//End Serial.available()

  lastSerialState = SerialState; //Update serial state
  FastLED.show(); //Send LED color states to the LEDs
  //delay(10); //Simple debounce delay
  
}//End Void Loop
