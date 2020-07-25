#pragma once

#include "chunk.h"
#include "value.h"

#define STACK_MAX 256

typedef enum {
	INTERPRET_OK,
	INTERPRET_COMPILE_ERROR,
	INTERPRET_RUNTIME_ERROR
} InterpretResult;

void initVM();
void freeVM();
InterpretResult interpret(char* source);
void push(Value value);
Value pop();