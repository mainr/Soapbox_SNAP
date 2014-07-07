#include <Arduino.h>
#include "Memory.h"
#include "Program.h"

#ifndef Engine_h
#define Engine_h

const int ENGINE_EEPROM_BYTES = 2;

const byte ERROR_NONE = 0;
const byte ERROR_CODE_NEXT_INSTRUCTION_FAILED = 1; // failed to get next instruction, or missing end of program

const byte INSTRUCTION_SERIES_END = 0x03;
const byte INSTRUCTION_PARALLEL_END = 0x16;

enum comparison {
  equalTo,
  greaterThan,
  greaterThanOrEqual,
  lessThan,
  lessThanOrEqual,
  notEqual
};

class Engine {  
  public:
    Engine(int eepromAddress, Program* program, Memory* memory);
    
    void preScan(); // call this in setup, or after a new program is downloaded
    void solveLogic(); // call this in loop, after scanInputs and before scanOutputs
    byte getErrorCode();
    boolean startEngine();
    boolean stopEngine();
    boolean getStatus();
    void readFromEeprom();
    void writeToEeprom();
    
  private:
    int _eepromAddress;
    Program* _program;
    Memory* _memory;
    int _instructionPointer;
    byte _bitsPerBooleanAddress;
    byte _bitsPerNumericAddress;
    boolean _running;
    boolean _preScan;
    unsigned long _time;
    unsigned long _elapsed;
    
    byte _loadedByte;
    byte _bitsInLoadedByte;
    boolean _programDone;
    boolean _rungCondition;
    
    void processProgram(boolean asPreScan);
    byte processNextInstruction();
    byte loadBitsIntoInstruction(byte instruction, byte bitsToLoad);
    byte getNextBit();
    
    void setError(byte errorCode);
    byte _errorInstructionPointer;
    byte _errorCode;
    
    void coil();
    void contactNO();
    void contactNC();
    void risingEdge();
    void fallingEdge();
    void setReset();
    void tmrOn();
    void tmrOff();
    void tmr(boolean onDelay);
    void parallelStart();
    void cntUp();
    void cntDn();
    void cnt(boolean up);
    
    void compare(comparison c);
    
    void mathAdd();
    void mathSubtract();
    void mathMultiply();
    void mathDivide();
    void mathChooseNumber();
    
    int getBooleanAddress(); // for output
    boolean getBooleanValue(); // for input
    byte getNumericAddress();
    NumericMemoryValue getNumericValue();
    
    NumericMemoryLocation loadBitsIntoNumericMemoryLocation(byte bitsToLoad);
};

#endif
