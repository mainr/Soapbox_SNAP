#include <Arduino.h>
#include "BoardSelect.h"

#ifndef Program_h
#define Program_h

const byte MAGIC_VALUE = 138;

const int GUID_LENGTH = 16;
const int RUNTIME_ID_INDEX = 0;
const int VERSION_ID_INDEX = RUNTIME_ID_INDEX + GUID_LENGTH; // 16
const int BITS_PER_BOOL_ADDRESS_INDEX = VERSION_ID_INDEX + GUID_LENGTH; // 32
const int BITS_PER_NUMERIC_ADDRESS_INDEX = BITS_PER_BOOL_ADDRESS_INDEX + 1; // 33
const int OPCODES_INDEX = BITS_PER_NUMERIC_ADDRESS_INDEX + 1; // 34

class Program {  
  public:
    Program(int eepromAddress);
    
    void setProgramSize(int value);
    int getProgramSize();
    void storeByte(byte value, int index);
    byte getByte(int index);
    boolean readFromEeprom();
    void writeToEeprom();
    void printRuntimeId();
    void printVersionId();
    
  private:
    int _eepromAddress;
    int _programSize;
    byte _programMemory[MAX_PROGRAM_SIZE];
    
    void printEmptyGuid();
    void printGuidAsHex(int startIndex);
    void printByteAsHex(byte value);
};

#endif
