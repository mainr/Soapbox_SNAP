#ifndef BoardSelect_h
#define BoardSelect_h

// make sure exactly one of these lines is uncommented:
#define UNO
//#define NANO

const int MAX_PROGRAM_SIZE = 750; // in bytes (includes the 34(?) header bytes)

const int MIN_ANALOG_INPUT = 0; // analog inputs
#ifdef UNO
  const int MAX_ANALOG_INPUT = 5;
#endif
#ifdef NANO
  const int MAX_ANALOG_INPUT = 7;
#endif

#endif
