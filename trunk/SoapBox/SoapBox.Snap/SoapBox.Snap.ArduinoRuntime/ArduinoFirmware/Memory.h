#include <Arduino.h>
#include "BoardSelect.h"

#ifndef Memory_h
#define Memory_h

union NumericMemoryLocation {
  long longValue;
  float floatValue;
};

typedef struct {
  boolean isFloat;
  NumericMemoryLocation value;
} NumericMemoryValue;

class Memory {  
  public:
    Memory();
    
    // boolean memory
    boolean readBoolean(int address);
    boolean writeBoolean(int address, boolean value); // returns false if address invalid
    
    // numeric memory (each mem location is 4 bytes, either a long or a float)
    NumericMemoryValue readNumeric(byte address);
    boolean writeNumeric(byte address, NumericMemoryValue value); // returns false if address invalid
    
  private:
    byte _booleans[NUM_BOOLEANS/8];
    byte _isNumberAFloat[NUM_NUMERICS/8];
    NumericMemoryLocation _numerics[NUM_NUMERICS];
};

#endif
