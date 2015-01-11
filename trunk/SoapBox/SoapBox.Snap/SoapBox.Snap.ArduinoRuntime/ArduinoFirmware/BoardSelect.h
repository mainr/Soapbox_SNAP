#ifndef BoardSelect_h
#define BoardSelect_h

// make sure exactly one of these lines is uncommented:
#define UNO
//#define NANO
//#define MEGA

const byte NUM_STRINGS = 0; // not implemented yet

const int MIN_ANALOG_INPUT = 0; // analog inputs
// 0 and 1 are serial port pins 
const int MIN_PIN_NUMBER = 2; // digital I/O
const int STATUS_LED_PIN = 13;
#ifdef UNO
  const int MAX_ANALOG_INPUT = 5;
  const int MAX_PROGRAM_SIZE = 750; // in bytes (includes the 34(?) header bytes)
  const int MAX_PIN_NUMBER = 12; // pin 13 is a status LED
  const int DEVICE_CONFIG_EEPROM_BYTES = 16;
  const int PWM_PINS[] = {3, 5, 6, 9, 10, 11};
  const int PWM_PIN_COUNT = 6; // must be length of the PWM_PINS array
  const int NUM_BOOLEANS = 512; // has to be a multiple of 8
  const byte NUM_NUMERICS = 24; // has to be a multiple of 8
#endif
#ifdef NANO
  const int MAX_ANALOG_INPUT = 7;
  const int MAX_PROGRAM_SIZE = 750; // in bytes (includes the 34(?) header bytes)
  const int MAX_PIN_NUMBER = 12; // pin 13 is a status LED
  const int DEVICE_CONFIG_EEPROM_BYTES = 16;
  const int PWM_PINS[] = {3, 5, 6, 9, 10, 11};
  const int PWM_PIN_COUNT = 6; // must be length of the PWM_PINS array
  const int NUM_BOOLEANS = 512; // has to be a multiple of 8
  const byte NUM_NUMERICS = 24; // has to be a multiple of 8
#endif
#ifdef MEGA
  const int MAX_ANALOG_INPUT = 15;
  const int MAX_PROGRAM_SIZE = 3750; // in bytes (includes the 34(?) header bytes)
  const int MAX_PIN_NUMBER = 53; // pin 13 is a status LED
  const int DEVICE_CONFIG_EEPROM_BYTES = 56;
  const int PWM_PINS[] = {2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 44, 45, 46};
  const int PWM_PIN_COUNT = 14; // must be length of the PWM_PINS array
  const int NUM_BOOLEANS = 1024; // has to be a multiple of 8
  const byte NUM_NUMERICS = 96; // has to be a multiple of 8
#endif

#endif
